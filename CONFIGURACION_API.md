# Configuración para Conectarse al API de Indicadores

## ? Correcciones Aplicadas

Se han realizado las siguientes correcciones para que la aplicación funcione correctamente con la API:

1. **Eliminados los datos de prueba en memoria** - La aplicación ahora solo usa la API real
2. **Convertidor JSON tolerante** - Maneja correctamente valores no numéricos sin fallar
3. **Ignorar campos extra** - El deserializador ignora campos del API que no existen en el DTO
4. **Manejo robusto de errores** - Mensajes claros cuando la API no está disponible

## ?? Configuración Actual

### URL del API
La aplicación está configurada para conectarse a:
```
https://localhost:7093/api/Indicadores
```

Esta configuración se encuentra en `appsettings.json`:
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7093/api/Indicadores"
  }
}
```

## ?? Pasos para Ejecutar la Aplicación

### 1. Asegúrate de que el API Backend esté ejecutándose

El API debe estar corriendo en `https://localhost:7093` antes de iniciar esta aplicación.

**Pasos:**
1. Abre el proyecto del API en otra instancia de Visual Studio
2. Presiona F5 para ejecutar el API
3. Verifica que el API esté respondiendo en: `https://localhost:7093/api/Indicadores`
4. Puedes probar el endpoint en el navegador o con Swagger

### 2. Ejecuta esta aplicación MVC

Una vez que el API esté corriendo:
1. Presiona F5 en este proyecto
2. Navega a la sección "Indicadores"
3. Deberías ver los indicadores reales del API

## ?? Verificación

### Comprobar que el API está corriendo:
- Abre un navegador y ve a: `https://localhost:7093/api/Indicadores`
- Deberías ver un JSON con la lista de indicadores

### Si el API no está disponible:
La aplicación mostrará un error claro:
```
No se pudo conectar con el API en https://localhost:7093/api/Indicadores. 
Verifica que el servicio esté en ejecución.
```

## ??? Cambiar la URL del API

Si tu API está corriendo en un puerto diferente:

1. Edita `appsettings.json`
2. Cambia la URL en `ApiSettings.BaseUrl`
3. Ejemplo para otro puerto:
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:5001/api/Indicadores"
  }
}
```

## ?? Estructura del DTO

La aplicación espera que el API devuelva indicadores con esta estructura mínima:
```json
{
  "id": 1,
  "nombre": "Nombre del Indicador",
  "descripcion": "Descripción",
  "valor": "123.45" // Puede ser string o número
  "unidad": "unidad",
  "fecha": "2024-01-15T00:00:00",
  "fuente": "Fuente de datos"
}
```

**Importante:** 
- El API puede devolver campos adicionales (tipo, categoria, etc.) que serán ignorados automáticamente
- El campo `valor` puede venir como string ("123.45") o como número (123.45)
- Si `valor` contiene texto no numérico, se convertirá a 0 automáticamente

## ? Solución de Problemas

### Error: "No se pudo conectar con el API"
? Verifica que el API backend esté ejecutándose
? Revisa que el puerto en `appsettings.json` sea correcto
? Asegúrate de que no haya firewall bloqueando la conexión

### Error: "Error al procesar la respuesta del API"
? Verifica que el API devuelva JSON válido
? Los campos adicionales del API se ignoran automáticamente
? Revisa los logs para ver el JSON completo que está recibiendo

### Los indicadores no se muestran
? Abre las herramientas de desarrollo del navegador (F12)
? Ve a la pestaña "Network" y busca la llamada al API
? Revisa la consola de Visual Studio para ver los logs

## ?? Estado Actual

- ? Convertidor JSON funcional para campos decimales
- ? Ignora campos extras del API
- ? Sin datos de prueba en memoria
- ? Conexión directa al API real
- ? Manejo robusto de errores
- ? Mensajes claros al usuario

**La aplicación está lista para consumir los indicadores reales del API.** ??
