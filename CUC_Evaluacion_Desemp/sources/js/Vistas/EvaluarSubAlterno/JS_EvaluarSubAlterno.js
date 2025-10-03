//*******************************************************************************************************************************
//***********************************  Funcion Principal                    *****************************************************
//*******************************************************************************************************************************

$(document).ready(function () {
    inicializarEventos();
});
function inicializarEventos() {
    functionModalObjetivos();
}

document.getElementById("btnGuardarSeguimiento").addEventListener("click", enviarEvaluacion);

//*******************************************************************************************************************************
//***********************************  Seccion Objetivos                    *****************************************************
function functionModalObjetivos() {
    // Evento para abrir el modal con los datos del botón
    $(document).on("click", ".btn-editar-actual", function () {
        let id = $(this).data("id");
        let tipo = $(this).data("tipo");
        let nombre = $(this).data("nombre");
        let peso = $(this).data("peso");
        let actual = $(this).data("actual");
        let meta = $(this).data("meta");

        $("#modalTipoObjetivo").text(tipo);
        $("#modalNombreObjetivo").text(nombre);
        $("#modalPesoObjetivo").text(peso + "%");
        $("#modalMetaObjetivo").text(meta);
        $("#modalValorActual").val(actual);
        $("#maxPeso").text(peso);

        // Guardar el ID del objetivo en el botón confirmar
        $("#modalBtnConfirmar").data("id", id);

        $("#editarActualModal").modal("show");
    });

    // Evento para confirmar el cambio
    $("#modalBtnConfirmar").off("click").on("click", function () {
        let id = $(this).data("id");
        let nuevoValor = parseInt($("#modalValorActual").val());
        let max = parseInt($("#maxPeso").text());

        if (isNaN(nuevoValor) || nuevoValor < 1 || nuevoValor > max) {
            $("#valorError").removeClass("d-none");
            return;
        }

        $("#valorError").addClass("d-none");

        // Buscar la fila correspondiente en la tabla y actualizar valor
        let fila = $("#tbObjetivosAsignados").find(`button[data-id='${id}']`).closest("tr");
        fila.find("td:eq(4)").html(nuevoValor + "<span>%</span>");

        // Actualizar también el atributo data del botón
        fila.find(".btn-editar-actual").data("actual", nuevoValor);

        //llamamos la funcion que actualiza la tb resultados
        actualizarObjetivosEnTbResultados();

        let modal = bootstrap.Modal.getInstance(document.getElementById("editarActualModal"));
        modal.hide();
    });
}

// Función para cerrar el modal desde el botón de cerrar
function cerrarModal() {
    let modal = bootstrap.Modal.getInstance(document.getElementById("editarActualModal"));
    modal.hide();
    $("#valorError").addClass("d-none");
    document.activeElement.blur();
}
//*******************************************************************************************************************************
//***********************************  Seccion Tabla Resultados             *****************************************************
function actualizarObjetivosEnTbResultados() {
    let acumulados = {};

    // Recorremos la tabla de objetivos asignados
    $("#tbObjetivosAsignados tbody tr").each(function () {
        let tipo = $(this).find(".btn-editar-actual").data("tipo");
        let actualTxt = $(this).find("td:eq(4)").text().replace("%", "").trim();
        let actual = parseInt(actualTxt) || 0;

        if (!acumulados[tipo]) {
            acumulados[tipo] = 0;
        }
        acumulados[tipo] += actual;
    });

    // Recorremos la tabla de resultados y actualiza los inputs
    $("#tablaResultados tbody tr").each(function () {
        let tipoTexto = $(this).find("td:first").text().trim();
        let input = $(this).find(".input-calificacion");

        if (acumulados[tipoTexto] !== undefined) {
            input.val(acumulados[tipoTexto]);
        }
    });

    // Calculamos el total final
    let total = 0;
    $("#tablaResultados .input-calificacion").each(function () {
        let val = parseInt($(this).val()) || 0;
        total += val;
    });
    $("#resultado-total").val(total);
}//fin

