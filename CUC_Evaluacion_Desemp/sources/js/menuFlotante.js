document.addEventListener('DOMContentLoaded', function () {

    document.querySelectorAll('.main-menu-item').forEach(item => {
        item.addEventListener('click', function (e) {
            e.preventDefault();
            const target = this.getAttribute('data-target');
            showFloatingMenu(target);
        });
    });

    function showFloatingMenu(menuKey) {

        if (!window.menuData) {
            console.error("menuData no está definido");
            return;
        }

        if (!window.menuData[menuKey]) {
            console.error(`No existe el menú: ${menuKey}`);
            return;
        }
        const container = document.getElementById('floating-menu-container');
        const menu = window.menuData[menuKey]; // Accede al objeto global

        let menuHTML = `
            <button class="close-floating-menu">&times;</button>
            <div class="floating-menu">
        `;

        menu.sections.forEach(section => {
            menuHTML += `
                <div class="menu-section">
                    <h3>${section.title}</h3>
                    <ul>
            `;

            section.items.forEach(item => {
                menuHTML += `
                    <li><a href="${item.url}">${item.text}</a></li>
                `;
            });

            menuHTML += `
                    </ul>
                </div>
            `;
        });

        menuHTML += `</div>`;
        container.innerHTML = menuHTML;
        container.style.display = 'flex';

        // Evento para cerrar el menú
        container.querySelector('.close-floating-menu').addEventListener('click', function () {
            container.style.display = 'none';
        });
    }

    // Cerrar menú al hacer clic fuera
    document.addEventListener('click', function (e) {
        const container = document.getElementById('floating-menu-container');
        const sidebar = document.querySelector('.sidebar');

        if (container.style.display === 'flex' &&
            !container.contains(e.target) &&
            !sidebar.contains(e.target)) {
            container.style.display = 'none';
        }
    });
});