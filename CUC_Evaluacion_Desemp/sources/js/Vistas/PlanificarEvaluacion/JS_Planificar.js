//*****************************   Eventos
//***********************************************************************
document.querySelector('#tbCompetenciasSelect').addEventListener('click', function (e) {
    if (e.target.closest('button.btn-danger')) {
        setTimeout(() => actualizarResultadosGlobales(), 50);
    }
});

document.getElementById("btnEnviarEvaluacion").addEventListener("click", enviarEvaluacion);

// Elimina la fila de la tabla y actualiza el total
document.querySelector("#tablaObjetivos tbody").addEventListener("click", function (e) {
    if (e.target.classList.contains("btn-eliminar-objetivo") || e.target.closest(".btn-eliminar-objetivo")) {
        const fila = e.target.closest("tr");
        fila.remove();
        actualizarResultadosGlobales();
    }
});

//*****************************   FUNCIONES
//***********************************************************************
//-----------TRANSVERSALES
function renderizarTablaAgrupada(data) {
    const contenedor = document.getElementById("previewBody");
    contenedor.innerHTML = "";

    if (data.length === 0) {
        contenedor.innerHTML = `<tr><td colspan="7" class="text-center">No se han asignado comportamientos</td></tr>`;
        return;
    }

    const competenciasMap = new Map();

    data.forEach(item => {
        if (!competenciasMap.has(item.idCompetencia)) {
            competenciasMap.set(item.idCompetencia, {
                Competencia: item.Competencia,
                DescriCompetencia: item.DescriCompetencia,
                Datos: []
            });
        }
        competenciasMap.get(item.idCompetencia).Datos.push(item);
    });

    competenciasMap.forEach(({ Competencia, DescriCompetencia, Datos }) => {
        // Título
        const titulo = document.createElement("tr");
        const thTitulo = document.createElement("th");
        thTitulo.colSpan = 7;
        thTitulo.className = "text-center";
        thTitulo.style.backgroundColor = "#f8f9fa";
        thTitulo.style.fontWeight = "bold";
        thTitulo.textContent = "Competencia: " + Competencia;
        titulo.appendChild(thTitulo);
        contenedor.appendChild(titulo);

        // Descripción
        const descripcion = document.createElement("tr");
        const tdDescripcion = document.createElement("td");
        tdDescripcion.colSpan = 7;
        tdDescripcion.className = "text-center";
        tdDescripcion.style.fontWeight = "bold";
        tdDescripcion.textContent = "Descripción: " + DescriCompetencia;
        descripcion.appendChild(tdDescripcion);
        contenedor.appendChild(descripcion);

        // Niveles
        const niveles = [...new Set(Datos.map(d => d.Nivel))];

        // Encabezado
        const encabezado = document.createElement("tr");
        encabezado.innerHTML = `<th>Comportamientos</th>` +
            niveles.map(n => `<th>${n}</th>`).join("") +
            "";
        //    `<th>Asignar</th><th>Observaciones</th>`;
        contenedor.appendChild(encabezado);

        // Agrupar comportamientos
        const comportamientoMap = new Map();
        Datos.forEach(item => {
            if (!comportamientoMap.has(item.Comportamiento)) {
                comportamientoMap.set(item.Comportamiento, {});
            }
            comportamientoMap.get(item.Comportamiento)[item.Nivel] = item.Descripcion;
        });

        comportamientoMap.forEach((descripcionesPorNivel, comportamiento) => {
            const fila = document.createElement("tr");

            const tdComport = document.createElement("td");
            tdComport.innerHTML = `<strong>${comportamiento}</strong>`;
            fila.appendChild(tdComport);

            niveles.forEach(nivel => {
                const td = document.createElement("td");
                td.textContent = descripcionesPorNivel[nivel] || "";
                fila.appendChild(td);
            });
            /*
            // Columna Asignar
            const tdAsignar = document.createElement("td");
            tdAsignar.style.minWidth = "160px";

            const select = document.createElement("select");
            select.className = "form-select";
            select.style.minWidth = "150px";

            const optionDefault = document.createElement("option");
            optionDefault.textContent = "Seleccione";
            optionDefault.value = "";
            select.appendChild(optionDefault);
            niveles.forEach(nivel => {
                const option = document.createElement("option");
                option.value = nivel;
                option.textContent = nivel;
                select.appendChild(option);
            });
            tdAsignar.appendChild(select);
            fila.appendChild(tdAsignar);

            // Columna Observaciones
            const tdObs = document.createElement("td");
            const inputObs = document.createElement("input");
            inputObs.type = "text";
            inputObs.className = "form-control";
            tdObs.appendChild(inputObs);
            fila.appendChild(tdObs);
                    */
            contenedor.appendChild(fila);
        });

        // Espaciado entre competencias
        const espacio = document.createElement("tr");
        const tdEspacio = document.createElement("td");
        tdEspacio.colSpan = 7;
        tdEspacio.innerHTML = "<hr/>";
        espacio.appendChild(tdEspacio);
        contenedor.appendChild(espacio);
    });
}

