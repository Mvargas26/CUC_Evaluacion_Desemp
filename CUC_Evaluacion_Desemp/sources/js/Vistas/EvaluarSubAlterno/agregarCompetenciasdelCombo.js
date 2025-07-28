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


