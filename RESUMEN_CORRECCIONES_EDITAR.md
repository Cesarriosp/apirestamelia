# ?? Correcciones Aplicadas - Error al Editar Indicadores

## ? Cambios Realizados

### 1. **Servicio API Mejorado** (`IndicadoresApiService.cs`)

#### Método `ActualizarAsync`:
- ? **Logging detallado**: Registra el JSON completo antes de enviar
- ? **Captura de errores del API**: Obtiene el mensaje de error antes de lanzar excepción
- ? **Diferenciación de 404**: Retorna `false` si no encuentra el indicador
- ? **Mensajes claros**: Excepciones con información específica del error

**Antes**:
```csharp
var json = JsonSerializer.Serialize(indicador, _jsonOptions);
var content = new StringContent(json, Encoding.UTF8, "application/json");
var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);

if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    return false;

response.EnsureSuccessStatusCode(); // ? No captura detalles del error
return true;
```

**Después**:
```csharp
var json = JsonSerializer.Serialize(indicador, _jsonOptions);
_logger.LogInformation("Actualizando indicador ID {Id} en el API: {Json}", id, json);

var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);

if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
{
    _logger.LogWarning("Indicador con ID {Id} no encontrado en el API", id);
    return false;
}

if (!response.IsSuccessStatusCode) // ? Captura el error completo
{
    var errorContent = await response.Content.ReadAsStringAsync();
    _logger.LogError("Error al actualizar indicador ID {Id}. Código: {StatusCode}, Respuesta: {ErrorContent}", 
        id, response.StatusCode, errorContent);
    throw new HttpRequestException($"Error del API ({response.StatusCode}): {errorContent}");
}

_logger.LogInformation("Indicador ID {Id} actualizado exitosamente", id);
return true;
```

### 2. **Controlador Mejorado** (`IndicadoresController.cs`)

#### Método `Edit` (POST):
- ? **Logging de intentos**: Registra cuando se intenta actualizar un indicador
- ? **Manejo detallado de excepciones**: Muestra mensajes específicos según el tipo de error
- ? **Información al usuario**: TempData con mensajes claros del error
- ? **Validación de resultado**: Diferencia entre "no encontrado" y otros errores

