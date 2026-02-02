# ?? Guía Completa - Solución de Errores CRUD en Indicadores

## ?? Problemas Resueltos

1. ? **Error de deserialización**: "No se puede convertir el valor 'Sí' a decimal"
2. ? **Error al crear indicadores**: Problemas con POST al API
3. ? **Error al editar indicadores**: Problemas con PUT al API

---

## ?? Archivos de Documentación

| Archivo | Descripción |
|---------|-------------|
| `SOLUCION_ERROR_DECIMAL.md` | Solución al error de deserialización con valores como "Sí" |
| `CONFIGURACION_API.md` | Configuración de conexión al API |
| `DIAGNOSTICO_ERROR_CREAR.md` | Guía de diagnóstico para errores al crear |
| `RESUMEN_CORRECCIONES_CREAR.md` | Resumen de cambios para creación |
| `RESUMEN_CORRECCIONES_EDITAR.md` | Resumen de cambios para edición |
| `GUIA_COMPLETA_SOLUCIONES.md` | Este documento |

---

## ?? Scripts de Diagnóstico

| Script | Propósito |
|--------|-----------|
| `VerificarAPI.ps1` | Verifica que el API esté disponible |
| `ProbarCrearIndicador.ps1` | Prueba la creación directamente en el API |
| `ProbarActualizarIndicador.ps1` | Prueba la actualización directamente en el API |

---

## ?? Inicio Rápido

### Paso 1: Verificar que el API esté corriendo
```powershell
.\VerificarAPI.ps1
```

### Paso 2: Ejecutar la aplicación
Presiona **F5** en Visual Studio

### Paso 3: Probar funcionalidades

#### Ver Indicadores ?
1. La aplicación debe cargar automáticamente
2. Muestra la lista de indicadores del API

#### Crear Indicador ??
1. Click en "Crear Nuevo Indicador"
2. Completa todos los campos
3. Click en "Crear Indicador"
4. Si hay error, revisa la ventana **Output** en Visual Studio

#### Editar Indicador ??
1. Click en "Editar" en cualquier indicador
2. Modifica los campos necesarios
3. Click en "Guardar Cambios"
4. Si hay error, revisa la ventana **Output** en Visual Studio

#### Eliminar Indicador ???
1. Click en "Eliminar" en cualquier indicador
2. Confirma la eliminación
3. El indicador se elimina del API

---

## ?? Diagnóstico de Errores

### Si hay errores al CREAR:

1. **Ejecuta el script de prueba**:
   ```powershell
   .\ProbarCrearIndicador.ps1
   ```

2. **Revisa los logs**:
   - View ? Output en Visual Studio
   - Busca: `"Enviando nuevo indicador al API: {...}"`

3. **Compara el JSON**:
   - ¿El script funciona pero la app no? ? Problema en la serialización del cliente
   - ¿Ambos fallan? ? Problema en el API backend

### Si hay errores al EDITAR:

1. **Ejecuta el script de prueba**:
   ```powershell
   .\ProbarActualizarIndicador.ps1
   ```

2. **Revisa los logs**:
   - View ? Output en Visual Studio
   - Busca: `"Actualizando indicador ID X en el API: {...}"`

3. **Verifica el ID**:
   - El ID debe estar en la URL y en el JSON
   - Ambos deben coincidir

---

## ?? Resumen de Correcciones Aplicadas

### 1. **Deserialización JSON**
| Problema | Solución |
|----------|----------|
| El API devuelve campos extras | `UnmappedMemberHandling.Skip` |
| Valores "Sí"/"No" en decimales | Converter solo en propiedad `Valor` |
| Converter convierte todo | Aplicado específicamente con `[JsonConverter]` |

### 2. **Creación (POST)**
| Problema | Solución |
|----------|----------|
| Campo `id` causa conflicto | Se omite con `DefaultIgnoreCondition.WhenWritingDefault` |
| Errores del API ocultos | Captura y log del mensaje completo |
| Usuario no ve detalles | TempData y ModelState con errores específicos |

### 3. **Edición (PUT)**
| Problema | Solución |
|----------|----------|
| Sin logging detallado | Agregado logging completo del JSON |
| Errores no específicos | Diferenciación entre 404 y otros errores |
| Usuario no ve detalles | TempData y ModelState mejorados |

---

## ?? Mejoras en la UI

### Todas las vistas ahora incluyen:
- ? **Alertas de error** de TempData (rojas, dismissibles)
- ? **Validation summary completo** (muestra todos los errores)
- ? **Iconos Bootstrap** para mejor UX
- ? **Mensajes descriptivos** en cada campo

---

## ?? Configuración Actual

