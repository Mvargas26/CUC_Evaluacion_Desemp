(function () {
    // ============================================
    // CONFIGURACIÓN Y ELEMENTOS DEL DOM
    // ============================================
    const elementos = {
        // Dropdowns
        tipoReporte: document.getElementById('tipoReporte'),
        departamento: document.getElementById('idDepartamento'),
        conglomerado: document.getElementById('idConglomerado'),
        puesto: document.getElementById('idPuesto'),
        periodo: document.getElementById('idPeriodo'),

        // Wrappers de filtros
        filtros: {
            departamento: document.getElementById('filtroDepartamentoWrapper'),
            funcionario: document.getElementById('filtroFuncionarioWrapper'),
            conglomerado: document.getElementById('filtroConglomeradoWrapper'),
            puesto: document.getElementById('filtroPuestoWrapper')
        },

        // Búsqueda de funcionario
        buscaFuncionario: document.getElementById('buscaFuncionario'),
        cedFuncionario: document.getElementById('cedFuncionarioSeleccionado'),
        resultadoFuncionario: document.getElementById('resultadoFuncionarioSeleccionado'),

        // Alertas y resultados
        alerta: document.getElementById('alertaValidacion'),
        detalleBody: document.getElementById('tbResultadosDetalleBody'),
        detalleContexto: document.getElementById('detalleContextoSeleccion'),
        totalRegistros: document.getElementById('totalRegistrosLabel'),

        // Botones
        btnBuscarFuncionario: document.getElementById('btnBuscarFuncionario'),
        btnBuscarReporte: document.getElementById('btnBuscarReporte'),
        btnExportarPDF: document.getElementById('btnExportarPDF')
    };

    if (!elementos.tipoReporte) return;

    // ============================================
    // CONFIGURACIÓN DE TIPOS DE REPORTE
    // ============================================
    const tiposReporte = {
        departamento: { label: 'Departamento', elemento: elementos.departamento },
        funcionario: { label: 'Funcionario', elemento: elementos.cedFuncionario },
        conglomerado: { label: 'Conglomerado', elemento: elementos.conglomerado },
        puesto: { label: 'Puesto', elemento: elementos.puesto }
    };

    // ============================================
    // FUNCIONES DE UTILIDAD
    // ============================================
    const utils = {
        ocultarFiltros() {
            Object.values(elementos.filtros).forEach(filtro =>
                filtro?.classList.add('d-none')
            );
        },

        mostrarFiltro(tipo) {
            elementos.filtros[tipo]?.classList.remove('d-none');
        },

        formatearNumero(valor, decimales = 2) {
            if (valor === null || valor === undefined || valor === '') return '';
            const num = Number(valor);
            return isNaN(num) ? valor : num.toFixed(decimales);
        },

        mostrarAlerta(mensaje, tipo = 'danger') {
            if (!elementos.alerta) return;
            elementos.alerta.innerText = mensaje;
            elementos.alerta.classList.remove('d-none', 'alert-danger', 'alert-warning');
            elementos.alerta.classList.add(`alert-${tipo}`);
        },

        limpiarAlerta() {
            if (!elementos.alerta) return;
            elementos.alerta.innerText = '';
            elementos.alerta.classList.add('d-none');
        },

        obtenerTextoSeleccionado(dropdown) {
            return dropdown.options[dropdown.selectedIndex]?.text || '';
        },

        obtenerClaseBadge(nivel) {
            const clases = {
                'Excelente': 'bg-success',
                'Muy Bueno': 'bg-info',
                'Bueno': 'bg-primary',
                'Deficiente': 'bg-danger'
            };
            return clases[nivel] || 'bg-secondary';
        }
    };

    // ============================================
    // VALIDACIÓN DE FILTROS
    // ============================================
    const validacion = {
        validarFiltros(tipo, periodo) {
            if (!tipo) {
                utils.mostrarAlerta('Debe seleccionar el tipo de reporte.');
                return false;
            }

            if (!periodo) {
                utils.mostrarAlerta('Debe seleccionar el período de evaluación.');
                return false;
            }

            return true;
        },

        obtenerDatosFiltro(tipo) {
            const config = tiposReporte[tipo];
            if (!config) return null;

            let valor, label;

            if (tipo === 'funcionario') {
                valor = elementos.cedFuncionario.value;
                label = elementos.resultadoFuncionario.innerText;
            } else {
                valor = config.elemento.value;
                label = utils.obtenerTextoSeleccionado(config.elemento);
            }

            if (!valor) {
                utils.mostrarAlerta(`Debe seleccionar el ${config.label.toLowerCase()}.`);
                return null;
            }

            return { valor, label };
        },

        construirContexto(tipo, filtroLabel, periodo) {
            const tipoLabel = tiposReporte[tipo]?.label || '';
            return `${tipoLabel}: ${filtroLabel || '-'} | Período ${periodo}`;
        }
    };

    // ============================================
    // RENDERIZADO DE RESULTADOS
    // ============================================
    const render = {
        detalle(datos) {
            if (!datos?.length) {
                elementos.detalleBody.innerHTML = `
                    <tr>
                        <td colspan="5" class="text-center text-muted py-4">
                            Sin resultados para los filtros indicados.
                        </td>
                    </tr>
                `;
                elementos.totalRegistros.innerText = 'Total registros: 0';
                return;
            }

            const filas = datos.map(row => `
                <tr>
                    <td>${row.Funcionario || ''}</td>
                    <td class="text-end">${utils.formatearNumero(row.NotaFinal)}</td>
                    <td class="text-center">
                        <span class="badge ${utils.obtenerClaseBadge(row.NivelDesempeno)}">
                            ${row.NivelDesempeno || 'N/A'}
                        </span>
                    </td>
                    <td><small class="text-muted">${row.DescripcionRubro || ''}</small></td>
                    <td>${row.Observaciones || ''}</td>
                </tr>
            `).join('');

            elementos.detalleBody.innerHTML = filas;
            elementos.totalRegistros.innerText = `Total registros: ${datos.length}`;
        },

        resultados(data) {
            if (!data) {
                utils.mostrarAlerta('No se recibieron datos del servidor.');
                return;
            }

            this.detalle(data.detalle);
        }
    };

    // ============================================
    // LLAMADA AL BACKEND
    // ============================================
    const api = {
        async obtenerReporte(filtros) {
            const formBody = `reporteGeneralData=${encodeURIComponent(JSON.stringify(filtros))}`;

            const response = await fetch(`${urlBase}Reportes/TraerReporteGeneralRH`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: formBody
            });

            if (!response.ok) {
                throw new Error(`Error HTTP: ${response.status}`);
            }

            const data = await response.json();

            if (data.error) {
                throw new Error(data.error);
            }

            return data;
        }
    };

    // ============================================
    // MANEJADORES DE EVENTOS
    // ============================================
    const handlers = {
        cambioTipoReporte() {
            utils.ocultarFiltros();
            const tipo = elementos.tipoReporte.value;
            if (tipo) utils.mostrarFiltro(tipo);
        },

        async buscarFuncionario() {
            const criterio = elementos.buscaFuncionario.value?.trim();
            if (!criterio) return;

            utils.limpiarAlerta();

            try {
                const resp = await fetch(`${urlBase}Reportes/BuscarFuncionariosPorCedONombre?criterio=${encodeURIComponent(criterio)}`, {
                    method: 'GET',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                if (!resp.ok) {
                    throw new Error(`Error HTTP: ${resp.status}`);
                }

                const data = await resp.json();

                if (data.error) {
                    utils.mostrarAlerta(data.error, 'warning');
                    return;
                }

                const resultados = data.data || [];
                const lista = document.getElementById('listaResultadosFuncionario');
                if (!lista) {
                    throw new Error('No se encontró el contenedor de resultados de funcionario.');
                }

                if (!resultados.length) {
                    lista.innerHTML = `
                        <div class="list-group-item text-muted text-center">
                            Sin coincidencias
                        </div>
                    `;
                    lista.classList.remove('d-none');
                    return;
                }

                lista.innerHTML = resultados.map(f => `
                    <button type="button"
                        class="list-group-item list-group-item-action"
                        data-ced="${f.cedula}"
                        data-nombre="${f.nombreCompleto}">
                        ${f.nombreCompleto} (${f.cedula})
                    </button>
                `).join('');

                lista.classList.remove('d-none');

                lista.querySelectorAll('button').forEach(btn => {
                    btn.addEventListener('click', () => {
                        const ced = btn.getAttribute('data-ced');
                        const nom = btn.getAttribute('data-nombre');

                        elementos.cedFuncionario.value = ced;
                        elementos.resultadoFuncionario.innerText = `${nom} (${ced})`;

                        // Limpieza visual
                        elementos.buscaFuncionario.value = '';
                        utils.limpiarAlerta();

                        // Ocultar lista
                        lista.classList.add('d-none');
                        lista.innerHTML = '';
                    });
                });

            } catch (err) {
                Swal.fire({
                    title: 'Error',
                    text: err.message || 'No se pudo buscar el funcionario.',
                    icon: 'error',
                    confirmButtonColor: '#1E376C'
                });
            }
        },

        async buscarReporte() {
            utils.limpiarAlerta();

            const tipo = elementos.tipoReporte.value;
            const periodo = elementos.periodo.value;

            if (!validacion.validarFiltros(tipo, periodo)) return;

            const datosFiltro = validacion.obtenerDatosFiltro(tipo);
            if (!datosFiltro) return;

            const payload = {
                tipoReporte: tipo,
                filtro: datosFiltro.valor,
                periodo: periodo
            };

            const contexto = validacion.construirContexto(tipo, datosFiltro.label, periodo);
            elementos.detalleContexto.innerText = contexto;

            try {
                const data = await api.obtenerReporte(payload);
                render.resultados(data);
                elementos.btnExportarPDF.disabled = false;
            } catch (error) {
                Swal.fire({
                    title: 'Error',
                    text: error.message || 'Ocurrió un error al obtener la información.',
                    icon: 'error',
                    confirmButtonColor: '#1E376C'
                });
            }
        },

        exportarPDF() {
            const tipo = elementos.tipoReporte.value;
            const config = tiposReporte[tipo];
            if (!config) return;

            const dataExport = {
                tipoReporte: tipo,
                filtro: config.elemento.value,
                periodo: elementos.periodo.value
            };

            console.log('Exportar PDF con:', dataExport);
        }
    };

    // ============================================
    // INICIALIZACIÓN DE EVENTOS
    // ============================================
    elementos.tipoReporte.addEventListener('change', handlers.cambioTipoReporte);
    elementos.btnBuscarFuncionario?.addEventListener('click', handlers.buscarFuncionario);
    elementos.btnBuscarReporte?.addEventListener('click', handlers.buscarReporte);
    elementos.btnExportarPDF?.addEventListener('click', handlers.exportarPDF);

})();