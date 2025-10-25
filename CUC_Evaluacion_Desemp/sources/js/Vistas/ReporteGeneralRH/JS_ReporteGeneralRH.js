(function () {
    const ddlTipoReporte = document.getElementById('tipoReporte');
    const ddlDepartamento = document.getElementById('idDepartamento');
    const ddlConglomerado = document.getElementById('idConglomerado');
    const ddlPuesto = document.getElementById('idPuesto');
    const ddlPeriodo = document.getElementById('idPeriodo');

    const filtroDepartamentoWrapper = document.getElementById('filtroDepartamentoWrapper');
    const filtroFuncionarioWrapper = document.getElementById('filtroFuncionarioWrapper');
    const filtroConglomeradoWrapper = document.getElementById('filtroConglomeradoWrapper');
    const filtroPuestoWrapper = document.getElementById('filtroPuestoWrapper');

    const txtBuscaFuncionario = document.getElementById('buscaFuncionario');
    const cedFuncionarioSeleccionado = document.getElementById('cedFuncionarioSeleccionado');
    const resFunSel = document.getElementById('resultadoFuncionarioSeleccionado');

    const alertaValidacion = document.getElementById('alertaValidacion');

    const tbConsolidadoBody = document.getElementById('tbConsolidadoBody');
    const tbResultadosDetalleBody = document.getElementById('tbResultadosDetalleBody');

    const resumenContextoSeleccion = document.getElementById('resumenContextoSeleccion');
    const detalleContextoSeleccion = document.getElementById('detalleContextoSeleccion');
    const totalRegistrosLabel = document.getElementById('totalRegistrosLabel');

    const btnBuscarFuncionario = document.getElementById('btnBuscarFuncionario');
    const btnBuscarReporte = document.getElementById('btnBuscarReporte');
    const btnExportarPDF = document.getElementById('btnExportarPDF');

    if (!ddlTipoReporte) return;

    ddlTipoReporte.addEventListener('change', function () {
        ocultarTodosLosFiltros();

        switch (this.value) {
            case 'departamento':
                filtroDepartamentoWrapper.classList.remove('d-none');
                break;
            case 'funcionario':
                filtroFuncionarioWrapper.classList.remove('d-none');
                break;
            case 'conglomerado':
                filtroConglomeradoWrapper.classList.remove('d-none');
                break;
            case 'puesto':
                filtroPuestoWrapper.classList.remove('d-none');
                break;
        }
    });

    if (btnBuscarFuncionario) {
        btnBuscarFuncionario.addEventListener('click', function () {
            const criterio = (txtBuscaFuncionario.value || '').trim();
            if (criterio === '') return;

            cedFuncionarioSeleccionado.value = "0101010101";
            resFunSel.innerText = "Juan Pérez Mora (0101010101)";
        });
    }

    if (btnBuscarReporte) {
        btnBuscarReporte.addEventListener('click', async function () {
            limpiarAlerta();

            const tipo = ddlTipoReporte.value;
            const periodo = ddlPeriodo.value;

            let filtroValor = '';
            let filtroLabel = '';

            if (!tipo) {
                mostrarError('Debe seleccionar el tipo de reporte.');
                return;
            }

            if (!periodo) {
                mostrarError('Debe seleccionar el período de evaluación.');
                return;
            }

            if (tipo === 'departamento') {
                filtroValor = ddlDepartamento.value;
                filtroLabel = ddlDepartamento.options[ddlDepartamento.selectedIndex]?.text || '';
                if (!filtroValor) {
                    mostrarError('Debe seleccionar el departamento.');
                    return;
                }
            }

            if (tipo === 'funcionario') {
                filtroValor = cedFuncionarioSeleccionado.value;
                filtroLabel = resFunSel.innerText;
                if (!filtroValor) {
                    mostrarError('Debe seleccionar el funcionario.');
                    return;
                }
            }

            if (tipo === 'conglomerado') {
                filtroValor = ddlConglomerado.value;
                filtroLabel = ddlConglomerado.options[ddlConglomerado.selectedIndex]?.text || '';
                if (!filtroValor) {
                    mostrarError('Debe seleccionar el conglomerado.');
                    return;
                }
            }

            if (tipo === 'puesto') {
                filtroValor = ddlPuesto.value;
                filtroLabel = ddlPuesto.options[ddlPuesto.selectedIndex]?.text || '';
                if (!filtroValor) {
                    mostrarError('Debe seleccionar el puesto.');
                    return;
                }
            }

            const payload = { tipoReporte: tipo, filtro: filtroValor, periodo: periodo };
            const contexto = construirContextoResumen(tipo, filtroLabel, periodo);
            resumenContextoSeleccion.innerText = contexto;
            detalleContextoSeleccion.innerText = contexto;

            await enviarPeticionReporteRRHH(payload);
        });

    }

    if (btnExportarPDF) {
        btnExportarPDF.addEventListener('click', function () {
            const tipo = ddlTipoReporte.value;
            const periodo = ddlPeriodo.value;

            let filtroValor = '';

            if (tipo === 'departamento') filtroValor = ddlDepartamento.value;
            if (tipo === 'funcionario') filtroValor = cedFuncionarioSeleccionado.value;
            if (tipo === 'conglomerado') filtroValor = ddlConglomerado.value;
            if (tipo === 'puesto') filtroValor = ddlPuesto.value;

            const dataExport = {
                tipoReporte: tipo,
                filtro: filtroValor,
                periodo: periodo
            };

            console.log('Exportar PDF con:', dataExport);
        });
    }

    function ocultarTodosLosFiltros() {
        filtroDepartamentoWrapper.classList.add('d-none');
        filtroFuncionarioWrapper.classList.add('d-none');
        filtroConglomeradoWrapper.classList.add('d-none');
        filtroPuestoWrapper.classList.add('d-none');
    }

    function mostrarError(msg) {
        if (!alertaValidacion) return;
        alertaValidacion.innerText = msg;
        alertaValidacion.classList.remove('d-none');
        alertaValidacion.classList.remove('alert-warning');
        alertaValidacion.classList.add('alert-danger');
    }

    function limpiarAlerta() {
        if (!alertaValidacion) return;
        alertaValidacion.innerText = '';
        alertaValidacion.classList.add('d-none');
    }

    function construirContextoResumen(tipo, filtroLabel, periodo) {
        let tipoTxt = '';
        if (tipo === 'departamento') tipoTxt = 'Departamento';
        if (tipo === 'funcionario') tipoTxt = 'Funcionario';
        if (tipo === 'conglomerado') tipoTxt = 'Conglomerado';
        if (tipo === 'puesto') tipoTxt = 'Puesto';

        return tipoTxt + ': ' + (filtroLabel || '-') + ' | Período ' + periodo;
    }

    async function enviarPeticionReporteRRHH(filtros) {
        try {
            const formBody = `reporteGeneralData=${encodeURIComponent(JSON.stringify(filtros))}`;

            const response = await fetch(`${urlBase}Reportes/TraerReporteGeneralRH`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: formBody
            });

            if (!response.ok) throw new Error(`Error HTTP: ${response.status}`);

            const data = await response.json();

            if (data.error) {
                throw new Error(data.error);
            }

            renderResultados(data);
            btnExportarPDF.disabled = false;

        } catch (e) {
            Swal.fire({
                title: 'Error',
                text: e.message || 'Ocurrió un error al obtener la información.',
                icon: 'error',
                confirmButtonColor: '#1E376C'
            });
        }
    }




})();
