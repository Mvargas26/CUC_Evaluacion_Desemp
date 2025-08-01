document.querySelector('#tbCompetenciasSelect').addEventListener('click', function (e) {
    if (e.target.closest('button.btn-danger')) {
        setTimeout(() => actualizarCompetenciasEnResultados(), 50);
    }
});
function actualizarCompetenciasEnResultados() {
    // Obtenemos todos los tipos definidos en tablaResultados
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
                )
            };
        }
    });

    const sumasPorTipo = {};
    const filas = document.querySelectorAll('#tbCompetenciasSelect tbody tr');
    console.log("Cantidad de filas en tabla competencias:", filas.length);

    const tiposProcesados = new Set();

    if (filas.length > 0) {
        filas.forEach(row => {
            const idTipo = row.getAttribute('data-id-tipo');
            if (idTipo && !tiposProcesados.has(idTipo)) {
                console.log("El tipo fue: " + idTipo);
                tiposProcesados.add(idTipo);

                if (tiposDefinidos[idTipo]) {
                    sumasPorTipo[idTipo] = tiposDefinidos[idTipo].porcentajeDeseado;
                }
            }
        });
    } else {
        // Si la tabla de competencias está vacía, poner en 0 todos los tipos definidos
        for (const idTipo in tiposDefinidos) {
            sumasPorTipo[idTipo] = 0;
        }
    }

    // Actualizamos inputs en tablaResultados
    let sumaTotal = 0;
    for (const [idTipo, sumaActual] of Object.entries(sumasPorTipo)) {
        sumaTotal += sumaActual;

        if (tiposDefinidos[idTipo]) {
            tiposDefinidos[idTipo].input.value = sumaActual.toFixed(2);

            // Cambiar color si no coincide con el deseado
            const diferencia = Math.abs(sumaActual - tiposDefinidos[idTipo].porcentajeDeseado);
            tiposDefinidos[idTipo].input.style.backgroundColor = diferencia > 1 ? '#e62929' : '';
        }
    }

    // Actualizamos suma total
    const resultadoTotal = document.getElementById('resultado-total');
    if (resultadoTotal) {
        resultadoTotal.value = sumaTotal.toFixed(2);
        resultadoTotal.style.backgroundColor = Math.abs(sumaTotal - 100) > 1 ? '#e62929' : '';
    }
}
