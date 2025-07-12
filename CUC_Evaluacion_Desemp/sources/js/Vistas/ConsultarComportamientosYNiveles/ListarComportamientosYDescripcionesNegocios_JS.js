document.getElementById("selectCompetencia").addEventListener("change", function () {
    const value = this.value;
    if (!value) return;

    const partes = value.split("|");
    const idCompetencia = partes[0];

    fetch(`/Mantenimientos/ObtenerComportamientosYDescripcionesPorCompetencia?idCompetencia=${idCompetencia}`)
        .then(response => response.json())
        .then(data => {
            if (data.error) {
                alert(data.mensaje);
            } else {
                renderizarTablaAgrupada(data);
            }
        });
});

/********************************************************************************/
/********************* Funciones Internas       *****************************/
/********************************************************************************/
function renderizarTablaAgrupada(data) {
    const tablaBody = document.getElementById("previewBody");
    const tablaHead = document.getElementById("headerPreview");

    tablaBody.innerHTML = "";
    tablaHead.innerHTML = "";

    if (data.length === 0) {
        //si viene vacio limpiamos titulo y descripcion
        document.querySelector("#competenciaTitleRow th").textContent = "";
        document.querySelector("#competenciaDescRow td").textContent = "";
        //luego la tabla
        tablaBody.innerHTML = `<tr><td colspan="5" class="text-center">No se han asignado comportamientos</td></tr>`;
        return;
    }

    // Actualizamos contenido de título y descripción
    const Competencia = data[0].Competencia;
    const DescriCompetencia = data[0].DescriCompetencia;
    document.querySelector("#competenciaTitleRow th").textContent = "Competencia: "+ Competencia;
    document.querySelector("#competenciaDescRow td").textContent = "Descripción: "+ DescriCompetencia;

    // Obtenemos niveles únicos ordenados
    const niveles = [...new Set(data.map(item => item.Nivel))];

    // Agrupamos por Comportamiento
    const comportamientoMap = new Map();
    data.forEach(item => {
        if (!comportamientoMap.has(item.Comportamiento)) {
            comportamientoMap.set(item.Comportamiento, {});
        }
        comportamientoMap.get(item.Comportamiento)[item.Nivel] = item.Descripcion;
    });

    // Encabezados
    let headerHtml = "<th>Comportamientos</th>";
    niveles.forEach(n => {
        headerHtml += `<th>${n}</th>`;
    });
    tablaHead.innerHTML = headerHtml;

    // Filas
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

        tablaBody.appendChild(fila);
    });
}

