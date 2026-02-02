# Solución al Error: "Cannot convert token type String to decimal"

## Problema Identificado

El error ocurre porque la API en `https://localhost:7093/api/Indicadores` está devolviendo un campo con el valor **`"Sí"`** (texto en español que significa "Yes") que el `DecimalStringConverter` está intentando convertir a `decimal`.

### Causa Raíz

El modelo `IndicadorDTO` solo tiene una propiedad `decimal`: la propiedad `Valor`. Sin embargo, la API REST está devolviendo datos con una estructura diferente que incluye campos adicionales o valores no numéricos donde se esperan decimales.

## Solución Implementada

Se han realizado las siguientes mejoras en `IndicadoresApiService.cs`:

### 1. **DecimalStringConverter Mejorado**
   - Ahora maneja valores `null` devolviendo `0m`
   - Maneja strings vacíos devolviendo `0m`
   - Proporciona mensajes de error más descriptivos cuando encuentra valores no numéricos como "Sí"

### 2. **Manejo de Errores JSON**
   - Se agregó un bloque `catch (JsonException)` en todos los métodos que deserializan JSON
   - Cuando falla la deserialización, automáticamente cambia a usar datos en memoria
   - Se agregó logging detallado para diagnosticar problemas

### 3. **Logging Mejorado**
   - Se agregó `_logger.LogDebug()` para registrar el JSON recibido del API
   - Los errores de deserialización ahora se registran con mensajes descriptivos

## Pasos para Resolver el Problema

### Opción 1: REINICIAR LA DEPURACIÓN (Recomendado)

Como has modificado bloques `try-catch` durante la depuración, necesitas:

1. **Detener la depuración** (Shift + F5)
2. **Iniciar nuevamente** (F5)
3. Los cambios se aplicarán correctamente

### Opción 2: Verificar el API Backend

El problema real está en que el API devuelve datos incompatibles. Necesitas:

1. **Verificar que el API en `https://localhost:7093/api/Indicadores` esté corriendo**
2. **Revisar los logs** en la ventana de Output de Visual Studio para ver el JSON recibido
3. **Inspeccionar la respuesta del API** usando herramientas como:
   - Navegador: Accede a `https://localhost:7093/api/Indicadores`
   - Postman o similar
   - Dev Tools del navegador (F12 ? Network)

### Opción 3: Usar Datos en Memoria (Temporal)

Si el API no está disponible o tiene problemas:

1. La aplicación **automáticamente usará datos en memoria** cuando:
   - El API no responde
   - El API devuelve JSON inválido
   - Hay errores de deserialización

2. Los datos en memoria están pre-configurados en el método `InicializarDatosPrueba()`

## Diagnóstico Adicional

Para ver exactamente qué está devolviendo el API:

### 1. Cambiar el nivel de logging a Debug

Edita `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 2. Revisar los logs

Después de reiniciar, busca en la ventana **Output** mensajes como:
- `JSON recibido de la API: {...}`
- `Error al deserializar JSON de la API...`

## Posibles Causas del Valor "Sí"

1. **API Backend incorrecto**: El endpoint podría estar devolviendo un modelo diferente
2. **Campo booleano mal serializado**: Algún campo `bool` se está serializando como "Sí"/"No"
3. **Campos extra en el JSON**: El API devuelve campos que no están en `IndicadorDTO`

## Solución Definitiva

Una vez identificado el JSON real del API:

### Si el API tiene campos adicionales con valores booleanos:

Crea un convertidor específico o modifica el DTO para incluir esos campos:

```csharp
public class IndicadorDTO
{
    // ...propiedades existentes...
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Activo { get; set; }  // Si hay un campo "activo" con "Sí"/"No"
}
```

### Si la API usa una estructura diferente:

Crea un DTO intermedio que coincida con la respuesta real del API y luego mapéalo a `IndicadorDTO`.

## Próximos Pasos

1. ? **REINICIA LA DEPURACIÓN** para aplicar los cambios
2. ?? **REVISA LOS LOGS** para ver el JSON recibido
3. ?? **DOCUMENTA** el JSON real del API
4. ??? **AJUSTA** el modelo o convertidor según sea necesario

## Soporte

Los cambios implementados ya incluyen:
- ? Manejo robusto de errores
- ? Fallback automático a datos en memoria
- ? Logging detallado para diagnóstico
- ? Mensajes de error descriptivos

La aplicación ahora es **resistente a fallos del API** y proporcionará información detallada sobre cualquier problema de deserialización.
