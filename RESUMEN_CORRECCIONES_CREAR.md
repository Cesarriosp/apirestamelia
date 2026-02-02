# ?? Correcciones Aplicadas - Error al Crear Indicadores

## ? Cambios Realizados

### 1. **Servicio API Mejorado** (`IndicadoresApiService.cs`)

#### Método `CrearAsync`:
- ? **Logging detallado**: Registra el JSON completo antes de enviar
- ? **Captura de errores del API**: Obtiene el mensaje de error antes de lanzar excepción
- ? **Omisión del campo Id**: Usa `JsonIgnoreCondition.WhenWritingDefault` para no enviar `id: 0`
- ? **Mensajes claros**: Excepciones con información específica del error

**Antes**:
```csharp
var json = JsonSerializer.Serialize(indicador, _jsonOptions);
var content = new StringContent(json, Encoding.UTF8, "application/json");
var response = await _httpClient.PostAsync(_baseUrl, content);
response.EnsureSuccessStatusCode(); // ? No captura detalles del error
```

**Después**:
```csharp
// Opciones para omitir id = 0
var serializerOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
};

var json = JsonSerializer.Serialize(indicador, serializerOptions);
_logger.LogInformation("Enviando nuevo indicador al API: {Json}", json);

var response = await _httpClient.PostAsync(_baseUrl, content);

if (!response.IsSuccessStatusCode) // ? Captura el error completo
{
    var errorContent = await response.Content.ReadAsStringAsync();
    _logger.LogError("Error al crear indicador. Código: {StatusCode}, Respuesta: {ErrorContent}", 
        response.StatusCode, errorContent);
    throw new HttpRequestException($"Error del API ({response.StatusCode}): {errorContent}");
}
```

### 2. **Controlador Mejorado** (`IndicadoresController.cs`)

#### Método `Create`:
- ? **Logging de intentos**: Registra cuando se intenta crear un indicador
- ? **Manejo detallado de excepciones**: Muestra mensajes específicos según el tipo de error
- ? **Información al usuario**: TempData con mensajes claros del error
- ? **Validación de resultado**: Verifica que el API devuelva el indicador creado

**Mejoras**:
```csharp
try
{
    _logger.LogInformation("Intentando crear indicador: {Nombre}", indicador.Nombre);
    
    var resultado = await _apiService.CrearAsync(indicador);
    
    if (resultado != null)
    {
        _logger.LogInformation("Indicador creado exitosamente con ID: {Id}", resultado.Id);
        TempData["Success"] = "Indicador creado correctamente.";
        return RedirectToAction(nameof(Index));
    }
    // ... manejo de null
}
catch (InvalidOperationException ex)
{
    var errorMessage = ex.Message;
    if (ex.InnerException != null)
    {
        errorMessage += $" Detalle: {ex.InnerException.Message}";
    }
    
    ModelState.AddModelError("", errorMessage);
    TempData["Error"] = errorMessage;
    return View(indicador);
}
```

### 3. **Vista Mejorada** (`Create.cshtml`)

- ? **Visualización de errores de TempData**: Muestra alertas rojas con el error
- ? **Validation Summary completo**: Cambiado de `ModelOnly` a `All`
- ? **Alertas dismissibles**: Usuario puede cerrar mensajes de error

**Cambios**:
```razor
@if (TempData["Error"] != null)
{
    <div class="alert alert-danger alert-dismissible fade show" role="alert">
        <i class="bi bi-exclamation-triangle-fill"></i>
        <strong>Error:</strong> @TempData["Error"]
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
}

<div asp-validation-summary="All" class="alert alert-danger"></div>
```

### 4. **Logging Habilitado** (`appsettings.json`)

