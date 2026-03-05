function switchAuthTab(tab) {
    const loginTab = document.getElementById('loginTab');
    const registerTab = document.getElementById('registerTab');
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');

    if (tab === 'login') {
        loginTab.classList.add('active');
        registerTab.classList.remove('active');
        loginForm.classList.add('active');
        registerForm.classList.remove('active');
    } else {
        registerTab.classList.add('active');
        loginTab.classList.remove('active');
        registerForm.classList.add('active');
        loginForm.classList.remove('active');
    }
}

const API_BASE = 'http://localhost:8079/api';

function showNotification(message, type) {
    const el = document.getElementById('notification');
    const msg = document.getElementById('notificationMessage');
    if (el && msg) {
        msg.textContent = message;
        el.style.borderLeftColor = type === 'success' ? '#28a745' : type === 'warning' ? '#ffc107' : '#9370DB';
        el.style.display = 'block';
        setTimeout(() => el.style.display = 'none', 4000);
    }
}

function handleLogin(event) {
    if (event) event.preventDefault();

    const email = $('#loginEmail').val().trim();
    const password = $('#loginPassword').val();

    $.ajax({
        url: API_BASE + '/auth/login',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ email, password }),
        success: function(response) {
            const token = response.token;
            localStorage.setItem('token', token);

            const decoded = jwt_decode(token);
            const user = {
                id: decoded.userId || decoded.sub,
                email: decoded.sub,
                firstName: decoded.firstName || '',
                lastName: decoded.lastName || '',
                role: decoded.role || 'USER',
                isAdmin: decoded.role === 'ADMIN'
            };
            localStorage.setItem('currentUser', JSON.stringify(user));

            showNotification('Добро пожаловать!', 'success');
            setTimeout(() => {
                if (user.isAdmin) {
                    window.location.href = 'admin.html';
                } else {
                    window.location.href = 'index.html';
                }
            }, 500);
        },
        error: function(xhr) {
            let errorMsg = 'Неверный email или пароль';
            if (xhr.responseJSON) {
                errorMsg = xhr.responseJSON.error || xhr.responseJSON.message || errorMsg;
            } else if (xhr.responseText) {
                try {
                    const parsed = JSON.parse(xhr.responseText);
                    errorMsg = parsed.error || parsed.message || errorMsg;
                } catch (_) {}
            }
            showNotification(errorMsg, 'warning');
        }
    });
    return false;
}

function handleRegister(event) {
    if (event) event.preventDefault();

    const firstName = $('#regName').val().trim();
    const lastName = $('#regLastName').val().trim();
    const email = $('#regEmail').val().trim();
    const password = $('#regPassword').val();

    if (!firstName || !lastName || !email || !password) {
        showNotification('Заполните все поля', 'warning');
        return;
    }
    if (password.length < 8) {
        showNotification('Пароль должен содержать минимум 8 символов', 'warning');
        return;
    }

    $.ajax({
        url: API_BASE + '/auth/register',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ firstName, lastName, email, password }),
        success: function(response) {
            showNotification(response.message || 'Регистрация прошла успешно! Войдите в систему.', 'success');

            switchAuthTab('login');
            $('#loginEmail').val(email);
            $('#loginPassword').val('');
        },
        error: function(xhr) {
            let errorMsg = 'Ошибка регистрации';
            if (xhr.responseJSON) {
                errorMsg = xhr.responseJSON.error || xhr.responseJSON.message || errorMsg;
            } else if (xhr.responseText) {
                try {
                    const parsed = JSON.parse(xhr.responseText);
                    errorMsg = parsed.error || parsed.message || errorMsg;
                } catch (_) {}
            }
            showNotification(errorMsg, 'warning');
        }
    });
    return false;
}