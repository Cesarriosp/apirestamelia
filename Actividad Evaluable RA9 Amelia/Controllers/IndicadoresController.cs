using Microsoft.AspNetCore.Mvc;
using Actividad_Evaluable_RA9_Amelia.Models;
using Actividad_Evaluable_RA9_Amelia.Services;

namespace Actividad_Evaluable_RA9_Amelia.Controllers
{
    /// <summary>
    /// Controlador MVC para gestionar indicadores consumiendo el API REST
    /// No accede directamente a la base de datos, reutiliza el backend existente
    /// </summary>
    public class IndicadoresController : Controller
    {
        private readonly IndicadoresApiService _apiService;
        private readonly ILogger<IndicadoresController> _logger;

        public IndicadoresController(IndicadoresApiService apiService, ILogger<IndicadoresController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// GET: Indicadores - Lista todos los indicadores
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var indicadores = await _apiService.ObtenerTodosAsync();
                return View(indicadores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de indicadores");
                TempData["Error"] = "No se pudo conectar con el API REST. Por favor, verifica que el servicio esté en ejecución.";
                return View(new List<IndicadorDTO>());
            }
        }

        /// <summary>
        /// GET: Indicadores/Details/5 - Muestra los detalles de un indicador
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var indicador = await _apiService.ObtenerPorIdAsync(id);
                
                if (indicador == null)
                {
                    TempData["Error"] = "No se encontró el indicador solicitado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(indicador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los detalles del indicador {Id}", id);
                TempData["Error"] = "Error al obtener los detalles del indicador.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// GET: Indicadores/Create - Muestra el formulario de creación
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: Indicadores/Create - Crea un nuevo indicador
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IndicadorDTO indicador)
        {
            if (!ModelState.IsValid)
            {
                return View(indicador);
            }

            try
            {
                _logger.LogInformation("Intentando crear indicador: {Nombre}", indicador.Nombre);
                
                var resultado = await _apiService.CrearAsync(indicador);
                
                if (resultado != null)
                {
                    _logger.LogInformation("Indicador creado exitosamente con ID: {Id}", resultado.Id);
                    TempData["Success"] = "Indicador creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning("El API no devolvió un indicador después de la creación");
                    ModelState.AddModelError("", "El indicador fue creado pero no se pudo recuperar la información.");
                    return View(indicador);
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error de operación al crear el indicador");
                
                // Extraer mensaje de error más específico si existe
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $" Detalle: {ex.InnerException.Message}";
                }
                
                ModelState.AddModelError("", errorMessage);
                TempData["Error"] = errorMessage;
                return View(indicador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear el indicador");
                ModelState.AddModelError("", $"Error inesperado: {ex.Message}");
                TempData["Error"] = "Error al crear el indicador. Por favor, verifica los datos e intenta de nuevo.";
                return View(indicador);
            }
        }

        /// <summary>
        /// GET: Indicadores/Edit/5 - Muestra el formulario de edición
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var indicador = await _apiService.ObtenerPorIdAsync(id);
                
                if (indicador == null)
                {
                    TempData["Error"] = "No se encontró el indicador solicitado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(indicador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el indicador para editar {Id}", id);
                TempData["Error"] = "Error al obtener el indicador.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: Indicadores/Edit/5 - Actualiza un indicador existente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IndicadorDTO indicador)
        {
            if (id != indicador.Id)
            {
                TempData["Error"] = "El ID del indicador no coincide.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(indicador);
            }

            try
            {
                _logger.LogInformation("Intentando actualizar indicador ID {Id}: {Nombre}", id, indicador.Nombre);
                
                var resultado = await _apiService.ActualizarAsync(id, indicador);
                
                if (!resultado)
                {
                    _logger.LogWarning("Indicador con ID {Id} no encontrado al intentar actualizar", id);
                    TempData["Error"] = "No se encontró el indicador a actualizar.";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation("Indicador ID {Id} actualizado exitosamente", id);
                TempData["Success"] = "Indicador actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error de operación al actualizar el indicador ID {Id}", id);
                
                // Extraer mensaje de error más específico si existe
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $" Detalle: {ex.InnerException.Message}";
                }
                
                ModelState.AddModelError("", errorMessage);
                TempData["Error"] = errorMessage;
                return View(indicador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar el indicador ID {Id}", id);
                ModelState.AddModelError("", $"Error inesperado: {ex.Message}");
                TempData["Error"] = "Error al actualizar el indicador. Por favor, verifica los datos e intenta de nuevo.";
                return View(indicador);
            }
        }

        /// <summary>
        /// GET: Indicadores/Delete/5 - Muestra la confirmación de eliminación
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var indicador = await _apiService.ObtenerPorIdAsync(id);
                
                if (indicador == null)
                {
                    TempData["Error"] = "No se encontró el indicador solicitado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(indicador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el indicador para eliminar {Id}", id);
                TempData["Error"] = "Error al obtener el indicador.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: Indicadores/Delete/5 - Elimina un indicador
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var resultado = await _apiService.EliminarAsync(id);
                
                if (!resultado)
                {
                    TempData["Error"] = "No se encontró el indicador a eliminar.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Indicador eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el indicador {Id}", id);
                TempData["Error"] = "Error al eliminar el indicador. Por favor, intenta de nuevo.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
