# Aplicación MVC Cliente del API REST de Indicadores

## EcoData Solutions S.L.

Esta aplicación ASP.NET Core MVC actúa como cliente del API REST de indicadores ambientales, implementando una arquitectura de consumo de servicios sin acceso directo a la base de datos.

---

## ?? Descripción del Proyecto

La aplicación desarrollada es una **aplicación web MVC completa** que consume un API REST existente de gestión de indicadores ambientales. El proyecto demuestra la **reutilización de servicios**, la **separación de responsabilidades** entre cliente y servidor, y la **integración de aplicaciones web híbridas**.

### Características Principales

- ? **Consumo de API REST**: Todas las operaciones CRUD se realizan a través del API REST
- ? **Sin acceso directo a BD**: No hay conexión directa a la base de datos
- ? **Validaciones en Cliente**: DTOs con validaciones básicas usando Data Annotations
- ? **Interfaz Responsive**: Diseño moderno con Bootstrap 5
- ? **Gestión completa**: Operaciones de Consulta, Alta, Modificación y Eliminación
- ? **Arquitectura MVC**: Separación clara de responsabilidades

---

## ??? Arquitectura de la Solución

### Componentes Principales

```
???????????????????????????????????????????????
?      Aplicación MVC (Cliente)               ?
?  ??????????????  ????????????  ??????????? ?
?  ?Controladores?  ? Vistas   ?  ?  DTOs   ? ?
?  ??????????????  ????????????  ??????????? ?
?         ?              ?              ?      ?
?  ????????????????????????????????????????  ?
?  ?   IndicadoresApiService (HttpClient) ?  ?
?  ????????????????????????????????????????  ?
???????????????????????????????????????????????
                    ? HTTP/REST
???????????????????????????????????????????????
?         API REST (Servidor)                 ?
?  https://localhost:7093/api/Indicadores     ?
?  ????????????????  ??????????????????????  ?
?  ?  Controllers ?  ?  Lógica de Negocio ?  ?
?  ????????????????  ??????????????????????  ?
?                         ?                   ?
?                 ????????????????            ?
?                 ? Base de Datos?            ?
?                 ????????????????            ?
???????????????????????????????????????????????
```

### Tecnologías Utilizadas

- **Framework**: ASP.NET Core 8.0 MVC
- **Lenguaje**: C# 12
- **Frontend**: Bootstrap 5.3, Bootstrap Icons
- **Validación**: jQuery Validate, Unobtrusive Validation
- **Consumo API**: HttpClient con inyección de dependencias
- **Serialización**: System.Text.Json

---

## ?? Integración con el API REST

### Configuración

La URL del API REST se configura en `appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7093/api/Indicadores"
  }
}
```

### Endpoints Consumidos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/Indicadores` | Obtener todos los indicadores |
| GET | `/api/Indicadores/{id}` | Obtener un indicador específico |
| POST | `/api/Indicadores` | Crear un nuevo indicador |
| PUT | `/api/Indicadores/{id}` | Actualizar un indicador |
| DELETE | `/api/Indicadores/{id}` | Eliminar un indicador |

### Servicio de Consumo (IndicadoresApiService)

El servicio `IndicadoresApiService` encapsula toda la lógica de comunicación con el API REST:

- **Uso de HttpClient**: Inyectado mediante DI con configuración personalizada
- **Manejo de errores**: Try-catch con excepciones descriptivas
- **Serialización JSON**: Configuración case-insensitive y camelCase
- **Certificados SSL**: Aceptación de certificados autofirmados en desarrollo

```csharp
public async Task<List<IndicadorDTO>> ObtenerTodosAsync()
{
    var response = await _httpClient.GetAsync(_baseUrl);
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<List<IndicadorDTO>>(content, _jsonOptions);
}
```

---

## ?? Estructura del Proyecto

```
Actividad Evaluable RA9 Amelia/
?
??? Controllers/
?   ??? IndicadoresController.cs      # Controlador MVC principal
?
??? Models/
?   ??? IndicadorDTO.cs                # DTO con validaciones
?
??? Services/
?   ??? IndicadoresApiService.cs       # Servicio de consumo del API
?
??? Views/
?   ??? Shared/
?   ?   ??? _Layout.cshtml             # Layout principal
?   ?   ??? _ValidationScriptsPartial.cshtml
?   ??? Indicadores/
?   ?   ??? Index.cshtml               # Lista de indicadores
?   ?   ??? Details.cshtml             # Detalles de un indicador
?   ?   ??? Create.cshtml              # Formulario de creación
?   ?   ??? Edit.cshtml                # Formulario de edición
?   ?   ??? Delete.cshtml              # Confirmación de eliminación
?   ??? _ViewImports.cshtml
?   ??? _ViewStart.cshtml
?
??? wwwroot/
?   ??? css/
?   ?   ??? site.css                   # Estilos personalizados
?   ??? js/
?       ??? site.js                    # JavaScript personalizado
?
??? Program.cs                         # Configuración de la aplicación
??? appsettings.json                   # Configuración
??? README.md                          # Esta documentación
```