function actualizarCompetenciasEnTbResultados() {
    let total = 0;

    $('#tbTransversalesEval select.select-nivel-obtenido, #tbCompetenciasEval select.select-nivel-obtenido').each(function () {
        const v = parseFloat($(this).val() || '0');
        if (!isNaN(v)) total += v;
    });

    const max = Number($('#MaximoPuntosCompetencias').val() || 0);
    const porcLogro = max > 0 ? (total * 100) / max : 0;

    let $filaComp = $('#tablaResultados tbody tr').has('td[data-tipo-categoria="Competencia"]').first();
    if (!$filaComp.length) {
        $filaComp = $('#tablaResultados tbody tr').filter(function () {
            const txt = $(this).find('td:first').text().trim().toLowerCase();
            return txt.includes('competencia');
        }).first();
    }

    if ($filaComp.length) {
        const porcTxt = $filaComp.find('td').eq(1).text();
        const porcCategoria = parseFloat(porcTxt.replace('%', '').replace(',', '.').trim()) || 0;
        const ponderado = (porcLogro * porcCategoria) / 100;
        $filaComp.find('.input-calificacion').val(ponderado.toFixed(2));
    }

    let totalGeneral = 0;
    $('#tablaResultados .input-calificacion').each(function () {
        const v = parseFloat($(this).val() || '0');
        if (!isNaN(v)) totalGeneral += v;
    });
    $('#resultado-total').val(totalGeneral.toFixed(2));

    return { total, porcLogro, totalGeneral };
}

// Disparo al cambiar los combos de competencias/transversales
$(document).on('change', '#tbTransversalesEval select.select-nivel-obtenido, #tbCompetenciasEval select.select-nivel-obtenido', function () {
    actualizarCompetenciasEnTbResultados();
});
//*******************************************************************************************************************************
//--------------- Envio de las tablas --------------
function enviarEvaluacion() {

    // Recolectar datos de las tablas que pintamos
    const objetivos = obtenerDatosTablaObjetivos('#tbObjetivosAsignados tbody tr');
    const competenciasTransversalesPlanas = obtenerDatosTablaTransversales('#tbTransversalesEval tr');
    const competenciasPlanas = obtenerDatosTablaCompetencia('#tbCompetenciasEval tbody tr');
    const competenciasTransversales = agruparCompetencias(competenciasTransversalesPlanas);
    const competencias = agruparCompetencias(competenciasPlanas);

    //Recolectamos lo otro
    const observaciones = document.getElementById('txtObservaciones').value;
    const cedFuncionario = document.getElementById('ceduFuncionario').innerText;
    const idConglo = document.getElementById('idConglo').innerText;

    // Valida que ambas tablas tengan datos ****************************
    if (competenciasTransversales.length === 0) {
        alert('No se detectaron las competencias transversales.');
        return;
    }
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
    
    // 4. Mostrar errores o enviar datos
    if (errores.length > 0) {
        alert('Errores de validación:\n\n' + errores.join('\n'));
        return false;
    }

    // Crear objeto con todos los datos
    const evaluacionData = {
        objetivos: objetivos,
        competenciasTransversales: competenciasTransversales,
        competencias: competencias,
        observaciones: observaciones,
        cedFuncionario: cedFuncionario,
        idConglo: idConglo

    };

    console.log('TX', obtenerDatosTablaTransversales('#tbTransversalesEval tr')[0]);
    console.log('CX', obtenerDatosTablaCompetencia('#tbCompetenciasEval tbody tr')[0]);
    console.log('G.TX', agruparCompetencias(obtenerDatosTablaTransversales('#tbTransversalesEval tr'))[0]);

    // Enviar al servidor
    enviarPeticionEvaluacion(evaluacionData);
}
function parseNum(s) {
    if (s == null) return 0;
    const t = String(s).replace('%', '').replace(',', '.').trim();
    const n = parseFloat(t);
    return isNaN(n) ? 0 : n;
}
function obtenerDatosTablaObjetivos(selector) {
    const filas = document.querySelectorAll(selector);
    return Array.from(filas).map(fila => {
        const c = fila.querySelectorAll('td');
        const btn = fila.querySelector('.btn-editar-actual');
        const id = btn ? btn.getAttribute('data-id') : '';
        const idEvaxObj = (c[6]?.textContent?.trim()) || (btn?.getAttribute('data-IdEvaxObj')) || '';
        const tipo = c[0]?.textContent.trim() || '';
        const nombre = c[1]?.textContent.trim() || '';
        const peso = parseNum(c[2]?.textContent);
        const meta = (c[3]?.textContent || btn?.getAttribute('data-meta') || '').trim();
        const actual = parseNum(c[4]?.textContent) || parseNum(btn?.getAttribute('data-actual'));
        return { id, idEvaxObj, nombre, tipo, peso, meta, actual };
    }).filter(x => x.idEvaxObj);
}

