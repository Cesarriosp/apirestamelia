// JavaScript personalizado para la aplicación EcoData Solutions

// Confirmación de eliminación mejorada
document.addEventListener('DOMContentLoaded', function () {
    // Auto-cerrar alertas después de 5 segundos
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });

    // Validación adicional en el cliente
    const forms = document.querySelectorAll('form');
    forms.forEach(function (form) {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });
});

// Función para formatear fechas
function formatDate(dateString) {
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    return new Date(dateString).toLocaleDateString('es-ES', options);
}

// Mostrar loader durante las peticiones
function showLoader() {
    // Implementar si se desea un indicador de carga
    console.log('Cargando...');
}

function hideLoader() {
    console.log('Carga completa');
}
