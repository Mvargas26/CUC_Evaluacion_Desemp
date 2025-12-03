(function () {
    // ============================================
    // VARIABLES GLOBALES
    // ============================================
    let datosReporteActual = null;
    let filtrosActuales = null;
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
        },
        mostrarCargando(enMostrar = true) {
            if (enMostrar) {
                elementos.detalleBody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center py-4">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Cargando...</span>
                    </div>
                    <p class="mt-2 text-muted">Buscando datos...</p>
                </td>
            </tr>
        `;
                elementos.btnBuscarReporte.disabled = true;
                elementos.btnBuscarReporte.innerHTML = '<i class="bi bi-hourglass-split"></i> Procesando...';
            } else {
                elementos.btnBuscarReporte.disabled = false;
                elementos.btnBuscarReporte.innerHTML = '<i class="bi bi-search"></i> Buscar Reporte';
            }
        },

        mostrarAlertaTemporal(mensaje, tipo = 'success') {
            // Simple alerta sin SweetAlert
            const alertaTemp = document.createElement('div');
            alertaTemp.className = `alert alert-${tipo} alert-dismissible fade show position-fixed top-0 end-0 m-3`;
            alertaTemp.style.zIndex = '9999';
            alertaTemp.innerHTML = `
        ${mensaje}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
            document.body.appendChild(alertaTemp);

            setTimeout(() => {
                alertaTemp.remove();
            }, 3000);
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
            const periodoText = utils.obtenerTextoSeleccionado(elementos.periodo);

            if (!validacion.validarFiltros(tipo, periodo)) return;

            const datosFiltro = validacion.obtenerDatosFiltro(tipo);
            if (!datosFiltro) return;

            // Guardar filtros actuales para exportar
            filtrosActuales = {
                tipoReporte: tipo,
                filtro: datosFiltro.valor,
                periodo: periodo,
                periodoText: periodoText
            };

            // Usar el texto completo del periodo, no solo el value
            const contexto = validacion.construirContexto(tipo, datosFiltro.label, periodoText);
            elementos.detalleContexto.innerText = contexto;

            // Usar mostrarCargando si lo agregaste
            if (utils.mostrarCargando) {
                utils.mostrarCargando(true);
            } else {
                // Fallback si no está definida
                elementos.detalleBody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center py-4">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Cargando...</span>
                    </div>
                </td>
            </tr>
        `;
            }

            try {
                const data = await api.obtenerReporte(filtrosActuales);
                datosReporteActual = data;

                // Asegúrate que render.resultados pueda manejar la respuesta
                if (Array.isArray(data)) {
                    render.detalle(data);
                } else {
                    render.resultados(data);
                }

                elementos.btnExportarPDF.disabled = false;

            } catch (error) {
                Swal.fire({
                    title: 'Error',
                    text: error.message || 'Ocurrió un error al obtener la información.',
                    icon: 'error',
                    confirmButtonColor: '#1E376C'
                });

                elementos.btnExportarPDF.disabled = true;
            } finally {
                if (utils.mostrarCargando) {
                    utils.mostrarCargando(false);
                } else {
                    elementos.btnBuscarReporte.disabled = false;
                    elementos.btnBuscarReporte.innerHTML = '<i class="bi bi-search"></i> Buscar Reporte';
                }
            }
        },

        exportarPDF: async function () {
            if (!filtrosActuales) {
                utils.mostrarAlerta('Primero debe generar un reporte.', 'warning');
                return;
            }

            try {
                const body = `reporteGeneralData=${encodeURIComponent(JSON.stringify(filtrosActuales))}`;

                const response = await fetch(`${urlBase}Reportes/ExportarReportePDFGeneral`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    body: body
                });

                if (!response.ok) {
                    throw new Error(`Error HTTP: ${response.status}`);
                }

                const data = await response.json();

                if (!data.success) {
                    throw new Error(data.error || "No se pudo generar el PDF");
                }

                await Swal.fire({
                    title: 'PDF generado correctamente',
                    text: '¿Desea abrir el reporte?',
                    icon: 'success',
                    showCancelButton: true,
                    confirmButtonText: 'Abrir PDF',
                    cancelButtonText: 'Cerrar',
                    confirmButtonColor: '#1E376C',
                    cancelButtonColor: '#6c757d'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.open(data.rutaDescarga, '_blank');
                    }
                });

            } catch (error) {
                Swal.fire({
                    title: 'Error',
                    text: error.message,
                    icon: 'error',
                    confirmButtonColor: '#1E376C'
                });
            }
        }

    };

    // ============================================
    // INICIALIZACIÓN DE EVENTOS
    // ============================================
    elementos.tipoReporte.addEventListener('change', handlers.cambioTipoReporte);
    elementos.btnBuscarFuncionario?.addEventListener('click', handlers.buscarFuncionario);
    elementos.btnBuscarReporte?.addEventListener('click', handlers.buscarReporte);
    elementos.btnExportarPDF?.addEventListener('click', handlers.exportarPDF);

    elementos.periodo.addEventListener('change', () => {
        elementos.btnExportarPDF.disabled = true;
        datosReporteActual = null;

        if (datosReporteActual === null) {
            elementos.detalleBody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center text-muted py-4">
                    Sin resultados. Aún no se ha realizado la búsqueda.
                </td>
            </tr>
        `;
            elementos.totalRegistros.innerText = 'Total registros: 0';
            elementos.detalleContexto.innerText = '';
        }
    });
})();