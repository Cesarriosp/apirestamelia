# Script para verificar si el API de Indicadores está disponible
# Ejecuta este script antes de iniciar la aplicación MVC

$apiUrl = "https://localhost:7093/api/Indicadores"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Verificación del API de Indicadores" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Comprobando conexión con: $apiUrl" -ForegroundColor Yellow
Write-Host ""

try {
    # Ignorar errores de certificado SSL en desarrollo
    add-type @"
        using System.Net;
        using System.Security.Cryptography.X509Certificates;
        public class TrustAllCertsPolicy : ICertificatePolicy {
            public bool CheckValidationResult(
                ServicePoint srvPoint, X509Certificate certificate,
                WebRequest request, int certificateProblem) {
                return true;
            }
        }
"@
    [System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

    $response = Invoke-WebRequest -Uri $apiUrl -Method Get -UseBasicParsing -TimeoutSec 5
    
    if ($response.StatusCode -eq 200) {
        Write-Host "? ¡API DISPONIBLE!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Código de estado: $($response.StatusCode)" -ForegroundColor Green
        Write-Host ""
        
        # Intentar parsear el JSON
        try {
            $indicadores = $response.Content | ConvertFrom-Json
            Write-Host "Número de indicadores encontrados: $($indicadores.Count)" -ForegroundColor Green
            Write-Host ""
            
            if ($indicadores.Count -gt 0) {
                Write-Host "Primeros indicadores encontrados:" -ForegroundColor Cyan
                $indicadores | Select-Object -First 3 | ForEach-Object {
                    Write-Host "  - ID: $($_.id) | Nombre: $($_.nombre)" -ForegroundColor White
                }
            }
            
            Write-Host ""
            Write-Host "? La aplicación MVC está lista para ejecutarse." -ForegroundColor Green
            Write-Host "   Presiona F5 en Visual Studio para iniciar." -ForegroundColor Yellow
        }
        catch {
            Write-Host "??  Advertencia: No se pudo parsear la respuesta como JSON" -ForegroundColor Yellow
            Write-Host "   Respuesta recibida:" -ForegroundColor Gray
            Write-Host "   $($response.Content.Substring(0, [Math]::Min(500, $response.Content.Length)))" -ForegroundColor Gray
        }
    }
}
catch {
    Write-Host "? API NO DISPONIBLE" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "SOLUCIÓN:" -ForegroundColor Yellow
    Write-Host "1. Abre el proyecto del API en Visual Studio" -ForegroundColor White
    Write-Host "2. Presiona F5 para ejecutar el API" -ForegroundColor White
    Write-Host "3. Espera a que el API esté listo (verás un mensaje en la consola)" -ForegroundColor White
    Write-Host "4. Ejecuta este script nuevamente para verificar" -ForegroundColor White
    Write-Host "5. Si el API usa un puerto diferente, edita appsettings.json" -ForegroundColor White
    Write-Host ""
    Write-Host "??  NO EJECUTES la aplicación MVC hasta que el API esté disponible." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Presiona cualquier tecla para cerrar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
