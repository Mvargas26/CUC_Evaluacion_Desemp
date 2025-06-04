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