//-----------COMPETENCIAS
//agrega a la tabla las competencias que seleccionen
function agregarCompetenciaSelect() {
    const competenciaSeleccionada = document.getElementById("CompetenciaElejida").value;

    // Filtramos solo los datos de la competencia seleccionada
    const dataCompetencia = competenciasData.filter(item => item.Competencia === competenciaSeleccionada);

    if (dataCompetencia.length === 0) {
        alert("No se encontraron datos para la competencia seleccionada");
        return;
    }

    renderizarTablaCompetenciaSelect(dataCompetencia);
}

//pinta la competencia con niveles y lo demas
function renderizarTablaCompetenciaSelect(data) {
    const contenedor = document.querySelector("#tbCompetenciasSelect tbody");

    if (data.length === 0) {
        if (contenedor.querySelectorAll("tr").length === 0) {
            contenedor.innerHTML = `<tr><td colspan="7" class="text-center">No se han asignado competencias</td></tr>`;
        }
        return;
    }

    // Agrupar por competencia
    const competenciasMap = new Map();
    data.forEach(item => {
        if (!competenciasMap.has(item.idCompetencia)) {
            competenciasMap.set(item.idCompetencia, {
                idCompetencia: item.idCompetencia,
                Competencia: item.Competencia,
                DescriCompetencia: item.DescriCompetencia,
                Datos: []
            });
        }
        competenciasMap.get(item.idCompetencia).Datos.push(item);
    });

    competenciasMap.forEach((competencia) => {
        // Verificar si ya existe en la tabla
        if (contenedor.querySelector(`tr[data-id='${competencia.idCompetencia}']`)) {
            alert(`La competencia "${competencia.Competencia}" ya está agregada.`);
            return;
        }

        const niveles = [...new Set(competencia.Datos.map(d => d.Nivel))].sort();
        const totalColumnas = 1 + niveles.length; // calculamos cuantas columnas

        // Fila del título con botón eliminar
        const filaTitulo = document.createElement("tr");
        filaTitulo.setAttribute("data-id", competencia.idCompetencia);
        filaTitulo.setAttribute("data-id-tipo", competencia.Datos[0].idTipoCompetencia);//almacenamos el tipo de competencia tambien
        const thTitulo = document.createElement("th");
        thTitulo.colSpan = totalColumnas;
        thTitulo.style.backgroundColor = "#f8f9fa";
        thTitulo.style.textAlign = "center";
        thTitulo.innerHTML = `
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <span style="font-weight: bold; color: #0d6efd;">Competencia: ${competencia.Competencia}</span>
                <button class="btn btn-danger btn-sm" onclick="eliminarCompetencia(${competencia.idCompetencia})">
                    <i class="fas fa-trash-alt"></i> Eliminar
                </button>
            </div>
        `;
        filaTitulo.appendChild(thTitulo);
        contenedor.appendChild(filaTitulo);

        // Fila de descripción
        const descripcion = document.createElement("tr");
        const tdDescripcion = document.createElement("td");
        tdDescripcion.colSpan = totalColumnas;
        tdDescripcion.className = "text-center";
        tdDescripcion.style.fontWeight = "bold";
        tdDescripcion.textContent = "Descripción: " + competencia.DescriCompetencia;
        descripcion.appendChild(tdDescripcion);
        contenedor.appendChild(descripcion);

        // Encabezado de niveles
        const encabezado = document.createElement("tr");
        encabezado.innerHTML = `<th>Comportamientos</th>` +
            niveles.map(n => `<th>${n}</th>`).join("") +
            "";
        // + `<th>Asignar</th><th>Observaciones</th>`;
        contenedor.appendChild(encabezado);

        // Agrupar comportamientos
        const comportamientoMap = new Map();
        competencia.Datos.forEach(item => {
            if (!comportamientoMap.has(item.Comportamiento)) {
                comportamientoMap.set(item.Comportamiento, {});
            }
            comportamientoMap.get(item.Comportamiento)[item.Nivel] = item.Descripcion;
        });

        // Crear filas para cada comportamiento
        comportamientoMap.forEach((descripcionesPorNivel, comportamiento) => {
            const fila = document.createElement("tr");

            // Columna Comportamiento
            const tdComport = document.createElement("td");
            tdComport.innerHTML = `<strong>${comportamiento}</strong>`;
            fila.appendChild(tdComport);

            // Columnas de niveles
            niveles.forEach(nivel => {
                const td = document.createElement("td");
                td.textContent = descripcionesPorNivel[nivel] || "";
                fila.appendChild(td);
            });

            /*
            // Columna Asignar
            const tdAsignar = document.createElement("td");
            const select = document.createElement("select");
            select.className = "form-select";
            select.style.minWidth = "150px";

            const optionDefault = document.createElement("option");
            optionDefault.textContent = "Seleccione";
            optionDefault.value = "";
            select.appendChild(optionDefault);

            niveles.forEach(nivel => {
                const option = document.createElement("option");
                option.value = nivel;
                option.textContent = nivel;
                select.appendChild(option);
            });

            tdAsignar.appendChild(select);
            fila.appendChild(tdAsignar);

            // Columna Observaciones
            const tdObs = document.createElement("td");
            const inputObs = document.createElement("input");
            inputObs.type = "text";
            inputObs.className = "form-control";
            tdObs.appendChild(inputObs);
            fila.appendChild(tdObs);
            */

            contenedor.appendChild(fila);
        });

        // Fila de separación entre competencias
        const espacio = document.createElement("tr");
        const tdEspacio = document.createElement("td");
        tdEspacio.colSpan = totalColumnas;
        tdEspacio.innerHTML = "<hr/>";
        espacio.appendChild(tdEspacio);
        contenedor.appendChild(espacio);
    });
}

