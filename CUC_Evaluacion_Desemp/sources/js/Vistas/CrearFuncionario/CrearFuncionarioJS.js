//Evita que no se dejen conglomerados en blanco
document.querySelector("form").addEventListener("submit", function (e) {
    const checkboxes = document.querySelectorAll('input[name="IdConglomeradosSeleccionados"]:checked');
    const error = document.getElementById('checkboxErrorConglomerado');
    if (checkboxes.length === 0) {
        e.preventDefault();
        error.style.display = 'block';
    } else {
        error.style.display = 'none';
    }
});

//Evita que no se dejen conglomerados en blanco
document.querySelector("form").addEventListener("submit", function (e) {
    const checkboxes = document.querySelectorAll('input[name="IdAreasSeleccionadas"]:checked');
    const error = document.getElementById('checkboxErrorArea');
    if (checkboxes.length === 0) {
        e.preventDefault();
        error.style.display = 'block';
    } else {
        error.style.display = 'none';
    }
});