# ?? Guía Rápida de Ejecución

## Pasos para ejecutar la aplicación

### 1?? Verificar que el API REST esté corriendo

**IMPORTANTE**: Antes de ejecutar esta aplicación, debes tener el API REST de indicadores corriendo en:

```
https://localhost:7093/api/Indicadores
```

### 2?? Abrir el proyecto

- Abre la solución en Visual Studio 2022 o superior
- O navega a la carpeta del proyecto desde la terminal

### 3?? Ejecutar la aplicación

**Opción A - Visual Studio**:
- Presiona `F5` o haz clic en el botón "Start" (??)

**Opción B - Línea de comandos**:
```bash
cd "Actividad Evaluable RA9 Amelia"
dotnet run
```

### 4?? Acceder a la aplicación

La aplicación se abrirá automáticamente en tu navegador predeterminado. Si no lo hace, accede manualmente a:

```
https://localhost:[puerto]
```

(El puerto se mostrará en la consola al ejecutar)

---

## ?? Funcionalidades Principales

Una vez que la aplicación esté corriendo:

### Ver Lista de Indicadores
- La página principal muestra todos los indicadores del API REST
- Puedes ver los detalles de cada indicador

### Crear Nuevo Indicador
1. Haz clic en "Crear Nuevo Indicador"
2. Completa el formulario con los datos requeridos:
   - **Nombre**: ej. "Emisiones de CO2"
   - **Descripción**: ej. "Cantidad de CO2 emitido por actividades industriales"
   - **Valor**: ej. 1250.75
   - **Unidad**: ej. "toneladas"
   - **Fecha**: Selecciona una fecha
   - **Fuente** (opcional): ej. "Instituto Nacional de Estadística"
3. Haz clic en "Crear Indicador"

### Editar Indicador
1. Desde la lista, haz clic en "Editar" en el indicador deseado
2. Modifica los campos necesarios
3. Haz clic en "Guardar Cambios"

### Ver Detalles
1. Desde la lista, haz clic en "Ver" en el indicador deseado
2. Se mostrarán todos los detalles del indicador

### Eliminar Indicador
1. Desde la lista, haz clic en "Eliminar" en el indicador deseado
2. Confirma la eliminación en la pantalla de confirmación
3. El indicador será eliminado permanentemente

---

## ?? Solución de Problemas Comunes

### Problema: "No se pudo conectar con el API REST"

**Solución**:
1. Verifica que el API REST esté corriendo en `https://localhost:7093`
2. Comprueba que el API responda correctamente accediendo a:
   ```
   https://localhost:7093/api/Indicadores
   ```
3. Si el puerto es diferente, actualiza la configuración en `appsettings.json`:
   ```json
   {
     "ApiSettings": {
       "BaseUrl": "https://localhost:TU_PUERTO/api/Indicadores"
     }
   }
   ```

### Problema: Error de certificado SSL

**Solución**: En desarrollo, la aplicación ya está configurada para aceptar certificados SSL autofirmados. Si persiste el problema, ejecuta:

```bash
dotnet dev-certs https --trust
```

### Problema: Puerto ya en uso

**Solución**: 
1. Cambia el puerto en `Properties\launchSettings.json`
2. O detén la aplicación que esté usando el puerto

---

## ?? Checklist Antes de Ejecutar

- [ ] ? .NET 8 SDK instalado
- [ ] ? API REST corriendo en `https://localhost:7093`
- [ ] ? Proyecto compilado sin errores (`dotnet build`)
- [ ] ? Navegador web disponible

---

## ?? Características de la Aplicación

### ? Ventajas de esta arquitectura:
- **Reutilización total**: No duplica lógica del API REST
- **Sin acceso a BD**: Todo a través del API REST
- **Validaciones dobles**: Cliente (UX) + Servidor (Seguridad)
- **Arquitectura escalable**: Separación de responsabilidades clara
- **Interfaz moderna**: Bootstrap 5 responsive

### ?? Tecnologías utilizadas:
- ASP.NET Core 8.0 MVC
- HttpClient para consumo de API REST
- Bootstrap 5 para diseño responsive
- jQuery Validate para validaciones
- System.Text.Json para serialización

---

## ?? Documentación Completa

Para más detalles sobre la arquitectura, diseño y decisiones técnicas, consulta el archivo:

```
README.md
```

---

## ?? Objetivos Cumplidos (RA9)

? Reutilización de código e información existente  
? Identificación y uso de frameworks web (ASP.NET Core MVC)  
? Recuperación y procesamiento de repositorios existentes (API REST)  
? Uso de librerías y frameworks específicos (HttpClient, Bootstrap)  
? Programación basada en servicios de terceros  
? Pruebas, depuración y documentación completa  

---

**¡Listo para usar!** ??

Si tienes algún problema, revisa la sección de solución de problemas o consulta la documentación completa.
