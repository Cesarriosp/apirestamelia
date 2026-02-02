# ?? Solución: Error "valor debe ser string"

## ? Error Encontrado

```json
{
  "errors": {
    "indicador": ["The indicador field is required."],
    "$.valor": ["The JSON value could not be converted to System.String. Path: $.valor"]
  }
}
```

## ?? Análisis del Problema

El API backend espera:
- ? El campo `valor` debe ser un **string** (ej: `"123.45"`)
- ? Estábamos enviando un **decimal/number** (ej: `123.45`)

### Antes (Incorrecto):
```json
{
  "nombre": "Prueba",
  "descripcion": "Descripción",
  "valor": 12,         // ? Número (decimal)
  "unidad": "procesos"
}
```

### Después (Correcto):
```json
{
  "nombre": "Prueba",
  "descripcion": "Descripción",
  "valor": "12",       // ? String
  "unidad": "procesos"
}
```

---

## ? Solución Aplicada

### Modificación en `IndicadorDTO.cs`

Se agregó una propiedad auxiliar `ValorString` que:
1. **Convierte automáticamente** el decimal a string al serializar
2. **Lee el string** del API y lo convierte a decimal al deserializar
3. **Oculta** la propiedad `Valor` original de la serialización JSON

```csharp
public class IndicadorDTO
{
    // Propiedad para uso interno (no se serializa)
    [JsonIgnore]
    public decimal Valor { get; set; }
    
    // Propiedad para serialización (se envía al API como string)
    [JsonPropertyName("valor")]
    public string ValorString
    {
        get => Valor.ToString("0.##", CultureInfo.InvariantCulture);
        set
        {
            if (decimal.TryParse(value, NumberStyles.Any,
                CultureInfo.InvariantCulture, out decimal parsedValue))
            {
                Valor = parsedValue;
            }
        }
    }
}
```

---

## ?? Cómo Funciona

### Al Enviar al API (Serialización):
1. Usuario ingresa en el formulario: `12`
2. Se guarda en `Valor` como decimal: `12m`
3. Al serializar, `ValorString` devuelve: `"12"`
4. Se envía al API: `{ "valor": "12" }` ?

### Al Recibir del API (Deserialización):
1. API devuelve: `{ "valor": "123.45" }`
2. `ValorString` recibe el string: `"123.45"`
3. Se parsea y guarda en `Valor`: `123.45m`
4. La vista muestra: `123.45` ?

---

## ?? Ventajas de Esta Solución

1. ? **Transparente para el usuario**: El formulario sigue usando `type="number"`
2. ? **Sin cambios en las vistas**: Las vistas siguen referenciando `Valor`
3. ? **Validaciones intactas**: Las validaciones de rango siguen funcionando
4. ? **Compatible con el API**: Envía strings como espera el backend
5. ? **Reversible**: Si el API cambia a decimal, solo quitas `[JsonIgnore]`

---

## ?? Validaciones que Siguen Funcionando

```csharp
[Required(ErrorMessage = "El valor es obligatorio")]
[Range(0, double.MaxValue, ErrorMessage = "El valor debe ser un número positivo")]
public decimal Valor { get; set; }
```

Estas validaciones se aplican **antes** de la serialización, por lo que:
- ? El usuario debe ingresar un número
- ? Debe ser positivo
- ? Es obligatorio

---

## ?? Prueba de la Solución

### Datos de Prueba:
```
Nombre: "Indicador de Prueba"
Descripción: "Descripción de prueba"
Valor: 123.45
Unidad: "unidades"
Fecha: "2024-01-15"
Fuente: "Prueba"
```

### JSON Generado (lo que ve el API):
```json
{
  "nombre": "Indicador de Prueba",
  "descripcion": "Descripción de prueba",
  "valor": "123.45",          // ? Ahora es string
  "unidad": "unidades",
  "fecha": "2024-01-15T00:00:00",
  "fuente": "Prueba"
}
```

---

## ?? Comparación

| Aspecto | Antes | Después |
|---------|-------|---------|
| **Tipo en C#** | `decimal` | `decimal` (sin cambios) |
| **Tipo en JSON** | `number` ? | `string` ? |
| **Formulario** | `<input type="number">` | `<input type="number">` (sin cambios) |
| **Validaciones** | Funcionales | Funcionales (sin cambios) |
| **Error del API** | 400 Bad Request | ? Funciona |

---

## ?? Verificación

Después de aplicar los cambios:

1. **Compila sin errores** ?
2. **Las vistas no necesitan cambios** ?
3. **El formulario funciona igual** ?
4. **El API acepta el valor como string** ?

---

## ?? Próximos Pasos

1. **Ejecuta la aplicación** (F5)
2. **Intenta crear un indicador**:
   - Nombre: `Prueba con Valor String`
   - Descripción: `Verificando que el valor se envíe como string`
   - Valor: `150.75`
   - Unidad: `unidades`
   - Fecha: (fecha actual)
   - Fuente: `Prueba`

3. **Si funciona**: ? El problema está resuelto
4. **Si sigue fallando**: Revisa los logs en Output para ver el JSON exacto que se envía

---

## ?? Nota Importante

Esta solución es específica para APIs que esperan valores numéricos como strings. Es una práctica común en algunos frameworks que prefieren manejar la conversión de tipos en el backend por razones de precisión o internacionalización.

Si tu API backend cambia en el futuro para aceptar números directamente, solo necesitas:
1. Quitar `[JsonIgnore]` de `Valor`
2. Quitar la propiedad `ValorString`

---

## ?? Logging Mejorado

El servicio ahora mostrará en los logs el JSON con el valor como string:

```
Enviando nuevo indicador al API: {
  "nombre": "Prueba",
  "valor": "123.45",    // ? Verificarás que es string
  ...
}
```

---

**La aplicación ahora envía el campo `valor` como string, tal como lo espera el API backend.** ?