- ? **Nivel Information** para servicios y controladores
- ? **Registro detallado de operaciones**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Actividad_Evaluable_RA9_Amelia.Services": "Information",
      "Actividad_Evaluable_RA9_Amelia.Controllers": "Information"
    }
  }
}
```

---

## ?? Herramientas de Diagnóstico Creadas

### 1. **DIAGNOSTICO_ERROR_CREAR.md**
Guía completa con:
- Pasos para diagnosticar el error
- Errores comunes y soluciones
- Checklist de verificación
- Comandos de prueba

### 2. **ProbarCrearIndicador.ps1**
Script de PowerShell que:
- Crea un indicador directamente en el API
- Muestra el JSON enviado
- Captura y analiza errores
- Ayuda a identificar si el problema está en el API o el cliente

### 3. **VerificarAPI.ps1** (ya existente)
Verifica que el API esté disponible

---

## ?? Pasos para Diagnosticar

### Paso 1: Ejecutar el Script de Prueba

```powershell
.\ProbarCrearIndicador.ps1
```

**Si funciona**: El problema está en cómo la aplicación serializa/envía los datos
**Si falla**: El problema está en el API backend

### Paso 2: Intentar Crear desde la Aplicación

1. Ejecuta la aplicación (F5)
2. Ve a "Crear Nuevo Indicador"
3. Completa todos los campos
4. Click en "Crear Indicador"

### Paso 3: Revisar los Logs

En Visual Studio, ve a:
- **View** ? **Output**
- Selecciona **"Actividad Evaluable RA9 Amelia - ASP.NET Core Web Server"**

Busca estas líneas:
```
Intentando crear indicador: [nombre]
Enviando nuevo indicador al API: {json aquí}
```

Si hay error, verás:
```
Error al crear indicador. Código: XXX, Respuesta: [detalles del error]
```

---

## ? Errores Más Comunes

### Error 400 - Bad Request

**Posibles causas**:
1. El API espera campos adicionales (tipo, categoria, etc.)
2. El formato de fecha no es aceptado
3. El campo `id` no debería enviarse (YA CORREGIDO)
4. Algún campo tiene un tipo incorrecto

**Solución**:
- Compara el JSON enviado con lo que espera el API
- Usa el script `ProbarCrearIndicador.ps1` para ver qué funciona

### Error 415 - Unsupported Media Type

**Causa**: El API no acepta `application/json`

**Solución**: Verifica la configuración del API backend

### Error 500 - Internal Server Error

**Causa**: Error en el servidor del API

**Solución**: Revisa los logs del API backend

---

## ?? Qué Hacer Ahora

### Opción 1: Usar el Script de Prueba
```powershell
.\ProbarCrearIndicador.ps1
```

Esto te dirá si el API funciona correctamente.

### Opción 2: Ejecutar la Aplicación con Logging

1. La aplicación ya tiene logging habilitado
2. Ejecuta (F5)
3. Intenta crear un indicador
4. **Copia el mensaje de error completo de la ventana Output**
5. **Copia el JSON que se envió** (línea que dice "Enviando nuevo indicador al API")

Con esa información podré ayudarte específicamente.

---

## ?? JSON Esperado

La aplicación envía este formato:
```json
{
  "nombre": "Nombre del Indicador",
  "descripcion": "Descripción detallada",
  "valor": 100.5,
  "unidad": "unidades",
  "fecha": "2024-01-15T00:00:00",
  "fuente": "Fuente de datos"
}
```

**Nota**: El campo `id` se omite automáticamente ahora.

Si tu API espera un formato diferente (por ejemplo, con campos adicionales como `tipo` o `categoria`), necesitarás agregar esos campos al DTO.

---

## ? Compilación y Estado

- ? Código compila sin errores
- ? Mejoras aplicadas en servicio, controlador y vista
- ? Logging habilitado
- ? Scripts de diagnóstico creados

**La aplicación está lista para diagnosticar el error específico que estás experimentando.**

---

## ?? Próximos Pasos

1. **Ejecuta**: `.\ProbarCrearIndicador.ps1`
2. **Ejecuta**: La aplicación MVC (F5)
3. **Intenta**: Crear un indicador
4. **Copia**: El error completo de la ventana Output
5. **Comparte**: El mensaje de error y el JSON enviado

Con esa información podré darte una solución específica para tu caso.