//elimina la competencia de la tabla
function eliminarCompetencia(idCompetencia) {
    if (confirm('¿Está seguro que desea eliminar esta competencia de la lista?')) {
        const contenedor = document.querySelector("#tbCompetenciasSelect tbody");

        // Buscamos el título de la competencia
        const filaTitulo = contenedor.querySelector(`tr[data-id="${idCompetencia}"]`);
        if (filaTitulo) {
            // Eliminamos todas las filas siguientes hasta el próximo separador <hr/> o fin de tabla
            let filaActual = filaTitulo.nextElementSibling;
            filaTitulo.remove();

            while (filaActual) {
                const siguiente = filaActual.nextElementSibling;
                filaActual.remove();

                // salimos cuando lleguemos al separador (<hr/> en td)
                if (filaActual.querySelector("td hr")) break;
                filaActual = siguiente;
            }
        }

        // Actualizar array de datos global
        competenciasData = competenciasData.filter(item => item.idCompetencia !== idCompetencia);

        // Si ya no quedan competencias, mostrar mensaje vacío
        if (contenedor.querySelectorAll("tr").length === 0) {
            contenedor.innerHTML = `<tr><td colspan="7" class="text-center">No se han asignado competencias</td></tr>`;
        }
    }
}

//-----------OBJETIVOS
//agrega el objetivoa  la tabla
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
    btnEliminar.classList.add("btn", "btn-sm", "btn-danger", "btn-eliminar-objetivo");
    btnEliminar.innerText = "Eliminar";
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

    // Actualizar TOTALES
    actualizarResultadosGlobales();

    // Limpiar los inputs
    inputPesoObj.value = "";
    inputMetaObj.value = "";
}//fin

