// --- CREAR PESO X CONGLOMERADO ---
document.addEventListener("DOMContentLoaded", function () {

    var selectorCrear = document.getElementById("tipoRegistro");

    if (selectorCrear) {
        selectorCrear.addEventListener("change", function () {

            var tipo = this.value;

            var divObj = document.getElementById("divObjetivos");
            var divComp = document.getElementById("divCompetencias");

            divObj.classList.add("d-none");
            divComp.classList.add("d-none");

            divObj.querySelector("select").value = "";
            divComp.querySelector("select").value = "";

            if (tipo === "objetivo") {
                divObj.classList.remove("d-none");
            }
            else if (tipo === "competencia") {
                divComp.classList.remove("d-none");
            }
        });
    }
});

// --- EDITAR PESO X CONGLOMERADO ---
document.addEventListener("DOMContentLoaded", function () {

    var selectoresEditar = document.querySelectorAll(".tipoRegistroEditar");

    selectoresEditar.forEach(function (selector) {

        selector.addEventListener("change", function () {

            var id = this.getAttribute("data-id");

            var divObj = document.getElementById("divObjetivos-" + id);
            var divComp = document.getElementById("divCompetencias-" + id);

            divObj.classList.add("d-none");
            divComp.classList.add("d-none");

            divObj.querySelector("select").value = "";
            divComp.querySelector("select").value = "";

            if (this.value === "objetivo") {
                divObj.classList.remove("d-none");
            }
            else if (this.value === "competencia") {
                divComp.classList.remove("d-none");
            }
        });
    });

});

// --- SUMA AUTOMÁTICA DE PORCENTAJES ---
document.addEventListener("DOMContentLoaded", function () {

    function sumarPorcentajes() {
        var celdas = document.querySelectorAll(".col-porcentaje");
        var suma = 0;

        celdas.forEach(function (celda) {
            var valor = parseFloat(celda.textContent);
            if (!isNaN(valor)) {
                suma += valor;
            }
        });

        var total = document.getElementById("sumaTotalPorcentaje");
        if (total) {
            total.textContent = suma.toFixed(2) + "%";
        }
    }

    sumarPorcentajes();
});
