    document.addEventListener("DOMContentLoaded", function () {
        const inputFiltro = document.getElementById("filtroReportes");
    const filas = document.querySelectorAll("table tbody tr");

    inputFiltro.addEventListener("keyup", function () {
            const texto = this.value.toLowerCase();

            filas.forEach(fila => {
                const nombreArchivo = fila.querySelector("td:first-child").innerText.toLowerCase();

    if (nombreArchivo.includes(texto)) {
        fila.style.display = "";
                } else {
        fila.style.display = "none";
                }
            });
        });
    });

