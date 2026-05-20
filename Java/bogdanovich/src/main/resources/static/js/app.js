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
