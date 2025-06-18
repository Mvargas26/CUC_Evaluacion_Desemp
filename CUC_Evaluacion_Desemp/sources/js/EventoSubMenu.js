document.addEventListener("DOMContentLoaded", function () {
    const toggles = document.querySelectorAll(".toggle-submenu");

    toggles.forEach(toggle => {
        toggle.addEventListener("click", function (e) {
            e.preventDefault();
            const parentLi = this.closest("li");
            parentLi.classList.toggle("active");

            // Alternar ícono de flecha
            const icon = this.querySelector("i");
            if (icon) {
                icon.classList.toggle("fa-chevron-down");
                icon.classList.toggle("fa-chevron-up");
            }
        });
    });
});