**Mejoras**:
```csharp
try
{
    _logger.LogInformation("Intentando actualizar indicador ID {Id}: {Nombre}", id, indicador.Nombre);
    
    var resultado = await _apiService.ActualizarAsync(id, indicador);
    
    if (!resultado)
    {
        _logger.LogWarning("Indicador con ID {Id} no encontrado al intentar actualizar", id);
        TempData["Error"] = "No se encontró el indicador a actualizar.";
        return RedirectToAction(nameof(Index));
    }

    _logger.LogInformation("Indicador ID {Id} actualizado exitosamente", id);
    TempData["Success"] = "Indicador actualizado correctamente.";
    return RedirectToAction(nameof(Index));
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

### 3. **Vista Mejorada** (`Edit.cshtml`)

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

---

## ?? Herramientas de Diagnóstico

### **ProbarActualizarIndicador.ps1**
Script nuevo que:
- Obtiene un indicador existente del API
- Lo actualiza con nuevos valores
- Muestra el JSON enviado
- Captura y analiza errores
- Verifica que la actualización fue exitosa

---

## ?? Pasos para Diagnosticar

### Paso 1: Ejecutar el Script de Prueba

```powershell
.\ProbarActualizarIndicador.ps1
```

**Si funciona**: El problema está en cómo la aplicación serializa/envía los datos  
**Si falla**: El problema está en el API backend

### Paso 2: Intentar Editar desde la Aplicación

1. Ejecuta la aplicación (F5)
2. Ve a la lista de indicadores
3. Click en "Editar" en cualquier indicador
4. Modifica algunos campos
5. Click en "Guardar Cambios"

### Paso 3: Revisar los Logs

En Visual Studio, ve a:
- **View** ? **Output**
- Selecciona **"Actividad Evaluable RA9 Amelia - ASP.NET Core Web Server"**

Busca estas líneas:
```
Intentando actualizar indicador ID X: [nombre]
Actualizando indicador ID X en el API: {json aquí}
```

Si hay error, verás:
```
Error al actualizar indicador ID X. Código: XXX, Respuesta: [detalles del error]
```

---

## ? Errores Más Comunes al Editar

### Error 400 - Bad Request

**Posibles causas**:
1. El ID en la URL no coincide con el ID en el body
2. El formato de fecha no es aceptado
3. Algún campo tiene un tipo incorrecto
4. El API espera campos adicionales que no se envían

**Solución**:
- Verifica que `id` se envíe correctamente tanto en la ruta como en el JSON
- Compara el JSON enviado con lo que espera el API

### Error 404 - Not Found

**Causa**: El indicador con ese ID no existe en el API

**Solución**: 
- Verifica que el indicador exista
- Puede que haya sido eliminado por otro usuario

### Error 415 - Unsupported Media Type

**Causa**: El API no acepta `application/json`

**Solución**: Verifica la configuración del API backend

### Error 500 - Internal Server Error

**Causa**: Error en el servidor del API

**Solución**: Revisa los logs del API backend

---

## ?? JSON Esperado para Actualización

La aplicación envía este formato:
```json
{
  "id": 1,
  "nombre": "Nombre del Indicador Actualizado",
  "descripcion": "Descripción actualizada",
  "valor": 150.75,
  "unidad": "unidades",
  "fecha": "2024-01-15T00:00:00",
  "fuente": "Fuente actualizada"
}
```

**Importante**: El campo `id` debe estar presente y coincidir con el ID de la URL.

---

## ?? Diferencias entre Crear y Editar

| Aspecto | Crear (POST) | Editar (PUT) |
|---------|--------------|--------------|
| Método HTTP | POST | PUT |
| URL | `/api/Indicadores` | `/api/Indicadores/{id}` |
| Campo `id` | Se omite (DefaultIgnoreCondition.WhenWritingDefault) | Debe incluirse y coincidir con URL |
| Respuesta 404 | No aplica | Indica que no existe el indicador |
| Código éxito | 200 o 201 | 200 o 204 |

---

## ? Verificación Rápida

### Comprobar que un indicador existe:
```powershell
Invoke-RestMethod -Uri "https://localhost:7093/api/Indicadores/1" -Method Get -SkipCertificateCheck
```

### Actualizar directamente con PowerShell:
```powershell
$body = @{
    id = 1
    nombre = "Nombre Actualizado"
    descripcion = "Descripción actualizada"
    valor = 200.5
    unidad = "unidades"
    fecha = "2024-01-15T00:00:00"
    fuente = "Fuente actualizada"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7093/api/Indicadores/1" `
    -Method Put `
    -Body $body `
    -ContentType "application/json" `
    -SkipCertificateCheck
```

---

## ?? Comparación: Crear vs Editar

### **Crear**:
- ? No envía `id` (se omite cuando es 0)
- ? El API genera el ID automáticamente
- ? Retorna el objeto creado con el nuevo ID

### **Editar**:
- ? Envía `id` completo
- ? El ID en el body debe coincidir con el de la URL
- ? Retorna código 200 o 204 (sin contenido)

---

## ?? Información para Diagnóstico

Si el error persiste, proporciona:

1. **Mensaje de error completo** de la aplicación
2. **JSON que se envía** (del log "Actualizando indicador ID X")
3. **Respuesta del API** (del log "Error al actualizar indicador")
4. **ID del indicador** que estás intentando editar
5. **Código del controlador PUT** del API backend

---

## ? Estado Actual

- ? Código compila sin errores
- ? Logging habilitado para actualización
- ? Captura detallada de errores del API
- ? Mensajes claros al usuario
- ? Script de prueba creado
- ? Vista mejorada con visualización de errores

---

## ?? Siguiente Paso

**Ejecuta la aplicación ahora e intenta editar un indicador.**

Los logs mejorados te mostrarán:
- ? Qué JSON se envía exactamente (incluyendo el ID)
- ? Qué responde el API
- ? Si es un error 404 (no existe) o un error de validación

**Si el error persiste, usa el script de prueba:**
```powershell
.\ProbarActualizarIndicador.ps1
```

Esto confirmará si el problema está en el API o en cómo la aplicación envía los datos.
