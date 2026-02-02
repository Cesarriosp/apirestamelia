using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Actividad_Evaluable_RA9_Amelia.Models
{
    /// <summary>
    /// DTO para representar un indicador consumido del API REST
    /// </summary>
    public class IndicadorDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio")]
        [StringLength(100, ErrorMessage = "El tipo no puede exceder los 100 caracteres")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ámbito es obligatorio")]
        [StringLength(100, ErrorMessage = "El ámbito no puede exceder los 100 caracteres")]
        public string Ambito { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(100, ErrorMessage = "La categoría no puede exceder los 100 caracteres")]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El valor es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El valor debe ser un número positivo")]
        [JsonIgnore] // No serializar el valor como decimal
        public decimal Valor { get; set; }
        
        // El API espera "valor" como string
        [JsonPropertyName("valor")]
        public string ValorString
        {
            get => Valor.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && 
                    decimal.TryParse(value, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal parsedValue))
                {
                    Valor = parsedValue;
                }
            }
        }

        [Required(ErrorMessage = "La unidad es obligatoria")]
        [StringLength(50, ErrorMessage = "La unidad no puede exceder los 50 caracteres")]
        public string Unidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [StringLength(100, ErrorMessage = "La fuente no puede exceder los 100 caracteres")]
        public string? Fuente { get; set; }
    }
}
