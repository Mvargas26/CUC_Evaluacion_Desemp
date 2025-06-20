$(document).ready(function () {
    // Ocultar con efecto fade out después de 10 segundos
    setTimeout(function () {
        $('.alert-success, .alert-danger').fadeOut(1000, function () {
            $(this).remove();
        });
    }, 10000);

    // Agregar botón de cerrar
    $('.alert').append('<button type="button" class="btn-close position-absolute end-0 me-2" data-bs-dismiss="alert" aria-label="Close"></button>')
        .css('position', 'relative');
});