//-----------TABLA DE SUMAS TOTALES
function actualizarResultadosGlobales() {
    // 1. Obtenmos los tipos
    const tiposDefinidos = {};
    document.querySelectorAll('#tablaResultados tbody tr').forEach(row => {
        const idTipoCell = row.querySelector('td:nth-child(4)');
        if (idTipoCell && idTipoCell.textContent !== '-') {
            const idTipo = idTipoCell.textContent.trim();
            tiposDefinidos[idTipo] = {
                input: row.querySelector('td:nth-child(3) input'),
                porcentajeDeseado: parseFloat(
                    row.querySelector('td:nth-child(2) strong')?.textContent
                        .replace('Deseado:', '')
                        .replace('%', '')
                        .trim() || '0'
                ),
                esCompetencia: false 
            };
        }
    });

    // 2. Sacamos los tipos de competencias
    const tiposCompetencias = new Set();
    document.querySelectorAll('#tbCompetenciasSelect tbody tr[data-id-tipo]').forEach(row => {
        const idTipo = row.getAttribute('data-id-tipo');
        if (idTipo && tiposDefinidos[idTipo]) {
            tiposCompetencias.add(idTipo);
            tiposDefinidos[idTipo].esCompetencia = true;
        }
    });

    const sumasPorTipo = {};
    let sumaTotal = 0;

    // Sumamos los tipos objetivos nadamas
    document.querySelectorAll('#tablaObjetivos tbody tr').forEach(row => {
        const idTipo = row.querySelector('td:nth-child(3)')?.textContent.trim();
        const peso = parseFloat(row.querySelector('td:nth-child(4)')?.textContent.replace('%', '') || '0');

        if (idTipo && !tiposCompetencias.has(idTipo)) {
            sumasPorTipo[idTipo] = (sumasPorTipo[idTipo] || 0) + peso;
        }
    });

    // Asignar porcentaje a competencias con solo que detecte 1
    tiposCompetencias.forEach(idTipo => {
        if (tiposDefinidos[idTipo]) {
            sumasPorTipo[idTipo] = tiposDefinidos[idTipo].porcentajeDeseado;
        }
    });

    // 4. Actualizar vista para todos los tipos definidos
    for (const [idTipo, def] of Object.entries(tiposDefinidos)) {
        const sumaActual = sumasPorTipo[idTipo] || 0; // Si no hay datos es 0
        def.input.value = sumaActual.toFixed(2);
        const diferencia = Math.abs(sumaActual - def.porcentajeDeseado);
        def.input.style.backgroundColor = diferencia > 1 ? '#e62929' : '';
        sumaTotal += sumaActual;
    }

    // 5. Actualizar total general
    const resultadoTotal = document.getElementById('resultado-total');
    if (resultadoTotal) {
        resultadoTotal.value = sumaTotal.toFixed(2);
        resultadoTotal.style.backgroundColor = Math.abs(sumaTotal - 100) > 1 ? '#e62929' : '';
    }
}