function obtenerDatosTablaTransversales(selector) {
    const filas = document.querySelectorAll(selector);
    const datos = [];
    let competenciaActual = null;
    let tipoCompetenciaActual = null;

    Array.from(filas).forEach(fila => {
        if (fila.hasAttribute('data-id')) {
            competenciaActual = fila.getAttribute('data-id');
            tipoCompetenciaActual = fila.getAttribute('data-id-tipo');
        }
        if (fila.hasAttribute('data-comportamiento-id')) {
            const idComportamiento = fila.getAttribute('data-comportamiento-id');
            const select = fila.querySelector('select.select-nivel-obtenido');
            if (!select) return;
            const opt = select.selectedOptions && select.selectedOptions[0];
            if (!opt) return;
            const idNivel = opt.getAttribute('data-idnivel');
            const valor = parseFloat(opt.value) || 0;
            const idEvaxComp = parseInt(opt.getAttribute('data-evaxcomp')) || 0;
            if (!idNivel) return;
            datos.push({
                idCompetencia: competenciaActual,
                idTipoCompetencia: tipoCompetenciaActual,
                idComportamiento: idComportamiento,
                idNivel: idNivel,
                valor: valor,
                idEvaxComp: idEvaxComp
            });
        }
    });
    return datos;
}//fin obtenerDatosTablaTransversales

function obtenerDatosTablaCompetencia(selector) {
    const filas = document.querySelectorAll(selector);
    const datos = [];
    let competenciaActual = null;
    let tipoCompetenciaActual = null;

    Array.from(filas).forEach(fila => {
        if (fila.hasAttribute('data-id')) {
            competenciaActual = fila.getAttribute('data-id');
            tipoCompetenciaActual = fila.getAttribute('data-id-tipo');
        }
        if (fila.hasAttribute('data-comportamiento-id')) {
            const idComportamiento = fila.getAttribute('data-comportamiento-id');
            const select = fila.querySelector('select.select-nivel-obtenido');
            if (!select) return;
            const opt = select.selectedOptions && select.selectedOptions[0];
            if (!opt) return;
            const idNivel = opt.getAttribute('data-idnivel');
            const valor = parseFloat(opt.value) || 0;
            const idEvaxComp = parseInt(opt.getAttribute('data-evaxcomp')) || 0;
            if (!idComportamiento || !idNivel) return;
            datos.push({
                idCompetencia: competenciaActual,
                idTipoCompetencia: tipoCompetenciaActual,
                idComportamiento: idComportamiento,
                idNivel: idNivel,
                valor: valor,
                idEvaxComp: idEvaxComp
            });
        }
    });
    return datos;
}

async function enviarPeticionEvaluacion(evaluacionData) {
    try {
        const formBody = `evaluacionData=${encodeURIComponent(JSON.stringify(evaluacionData))}`;

        const response = await fetch(`${urlBase}Evaluacion/GuardarSeguimiento`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: formBody
        });

        if (!response.ok) {
            throw new Error(`Error HTTP: ${response.status}`);
        }

        alert('Seguimiento creado correctamente');
        window.location.href = `${urlBase}Home/Index`;
    } catch (error) {
        alert(`Error: ${error.message}`);
    }
}
function agruparCompetencias(items) {
    const map = new Map();
    for (const it of items) {
        const compId = String(it.idCompetencia);
        const tipoId = String(it.idTipoCompetencia);
        const compoId = String(it.idComportamiento);
        const nivelId = String(it.idNivel);
        const valor = typeof it.valor === 'number' ? it.valor : parseFloat(it.valor) || 0;
        const evax = typeof it.idEvaxComp === 'number' ? it.idEvaxComp : parseInt(it.idEvaxComp) || 0;
        if (!map.has(compId)) map.set(compId, { idCompetencia: compId, idTipoCompetencia: tipoId, comportamientos: [] });
        const comp = map.get(compId);
        let compo = comp.comportamientos.find(c => c.idComportamiento === compoId);
        if (!compo) { compo = { idComportamiento: compoId, niveles: [] }; comp.comportamientos.push(compo); }
        const existe = compo.niveles.find(n => n.idNivel === nivelId);
        if (existe) { existe.valor = valor; existe.idEvaxComp = evax; }
        else { compo.niveles.push({ idNivel: nivelId, valor: valor, idEvaxComp: evax }); }
    }
    return Array.from(map.values());
}

