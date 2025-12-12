document.addEventListener('DOMContentLoaded', () => {
    // Toggle menú usuario
    const userIcon = document.getElementById('userIcon');
    const userMenu = document.getElementById('userMenu');

    if (userIcon) {
        userIcon.addEventListener('click', () => {
            userMenu.classList.toggle('show');
        });
    }
});
function guardarPassword() {
    const form = document.getElementById("formCambiarPassword");
    const pass = form.Password.value.trim();
    const conf = form.Confirmar.value.trim();
    const error = document.getElementById("passError");

    error.style.display = "none";

    if (!pass || !conf) {
        error.innerText = "Debe completar ambos campos.";
        error.style.display = "block";
        return;
    }

    if (pass !== conf) {
        error.innerText = "Las contraseñas no coinciden.";
        error.style.display = "block";
        return;
    }

    fetch('/Auth/CambiarPasswordAjax', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ password: pass })
    })
        .then(resp => resp.json())
        .then(data => {
            if (data.ok) {
                Swal.fire("Correcto", "El password fue actualizado.", "success");
                bootstrap.Modal.getInstance(document.getElementById("modalCambiarPassword")).hide();
            } else {
                Swal.fire("Error", data.mensaje || "No se pudo actualizar el password.", "error");
            }
        });
}

//funcion para mostrar/ocultar el password
function togglePass(idCampo, btn) {
    const input = document.getElementById(idCampo);
    const icon = btn.querySelector("i");

    if (input.type === "password") {
        input.type = "text";
        icon.classList.remove("bi-eye");
        icon.classList.add("bi-eye-slash");
    } else {
        input.type = "password";
        icon.classList.remove("bi-eye-slash");
        icon.classList.add("bi-eye");
    }
}
