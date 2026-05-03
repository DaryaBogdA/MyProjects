document.addEventListener('DOMContentLoaded', async () => {
    const uid = localStorage.getItem('userId');
    if (!uid) {
        window.location.href = '/login.html';
        return;
    }

    const root = document.getElementById('bookingsRoot');
    const tabMyBtn = document.getElementById('tabMyBtn');
    const tabIncomingBtn = document.getElementById('tabIncomingBtn');
    let mode = 'my';

    function esc(s) {
        if (!s) return '';
        return String(s)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    function fmtDate(s) {
        const d = new Date(s);
        if (Number.isNaN(d.getTime())) return esc(s);
        return d.toLocaleDateString('ru-RU');
    }

    function statusBadge(status) {
        const val = String(status || '').toLowerCase();
        const map = {
            pending: ['#fff4e6', '#b45309', 'Ожидает'],
            approved: ['#e8f3f2', '#2c7873', 'Подтверждено'],
            rejected: ['#fee2e2', '#b91c1c', 'Отклонено'],
        };
        const [bg, fg, text] = map[val] || ['#f3f4f6', '#6b7280', val || '—'];
        return `<span style="padding:4px 10px;border-radius:999px;background:${bg};color:${fg};font-size:0.82rem;font-weight:600;">${text}</span>`;
    }

    async function loadMyBookings() {
        const res = await fetch('/api/bookings/my', { headers: { 'X-User-ID': uid } });
        const data = await res.json().catch(() => []);
        if (!res.ok) throw new Error(data.error || 'Не удалось загрузить бронирования');
        return Array.isArray(data) ? data : [];
    }

    async function loadIncomingBookings() {
        const res = await fetch('/api/bookings/incoming', { headers: { 'X-User-ID': uid } });
        const data = await res.json().catch(() => []);
        if (!res.ok) throw new Error(data.error || 'Не удалось загрузить входящие заявки');
        return Array.isArray(data) ? data : [];
    }

    async function updateStatus(id, status) {
        const res = await fetch(`/api/bookings/${id}/status`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', 'X-User-ID': uid },
            body: JSON.stringify({ status }),
        });
        const data = await res.json().catch(() => ({}));
        if (!res.ok) throw new Error(data.error || 'Не удалось обновить статус');
    }

    async function render() {
        root.innerHTML = '<p style="color:var(--text-muted);">Загрузка...</p>';
        try {
            if (mode === 'my') {
                const items = await loadMyBookings();
                if (items.length === 0) {
                    root.innerHTML = '<p style="color:var(--text-muted);">У вас пока нет заявок на бронирование.</p>';
                    return;
                }
                root.innerHTML = items.map((b) => `
                    <article style="border:1px solid var(--border-color); border-radius:12px; padding:12px; background:white;">
                        <div style="display:flex;justify-content:space-between;gap:8px;align-items:center;flex-wrap:wrap;">
                            <strong>${esc(b.listing_title || 'Объявление')}</strong>
                            ${statusBadge(b.status)}
                        </div>
                        <div style="margin-top:8px;color:var(--text-muted);font-size:0.92rem;">
                            ${fmtDate(b.check_in)} - ${fmtDate(b.check_out)}
                        </div>
                        <div style="margin-top:8px;"><a href="/listing-details.html?id=${b.listing_id}" class="btn btn-outline btn-sm">Открыть объявление</a></div>
                    </article>
                `).join('');
                return;
            }

            const incoming = await loadIncomingBookings();
            if (incoming.length === 0) {
                root.innerHTML = '<p style="color:var(--text-muted);">На ваши объявления пока нет заявок.</p>';
                return;
            }
            root.innerHTML = incoming.map((b) => `
                <article style="border:1px solid var(--border-color); border-radius:12px; padding:12px; background:white;">
                    <div style="display:flex;justify-content:space-between;gap:8px;align-items:center;flex-wrap:wrap;">
                        <strong>${esc(b.listing_title || 'Объявление')}</strong>
                        ${statusBadge(b.status)}
                    </div>
                    <div style="margin-top:8px;color:var(--text-muted);font-size:0.92rem;">
                        Гость: ${esc(b.guest_name || 'Пользователь')}<br>
                        Период: ${fmtDate(b.check_in)} - ${fmtDate(b.check_out)}
                    </div>
                    <div style="margin-top:10px;display:flex;gap:8px;flex-wrap:wrap;">
                        <a href="/listing-details.html?id=${b.listing_id}" class="btn btn-outline btn-sm">Объявление</a>
                        ${String(b.status).toLowerCase() === 'pending' ? `
                            <button class="btn btn-primary btn-sm" data-action="approve" data-id="${b.id}">Подтвердить</button>
                            <button class="btn btn-outline btn-sm" data-action="reject" data-id="${b.id}">Отклонить</button>
                        ` : ''}
                    </div>
                </article>
            `).join('');

            root.querySelectorAll('button[data-action]').forEach((btn) => {
                btn.addEventListener('click', async () => {
                    const id = Number(btn.dataset.id);
                    const action = btn.dataset.action;
                    try {
                        await updateStatus(id, action === 'approve' ? 'approved' : 'rejected');
                        await render();
                    } catch (e) {
                        alert(e.message || 'Ошибка');
                    }
                });
            });
        } catch (e) {
            root.innerHTML = `<p class="error">${esc(e.message || 'Ошибка')}</p>`;
        }
    }

    tabMyBtn?.addEventListener('click', async () => {
        mode = 'my';
        tabMyBtn.classList.add('btn-primary');
        tabMyBtn.classList.remove('btn-outline');
        tabIncomingBtn.classList.add('btn-outline');
        tabIncomingBtn.classList.remove('btn-primary');
        await render();
    });

    tabIncomingBtn?.addEventListener('click', async () => {
        mode = 'incoming';
        tabIncomingBtn.classList.add('btn-primary');
        tabIncomingBtn.classList.remove('btn-outline');
        tabMyBtn.classList.add('btn-outline');
        tabMyBtn.classList.remove('btn-primary');
        await render();
    });

    await render();
});
