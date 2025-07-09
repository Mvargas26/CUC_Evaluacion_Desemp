let columnasUsadas = [];

function pasarAPrevizualizacion() {
    const columnas = nombresNiveles;

    const comportamientoSeleccionado = document.getElementById("selectComportamiento");
    const partes = comportamientoSeleccionado.value.split("|");
    const idComport = partes[0];
    const nombreComport = partes[1];

    const tablaBody = document.getElementById("previewBody");
    const tablaHead = document.getElementById("headerPreview");

    const yaExiste = Array.from(tablaBody.rows).some(row =>
        row.dataset.idcomport === idComport
    );

    if (yaExiste) return;

    const niveles = document.querySelectorAll("#tablaNiveles tbody tr");
    const nivelesSeleccionados = [];

    niveles.forEach((tr, index) => {
        const input = tr.querySelector("input[type='text']");
        const descripcion = input.value.trim();
        if (descripcion !== "") {
            nivelesSeleccionados.push({
                nombre: columnas[index],
                descripcion: descripcion
            });
            if (!columnasUsadas.includes(columnas[index])) {
                columnasUsadas.push(columnas[index]);
            }
        }
    });

    if (nivelesSeleccionados.length === 0) return;

    tablaHead.innerHTML = "<th>Comportamientos</th>";
    columnasUsadas.forEach(nivel => {
        tablaHead.innerHTML += `<th>${nivel}</th>`;
    });
    tablaHead.innerHTML += "<th>Acciones</th>";

    const fila = document.createElement("tr");
    fila.dataset.idcomport = idComport;

    const tdComportamiento = document.createElement("td");
    tdComportamiento.textContent = nombreComport;
    fila.appendChild(tdComportamiento);

    columnasUsadas.forEach(nivel => {
        const encontrado = nivelesSeleccionados.find(n => n.nombre === nivel);
        const td = document.createElement("td");
        td.classList.add(`nivel-${normalizarTexto(nivel)}`);
        td.textContent = encontrado ? encontrado.descripcion : "";
        fila.appendChild(td);
    });

    const tdAcciones = document.createElement("td");
    tdAcciones.innerHTML = `<button type="button" class="btn btn-danger btn-sm" onclick="eliminarFila(this)"><i class="fas fa-trash-alt"></i></button>`;
    fila.appendChild(tdAcciones);

    tablaBody.appendChild(fila);
}

function eliminarFila(boton) {
    const fila = boton.closest("tr");
    if (fila) fila.remove();
}

function normalizarTexto(texto) {
    return texto
        .normalize("NFD").replace(/[\u0300-\u036f]/g, "")
        .toLowerCase().replace(/\s+/g, "-");
}
