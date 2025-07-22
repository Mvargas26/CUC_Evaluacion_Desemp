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
    const contenedor = document.getElementById("tbCompetenciasSelect");
    contenedor.innerHTML = "";

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
        const grupoCompetencia = document.createElement("tbody");
        grupoCompetencia.setAttribute('data-competencia-id', competencia.idCompetencia);

        // Fila del título con botón eliminar
        const filaTitulo = document.createElement("tr");
        filaTitulo.innerHTML = `
            <th colspan="7" style="background-color: #f8f9fa; text-align: center;">
                <div style="display: flex; justify-content: space-between; align-items: center;">
                    <span style="font-weight: bold ;color: #0d6efd;">Competencia: ${competencia.Competencia}</span>
                    <button class="btn btn-danger btn-sm" onclick="eliminarCompetencia(${competencia.idCompetencia})">
                        <i class="fas fa-trash-alt"></i> Eliminar
                    </button>
                </div>
            </th>
        `;
        grupoCompetencia.appendChild(filaTitulo);

        // Descripción
        const descripcion = document.createElement("tr");
        const tdDescripcion = document.createElement("td");
        tdDescripcion.colSpan = 7;
        tdDescripcion.className = "text-center";
        tdDescripcion.style.fontWeight = "bold";
        tdDescripcion.textContent = "Descripción: " + competencia.DescriCompetencia;
        descripcion.appendChild(tdDescripcion);
        grupoCompetencia.appendChild(descripcion);

        // Obtener niveles únicos
        const niveles = [...new Set(competencia.Datos.map(d => d.Nivel))].sort();

        // Encabezado de la tabla
        const encabezado = document.createElement("tr");
        encabezado.innerHTML = `<th>Comportamientos</th>` +
            niveles.map(n => `<th>${n}</th>`).join("") +
            `<th>Asignar</th><th>Observaciones</th>`;
        grupoCompetencia.appendChild(encabezado);

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

            // Columnas para cada nivel
            niveles.forEach(nivel => {
                const td = document.createElement("td");
                td.textContent = descripcionesPorNivel[nivel] || "";
                fila.appendChild(td);
            });

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

            grupoCompetencia.appendChild(fila);
        });

        // Espaciado entre competencias
        const espacio = document.createElement("tr");
        const tdEspacio = document.createElement("td");
        tdEspacio.colSpan = 7;
        tdEspacio.innerHTML = "<hr />";
        espacio.appendChild(tdEspacio);
        grupoCompetencia.appendChild(espacio);

        contenedor.appendChild(grupoCompetencia);
    });
}
function eliminarCompetencia(idCompetencia) {
    if (confirm('¿Está seguro que desea eliminar esta competencia de la lista?')) {
        // Encontrar y eliminar el grupo de la competencia
        const elemento = document.querySelector(`tbody[data-competencia-id="${idCompetencia}"]`);
        if (elemento) {
            elemento.remove();
        }

        // Actualizar el array de datos
         competenciasData = competenciasData.filter(item => item.idCompetencia !== idCompetencia);
    }
}