---

## ?? DTOs y Validaciones

### IndicadorDTO

El DTO incluye validaciones básicas del lado del cliente:

```csharp
public class IndicadorDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "El valor es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El valor debe ser un número positivo")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "La unidad es obligatoria")]
    [StringLength(50, ErrorMessage = "La unidad no puede exceder los 50 caracteres")]
    public string Unidad { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria")]
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; }

    [StringLength(100, ErrorMessage = "La fuente no puede exceder los 100 caracteres")]
    public string? Fuente { get; set; }
}
```

**Propósito de las validaciones**:
- Mejorar la experiencia de usuario con feedback inmediato
- Reducir peticiones inválidas al API REST
- **NO sustituyen** las validaciones del servidor

---

## ?? Guía de Uso

### Requisitos Previos

1. **API REST en ejecución**: El API de indicadores debe estar corriendo en `https://localhost:7093`
2. **.NET 8 SDK** instalado
3. Visual Studio 2022 o superior (o VS Code con extensiones de C#)

### Pasos para Ejecutar

1. **Verificar que el API REST esté activo**:
   ```bash
   # El API debe estar corriendo en https://localhost:7093
   ```

2. **Abrir la solución**:
   - Abrir en Visual Studio la carpeta del proyecto

3. **Restaurar dependencias**:
   ```bash
   dotnet restore
   ```

4. **Compilar el proyecto**:
   ```bash
   dotnet build
   ```

5. **Ejecutar la aplicación**:
   ```bash
   dotnet run
   ```
   O presionar F5 en Visual Studio

6. **Acceder a la aplicación**:
   - La aplicación se abrirá automáticamente en el navegador
   - URL por defecto: `https://localhost:[puerto]`

### Operaciones Disponibles

#### 1. Consultar Indicadores
- Acceder a la página principal (`/Indicadores/Index`)
- Visualizar la lista completa de indicadores
- Ver detalles de cada indicador

#### 2. Crear Nuevo Indicador
- Hacer clic en "Crear Nuevo Indicador"
- Completar el formulario con validaciones
- Enviar al API REST para su creación

#### 3. Editar Indicador
- Desde la lista, hacer clic en "Editar"
- Modificar los campos necesarios
- Guardar cambios (se actualizan en el API)

#### 4. Eliminar Indicador
- Desde la lista, hacer clic en "Eliminar"
- Confirmar la eliminación
- El indicador se elimina del API REST

---

## ?? Capturas de Funcionalidades

### Página Principal - Listado
- Tabla responsive con todos los indicadores
- Acciones: Ver, Editar, Eliminar
- Información sobre la arquitectura de la aplicación

### Formulario de Creación
- Validaciones en tiempo real
- Campos con descripción y ejemplos
- Feedback visual para errores

### Detalles del Indicador
- Vista detallada de todos los campos
- Acciones rápidas: Editar, Eliminar, Volver

### Confirmación de Eliminación
- Advertencia clara sobre la acción
- Muestra todos los datos antes de eliminar
- Botón de cancelación

---

## ?? Ventajas de la Reutilización

Esta aplicación demuestra las siguientes ventajas de reutilizar servicios existentes:

### 1. **Separación de Responsabilidades**
- El frontend (MVC) se concentra en la presentación
- El backend (API REST) mantiene la lógica de negocio
- No hay duplicación de código

### 2. **Escalabilidad**
- El API puede servir a múltiples clientes (web, móvil, desktop)
- Cambios en el backend no afectan al cliente (siempre que se mantenga el contrato)
- Fácil integración de nuevos clientes

### 3. **Mantenibilidad**
- La lógica de negocio está centralizada en el API
- Las validaciones principales están en un solo lugar
- Actualizaciones más simples y controladas

### 4. **Seguridad**
- El cliente no tiene acceso directo a la base de datos
- Todas las operaciones pasan por el API REST
- Control de acceso centralizado

### 5. **Reutilización de Código**
- No se duplica la lógica de acceso a datos
- Aprovechamiento de servicios ya probados y validados
- Reducción del tiempo de desarrollo

---

## ?? Pruebas Realizadas

### Pruebas Funcionales

? **Obtener lista de indicadores**
- La aplicación obtiene correctamente todos los indicadores del API
- Manejo de lista vacía

? **Crear nuevo indicador**
- Validaciones del lado del cliente funcionan correctamente
- El indicador se crea en el API y se muestra en la lista

? **Ver detalles de un indicador**
- Los detalles se obtienen del API correctamente
- Manejo de indicadores no encontrados

? **Editar indicador existente**
- Los datos se cargan correctamente en el formulario
- Las actualizaciones se reflejan en el API

? **Eliminar indicador**
- La confirmación previene eliminaciones accidentales
- El indicador se elimina correctamente del API

### Pruebas de Integración

? **Conexión con el API REST**
- HttpClient configurado correctamente
- Manejo de certificados SSL en desarrollo
- Timeouts y reintentos

? **Manejo de errores**
- Mensajes descriptivos cuando el API no está disponible
- Feedback visual para operaciones exitosas y fallidas
- Logging de errores

### Pruebas de Validación

? **Validaciones del cliente**
- Campos requeridos
- Longitudes máximas
- Tipos de datos correctos
- Rangos de valores

---

## ?? Decisiones de Diseño

### Consumo desde Controladores vs JavaScript

Se ha optado por **consumir el API desde los controladores MVC** por las siguientes razones:

**Ventajas**:
1. **Seguridad**: Las claves y configuraciones no se exponen al cliente
2. **Control**: Mejor manejo de errores y logging
3. **SEO**: El contenido se renderiza en el servidor
4. **Simplicidad**: Menos complejidad en el frontend

**Implementación**:
- HttpClient inyectado mediante Dependency Injection
- Servicio dedicado (`IndicadoresApiService`) para encapsular la lógica
- Async/await para operaciones asíncronas eficientes

### Validaciones en Dos Niveles

**Cliente (MVC)**:
- Validaciones básicas con Data Annotations
- Feedback inmediato al usuario
- Reducción de peticiones inválidas

**Servidor (API)**:
- Validaciones completas y lógica de negocio
- Fuente única de verdad
- Seguridad reforzada

### Diseño UI/UX

- **Bootstrap 5**: Framework moderno y responsive
- **Bootstrap Icons**: Iconografía consistente
- **Colores semánticos**: Verde (crear), Amarillo (editar), Rojo (eliminar)
- **Mensajes de feedback**: Alerts para éxito, error e información

---

## ?? Documentación de Código

El código incluye:
- **Comentarios XML** en clases y métodos públicos
- **Summaries** descriptivos para IntelliSense
- **Nombres descriptivos** siguiendo convenciones C#
- **Separación de concerns** clara

Ejemplo:
```csharp
/// <summary>
/// Servicio para consumir el API REST de Indicadores
/// Implementa las operaciones CRUD reutilizando el backend existente
/// </summary>
public class IndicadoresApiService
{
    /// <summary>
    /// Obtiene todos los indicadores del API REST
    /// </summary>
    public async Task<List<IndicadorDTO>> ObtenerTodosAsync()
    {
        // ...
    }
}
```

---

## ?? Cumplimiento de los Criterios de Evaluación RA9

| Criterio | Cumplimiento | Evidencia |
|----------|--------------|-----------|
| **9a** - Reconocimiento de ventajas de reutilización | ? | Documentación de ventajas, separación cliente-servidor |
| **9b** - Identificación de tecnologías y frameworks | ? | ASP.NET Core MVC, HttpClient, Bootstrap 5 |
| **9c** - Creación de aplicación que recupera repositorios existentes | ? | Consumo completo del API REST de indicadores |
| **9d** - Creación de repositorios desde información existente | ? | DTOs basados en la estructura del API |
| **9e** - Uso de librerías y frameworks | ? | Bootstrap, jQuery Validate, System.Text.Json |
| **9f** - Programación usando información de terceros | ? | Consumo del API REST sin acceso a BD |
| **9g** - Análisis de librerías relacionadas con datos | ? | HttpClient, JSON serialization, DTOs |
| **9h** - Pruebas, depuración y documentación | ? | Documentación completa, manejo de errores, logging |

---

## ?? Configuración Adicional

### Cambiar la URL del API

Editar `appsettings.json`:
```json
{
  "ApiSettings": {
    "BaseUrl": "https://tu-servidor:puerto/api/Indicadores"
  }
}
```

### Ajustar Timeouts

Modificar en `Program.cs`:
```csharp
builder.Services.AddHttpClient<IndicadoresApiService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60); // Aumentar timeout
});
```

---

## ?? Solución de Problemas

### Error: "No se pudo conectar con el API REST"

**Causa**: El API REST no está en ejecución o la URL es incorrecta

**Solución**:
1. Verificar que el API esté corriendo en `https://localhost:7093`
2. Comprobar la configuración en `appsettings.json`
3. Revisar los logs de la aplicación

### Error: "SSL Certificate"

**Causa**: Certificado SSL autofirmado no confiable

**Solución**: En desarrollo, la aplicación ya acepta certificados autofirmados (configurado en `Program.cs`)

### Error: "ModelState invalid"

**Causa**: Validaciones del formulario no superadas

**Solución**: Revisar los campos del formulario y completar según las validaciones requeridas

---

## ?? Autor

**Actividad Evaluable RA9 - Amelia**  
EcoData Solutions S.L.  
Desarrollo Web en Entorno Servidor

---

## ?? Licencia

Este proyecto es de uso educativo para la evaluación del RA9 del módulo de Desarrollo Web en Entorno Servidor.

---

## ?? Soporte

Para cualquier duda o problema:
1. Revisar esta documentación
2. Consultar los comentarios en el código
3. Verificar los logs de la aplicación
4. Contactar con el responsable del módulo

---

**Última actualización**: 2024

**Versión**: 1.0.0
