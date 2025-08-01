document.getElementById("btnEnviarEvaluacion").addEventListener("click", enviarEvaluacion);

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

// Función auxiliar para extraer datos de las filas de la tabla
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