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
            `<th>Asignar</th><th>Observaciones</th>`;
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
