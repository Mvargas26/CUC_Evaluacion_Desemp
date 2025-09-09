$(document).ready(function () {
    // Variables para mantener el estado
    let campoActualEdicion = null;
    let pesoObjetivo = 0;
    let datosOriginales = null;

    // Función para cerrar el modal y resetear
    const cerrarModal = () => {
        $('#editarActualModal').modal('hide');
        resetModal();
    };

    // Función para resetear el estado del modal
    const resetModal = () => {
        $('#modalValorActual').val('');
        $('#valorError').addClass('d-none');
        campoActualEdicion = null;
        pesoObjetivo = 0;
        datosOriginales = null;
    };

    // Función para validar el valor ingresado
    const validarValor = (valor) => {
        const numValor = parseFloat(valor);
        return !isNaN(numValor) && numValor >= 1 && numValor <= pesoObjetivo;
    };

    // Configurar el modal para evitar cierre accidental
    $('#editarActualModal').modal({
        backdrop: 'static',
        keyboard: false,
        show: false
    });

    // Manejador para abrir el modal
    $(document).on('click', '.btn-editar-actual', function () {
        const button = $(this);
        datosOriginales = {
            tipo: button.data('tipo'),
            nombre: button.data('nombre'),
            peso: button.data('peso'),
            meta: button.data('meta'),
            actual: button.data('actual'),
            id: button.data('id')
        };

        // Llenar datos en el modal
        $('#modalTipoObjetivo').text(datosOriginales.tipo);
        $('#modalNombreObjetivo').text(datosOriginales.nombre);
        $('#modalPesoObjetivo').text(`${datosOriginales.peso}%`);
        $('#modalMetaObjetivo').text(datosOriginales.meta);
        $('#modalValorActual').val(datosOriginales.actual);
        $('#maxPeso').text(datosOriginales.peso);

        // Configurar validación inicial
        pesoObjetivo = parseFloat(datosOriginales.peso);
        campoActualEdicion = button.closest('tr').find('td:nth-child(5)');

        // Mostrar modal
        $('#editarActualModal').modal('show');
    });

    // Validación en tiempo real
    $('#modalValorActual').on('input', function () {
        const valor = $(this).val();
        const errorElement = $('#valorError');

        if (validarValor(valor)) {
            errorElement.addClass('d-none');
        } else {
            errorElement.removeClass('d-none');
            errorElement.text(`El valor debe estar entre 1 y ${pesoObjetivo}%`);
        }
    });

    // Manejador para confirmar cambios
    $('#modalBtnConfirmar').click(function () {
        const nuevoValor = $('#modalValorActual').val();
        const errorElement = $('#valorError');

        if (!validarValor(nuevoValor)) {
            errorElement.text(`El valor debe estar entre 1 y ${pesoObjetivo}%`).removeClass('d-none');
            return;
        }

        // Actualizar visualmente en la tabla
        if (campoActualEdicion) {
            campoActualEdicion.text(`${nuevoValor}%`);
            campoActualEdicion.data('actual', nuevoValor); // Guardar dato para posterior envío
        }

        //llamamos la funcion de suma automatica
        actualizarSumasObjetivos();

        cerrarModal();
    });

    // Manejadores para cerrar el modal
    $('.modal-header .close, .modal-footer .btn-secondary').on('click', cerrarModal);

    // Prevenir cierre del modal al hacer clic fuera
    $('#editarActualModal').on('click.dismiss.bs.modal', function (e) {
        if (e.target === this) {
            e.preventDefault();
            e.stopPropagation();
        }
    });

    function actualizarSumasObjetivos() {
        // Resetear valores previos
        window.sumasGlobales.objetivos = {};

        // Procesar tabla de Objetivos
        document.querySelectorAll('#tbObjetivosAsignados tbody tr').forEach(row => {
            const tipoCell = row.querySelector('td:nth-child(1)'); // Columna Tipo
            const valorCell = row.querySelector('td:nth-child(5)'); // Columna Actual

            if (tipoCell && valorCell) {
                const tipo = tipoCell.textContent.trim();
                const valor = parseFloat(valorCell.textContent.replace('%', '')) || 0;

                window.sumasGlobales.objetivos[tipo] =
                    (window.sumasGlobales.objetivos[tipo] || 0) + valor;
            }
        });

        actualizarTablaResultados();
    }

});