//--------------- Envio de las tablas --------------
function enviarEvaluacion() {

    // Recolectar datos de ambas tablas
    const objetivos = obtenerDatosTabla('#tablaObjetivos tbody tr');
    const competencias = obtenerDatosTablaCompetencia('#tbCompetenciasSelect tbody tr');

    //Recolectar otraso datos
    const observaciones = document.getElementById('txtObservaciones').value;
    const cedFuncionario = document.getElementById('ceduFuncionario').innerText;
    const idConglo = document.getElementById('idConglo').innerText;

    // Valida que ambas tablas tengan datos ****************************
    if (objetivos.length === 0 && competencias.length === 0) {
        alert('Ambas tablas están vacías. Por favor agregue al menos un objetivo y una competencia.');
        return;
    }

    if (objetivos.length === 0) {
        alert('La tabla de objetivos está vacía. Por favor agregue al menos un objetivo.');
        return;
    }

    if (competencias.length === 0) {
        alert('La tabla de competencias está vacía. Por favor agregue al menos una competencia.');
        return;
    }

    // Valida porcentajes esten completos *****************************
    // 2. Validar coincidencia exacta en tablaResultados
    const errores = [];
    let totalActual = 0;
    let totalDeseado = 0;

    document.querySelectorAll('#tablaResultados tbody tr').forEach(row => {
        const idTipoCell = row.querySelector('td:nth-child(4)');
        if (!idTipoCell || idTipoCell.textContent === '-') return;

        const input = row.querySelector('td:nth-child(3) input');
        const porcentajeDeseadoText = row.querySelector('td:nth-child(2) strong')?.textContent;

        if (input && porcentajeDeseadoText) {
            const valorActual = parseFloat(input.value) || 0;
            const porcentajeDeseado = parseFloat(porcentajeDeseadoText.replace('Deseado:', '').replace('%', '').trim());

            totalActual += valorActual;
            totalDeseado += porcentajeDeseado;

            if (valorActual !== porcentajeDeseado) {
                const nombreTipo = row.querySelector('td:first-child strong')?.textContent.trim();
                errores.push(
                    `El tipo "${nombreTipo}" tiene ${valorActual}% pero debería tener ${porcentajeDeseado}%`
                );
            }
        }
    });

    // 3. Validar suma total
    if (Math.abs(totalActual - 100) > 0.01) {
        errores.push(`La suma total es ${totalActual}% pero debe ser exactamente 100%`);
    }

    // 4. Mostrar errores o enviar datos
    if (errores.length > 0) {
        alert('Errores de validación:\n\n' + errores.join('\n'));
        return false;
    }

    // Crear objeto con todos los datos
    const evaluacionData = {
        objetivos: objetivos,
        competencias: competencias,
        observaciones: observaciones,
        cedFuncionario: cedFuncionario,
        idConglo: idConglo

    };

    // Enviar al servidor
    enviarPeticionEvaluacion(evaluacionData);
}

// Función para extraer datos de las filas de la tabla
function obtenerDatosTabla(selector) {
    const filas = document.querySelectorAll(selector);
    return Array.from(filas).map(fila => {
        const celdas = fila.querySelectorAll('td');
        return {
            id: celdas[0]?.innerText.trim() || '',
            nombre: celdas[1]?.innerText.trim() || '',
            tipo: celdas[2]?.innerText.trim() || '',
            peso: celdas[3]?.innerText.replace('%', '').trim() || '',
            meta: celdas[4]?.innerText.trim() || '',
            actual: celdas[5]?.innerText.replace('%', '').trim() || ''
        };
    });
}
function obtenerDatosTablaCompetencia(selector) {
    const filas = document.querySelectorAll(selector);
    return Array.from(filas).map(fila => {
        const celdas = fila.querySelectorAll('td');
        return {
            idCompetencia: fila.getAttribute('data-id') || null,
        };
    });
}
//fucion de la peticion
async function enviarPeticionEvaluacion(evaluacionData) {
    try {
        const formBody = `evaluacionData=${encodeURIComponent(JSON.stringify(evaluacionData))}`;

        const response = await fetch(`${urlBase}Evaluacion/GuardarPlanificacion`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: formBody
        });

        const data = await response.json();

        if (!data.success) throw new Error(data.error);

        alert('Evaluación enviada correctamente');
        window.location.href = data.redirectUrl || `${urlBase}Evaluacion/Index`;
    } catch (error) {
        alert(`Error: ${error.message}`);
    }
}

