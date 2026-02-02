# Script para probar la actualización de un indicador directamente en el API
# Esto ayuda a identificar si el problema está en el API o en la aplicación cliente

$apiUrl = "https://localhost:7093/api/Indicadores"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Prueba de Actualización de Indicador" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configurar para ignorar certificados SSL en desarrollo
if (-not ([System.Management.Automation.PSTypeName]'ServerCertificateValidationCallback').Type) {
    $certCallback = @"
        using System;
        using System.Net;
        using System.Net.Security;
        using System.Security.Cryptography.X509Certificates;
        public class ServerCertificateValidationCallback
        {
            public static void Ignore()
            {
                if(ServicePointManager.ServerCertificateValidationCallback ==null)
                {
                    ServicePointManager.ServerCertificateValidationCallback += 
                        delegate
                        (
                            Object obj, 
                            X509Certificate certificate, 
                            X509Chain chain, 
                            SslPolicyErrors errors
                        )
                        {
                            return true;
                        };
                }
            }
        }
"@
    Add-Type $certCallback
}
[ServerCertificateValidationCallback]::Ignore()

# Paso 1: Obtener un indicador existente
Write-Host "Paso 1: Obteniendo lista de indicadores..." -ForegroundColor Yellow
try {
    $indicadores = Invoke-RestMethod -Uri $apiUrl -Method Get
    
    if ($indicadores.Count -eq 0) {
        Write-Host "? No hay indicadores en el API para probar la actualización" -ForegroundColor Red
        Write-Host "   Crea primero un indicador usando ProbarCrearIndicador.ps1" -ForegroundColor Yellow
        exit
    }
    
    Write-Host "? Se encontraron $($indicadores.Count) indicadores" -ForegroundColor Green
    Write-Host ""
    
    # Usar el primer indicador
    $indicadorOriginal = $indicadores[0]
    $idActualizar = $indicadorOriginal.id
    
    Write-Host "Indicador seleccionado para actualizar:" -ForegroundColor Cyan
    Write-Host "  ID: $($indicadorOriginal.id)" -ForegroundColor White
    Write-Host "  Nombre: $($indicadorOriginal.nombre)" -ForegroundColor White
    Write-Host "  Valor: $($indicadorOriginal.valor)" -ForegroundColor White
    Write-Host ""
}
catch {
    Write-Host "? Error al obtener indicadores del API" -ForegroundColor Red
    Write-Host "   Asegúrate de que el API esté ejecutándose" -ForegroundColor Yellow
    exit
}

# Paso 2: Preparar datos actualizados
Write-Host "Paso 2: Preparando datos actualizados..." -ForegroundColor Yellow

# IMPORTANTE: El campo "valor" debe ser STRING
$valorNuevo = [double]$indicadorOriginal.valor + 10.5

$indicadorActualizado = @{
    id = $indicadorOriginal.id
    tipo = $indicadorOriginal.tipo
    ambito = $indicadorOriginal.ambito
    categoria = $indicadorOriginal.categoria
    nombre = $indicadorOriginal.nombre + " (Actualizado)"
    descripcion = $indicadorOriginal.descripcion + " - Actualizado desde PowerShell"
    valor = $valorNuevo.ToString("0.##")  # ? Convertir a STRING
    unidad = $indicadorOriginal.unidad
    fecha = $indicadorOriginal.fecha
    fuente = "Script de Prueba PowerShell - Actualización"
}

Write-Host ""
Write-Host "Datos que se enviarán:" -ForegroundColor Cyan
$indicadorActualizado | Format-List

Write-Host ""
Write-Host "NOTA: El campo 'valor' se envía como STRING: ""$($indicadorActualizado.valor)""" -ForegroundColor Yellow
Write-Host ""

# Paso 3: Actualizar el indicador
Write-Host "Paso 3: Enviando solicitud PUT a: $apiUrl/$idActualizar" -ForegroundColor Yellow
Write-Host ""

