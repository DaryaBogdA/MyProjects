function todayIso() {
    const d = new Date();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return d.getFullYear() + '-' + m + '-' + day;
}

function setMinDateToday(selector) {
    $(selector).attr('min', todayIso());
}

function authHeaders() {
    const token = localStorage.getItem('token');
    return token ? { 'Authorization': 'Bearer ' + token } : {};
}

function normalizeImage(url) {
    if (!url) return 'https://images.unsplash.com/photo-1488646953014-85cb44e25828?auto=format&fit=crop&w=800&q=80';
    if (url.startsWith('http://') || url.startsWith('https://') || url.startsWith('/api/assets/') || url.startsWith('/api/uploads/')) {
        return url;
    }
    if (url.startsWith('../static/')) return '/api/assets/' + url.substring('../static/'.length);
    if (url.startsWith('/static/')) return '/api/assets/' + url.substring('/static/'.length);
    return '/api/assets/img/countries/' + url;
}

function statusLabel(status) {
    const map = {
        PENDING: 'На рассмотрении',
        CONFIRMED: 'Подтверждено',
        CANCELLED: 'Отменено',
        PAID: 'Оплачено',
        COMPLETED: 'Завершено'
    };
    return map[status] || status;
}

function canDownloadBookingDoc(status) {
    return status === 'CONFIRMED' || status === 'PAID' || status === 'COMPLETED';
}

async function downloadBookingDocument(bookingId) {
    const res = await fetch(API_BASE + '/bookings/' + bookingId + '/document', {
        headers: authHeaders()
    });
    if (!res.ok) {
        let msg = 'Не удалось скачать файл';
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch (e) { /* ignore */ }
        alert(msg);
        return;
    }
    const blob = await res.blob();
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'travelcanvas-booking-' + bookingId + '.txt';
    document.body.appendChild(a);
    a.click();
    a.remove();
    URL.revokeObjectURL(url);
}

function requireAuth(redirectTo) {
    if (!localStorage.getItem('token')) {
        window.location.href = redirectTo || (window.SITE_BASE + 'login.html');
        return false;
    }
    return true;
}

async function loadCurrentUser() {
    try {
        return await $.ajax({
            url: API_BASE + '/auth/me',
            method: 'GET',
            headers: authHeaders()
        });
    } catch (e) {
        return null;
    }
}
(function() {
    const hotkeys = {
        'h': { url: 'main.html', desc: 'Главная' },
        't': { url: 'chooseTour.html', desc: 'Выбрать тур' },
        'c': { url: 'tourCreate.html', desc: 'Собрать тур' },
        'm': { url: 'myTrips.html', desc: 'Мои туры' },
        'b': { url: 'myBookings.html', desc: 'Мои бронирования' },
        'p': { url: 'profile.html', desc: 'Профиль' },
        '?': { url: 'help.html', desc: 'Помощь' }
    };

    function addAdminHotkeys() {
        hotkeys['a'] = { url: 'admin.html', desc: 'Админ-панель' };
        showHotkeyHint();
    }

    function showHotkeyHint() {
        if (document.getElementById('hotkeyHint')) return;
        const hint = document.createElement('div');
        hint.id = 'hotkeyHint';
        hint.style.cssText = `
            position: fixed;
            bottom: 20px;
            right: 20px;
            background: #1e293b;
            color: #f8fafc;
            padding: 12px 18px;
            border-radius: 16px;
            font-size: 0.8rem;
            font-family: monospace;
            z-index: 9999;
            box-shadow: 0 4px 12px rgba(0,0,0,0.2);
            backdrop-filter: blur(4px);
            opacity: 0.9;
            transition: opacity 0.3s;
            cursor: pointer;
        `;
        let text = ' Горячие клавиши (Alt + буква):<br>';
        for (const [key, val] of Object.entries(hotkeys)) {
            text += `&nbsp;&nbsp;<kbd style="background:#0f172a;padding:2px 6px;border-radius:6px;">Alt+${key.toUpperCase()}</kbd> → ${val.desc}<br>`;
        }
        hint.innerHTML = text + '<span style="font-size:0.7rem;">(нажмите, чтобы скрыть)</span>';
        document.body.appendChild(hint);
        hint.addEventListener('click', () => {
            hint.style.opacity = '0';
            setTimeout(() => hint.remove(), 300);
        });
        setTimeout(() => {
            if (hint && hint.style.opacity !== '0') {
                hint.style.opacity = '0';
                setTimeout(() => hint.remove(), 300);
            }
        }, 15000);
    }

    function onKeyDown(e) {
        if (!e.altKey) return;
        const target = e.target;
        if (target.tagName === 'INPUT' || target.tagName === 'TEXTAREA' || target.tagName === 'SELECT') return;
        const key = e.key.toLowerCase();
        if (hotkeys[key]) {
            e.preventDefault();
            const url = hotkeys[key].url;
            if (url) window.location.href = url;
        }
        if (e.ctrlKey && e.shiftKey && e.key === 'S' && window.location.pathname.includes('tourCreate.html')) {
            e.preventDefault();
            const saveBtn = document.getElementById('saveTripBtn');
            if (saveBtn) saveBtn.click();
        }
        if (e.ctrlKey && e.shiftKey && e.key === 'B' && window.location.pathname.includes('tourCreate.html')) {
            e.preventDefault();
            const bookBtn = document.getElementById('bookTripBtn');
            if (bookBtn && !bookBtn.disabled) bookBtn.click();
        }
    }

    async function initHotkeys() {
        showHotkeyHint();
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const user = await $.ajax({
                    url: API_BASE + '/auth/me',
                    headers: authHeaders(),
                });
                if (user && user.role === 'ADMIN') {
                    addAdminHotkeys();
                }
            } catch (e) {  }
        }
        window.addEventListener('keydown', onKeyDown);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initHotkeys);
    } else {
        initHotkeys();
    }
})();


