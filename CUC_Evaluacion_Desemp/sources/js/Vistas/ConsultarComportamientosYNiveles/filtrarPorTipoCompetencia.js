$(document).ready(function () {
    // Guardamos todas las opciones originales del combo de competencias
    var $selectCompetencia = $('#selectCompetencia');
    var originalOptions = $selectCompetencia.html();

    // detectamos el cambio del combo de tipo competencia
    $('#selectTipoCompetencia').change(function () {
        var selectedTipoId = $(this).val();

        // Limpiamos la tabla
        limpiarTabla();

        if (selectedTipoId) {
            // Habilitar el combo de competencias
            $selectCompetencia.prop('disabled', false);

            // Restaurar todas las opciones
            $selectCompetencia.html(originalOptions);

            // Filtrar mostrando solo las competencias del tipo seleccionado
            $selectCompetencia.find('option').each(function () {
                var $option = $(this);
                // solo si coincide con el tipo o es la opción por defecto
                if ($option.val() && $option.data('tipocompetencia') != selectedTipoId) {
                    $option.hide();
                } else {
                    $option.show();
                }
            });

            // Restablecer la selección
            $selectCompetencia.val('');
        } else {
            // Si no hay tipo seleccionado, deshabilitar y resetear
            $selectCompetencia.prop('disabled', true).val('');
        }
    });
});

function limpiarTabla() {
    // Limpiar el título de la tabla
    $('#competenciaTitleRow th').text('');

    // Limpiar la descripción
    $('#competenciaDescRow td').text('');

    // Limpiar el cuerpo de la tabla
    $('#previewBody').empty();
}
