document.addEventListener('DOMContentLoaded', function () {
    $("#header").load("blocks/header.html", function() {
        updateMenu();
    });
    $("#footer").load("blocks/footer.html");

    function updateMenu() {
        const token = localStorage.getItem('token');
        const loginLink = document.getElementById('login-link');
        const registerLink = document.getElementById('register-link');
        const profileLink = document.getElementById('profile-link');
        const logoutLink = document.getElementById('logout-link');

        if (token) {
            loginLink.style.display = 'none';
            registerLink.style.display = 'none';
            profileLink.style.display = 'list-item';
            logoutLink.style.display = 'list-item';

            const logoutBtn = document.getElementById('logout-btn');
            if (logoutBtn) {
                logoutBtn.addEventListener('click', function (e) {
                    e.preventDefault();
                    localStorage.removeItem('token');
                    window.location.href = 'main.html';
                });
            }
        } else {
            loginLink.style.display = 'list-item';
            registerLink.style.display = 'list-item';
            profileLink.style.display = 'none';
            logoutLink.style.display = 'none';
        }
    }
});