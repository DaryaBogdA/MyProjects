document.addEventListener('DOMContentLoaded', function () {
    const siteBase = window.SITE_BASE || '/api/site/';

    $('#header').load(siteBase + 'blocks/header.html', function (_response, status) {
        if (status === 'error') {
            console.error('Не удалось загрузить header:', siteBase + 'blocks/header.html');
            return;
        }
        updateMenu();
        bindMobileMenu();
    });

    $('#footer').load(siteBase + 'blocks/footer.html');

    function updateMenu() {
        const token = localStorage.getItem('token');
        const loginLink = document.getElementById('login-link');
        const registerLink = document.getElementById('register-link');
        const profileLink = document.getElementById('profile-link');
        const logoutLink = document.getElementById('logout-link');
        const adminLink = document.getElementById('admin-link');

        if (!loginLink || !registerLink || !profileLink || !logoutLink) {
            return;
        }

        if (token) {
            loginLink.style.display = 'none';
            registerLink.style.display = 'none';
            profileLink.style.display = 'list-item';
            logoutLink.style.display = 'list-item';

            if (typeof loadCurrentUser === 'function') {
                loadCurrentUser().then(function (user) {
                    if (adminLink && user && user.role === 'ADMIN') {
                        adminLink.style.display = 'list-item';
                    }
                });
            }

            const logoutBtn = document.getElementById('logout-btn');
            if (logoutBtn) {
                logoutBtn.addEventListener('click', function (e) {
                    e.preventDefault();
                    localStorage.removeItem('token');
                    window.location.href = siteBase + 'main.html';
                });
            }
        } else {
            loginLink.style.display = 'list-item';
            registerLink.style.display = 'list-item';
            profileLink.style.display = 'none';
            logoutLink.style.display = 'none';
            if (adminLink) adminLink.style.display = 'none';
        }
    }

    function bindMobileMenu() {
        const header = document.querySelector('header');
        const menuToggle = document.querySelector('.menu-toggle');
        const navLinks = document.querySelector('.nav-links');
        if (!header || !menuToggle || !navLinks) return;

        menuToggle.addEventListener('click', function () {
            navLinks.classList.toggle('active');
            const icon = menuToggle.querySelector('i');
            if (!icon) return;
            if (navLinks.classList.contains('active')) {
                icon.classList.replace('fa-bars', 'fa-times');
            } else {
                icon.classList.replace('fa-times', 'fa-bars');
            }
        });
    }
});


document.addEventListener('DOMContentLoaded', function() {
    const menuToggle = document.querySelector('.menu-toggle');
    const navLinks = document.querySelector('.nav-links');
    if (menuToggle && navLinks) {
        menuToggle.addEventListener('click', function() {
            navLinks.classList.toggle('active');
            const icon = menuToggle.querySelector('i');
            if (icon) {
                if (navLinks.classList.contains('active')) {
                    icon.classList.remove('fa-bars');
                    icon.classList.add('fa-times');
                } else {
                    icon.classList.remove('fa-times');
                    icon.classList.add('fa-bars');
                }
            }
        });

        navLinks.querySelectorAll('a').forEach(link => {
            link.addEventListener('click', () => {
                navLinks.classList.remove('active');
                const icon = menuToggle.querySelector('i');
                if (icon) {
                    icon.classList.remove('fa-times');
                    icon.classList.add('fa-bars');
                }
            });
        });
    }
});