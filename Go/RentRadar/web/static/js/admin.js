document.addEventListener('DOMContentLoaded', async () => {
    const msg = document.getElementById('adminMessage');
    const uid = localStorage.getItem('userId');
    if (!uid) {
        window.location.href = '/login.html';
        return;
    }

    const headers = () => ({ 'X-User-ID': uid, 'Content-Type': 'application/json' });

    try {
        const meRes = await fetch('/api/me', { headers: { 'X-User-ID': uid } });
        const meData = await meRes.json();
        if (!meRes.ok || meData.role !== 'admin') {
            window.location.href = '/index.html';
            return;
        }
    } catch (_) {
        window.location.href = '/login.html';
        return;
    }

    function showMsg(text, isError) {
        if (!msg) return;
        msg.innerHTML = `<p class="${isError ? 'error' : ''}" style="padding:10px;border-radius:8px;${isError ? 'background:#fee2e2;color:#991b1b;' : ''}">${escapeHtml(text)}</p>`;
        setTimeout(() => {
            if (msg) msg.innerHTML = '';
        }, 4000);
    }

    function modBadge(st) {
        const s = (st || '').toLowerCase();
        if (s === 'pending') return '<span class="badge-st badge-pending">модерация</span>';
        if (s === 'approved') return '<span class="badge-st badge-approved">одобрено</span>';
        if (s === 'rejected') return '<span class="badge-st badge-rejected">отклонено</span>';
        return escapeHtml(st || '');
    }

    document.querySelectorAll('.admin-tab').forEach((btn) => {
        btn.addEventListener('click', () => {
            document.querySelectorAll('.admin-tab').forEach((b) => b.classList.remove('active'));
            document.querySelectorAll('.admin-panel').forEach((p) => p.classList.remove('active'));
            btn.classList.add('active');
            const id = 'panel-' + btn.dataset.panel;
            const panel = document.getElementById(id);
            if (panel) panel.classList.add('active');
            if (btn.dataset.panel === 'users') loadUsers();
            if (btn.dataset.panel === 'listings-pending') loadPendingListings();
            if (btn.dataset.panel === 'listings-all') loadAllListings();
            if (btn.dataset.panel === 'reviews-pending') loadPendingReviews();
            if (btn.dataset.panel === 'reports') loadReports();
            if (btn.dataset.panel === 'exports') initExportsPanel();
        });
    });

    let viewingUserId = null;

    async function loadUsers() {
        const tbody = document.getElementById('usersBody');
        const empty = document.getElementById('usersEmpty');
        try {
            const res = await fetch('/api/admin/users', { headers: { 'X-User-ID': uid } });
            const data = await res.json();
            if (!res.ok) throw new Error(data.error || 'Ошибка загрузки пользователей');
            tbody.innerHTML = '';
            if (!Array.isArray(data) || data.length === 0) {
                empty.hidden = false;
                return;
            }
            empty.hidden = true;
            data.forEach((u) => {
                const tr = document.createElement('tr');
                const blockedBadge = u.is_blocked
                    ? '<span class="badge-st badge-blocked">заблокирован</span>'
                    : '<span class="badge-st badge-ok">активен</span>';
                tr.innerHTML = `
                    <td>#${u.id}</td>
                    <td>${escapeHtml((u.first_name || '') + ' ' + (u.last_name || ''))}</td>
                    <td>${escapeHtml(u.email || '')}<br><small>${escapeHtml(u.phone || '')}</small></td>
                    <td>${escapeHtml(u.role || 'user')}</td>
                    <td>${blockedBadge}</td>
                    <td><button type="button" class="btn btn-outline btn-xs user-view-btn" data-id="${u.id}">Профиль</button></td>`;
                tbody.appendChild(tr);
            });
            tbody.querySelectorAll('.user-view-btn').forEach((b) =>
                b.addEventListener('click', () => openUserModal(Number(b.dataset.id)))
            );
        } catch (e) {
            showMsg(e.message, true);
        }
    }

    const userModal = document.getElementById('userModal');
    document.getElementById('userModalClose')?.addEventListener('click', () => {
        userModal.classList.remove('show');
        viewingUserId = null;
    });
    userModal?.addEventListener('click', (e) => {
        if (e.target === userModal) {
            userModal.classList.remove('show');
            viewingUserId = null;
        }
    });

    async function openUserModal(id) {
        viewingUserId = id;
        const content = document.getElementById('userModalContent');
        content.innerHTML = '<p>Загрузка...</p>';
        userModal.classList.add('show');
        try {
            const res = await fetch(`/api/admin/users/${id}`, { headers: { 'X-User-ID': uid } });
            const u = await res.json();
            if (!res.ok) throw new Error(u.error || 'Ошибка');
            content.innerHTML = `
                <div class="user-detail-line"><strong>ID:</strong> ${u.id}</div>
                <div class="user-detail-line"><strong>Имя:</strong> ${escapeHtml(u.first_name || '')} ${escapeHtml(u.last_name || '')}</div>
                <div class="user-detail-line"><strong>Email:</strong> ${escapeHtml(u.email || '—')}</div>
                <div class="user-detail-line"><strong>Телефон:</strong> ${escapeHtml(u.phone || '—')}</div>
                <div class="user-detail-line"><strong>Роль:</strong> ${escapeHtml(u.role || '')}</div>
                <div class="user-detail-line"><strong>Заблокирован:</strong> ${u.is_blocked ? 'да' : 'нет'}</div>`;
        } catch (e) {
            content.innerHTML = `<p class="error">${escapeHtml(e.message)}</p>`;
        }
    }

    document.getElementById('userModalBlock')?.addEventListener('click', async () => {
        if (!viewingUserId) return;
        try {
            const res = await fetch(`/api/admin/users/${viewingUserId}/block`, {
                method: 'POST',
                headers: { 'X-User-ID': uid },
            });
            const d = await res.json();
            if (!res.ok) throw new Error(d.error || 'Ошибка');
            showMsg('Пользователь заблокирован');
            userModal.classList.remove('show');
            await loadUsers();
        } catch (e) {
            alert(e.message);
        }
    });

    document.getElementById('userModalUnblock')?.addEventListener('click', async () => {
        if (!viewingUserId) return;
        try {
            const res = await fetch(`/api/admin/users/${viewingUserId}/unblock`, {
                method: 'POST',
                headers: { 'X-User-ID': uid },
            });
            const d = await res.json();
            if (!res.ok) throw new Error(d.error || 'Ошибка');
            showMsg('Пользователь разблокирован');
            userModal.classList.remove('show');
            await loadUsers();
        } catch (e) {
            alert(e.message);
        }
    });

    async function loadPendingListings() {
        const tbody = document.getElementById('pendingBody');
        const empty = document.getElementById('pendingEmpty');
        const table = tbody?.closest('.table-wrap')?.querySelector('table');
        try {
            const res = await fetch('/api/admin/pending-listings', { headers: { 'X-User-ID': uid } });
            const data = await res.json();
            if (!res.ok) throw new Error(data.error || 'Ошибка');
            const items = Array.isArray(data) ? data : [];
            tbody.innerHTML = '';
            if (items.length === 0) {
                empty.hidden = false;
                if (table) table.hidden = true;
                return;
            }
            empty.hidden = true;
            if (table) table.hidden = false;
            items.forEach((listing) => {
                const typeLabel = listing.listing_type === 'sale' ? 'Продажа' : 'Аренда';
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>#${listing.id}</td>
                    <td>#${listing.user_id}</td>
                    <td>${escapeHtml(listing.title || '')}</td>
                    <td>${escapeHtml(listing.city || '')}</td>
                    <td>${escapeHtml(typeLabel)}<br>${listing.price != null ? Number(listing.price).toLocaleString() + ' BYN' : ''}</td>
                    <td>
                        <div class="row-actions">
                            <button type="button" class="btn btn-primary btn-xs lst-approve" data-id="${listing.id}">Одобрить</button>
                            <button type="button" class="btn btn-outline btn-xs lst-reject" data-id="${listing.id}">Отклонить</button>
                            <button type="button" class="btn btn-outline btn-xs lst-edit" data-id="${listing.id}">Изменить</button>
                            <button type="button" class="btn btn-danger btn-xs lst-del" data-id="${listing.id}">Снять</button>
                        </div>
                    </td>`;
                tbody.appendChild(tr);
            });
            bindListingRowActions(tbody);
        } catch (e) {
            showMsg(e.message, true);
        }
    }

    async function postListingAction(path) {
        const res = await fetch(path, { method: 'POST', headers: { 'X-User-ID': uid } });
        const d = await res.json().catch(() => ({}));
        if (!res.ok) throw new Error(d.error || 'Ошибка');
    }

    function bindListingRowActions(tbody) {
        tbody.querySelectorAll('.lst-approve').forEach((b) =>
            b.addEventListener('click', async () => {
                try {
                    await postListingAction(`/api/admin/listings/${b.dataset.id}/approve`);
                    showMsg('Одобрено');
                    loadPendingListings();
                    loadAllListings();
                } catch (e) {
                    alert(e.message);
                }
            })
        );
        tbody.querySelectorAll('.lst-reject').forEach((b) =>
            b.addEventListener('click', async () => {
                try {
                    await postListingAction(`/api/admin/listings/${b.dataset.id}/reject`);
                    showMsg('Отклонено');
                    loadPendingListings();
                    loadAllListings();
                } catch (e) {
                    alert(e.message);
                }
            })
        );
        tbody.querySelectorAll('.lst-edit').forEach((b) =>
            b.addEventListener('click', () => openListingEdit(Number(b.dataset.id)))
        );
        tbody.querySelectorAll('.lst-del').forEach((b) =>
            b.addEventListener('click', async () => {
                if (!confirm('Снять объявление с публикации?')) return;
                try {
                    const res = await fetch(`/api/admin/listings/${b.dataset.id}`, {
                        method: 'DELETE',
                        headers: { 'X-User-ID': uid },
                    });
                    const d = await res.json().catch(() => ({}));
                    if (!res.ok) throw new Error(d.error || 'Ошибка');
                    showMsg('Объявление снято');
                    loadPendingListings();
                    loadAllListings();
                } catch (e) {
                    alert(e.message);
                }
            })
        );
    }

    async function loadAllListings() {
        const tbody = document.getElementById('allListingsBody');
        const empty = document.getElementById('allListingsEmpty');
        const filter = document.getElementById('listingsStatusFilter')?.value || 'all';
        const incl = document.getElementById('listingsIncludeInactive')?.checked ? '1' : '0';
        const q = new URLSearchParams({ include_inactive: incl });
        if (filter && filter !== 'all') q.set('status', filter);
        try {
            const res = await fetch(`/api/admin/listings?${q}`, { headers: { 'X-User-ID': uid } });
            const data = await res.json();
            if (!res.ok) throw new Error(data.error || 'Ошибка');
            const items = Array.isArray(data) ? data : [];
            tbody.innerHTML = '';
            if (items.length === 0) {
                empty.hidden = false;
                return;
            }
            empty.hidden = true;
            items.forEach((listing) => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>#${listing.id}</td>
                    <td>#${listing.user_id}</td>
                    <td>${escapeHtml(listing.title || '')}</td>
                    <td>${modBadge(listing.moderation_status)}</td>
                    <td>${listing.is_active ? 'да' : 'нет'}</td>
                    <td>
                        <div class="row-actions">
                            <button type="button" class="btn btn-outline btn-xs lst-edit" data-id="${listing.id}">Изменить</button>
                            <button type="button" class="btn btn-danger btn-xs lst-del" data-id="${listing.id}">Снять</button>
                        </div>
                    </td>`;
                tbody.appendChild(tr);
            });
            bindListingRowActions(tbody);
        } catch (e) {
            showMsg(e.message, true);
        }
    }

    document.getElementById('reloadListingsBtn')?.addEventListener('click', loadAllListings);
    document.getElementById('listingsStatusFilter')?.addEventListener('change', loadAllListings);
    document.getElementById('listingsIncludeInactive')?.addEventListener('change', loadAllListings);

    const listingModal = document.getElementById('listingModal');

    async function openListingEdit(id) {
        listingModal.classList.add('show');
        document.getElementById('editListingId').value = String(id);
        try {
            const res = await fetch(`/api/listings/${id}`, { headers: { 'X-User-ID': uid } });
            const L = await res.json();
            if (!res.ok) throw new Error(L.error || 'Ошибка загрузки');
            document.getElementById('editTitle').value = L.title || '';
            document.getElementById('editDescription').value = L.description || '';
            document.getElementById('editListingType').value = L.listing_type === 'sale' ? 'sale' : 'rent';
            document.getElementById('editModeration').value = L.moderation_status || 'pending';
            document.getElementById('editPrice').value = L.price != null ? String(L.price) : '';
            document.getElementById('editRooms').value = L.rooms != null ? String(L.rooms) : '';
            document.getElementById('editArea').value = L.area != null ? String(L.area) : '';
            document.getElementById('editFloor').value = L.floor != null ? String(L.floor) : '';
            document.getElementById('editTotalFloors').value = L.total_floors != null ? String(L.total_floors) : '';
            document.getElementById('editAddress').value = L.address || '';
            document.getElementById('editCity').value = L.city || '';
            document.getElementById('editDistrict').value = L.district || '';
            let af = '';
            if (L.available_from) {
                const d = new Date(L.available_from);
                if (!isNaN(d.getTime())) af = d.toISOString().slice(0, 10);
            }
            document.getElementById('editAvailableFrom').value = af;
            document.getElementById('editUtilities').value = L.utilities_included ? 'true' : 'false';
            document.getElementById('editPhotos').value = L.photos || '';
        } catch (e) {
            alert(e.message);
            listingModal.classList.remove('show');
        }
    }

    document.getElementById('listingModalClose')?.addEventListener('click', () => listingModal.classList.remove('show'));
    listingModal?.addEventListener('click', (e) => {
        if (e.target === listingModal) listingModal.classList.remove('show');
    });

    document.getElementById('listingEditForm')?.addEventListener('submit', async (e) => {
        e.preventDefault();
        const id = document.getElementById('editListingId').value;
        const body = {
            title: document.getElementById('editTitle').value.trim(),
            description: document.getElementById('editDescription').value.trim(),
            listing_type: document.getElementById('editListingType').value,
            moderation_status: document.getElementById('editModeration').value,
            price: parseFloat(document.getElementById('editPrice').value) || 0,
            rooms: parseInt(document.getElementById('editRooms').value, 10) || 0,
            area: parseFloat(document.getElementById('editArea').value) || 0,
            floor: parseInt(document.getElementById('editFloor').value, 10) || 0,
            total_floors: parseInt(document.getElementById('editTotalFloors').value, 10) || 0,
            address: document.getElementById('editAddress').value.trim(),
            city: document.getElementById('editCity').value.trim(),
            district: document.getElementById('editDistrict').value.trim(),
            available_from: document.getElementById('editAvailableFrom').value.trim(),
            utilities_included: document.getElementById('editUtilities').value === 'true',
            photos: document.getElementById('editPhotos').value.trim(),
        };
        try {
            const res = await fetch(`/api/admin/listings/${id}`, {
                method: 'PUT',
                headers: headers(),
                body: JSON.stringify(body),
            });
            const d = await res.json().catch(() => ({}));
            if (!res.ok) throw new Error(d.error || 'Ошибка сохранения');
            showMsg('Объявление обновлено');
            listingModal.classList.remove('show');
            loadPendingListings();
            loadAllListings();
        } catch (err) {
            alert(err.message);
        }
    });

    async function loadPendingReviews() {
        const tbody = document.getElementById('pendingReviewsBody');
        const empty = document.getElementById('pendingReviewsEmpty');
        const table = tbody?.closest('.table-wrap')?.querySelector('table');
        try {
            const res = await fetch('/api/admin/reviews/pending', { headers: { 'X-User-ID': uid } });
            const data = await res.json();
            if (!res.ok) throw new Error(data.error || 'Ошибка');
            const items = Array.isArray(data) ? data : [];
            tbody.innerHTML = '';
            if (items.length === 0) {
                empty.hidden = false;
                if (table) table.hidden = true;
                return;
            }
            empty.hidden = true;
            if (table) table.hidden = false;
            items.forEach((rv) => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>#${rv.id}</td>
                    <td>#${rv.listing_id} ${escapeHtml(rv.listing_title || '')}</td>
                    <td>${escapeHtml(rv.author_name || '')} (#${rv.user_id})</td>
                    <td>${rv.rating}</td>
                    <td style="max-width:280px;">${escapeHtml((rv.comment || '').slice(0, 200))}</td>
                    <td>
                        <div class="row-actions">
                            <button type="button" class="btn btn-primary btn-xs rv-approve" data-id="${rv.id}">Одобрить</button>
                            <button type="button" class="btn btn-outline btn-xs rv-reject" data-id="${rv.id}">Скрыть</button>
                        </div>
                    </td>`;
                tbody.appendChild(tr);
            });
            tbody.querySelectorAll('.rv-approve').forEach((b) =>
                b.addEventListener('click', async () => {
                    try {
                        const r = await fetch(`/api/admin/reviews/${b.dataset.id}/moderation`, {
                            method: 'PUT',
                            headers: headers(),
                            body: JSON.stringify({ moderation_status: 'approved' }),
                        });
                        const d = await r.json().catch(() => ({}));
                        if (!r.ok) throw new Error(d.error || 'Ошибка');
                        showMsg('Отзыв опубликован');
                        loadPendingReviews();
                    } catch (e) {
                        alert(e.message);
                    }
                })
            );
            tbody.querySelectorAll('.rv-reject').forEach((b) =>
                b.addEventListener('click', async () => {
                    try {
                        const r = await fetch(`/api/admin/reviews/${b.dataset.id}/moderation`, {
                            method: 'PUT',
                            headers: headers(),
                            body: JSON.stringify({ moderation_status: 'rejected' }),
                        });
                        const d = await r.json().catch(() => ({}));
                        if (!r.ok) throw new Error(d.error || 'Ошибка');
                        showMsg('Отзыв скрыт');
                        loadPendingReviews();
                    } catch (e) {
                        alert(e.message);
                    }
                })
            );
        } catch (e) {
            showMsg(e.message, true);
        }
    }

    function reportStatusLabel(st) {
        const s = (st || '').toLowerCase();
        if (s === 'resolved') return 'Рассмотрена';
        if (s === 'dismissed') return 'Отклонена';
        return '—';
    }

    async function loadReports() {
        const tbody = document.getElementById('reportsBody');
        const empty = document.getElementById('reportsEmpty');
        const filter = document.getElementById('reportsStatusFilter')?.value || 'all';
        const q = filter === 'all' ? '?status=all' : `?status=${encodeURIComponent(filter)}`;
        try {
            const res = await fetch(`/api/admin/reports${q}`, { headers: { 'X-User-ID': uid } });
            const data = await res.json();
            if (!res.ok) throw new Error(data.error || 'Ошибка');
            const items = Array.isArray(data) ? data : [];
            tbody.innerHTML = '';
            if (items.length === 0) {
                empty.hidden = false;
                return;
            }
            empty.hidden = true;
            items.forEach((rep) => {
                const tr = document.createElement('tr');
                const lid = rep.listing_id != null ? `#${rep.listing_id}` : '—';
                tr.innerHTML = `
                    <td>#${rep.id}</td>
                    <td>${escapeHtml(rep.reporter_name || '')} (#${rep.reporter_id})</td>
                    <td>${escapeHtml(rep.reported_user_name || '')} (#${rep.reported_user_id})</td>
                    <td>${lid}</td>
                    <td style="max-width:240px;">${escapeHtml(rep.reason || '')}</td>
                    <td>${escapeHtml(reportStatusLabel(rep.status))}</td>
                    <td>
                        <div class="row-actions">
                            <button type="button" class="btn btn-primary btn-xs rep-resolve" data-id="${rep.id}" ${rep.status !== 'pending' ? 'disabled' : ''}>Рассмотрена</button>
                            <button type="button" class="btn btn-outline btn-xs rep-dismiss" data-id="${rep.id}" ${rep.status !== 'pending' ? 'disabled' : ''}>Отклонена</button>
                        </div>
                    </td>`;
                tbody.appendChild(tr);
            });
            tbody.querySelectorAll('.rep-resolve').forEach((b) =>
                b.addEventListener('click', async () => {
                    try {
                        const r = await fetch(`/api/admin/reports/${b.dataset.id}`, {
                            method: 'PUT',
                            headers: headers(),
                            body: JSON.stringify({ status: 'resolved' }),
                        });
                        const d = await r.json().catch(() => ({}));
                        if (!r.ok) throw new Error(d.error || 'Ошибка');
                        showMsg('Статус: Рассмотрена');
                        loadReports();
                    } catch (e) {
                        alert(e.message);
                    }
                })
            );
            tbody.querySelectorAll('.rep-dismiss').forEach((b) =>
                b.addEventListener('click', async () => {
                    try {
                        const r = await fetch(`/api/admin/reports/${b.dataset.id}`, {
                            method: 'PUT',
                            headers: headers(),
                            body: JSON.stringify({ status: 'dismissed' }),
                        });
                        const d = await r.json().catch(() => ({}));
                        if (!r.ok) throw new Error(d.error || 'Ошибка');
                        showMsg('Статус: Отклонена');
                        loadReports();
                    } catch (e) {
                        alert(e.message);
                    }
                })
            );
        } catch (e) {
            showMsg(e.message, true);
        }
    }

    document.getElementById('reloadReportsBtn')?.addEventListener('click', loadReports);
    document.getElementById('reportsStatusFilter')?.addEventListener('change', loadReports);

    function downloadTextFile(filename, content, mimeType) {
        const blob = new Blob([content], { type: mimeType });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        a.remove();
        URL.revokeObjectURL(url);
    }

    async function fetchAdminJson(path) {
        const r = await fetch(path, { headers: { 'X-User-ID': uid } });
        const d = await r.json().catch(() => ({}));
        if (!r.ok) throw new Error(d.error || 'Ошибка экспорта');
        return Array.isArray(d) ? d : [];
    }

    let exportsInitialized = false;
    function initExportsPanel() {
        if (exportsInitialized) return;
        exportsInitialized = true;
        document.getElementById('exportUsersJsonBtn')?.addEventListener('click', async () => {
            const data = await fetchAdminJson('/api/admin/users');
            downloadTextFile('users.json', JSON.stringify(data, null, 2), 'application/json;charset=utf-8');
        });
        document.getElementById('exportPopularityJsonBtn')?.addEventListener('click', async () => {
            const data = await fetchAdminJson('/api/admin/listings?include_inactive=1');
            data.sort((a, b) => Number(b.views_count || 0) - Number(a.views_count || 0));
            downloadTextFile('listing_popularity.json', JSON.stringify(data, null, 2), 'application/json;charset=utf-8');
        });
        document.getElementById('exportReportsJsonBtn')?.addEventListener('click', async () => {
            const data = await fetchAdminJson('/api/admin/reports?status=all');
            downloadTextFile('user_reports.json', JSON.stringify(data, null, 2), 'application/json;charset=utf-8');
        });
        document.getElementById('exportReviewsJsonBtn')?.addEventListener('click', async () => {
            const data = await fetchAdminJson('/api/admin/reviews/pending');
            downloadTextFile('reviews_pending.json', JSON.stringify(data, null, 2), 'application/json;charset=utf-8');
        });
    }

    await loadUsers();
});
