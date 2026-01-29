const galeria = document.getElementById("galeria");

const imagenes = [
  "images/arbol.svg",
  "images/energia-renovable.svg",
  "images/reciclaje.svg",
  "images/planeta-tierra.svg"
];

imagenes.forEach(img => {
  galeria.innerHTML += `
    <div class="col-md-3 mb-3">
      <div class="card">
        <img src="${img}" class="card-img-top" alt="Imagen de galería ambiental">
      </div>
    </div>
  `;
});
