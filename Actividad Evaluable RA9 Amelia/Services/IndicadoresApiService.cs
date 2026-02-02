using Actividad_Evaluable_RA9_Amelia.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Actividad_Evaluable_RA9_Amelia.Services
{
    /// <summary>
    /// Convertidor personalizado para manejar decimales que vienen como strings desde el API
    /// </summary>
    public class DecimalStringConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();
                
                // Si el string está vacío o es nulo, devolver 0
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    return 0m;
                }
                
                // Intentar parsear como decimal
                if (decimal.TryParse(stringValue, System.Globalization.NumberStyles.Any, 
                    System.Globalization.CultureInfo.InvariantCulture, out decimal value))
                {
                    return value;
                }
                
                // Si no se puede parsear (ej: "Sí", "No", etc.), devolver 0 en lugar de lanzar excepción
                // Esto permite que la deserialización continúe incluso con datos inesperados
                return 0m;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDecimal();
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                return 0m;
            }
            
            // Para otros tipos de token, devolver 0
            return 0m;
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    /// <summary>
    /// Servicio para consumir el API REST de Indicadores
    /// Implementa las operaciones CRUD reutilizando el backend existente
    /// </summary>
    public class IndicadoresApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<IndicadoresApiService> _logger;

        public IndicadoresApiService(HttpClient httpClient, IConfiguration configuration, ILogger<IndicadoresApiService> logger)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7093/api/Indicadores";
            _logger = logger;
            
            // Configurar opciones de serialización JSON
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                // Ignorar propiedades no mapeadas del JSON
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip
            };
        }

        /// <summary>
        /// Obtiene todos los indicadores del API REST
        /// </summary>
        public async Task<List<IndicadorDTO>> ObtenerTodosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                
                // Logging del JSON recibido para diagnóstico
                _logger.LogDebug("JSON recibido de la API: {Content}", content);
                
                var indicadores = JsonSerializer.Deserialize<List<IndicadorDTO>>(content, _jsonOptions);
                
                return indicadores ?? new List<IndicadorDTO>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al deserializar JSON de la API. El formato de respuesta no coincide con IndicadorDTO.");
                throw new InvalidOperationException("Error al procesar la respuesta del API. Verifica que el formato sea correcto.", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "API REST no disponible en {BaseUrl}", _baseUrl);
                throw new InvalidOperationException($"No se pudo conectar con el API en {_baseUrl}. Verifica que el servicio esté en ejecución.", ex);
            }
        }

        /// <summary>
        /// Obtiene un indicador específico por su ID
        /// </summary>
        public async Task<IndicadorDTO?> ObtenerPorIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("JSON recibido de la API para ID {Id}: {Content}", id, content);
                
                return JsonSerializer.Deserialize<IndicadorDTO>(content, _jsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al deserializar JSON de la API para ID {Id}.", id);
                throw new InvalidOperationException($"Error al procesar la respuesta del API para el indicador {id}.", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "API REST no disponible.");
                throw new InvalidOperationException($"No se pudo conectar con el API en {_baseUrl}.", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo indicador en el API REST
        /// </summary>
        public async Task<IndicadorDTO?> CrearAsync(IndicadorDTO indicador)
        {
            try
            {
                // Configurar opciones específicas para creación (ignorar Id = 0)
                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };
                
                // Logging del objeto antes de enviar
                var json = JsonSerializer.Serialize(indicador, serializerOptions);
                _logger.LogInformation("Enviando nuevo indicador al API: {Json}", json);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseUrl, content);
                
                // Si hay error, capturar el contenido de la respuesta antes de lanzar excepción
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error al crear indicador. Código: {StatusCode}, Respuesta: {ErrorContent}", 
                        response.StatusCode, errorContent);
                    throw new HttpRequestException($"Error del API ({response.StatusCode}): {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Indicador creado exitosamente. Respuesta: {ResponseContent}", responseContent);
                
                return JsonSerializer.Deserialize<IndicadorDTO>(responseContent, _jsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al deserializar respuesta del API al crear indicador.");
                throw new InvalidOperationException("Error al procesar la respuesta del API al crear el indicador.", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error HTTP al crear indicador.");
                throw new InvalidOperationException($"Error al comunicarse con el API: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un indicador existente en el API REST
        /// </summary>
        public async Task<bool> ActualizarAsync(int id, IndicadorDTO indicador)
        {
            try
            {
                // Logging del objeto antes de enviar
                var json = JsonSerializer.Serialize(indicador, _jsonOptions);
                _logger.LogInformation("Actualizando indicador ID {Id} en el API: {Json}", id, json);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Indicador con ID {Id} no encontrado en el API", id);
                    return false;
                }

                // Si hay otro error, capturar el contenido de la respuesta
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error al actualizar indicador ID {Id}. Código: {StatusCode}, Respuesta: {ErrorContent}", 
                        id, response.StatusCode, errorContent);
                    throw new HttpRequestException($"Error del API ({response.StatusCode}): {errorContent}");
                }

                _logger.LogInformation("Indicador ID {Id} actualizado exitosamente", id);
                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error HTTP al actualizar indicador ID {Id}", id);
                throw new InvalidOperationException($"Error al comunicarse con el API: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un indicador del API REST
        /// </summary>
        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;

                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "API REST no disponible.");
                throw new InvalidOperationException($"No se pudo conectar con el API en {_baseUrl}.", ex);
            }
        }
    }
}