try {
    # Convertir a JSON
    $jsonBody = $indicadorActualizado | ConvertTo-Json -Depth 5
    Write-Host "JSON que se enviará:" -ForegroundColor Cyan
    Write-Host $jsonBody -ForegroundColor Gray
    Write-Host ""

    # Realizar la solicitud PUT
    $response = Invoke-WebRequest -Uri "$apiUrl/$idActualizar" `
        -Method Put `
        -Body $jsonBody `
        -ContentType "application/json" `
        -UseBasicParsing

    if ($response.StatusCode -eq 200 -or $response.StatusCode -eq 204) {
        Write-Host "? ¡INDICADOR ACTUALIZADO EXITOSAMENTE!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Código de respuesta: $($response.StatusCode)" -ForegroundColor Green
        Write-Host ""
        
        # Verificar la actualización
        Write-Host "Verificando actualización..." -ForegroundColor Yellow
        $indicadorVerificado = Invoke-RestMethod -Uri "$apiUrl/$idActualizar" -Method Get
        
        Write-Host ""
        Write-Host "Estado actual del indicador:" -ForegroundColor Cyan
        $indicadorVerificado | Format-List
        
        Write-Host ""
        Write-Host "? El API funciona correctamente para actualización." -ForegroundColor Green
        Write-Host "   Si tu aplicación falla, el problema está en cómo se envían los datos." -ForegroundColor Yellow
    }
    else {
        Write-Host "??  Respuesta inesperada del servidor" -ForegroundColor Yellow
        Write-Host "Código: $($response.StatusCode)" -ForegroundColor Yellow
        Write-Host "Contenido: $($response.Content)" -ForegroundColor Gray
    }
}
catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    $statusDescription = $_.Exception.Response.StatusDescription
    
    Write-Host "? ERROR AL ACTUALIZAR INDICADOR" -ForegroundColor Red
    Write-Host ""
    Write-Host "Código de error: $statusCode - $statusDescription" -ForegroundColor Red
    Write-Host ""
    
    # Intentar obtener el contenido del error
    try {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $errorContent = $reader.ReadToEnd()
        
        Write-Host "Respuesta del servidor:" -ForegroundColor Yellow
        Write-Host $errorContent -ForegroundColor Gray
        Write-Host ""
        
        # Analizar errores comunes
        if ($statusCode -eq 400) {
            Write-Host "?? Error 400 - Bad Request" -ForegroundColor Yellow
            Write-Host "   Posibles causas:" -ForegroundColor White
            Write-Host "   - El formato de los datos no coincide con lo que espera el API" -ForegroundColor White
            Write-Host "   - El ID en la URL no coincide con el ID en el body" -ForegroundColor White
            Write-Host "   - Tipos de datos incorrectos" -ForegroundColor White
            Write-Host "   - El formato de fecha no es aceptado" -ForegroundColor White
        }
        elseif ($statusCode -eq 404) {
            Write-Host "?? Error 404 - Not Found" -ForegroundColor Yellow
            Write-Host "   El indicador con ID $idActualizar no existe en el API" -ForegroundColor White
        }
        elseif ($statusCode -eq 415) {
            Write-Host "?? Error 415 - Unsupported Media Type" -ForegroundColor Yellow
            Write-Host "   El API no acepta application/json" -ForegroundColor White
        }
        elseif ($statusCode -eq 500) {
            Write-Host "?? Error 500 - Internal Server Error" -ForegroundColor Yellow
            Write-Host "   Error en el servidor del API" -ForegroundColor White
            Write-Host "   Revisa los logs del API backend" -ForegroundColor White
        }
    }
    catch {
        Write-Host "Error general: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "SOLUCIÓN:" -ForegroundColor Yellow
    Write-Host "1. Verifica que el API esté ejecutándose correctamente" -ForegroundColor White
    Write-Host "2. Revisa los logs del API backend para más detalles" -ForegroundColor White
    Write-Host "3. Compara el JSON enviado con lo que espera el API" -ForegroundColor White
    Write-Host "4. Verifica que el indicador con ID $idActualizar exista" -ForegroundColor White
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Presiona cualquier tecla para cerrar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
