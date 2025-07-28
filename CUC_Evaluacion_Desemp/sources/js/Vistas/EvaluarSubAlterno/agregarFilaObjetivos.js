function agregarFilaObjetivos() {
    const selectObj = document.getElementById("selectObjetivo");
    const inputPesoObj = document.getElementById("inputPesoObj");
    const inputMetaObj = document.getElementById("inputMetaObj");

    const valorSeleccionado = selectObj.value;
    if (!valorSeleccionado) {
        alert("Seleccione un objetivo.");
        return;
    }

    // Extraer los valores
    const [idObj, nombreObj, tipoObj, idTipoObj] = valorSeleccionado.split("|");

    const pesoIngresado = inputPesoObj.value;
    if (!pesoIngresado) {
        alert("Ingrese un peso para el objetivo.");
        return;
    }
    if (pesoIngresado < 1) {
        alert("El peso minimo debe ser 1.");
        return;
    }

    const metaIngresada = inputMetaObj.value;
    if (!metaIngresada) {
        alert("Ingrese una meta para el objetivo.");
        return;
    }


    const tbody = document.querySelector("#tablaObjetivos tbody");
    const nuevaFila = document.createElement("tr");

    // Celda de id
    const tdID = document.createElement("td");
    tdID.innerText = idObj;
    // Celda de Objetivo
    const tdObjetivo = document.createElement("td");
    tdObjetivo.innerText = nombreObj;

    // Celda de Tipo (Ahora mostrará el IdTipoObjetivo en lugar de "tipoObj")
    const tdTipo = document.createElement("td");
    tdTipo.innerText = idTipoObj;

    // Celda de Peso
    const tdPeso = document.createElement("td");
    tdPeso.innerText = pesoIngresado + "%";

    // Celda de Meta
    const tdMeta = document.createElement("td");
    tdMeta.innerText = metaIngresada;

    // Celda Actual fijo a "0%"
    const tdActual = document.createElement("td");
    tdActual.innerText = "0%";

    // Celda de Acciones
    const tdAcciones = document.createElement("td");
    const btnEliminar = document.createElement("button");
    btnEliminar.classList.add("btn", "btn-sm", "btn-danger");
    btnEliminar.innerText = "Eliminar";
    btnEliminar.onclick = function () {
        tbody.removeChild(nuevaFila);
    };
    tdAcciones.appendChild(btnEliminar);

    // Agregar celdas a la fila
    nuevaFila.appendChild(tdID);
    nuevaFila.appendChild(tdObjetivo);
    nuevaFila.appendChild(tdTipo); // Se mostrará el IdTipoObjetivo
    nuevaFila.appendChild(tdPeso);
    nuevaFila.appendChild(tdMeta);
    nuevaFila.appendChild(tdActual);
    nuevaFila.appendChild(tdAcciones);

    // Insertar la fila en la tabla
    tbody.appendChild(nuevaFila);

    // Actualizar sumas por tipo
    actualizarSumasPorTipo();

    // Limpiar los inputs
    inputPesoObj.value = "";
    inputMetaObj.value = "";
}//fin agregarFilaObjetivos