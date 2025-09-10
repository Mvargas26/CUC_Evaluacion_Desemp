//*******************************************************************************************************************************
//***********************************  Funcion Principal                    *****************************************************
//*******************************************************************************************************************************

$(document).ready(function () {
    inicializarEventos();
});

function inicializarEventos() {
    functionModalObjetivos();
}

//*******************************************************************************************************************************
//***********************************  Seccion Objetivos                    *****************************************************
function functionModalObjetivos() {
    // Evento para abrir el modal con los datos del botón
    $(document).on("click", ".btn-editar-actual", function () {
        let id = $(this).data("id");
        let tipo = $(this).data("tipo");
        let nombre = $(this).data("nombre");
        let peso = $(this).data("peso");
        let actual = $(this).data("actual");
        let meta = $(this).data("meta");

        $("#modalTipoObjetivo").text(tipo);
        $("#modalNombreObjetivo").text(nombre);
        $("#modalPesoObjetivo").text(peso + "%");
        $("#modalMetaObjetivo").text(meta);
        $("#modalValorActual").val(actual);
        $("#maxPeso").text(peso);

        // Guardar el ID del objetivo en el botón confirmar
        $("#modalBtnConfirmar").data("id", id);

        $("#editarActualModal").modal("show");
    });

    // Evento para confirmar el cambio
    $("#modalBtnConfirmar").off("click").on("click", function () {
        let id = $(this).data("id");
        let nuevoValor = parseInt($("#modalValorActual").val());
        let max = parseInt($("#maxPeso").text());

        if (isNaN(nuevoValor) || nuevoValor < 1 || nuevoValor > max) {
            $("#valorError").removeClass("d-none");
            return;
        }

        $("#valorError").addClass("d-none");

        // Buscar la fila correspondiente en la tabla y actualizar valor
        let fila = $("#tbObjetivosAsignados").find(`button[data-id='${id}']`).closest("tr");
        fila.find("td:eq(4)").html(nuevoValor + "<span>%</span>");

        // Actualizar también el atributo data del botón
        fila.find(".btn-editar-actual").data("actual", nuevoValor);

        //llamamos la funcion que actualiza la tb resultados
        actualizarObjetivosEnTbResultados();

        let modal = bootstrap.Modal.getInstance(document.getElementById("editarActualModal"));
        modal.hide();
    });
}

// Función para cerrar el modal desde el botón de cerrar
function cerrarModal() {
    let modal = bootstrap.Modal.getInstance(document.getElementById("editarActualModal"));
    modal.hide();
    $("#valorError").addClass("d-none");
    document.activeElement.blur();
}
//*******************************************************************************************************************************
//***********************************  Seccion Tabla Resultados             *****************************************************
function actualizarObjetivosEnTbResultados() {
    let acumulados = {};

    // Recorremos la tabla de objetivos asignados
    $("#tbObjetivosAsignados tbody tr").each(function () {
        let tipo = $(this).find(".btn-editar-actual").data("tipo");
        let actualTxt = $(this).find("td:eq(4)").text().replace("%", "").trim();
        let actual = parseInt(actualTxt) || 0;

        if (!acumulados[tipo]) {
            acumulados[tipo] = 0;
        }
        acumulados[tipo] += actual;
    });

    // Recorremos la tabla de resultados y actualiza los inputs
    $("#tablaResultados tbody tr").each(function () {
        let tipoTexto = $(this).find("td:first").text().trim();
        let input = $(this).find(".input-calificacion");

        if (acumulados[tipoTexto] !== undefined) {
            input.val(acumulados[tipoTexto]);
        }
    });

    // Calculamos el total final
    let total = 0;
    $("#tablaResultados .input-calificacion").each(function () {
        let val = parseInt($(this).val()) || 0;
        total += val;
    });
    $("#resultado-total").val(total);
}//fin 
