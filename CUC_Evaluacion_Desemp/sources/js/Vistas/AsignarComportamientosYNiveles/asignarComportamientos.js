
let columnasUsadas = [];
let totalDescripciones = 0;
let totalComportamientos = 0;

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
        const input = tr.querySelector("textarea");
        //console.log(`Nivel #${index}`, input, "Valor:", input?.value);
        const descripcion = input.value.trim();
        if (descripcion !== "") {
            nivelesSeleccionados.push({
                nombre: columnas[index],
                descripcion: descripcion,
                idNivel: tr.querySelector("input[name*='idNivel']").value,
                nombreNivel: columnas[index]
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

        if (encontrado) {
            td.innerHTML += `
                <input type="hidden" name="descripciones[${totalDescripciones}].idComportamiento" value="${idComport}" />
                <input type="hidden" name="descripciones[${totalDescripciones}].idNivel" value="${encontrado.idNivel}" />
                <input type="hidden" name="descripciones[${totalDescripciones}].descripcion" value="${encontrado.descripcion}" />
                <input type="hidden" name="descripciones[${totalDescripciones}].NombreNivel" value="${encontrado.nombreNivel}" />
            `;
            totalDescripciones++;
        }

        fila.appendChild(td);
    });

    const tdAcciones = document.createElement("td");
    const maxNivel = Math.max(...nivelesSeleccionados.map(n => parseInt(n.idNivel)));

    tdAcciones.innerHTML = `
        <input type="hidden" name="comportamientos[${totalComportamientos}].idComportamiento" value="${idComport}" />
        <input type="hidden" name="comportamientos[${totalComportamientos}].Observaciones" value="" />
        <input type="hidden" name="comportamientos[${totalComportamientos}].NivelObtenido" value="${maxNivel}" />
        <button type="button" class="btn btn-danger btn-sm" onclick="eliminarFila(this)"><i class="fas fa-trash-alt"></i></button>
    `;
    fila.appendChild(tdAcciones);

    totalComportamientos++;

    tablaBody.appendChild(fila);

    limpiarTextAreasDeNiveles();

}//fin funcion

//******************************************************************************************
//******************         Funciones Internas            *********************************
//******************************************************************************************
function eliminarFila(boton) {
    const fila = boton.closest("tr");
    if (!fila) return;

    fila.remove();
    reindexarInputs();
}

function normalizarTexto(texto) {
    return texto
        .normalize("NFD").replace(/[\u0300-\u036f]/g, "")
        .toLowerCase().replace(/\s+/g, "-");
}
function reindexarInputs() {
    const filas = document.querySelectorAll("#previewBody tr");
    totalDescripciones = 0;
    totalComportamientos = 0;

    filas.forEach((fila, i) => {
        const idComport = fila.dataset.idcomport;
        const inputsDesc = fila.querySelectorAll("input[name^='descripciones']");
        const inputsComp = fila.querySelectorAll("input[name^='comportamientos']");

        let nivelCount = 0;
        inputsDesc.forEach(input => {
            if (input.name.includes(".idComportamiento")) {
                input.name = `descripciones[${totalDescripciones}].idComportamiento`;
            }
            if (input.name.includes(".idNivel")) {
                input.name = `descripciones[${totalDescripciones}].idNivel`;
            }
            if (input.name.includes(".descripcion")) {
                input.name = `descripciones[${totalDescripciones}].descripcion`;
            }
            if (input.name.includes(".NombreNivel")) {
                input.name = `descripciones[${totalDescripciones}].NombreNivel`;
            }

            nivelCount++;
            if (nivelCount % 4 === 0) totalDescripciones++; // Suponiendo 4 niveles máx por comportamiento
        });

        inputsComp.forEach(input => {
            if (input.name.includes(".idComportamiento")) {
                input.name = `comportamientos[${totalComportamientos}].idComportamiento`;
            }
            if (input.name.includes(".Observaciones")) {
                input.name = `comportamientos[${totalComportamientos}].Observaciones`;
            }
            if (input.name.includes(".NivelObtenido")) {
                input.name = `comportamientos[${totalComportamientos}].NivelObtenido`;
            }
        });

        totalComportamientos++;
    });
    
}//fin funcion

function limpiarTextAreasDeNiveles() {
    const textareas = document.querySelectorAll("#tablaNiveles textarea");
    textareas.forEach(textarea => {
        textarea.value = "";
    });
}//fin funcion
