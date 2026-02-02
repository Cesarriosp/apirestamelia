# Script para probar la creación de un indicador directamente en el API
# Esto ayuda a identificar si el problema está en el API o en la aplicación cliente

$apiUrl = "https://localhost:7093/api/Indicadores"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Prueba de Creación de Indicador" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Datos de prueba para el nuevo indicador
# IMPORTANTE: El campo "valor" debe ser STRING, no número
$nuevoIndicador = @{
    tipo = "Digitalización"
    ambito = "Empresarial"
    categoria = "Procesos"
    nombre = "Indicador de Prueba Script"
    descripcion = "Este indicador fue creado desde PowerShell para probar el API"
    valor = "150.75"  # ? STRING (con comillas)
    unidad = "unidades de prueba"
    fecha = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    fuente = "Script de Prueba PowerShell"
}

Write-Host "Datos del nuevo indicador:" -ForegroundColor Yellow
$nuevoIndicador | Format-List
Write-Host ""

Write-Host "NOTA IMPORTANTE:" -ForegroundColor Yellow
Write-Host "El campo 'valor' se envía como STRING: ""$($nuevoIndicador.valor)""" -ForegroundColor Yellow
Write-Host "Esto es requerido por el API backend." -ForegroundColor Yellow
Write-Host ""

try {
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

    Write-Host "Enviando solicitud POST a: $apiUrl" -ForegroundColor Yellow
    Write-Host ""

    # Convertir a JSON
    $jsonBody = $nuevoIndicador | ConvertTo-Json -Depth 5
    Write-Host "JSON que se enviará:" -ForegroundColor Cyan
    Write-Host $jsonBody -ForegroundColor Gray
    Write-Host ""
    
    Write-Host "Verificando que 'valor' es string en el JSON:" -ForegroundColor Yellow
    if ($jsonBody -match '"valor":\s*"') {
        Write-Host "? Correcto: 'valor' es un string" -ForegroundColor Green
    } else {
        Write-Host "? Error: 'valor' NO es un string en el JSON" -ForegroundColor Red
        Write-Host "   Esto causará un error 400 en el API" -ForegroundColor Red
    }
    Write-Host ""

    # Realizar la solicitud POST
    $response = Invoke-WebRequest -Uri $apiUrl `
        -Method Post `
        -Body $jsonBody `
        -ContentType "application/json" `
        -UseBasicParsing

    if ($response.StatusCode -eq 200 -or $response.StatusCode -eq 201) {
        Write-Host "? ¡INDICADOR CREADO EXITOSAMENTE!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Código de respuesta: $($response.StatusCode)" -ForegroundColor Green
        Write-Host ""
        
        try {
            $resultado = $response.Content | ConvertFrom-Json
            Write-Host "Indicador creado:" -ForegroundColor Cyan
            $resultado | Format-List
            
            Write-Host ""
            Write-Host "? El API funciona correctamente para creación." -ForegroundColor Green
            Write-Host "   La aplicación MVC ahora también debe funcionar." -ForegroundColor Yellow
        }
        catch {
            Write-Host "Respuesta del servidor:" -ForegroundColor Cyan
            Write-Host $response.Content -ForegroundColor Gray
        }
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
    
    Write-Host "? ERROR AL CREAR INDICADOR" -ForegroundColor Red
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
            
            if ($errorContent -match "valor.*string") {
                Write-Host ""
                Write-Host "   ??  CAUSA IDENTIFICADA:" -ForegroundColor Red
                Write-Host "   El API espera 'valor' como STRING, no como número" -ForegroundColor White
                Write-Host ""
                Write-Host "   Solución aplicada en la aplicación MVC:" -ForegroundColor Green
                Write-Host "   - El DTO ahora convierte automáticamente el valor a string" -ForegroundColor White
                Write-Host "   - Se envía como: ""123.45"" en lugar de: 123.45" -ForegroundColor White
            }
            else {
                Write-Host "   Posibles causas:" -ForegroundColor White
                Write-Host "   - El formato de los datos no coincide con lo que espera el API" -ForegroundColor White
                Write-Host "   - Faltan campos requeridos" -ForegroundColor White
                Write-Host "   - Tipos de datos incorrectos" -ForegroundColor White
            }
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
    Write-Host "2. Asegúrate de enviar 'valor' como STRING" -ForegroundColor White
    Write-Host "3. La aplicación MVC ya está corregida para enviar strings" -ForegroundColor White
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Intentar listar los indicadores para verificar si fue creado
Write-Host "Listando indicadores actuales en el API..." -ForegroundColor Yellow
try {
    $indicadores = Invoke-RestMethod -Uri $apiUrl -Method Get
    Write-Host "Total de indicadores: $($indicadores.Count)" -ForegroundColor Green
    
    if ($indicadores.Count -gt 0) {
        Write-Host ""
        Write-Host "Últimos 3 indicadores:" -ForegroundColor Cyan
        $indicadores | Select-Object -Last 3 | ForEach-Object {
            Write-Host "  - ID: $($_.id) | $($_.nombre) | Valor: $($_.valor)" -ForegroundColor White
        }
    }
}
catch {
    Write-Host "No se pudo obtener la lista de indicadores" -ForegroundColor Red
}

Write-Host ""
Write-Host "Presiona cualquier tecla para cerrar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
