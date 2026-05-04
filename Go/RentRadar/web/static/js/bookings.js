document.addEventListener('DOMContentLoaded', async () => {
    const uid = localStorage.getItem('userId');
    if (!uid) {
        window.location.href = '/login.html';
        return;
    }

    const root = document.getElementById('bookingsRoot');
    const tabMyBtn = document.getElementById('tabMyBtn');
    const tabIncomingBtn = document.getElementById('tabIncomingBtn');
    const myCountBadge = document.getElementById('myCountBadge');
    const incomingCountBadge = document.getElementById('incomingCountBadge');
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
        if (!s) return '—';
        const d = new Date(s);
        if (Number.isNaN(d.getTime())) return esc(s);
        return d.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' });
    }

    function getStatusBadge(status) {
        const val = String(status || '').toLowerCase();
        const statusConfig = {
            pending: { icon: 'fa-clock', text: 'Ожидает подтверждения', color: 'pending' },
            approved: { icon: 'fa-check-circle', text: 'Подтверждено', color: 'approved' },
            rejected: { icon: 'fa-times-circle', text: 'Отклонено', color: 'rejected' },
            cancelled: { icon: 'fa-ban', text: 'Отменено', color: 'cancelled' }
        };
        const config = statusConfig[val] || statusConfig.pending;
        return `<div class="booking-status ${config.color}"><i class="fas ${config.icon}"></i> ${config.text}</div>`;
    }

    function formatPrice(price) {
        if (!price || price === 0) return '—';
        return Number(price).toLocaleString() + ' BYN';
    }

    async function loadMyBookings() {
        const res = await fetch('/api/bookings/my', { headers: { 'X-User-ID': uid } });
        const data = await res.json().catch(() => []);
        if (!res.ok) throw new Error(data.error || 'Не удалось загрузить бронирования');

        const enriched = [];
        for (const item of (Array.isArray(data) ? data : [])) {
            try {
                const listingRes = await fetch(`/api/listings/${item.listing_id}`, { headers: { 'X-User-ID': uid } });
                const listing = await listingRes.json();
                enriched.push({
                    ...item,
                    price: listing.price,
                    photos: listing.photos,
                    listing_title: listing.title || item.listing_title
                });
            } catch {
                enriched.push(item);
            }
        }
        return enriched;
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

    function renderBookingCard(booking, isIncoming = false) {
        const firstPhoto = booking.photos ? booking.photos.split(',')[0] : 'https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=400&h=250&fit=crop';

        let actionsHtml = '';
        if (isIncoming && booking.status === 'pending') {
            actionsHtml = `
                <div class="booking-actions">
                    <button class="btn-booking btn-booking-primary" data-action="approve" data-id="${booking.id}">
                        <i class="fas fa-check"></i> Подтвердить
                    </button>
                    <button class="btn-booking btn-booking-danger" data-action="reject" data-id="${booking.id}">
                        <i class="fas fa-times"></i> Отклонить
                    </button>
                </div>
            `;
        } else if (!isIncoming && booking.status === 'pending') {
            actionsHtml = `
                <div class="booking-actions">
                    <a href="/listing-details.html?id=${booking.listing_id}" class="btn-booking btn-booking-outline" style="text-decoration: none;">
                        <i class="fas fa-info-circle"></i> К объявлению
                    </a>
                    <button class="btn-booking btn-booking-danger" data-action="cancel" data-id="${booking.id}">
                        <i class="fas fa-ban"></i> Отменить
                    </button>
                </div>
            `;
        } else {
            actionsHtml = `
                <div class="booking-actions">
                    <a href="/listing-details.html?id=${booking.listing_id}" class="btn-booking btn-booking-outline" style="text-decoration: none; width: 100%;">
                        <i class="fas fa-info-circle"></i> Перейти к объявлению
                    </a>
                </div>
            `;
        }

        return `
            <div class="booking-card" data-id="${booking.id}">
                <div class="booking-image">
                    <img src="${esc(firstPhoto)}" alt="${esc(booking.listing_title || 'Объявление')}">
                    ${getStatusBadge(booking.status)}
                </div>
                <div class="booking-content">
                    <div class="booking-title">
                        <a href="/listing-details.html?id=${booking.listing_id}">${esc(booking.listing_title || 'Объявление')}</a>
                    </div>
                    
                    <div class="booking-dates">
                        <div class="booking-date-item">
                            <div class="booking-date-label">Заезд</div>
                            <div class="booking-date-value"><i class="fas fa-calendar-alt"></i> ${fmtDate(booking.check_in)}</div>
                        </div>
                        <div class="booking-date-arrow">
                            <i class="fas fa-arrow-right"></i>
                        </div>
                        <div class="booking-date-item">
                            <div class="booking-date-label">Выезд</div>
                            <div class="booking-date-value"><i class="fas fa-calendar-check"></i> ${fmtDate(booking.check_out)}</div>
                        </div>
                    </div>
                    
                    <div class="booking-meta">
                        ${isIncoming ? `
                            <div class="booking-guest">
                                <i class="fas fa-user"></i>
                                <span>${esc(booking.guest_name || 'Пользователь')}</span>
                            </div>
                        ` : `
                            <div class="booking-guest">
                                <i class="fas fa-clock"></i>
                                <span>Создана: ${fmtDate(booking.created_at)}</span>
                            </div>
                        `}
                        <div class="booking-price">
                            <i class="fas fa-tag"></i>
                            <span>${formatPrice(booking.price)}</span>
                        </div>
                    </div>
                    
                    ${actionsHtml}
                </div>
            </div>
        `;
    }

    function renderEmptyState(type) {
        if (type === 'my') {
            return `
                <div style="grid-column: 1 / -1;">
                    <div class="bookings-empty">
                        <i class="far fa-calendar-check"></i>
                        <h3>Пока нет бронирований</h3>
                        <p>Найдите подходящее жильё и отправьте первую заявку на бронирование</p>
                        <a href="/index.html" class="btn btn-primary">
                            Найти жильё
                        </a>
                    </div>
                </div>
            `;
        } else {
            return `
                <div style="grid-column: 1 / -1;">
                    <div class="bookings-empty">
                        <i class="far fa-envelope-open"></i>
                        <h3>Нет входящих заявок</h3>
                        <p>Когда пользователи отправят заявки на бронирование, они появятся здесь</p>
                        <a href="/create-listing.html" class="btn btn-primary">
                             Разместить объявление
                        </a>
                    </div>
                </div>
            `;
        }
    }

    async function render() {
        root.innerHTML = '<div style="grid-column: 1 / -1; display: flex; justify-content: center; align-items: center; min-height: 300px;"><i class="fas fa-spinner fa-pulse fa-2x" style="color: var(--primary-teal);"></i></div>';

        try {
            if (mode === 'my') {
                const items = await loadMyBookings();
                if (myCountBadge) {
                    myCountBadge.textContent = items.length;
                    myCountBadge.style.display = items.length > 0 ? 'inline-block' : 'none';
                }

                if (items.length === 0) {
                    root.innerHTML = renderEmptyState('my');
                    return;
                }
                root.innerHTML = items.map(item => renderBookingCard(item, false)).join('');
                return;
            }

            const incoming = await loadIncomingBookings();
            if (incomingCountBadge) {
                incomingCountBadge.textContent = incoming.length;
                incomingCountBadge.style.display = incoming.length > 0 ? 'inline-block' : 'none';
            }

            if (incoming.length === 0) {
                root.innerHTML = renderEmptyState('incoming');
                return;
            }
            root.innerHTML = incoming.map(item => renderBookingCard(item, true)).join('');

            root.querySelectorAll('button[data-action="approve"]').forEach((btn) => {
                btn.addEventListener('click', async () => {
                    const id = Number(btn.dataset.id);
                    try {
                        await updateStatus(id, 'approved');
                        await render();
                    } catch (e) {
                        alert(e.message || 'Ошибка');
                    }
                });
            });

            root.querySelectorAll('button[data-action="reject"]').forEach((btn) => {
                btn.addEventListener('click', async () => {
                    const id = Number(btn.dataset.id);
                    try {
                        await updateStatus(id, 'rejected');
                        await render();
                    } catch (e) {
                        alert(e.message || 'Ошибка');
                    }
                });
            });
        } catch (e) {
            root.innerHTML = `<div style="grid-column: 1 / -1;"><div class="bookings-empty"><i class="fas fa-exclamation-triangle"></i><h3>Ошибка загрузки</h3><p>${esc(e.message || 'Попробуйте позже')}</p></div></div>`;
        }
    }

    document.addEventListener('click', async (e) => {
        const btn = e.target.closest('[data-action="cancel"]');
        if (!btn) return;
        const id = Number(btn.dataset.id);
        if (confirm('Отменить заявку на бронирование?')) {
            try {
                await updateStatus(id, 'cancelled');
                await render();
            } catch (err) {
                alert(err.message || 'Ошибка при отмене');
            }
        }
    });

    function updateTabStyles() {
        if (mode === 'my') {
            tabMyBtn?.classList.add('active');
            tabIncomingBtn?.classList.remove('active');
        } else {
            tabIncomingBtn?.classList.add('active');
            tabMyBtn?.classList.remove('active');
        }
    }

    tabMyBtn?.addEventListener('click', async () => {
        mode = 'my';
        updateTabStyles();
        await render();
    });

    tabIncomingBtn?.addEventListener('click', async () => {
        mode = 'incoming';
        updateTabStyles();
        await render();
    });

    updateTabStyles();
    await render();
});