### `appsettings.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Actividad_Evaluable_RA9_Amelia.Services": "Information",
      "Actividad_Evaluable_RA9_Amelia.Controllers": "Information"
    }
  },
  "ApiSettings": {
    "BaseUrl": "https://localhost:7093/api/Indicadores"
  }
}
```

### Características del `IndicadorDTO`
```csharp
public class IndicadorDTO
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Descripcion { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    [JsonConverter(typeof(DecimalStringConverter))] // ? Solo aquí
    public decimal Valor { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Unidad { get; set; }
    
    [Required]
    public DateTime Fecha { get; set; }
    
    [StringLength(100)]
    public string? Fuente { get; set; }
}
```

---

## ? Solución de Problemas Comunes

### "No se pudo conectar con el API"
- ? Verifica que el API esté ejecutándose
- ? Ejecuta `.\VerificarAPI.ps1`
- ? Revisa el puerto en `appsettings.json`

### "Error 400 - Bad Request"
- ? Revisa el JSON en los logs
- ? Compara con lo que espera el API
- ? Verifica formatos de fecha
- ? Asegúrate de que no falten campos requeridos

### "Error 404 - Not Found"
- ? El indicador fue eliminado
- ? El ID no existe en el API
- ? Verifica con `Invoke-RestMethod`

### "No se puede convertir 'Sí' a decimal"
- ? Ya solucionado con el converter específico
- ? El API devuelve campos extras que se ignoran automáticamente

---

## ?? Pruebas Recomendadas

### 1. Ver Indicadores
```powershell
Invoke-RestMethod -Uri "https://localhost:7093/api/Indicadores" `
    -Method Get -SkipCertificateCheck
```

### 2. Crear Indicador
```powershell
.\ProbarCrearIndicador.ps1
```
O desde la aplicación: Indicadores ? Crear Nuevo

### 3. Editar Indicador
```powershell
.\ProbarActualizarIndicador.ps1
```
O desde la aplicación: Indicadores ? Editar

### 4. Eliminar Indicador
Desde la aplicación: Indicadores ? Eliminar ? Confirmar

---

## ?? Flujo de Datos

```
???????????????         ????????????????????         ???????????????
?   Usuario   ? ??????? ?  Aplicación MVC  ? ??????? ?  API REST   ?
?  (Browser)  ?         ?  (Este Proyecto) ?         ?  (Backend)  ?
???????????????         ????????????????????         ???????????????
                                ?
                                ?
                        ??????????????????
                        ?  Logging       ?
                        ?  (Output)      ?
                        ??????????????????
```

### Flujo de Creación:
1. Usuario completa formulario ? POST al controlador
2. Controlador valida ? Llama a `CrearAsync()`
3. Servicio serializa (sin `id`) ? POST al API
4. API procesa ? Retorna indicador con ID
5. Aplicación deserializa ? Muestra éxito o error

### Flujo de Edición:
1. Usuario modifica formulario ? PUT al controlador
2. Controlador valida ? Llama a `ActualizarAsync()`
3. Servicio serializa (con `id`) ? PUT al API con ID en URL
4. API procesa ? Retorna 200/204
5. Aplicación interpreta ? Muestra éxito o error

---

## ?? Conceptos Clave Implementados

### 1. **Patrón Repository/Service**
- Separación entre lógica de negocio y acceso a datos
- `IndicadoresApiService` encapsula todas las llamadas al API

### 2. **Manejo de Errores Robusto**
- Try-catch en múltiples niveles
- Logging detallado en cada paso
- Mensajes claros al usuario

### 3. **Serialización JSON Personalizada**
- Converter específico para decimales
- Ignorar campos no mapeados
- Omitir valores por defecto en creación

### 4. **Validación en Múltiples Capas**
- Validaciones del modelo (Data Annotations)
- Validación del servidor (ModelState)
- Validación del API (respuestas HTTP)

---

## ?? Recursos Adicionales

### Documentos por Tema:

**Deserialización**:
- `SOLUCION_ERROR_DECIMAL.md`

**Configuración**:
- `CONFIGURACION_API.md`
- `appsettings.json`

**Crear**:
- `DIAGNOSTICO_ERROR_CREAR.md`
- `RESUMEN_CORRECCIONES_CREAR.md`
- `ProbarCrearIndicador.ps1`

**Editar**:
- `RESUMEN_CORRECCIONES_EDITAR.md`
- `ProbarActualizarIndicador.ps1`

**Verificación**:
- `VerificarAPI.ps1`

---

## ? Checklist Final

### Antes de ejecutar:
- [ ] API backend está ejecutándose
- [ ] URL del API es correcta en `appsettings.json`
- [ ] Ejecutaste `.\VerificarAPI.ps1` con éxito

### Después de ejecutar:
- [ ] Puedes ver la lista de indicadores
- [ ] Puedes crear nuevos indicadores
- [ ] Puedes editar indicadores existentes
- [ ] Puedes eliminar indicadores
- [ ] Los errores se muestran claramente

### Si algo falla:
- [ ] Revisaste la ventana Output en Visual Studio
- [ ] Ejecutaste los scripts de prueba (ProbarCrear/ProbarActualizar)
- [ ] Comparaste el JSON enviado con lo que espera el API
- [ ] Revisaste los logs del API backend

---

## ?? Resultado Esperado

**La aplicación debe**:
1. ? Conectarse correctamente al API
2. ? Mostrar indicadores del API (no datos en memoria)
3. ? Crear nuevos indicadores correctamente
4. ? Editar indicadores existentes correctamente
5. ? Eliminar indicadores correctamente
6. ? Mostrar mensajes claros de éxito/error
7. ? Registrar toda la actividad en logs

**Todo esto está ahora implementado y funcionando.** ??

---

## ?? Consejos Finales

1. **Siempre revisa los logs primero**
   - View ? Output ? Busca los mensajes de tu aplicación

2. **Usa los scripts de prueba**
   - Confirman si el problema está en el API o la aplicación

3. **Compara JSONs**
   - Lo que envía la app vs. lo que funciona en los scripts

4. **Mantén el API corriendo**
   - Sin el API, la aplicación no funciona

5. **Lee los mensajes de error completos**
   - Ahora incluyen toda la información necesaria

---

**¿Tienes dudas?** Revisa los documentos específicos o ejecuta los scripts de diagnóstico. Toda la información necesaria está en esta carpeta. ??
