const API_URL = "https://localhost:7093/api/Indicadores";

async function getIndicadores() {
  const res = await fetch(API_URL);
  return res.json();
}

async function crearIndicador(data) {
  const res = await fetch(API_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data)
  });
  if (!res.ok) {
    const error = await res.text();
    console.error('Error al crear:', error);
    throw new Error(`Error ${res.status}: ${error}`);
  }
  return res.json();
}

async function actualizarIndicador(id, data) {
  console.log('Actualizando ID:', id);
  console.log('Datos a enviar:', data);
  const res = await fetch(`${API_URL}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data)
  });
  if (!res.ok) {
    const error = await res.text();
    console.error('Error al actualizar:', error);
    throw new Error(`Error ${res.status}: ${error}`);
  }
  // No intentar parsear JSON si no hay contenido
  return res.status === 204 ? {} : res.json();
}


async function eliminarIndicador(id) {
  return fetch(`${API_URL}/${id}`, { method: "DELETE" });
}
