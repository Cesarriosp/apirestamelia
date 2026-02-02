# Diagnóstico de Error al Crear Indicadores

## ?? Mejoras Aplicadas

Se han realizado las siguientes mejoras para ayudarte a diagnosticar el error al crear indicadores:

### 1. **Logging Mejorado**
- El servicio ahora registra el JSON completo que se envía al API
- Captura y muestra errores específicos del API antes de lanzar excepciones
- Registra las respuestas exitosas para verificación

### 2. **Manejo de Campo `Id`**
- El campo `Id` se omite automáticamente cuando es 0 o default al crear
- Esto evita conflictos con APIs que generan el ID automáticamente

### 3. **Mensajes de Error Detallados**
- El controlador ahora muestra mensajes de error más específicos
- Los errores del API se muestran directamente al usuario
- Se incluyen detalles de excepciones internas

### 4. **Vista Mejorada**
- La vista Create ahora muestra errores de TempData
- Validation summary configurado para mostrar todos los errores

---

## ?? Cómo Diagnosticar el Error

### Paso 1: Habilitar Logging Detallado

Edita `appsettings.json` para ver los logs completos:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning",
      "Actividad_Evaluable_RA9_Amelia.Services": "Debug"
    }
  },
  "AllowedHosts": "*",
  "ApiSettings": {
    "BaseUrl": "https://localhost:7093/api/Indicadores"
  }
}
```

### Paso 2: Reproducir el Error

1. Ejecuta la aplicación (F5)
2. Ve a "Crear Nuevo Indicador"
3. Completa el formulario con datos de prueba:
   - **Nombre**: `Indicador de Prueba`
   - **Descripción**: `Descripción de prueba`
   - **Valor**: `100.50`
   - **Unidad**: `unidades`
   - **Fecha**: (fecha actual)
   - **Fuente**: `Prueba` (opcional)
4. Click en "Crear Indicador"

### Paso 3: Revisar los Logs

En la ventana de **Output** de Visual Studio, busca:

```
Enviando nuevo indicador al API: {Json}
```

Esto te mostrará exactamente qué se está enviando al API.

Si hay error, verás:
```
Error al crear indicador. Código: XXX, Respuesta: {...}
```

---

## ? Errores Comunes y Soluciones

### Error 400 - Bad Request

**Causa posible**: El API no acepta el formato de fecha o algún campo

**Solución**:
1. Verifica el formato de fecha que espera el API
2. El JSON enviado debería verse así:
```json
{
  "nombre": "Indicador de Prueba",
  "descripcion": "Descripción de prueba",
  "valor": 100.5,
  "unidad": "unidades",
  "fecha": "2024-01-15T00:00:00",
  "fuente": "Prueba"
}
```

Si el API espera un formato diferente de fecha, necesitarás ajustar el serializer.

### Error 415 - Unsupported Media Type

**Causa**: El API no acepta `application/json`

**Solución**: Verifica que el API backend esté configurado para aceptar JSON.

### Error 500 - Internal Server Error

**Causa**: Error en el servidor del API

**Solución**: 
1. Revisa los logs del API backend
2. Verifica que el modelo en el API coincida con el DTO

### Error de Validación

**Causa**: Los datos no cumplen las reglas de validación

**Solución**: Verifica:
- Nombre: máx 100 caracteres
- Descripción: máx 500 caracteres
- Valor: número positivo
- Unidad: máx 50 caracteres
- Fecha: requerida
- Fuente: opcional, máx 100 caracteres

---

## ?? Prueba con Curl

Puedes probar directamente el API con este comando (PowerShell):

```powershell
$body = @{
    nombre = "Indicador de Prueba"
    descripcion = "Descripción de prueba"
    valor = 100.5
    unidad = "unidades"
    fecha = "2024-01-15T00:00:00"
    fuente = "Prueba"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7093/api/Indicadores" `
    -Method Post `
    -Body $body `
    -ContentType "application/json" `
    -SkipCertificateCheck
```

Si esto funciona pero la aplicación no, el problema está en la serialización del cliente.

---

## ?? Verificar Estructura del API

Ejecuta este script para ver qué estructura espera el API:

```powershell
# Obtener un indicador existente para ver su estructura
Invoke-RestMethod -Uri "https://localhost:7093/api/Indicadores" `
    -Method Get `
    -SkipCertificateCheck | 
    Select-Object -First 1 | 
    ConvertTo-Json -Depth 5
```

Compara la estructura con tu `IndicadorDTO`.

---

## ?? Checklist de Diagnóstico

- [ ] Logs habilitados en `appsettings.json`
- [ ] Intentaste crear un indicador y capturaste el error
- [ ] Revisaste la ventana Output en Visual Studio
- [ ] Verificaste el JSON que se envía al API
- [ ] Comparaste con la estructura que espera el API
- [ ] Probaste crear directamente con curl/Invoke-RestMethod
- [ ] Revisaste los logs del API backend

---

## ?? Información para Soporte

Si necesitas ayuda, proporciona:

1. **Mensaje de error completo** de la aplicación
2. **JSON que se envía** (del log "Enviando nuevo indicador al API")
3. **Respuesta del API** (del log "Error al crear indicador")
4. **Estructura del modelo** del API backend
5. **Código del controlador** del API backend

---

## ? Siguiente Paso

**Ejecuta la aplicación nuevamente y intenta crear un indicador.**

Los logs mejorados te mostrarán exactamente:
- Qué datos se están enviando
- Qué responde el API
- Por qué está fallando

**Copia el mensaje de error completo y los logs, y te podré ayudar específicamente con tu caso.**
