let sports = [];
let cities = [];

document.addEventListener('DOMContentLoaded', async function() {
    if (!currentUser || !currentUser.isAdmin) {
        window.location.href = 'index.html';
        return;
    }
    document.getElementById('adminProfileSection').style.display = 'block';

    await loadSports();
    await loadCities();
    await loadPendingEvents();
    await loadAdminEvents();
    await loadUsers();
    await loadStats();

    switchAdminTab('moderation');
});

async function loadSports() {
    try {
        const response = await fetch(`${API_BASE}/sports`, {
            headers: getAuthHeaders()
        });
        if (response.ok) {
            sports = await response.json();
        } else {
            console.error('Ошибка загрузки видов спорта:', response.status);
        }
    } catch (e) {
        console.error('Ошибка загрузки видов спорта', e);
    }
}

async function loadCities() {
    try {
        const response = await fetch(`${API_BASE}/cities`, {
            headers: getAuthHeaders()
        });
        if (response.ok) {
            cities = await response.json();
        } else {
            console.error('Ошибка загрузки городов:', response.status);
        }
    } catch (e) {
        console.error('Ошибка загрузки городов', e);
    }
}

async function loadStats() {
    try {
        const response = await fetch(`${API_BASE}/admin/stats`, {
            headers: getAuthHeaders()
        });
        if (response.ok) {
            const stats = await response.json();
            document.getElementById('adminTotalEvents').textContent = stats.totalEvents;
            document.getElementById('adminTotalUsers').textContent = stats.totalUsers;
            document.getElementById('adminPendingEvents').textContent = stats.pendingEvents;
        }
    } catch (e) {
        console.error('Ошибка загрузки статистики', e);
    }
}

window.addEventListener('click', function(event) {
    const modal = document.getElementById('editEventModal');
    if (event.target === modal) {
        modal.style.display = 'none';
    }
});

async function loadPendingEvents() {
    const list = document.getElementById('pendingEventsList');
    const countBadge = document.getElementById('pendingCount');
    if (!list) return;
    try {
        const response = await fetch(`${API_BASE}/admin/events/pending`, {
            headers: getAuthHeaders()
        });
        if (!response.ok) throw new Error();
        const pending = await response.json();
        if (countBadge) countBadge.textContent = pending.length;
        if (pending.length === 0) {
            list.innerHTML = '<p class="info-text">Нет событий на модерации</p>';
            return;
        }
        list.innerHTML = '';
        pending.forEach(event => {
            const dateDisplay = new Date(event.date).toLocaleDateString('ru-RU', { year: 'numeric', month: 'long', day: 'numeric' });
            list.innerHTML += `
                <div class="moderation-item">
                    <div class="moderation-header">
                        <h4>${event.title}</h4>
                        <div class="moderation-actions">
                            <button class="btn-edit" onclick="showEventDetails(${event.id})">Подробнее</button>
                            <button class="btn-approve" onclick="approveEvent(${event.id})">✓ Одобрить</button>
                            <button class="btn-reject" onclick="rejectEvent(${event.id})">✗ Отклонить</button>
                        </div>
                    </div>
                    <div class="moderation-details">
                        <span>📅 ${dateDisplay}</span>
                        <span>📍 ${event.location}</span>
                        <span>💰 ${event.price} BYN</span>
                        <span>👥 ${event.maxParticipants} мест</span>
                    </div>
                    <div class="moderation-description">${event.description || ''}</div>
                </div>
            `;
        });
    } catch (e) {
        if (countBadge) countBadge.textContent = '0';
        list.innerHTML = '<p class="info-text error">Ошибка загрузки</p>';
    }
}

async function approveEvent(eventId) {
    try {
        const response = await fetch(`${API_BASE}/admin/events/${eventId}/approve`, {
            method: 'PUT',
            headers: getAuthHeaders()
        });
        if (response.ok) {
            showNotification('Событие одобрено', 'success');
            loadPendingEvents();
            loadAdminEvents();
            loadStats();
        } else {
            showNotification('Ошибка', 'warning');
        }
    } catch (e) {
        showNotification('Ошибка соединения', 'warning');
    }
}

async function rejectEvent(eventId) {
    try {
        const response = await fetch(`${API_BASE}/admin/events/${eventId}/reject`, {
            method: 'PUT',
            headers: getAuthHeaders()
        });
        if (response.ok) {
            showNotification('Событие отклонено', 'success');
            loadPendingEvents();
            loadAdminEvents();
            loadStats();
        } else {
            showNotification('Ошибка', 'warning');
        }
    } catch (e) {
        showNotification('Ошибка соединения', 'warning');
    }
}

async function loadAdminEvents() {
    const list = document.getElementById('adminEventsList');
    if (!list) return;
    try {
        const response = await fetch(`${API_BASE}/events`, {
            headers: getAuthHeaders()
        });
        if (!response.ok) throw new Error();
        const approvedEvents = await response.json();
        if (approvedEvents.length === 0) {
            list.innerHTML = '<p class="info-text">Нет одобренных событий</p>';
            return;
        }
        list.innerHTML = '';
        approvedEvents.forEach(event => {
            const dateDisplay = new Date(event.date).toLocaleDateString('ru-RU', { year: 'numeric', month: 'long', day: 'numeric' });
            list.innerHTML += `
                <div class="admin-event-item">
                    <div class="admin-event-info">
                        <h4>${event.title}</h4>
                        <p>${dateDisplay} • ${event.location} • Участников: ${event.currentParticipants}/${event.maxParticipants}</p>
                    </div>
                    <div class="admin-event-actions">
                        <button class="btn-edit" onclick="editEvent(${event.id})"> Ред.</button>
                        <button class="btn-delete" onclick="deleteEvent(${event.id})"> Удалить</button>
                    </div>
                </div>
            `;
        });
    } catch (e) {
        list.innerHTML = '<p class="info-text error">Ошибка загрузки</p>';
    }
}

function filterAdminEvents(searchTerm) {
    const items = document.querySelectorAll('.admin-event-item');
    items.forEach(item => {
        const title = item.querySelector('h4').textContent.toLowerCase();
        if (title.includes(searchTerm.toLowerCase())) {
            item.style.display = '';
        } else {
            item.style.display = 'none';
        }
    });
}

async function editEvent(eventId) {
    try {
        if (sports.length === 0) await loadSports();
        if (cities.length === 0) await loadCities();

        const response = await fetch(`${API_BASE}/admin/events/${eventId}`, {
            headers: getAuthHeaders()
        });
        if (!response.ok) throw new Error('Не удалось загрузить событие');
        const event = await response.json();

        document.getElementById('editEventId').value = event.id;
        document.getElementById('editEventTitle').value = event.title;
        document.getElementById('editEventDescription').value = event.description || '';

        const sportSelect = document.getElementById('editEventSport');
        sportSelect.innerHTML = '<option value="">Выберите вид спорта</option>';
        sports.forEach(s => {
            const option = document.createElement('option');
            option.value = s.id;
            option.textContent = s.name;
            if (event.sport && event.sport.id === s.id) option.selected = true;
            sportSelect.appendChild(option);
        });

        const citySelect = document.getElementById('editEventCity');
        citySelect.innerHTML = '<option value="">Выберите город</option>';
        cities.forEach(c => {
            const option = document.createElement('option');
            option.value = c.id;
            option.textContent = c.name;
            if (event.city && event.city.id === c.id) option.selected = true;
            citySelect.appendChild(option);
        });

        document.getElementById('editEventDate').value = event.date;
        document.getElementById('editEventTime').value = event.time;
        document.getElementById('editEventLocation').value = event.location;
        document.getElementById('editEventPrice').value = event.price;
        document.getElementById('editEventMaxParticipants').value = event.maxParticipants;
        document.getElementById('editEventImageUrl').value = event.imageUrl || '';

        document.getElementById('editEventModal').style.display = 'block';
    } catch (e) {
        showNotification('Ошибка загрузки события', 'warning');
    }
}

document.getElementById('editEventForm').addEventListener('submit', async function(e) {
    e.preventDefault();

    const id = document.getElementById('editEventId').value;
    const updatedEvent = {
        title: document.getElementById('editEventTitle').value,
        description: document.getElementById('editEventDescription').value,
        sportId: parseInt(document.getElementById('editEventSport').value),
        cityId: parseInt(document.getElementById('editEventCity').value),
        date: document.getElementById('editEventDate').value,
        time: document.getElementById('editEventTime').value,
        location: document.getElementById('editEventLocation').value,
        price: parseFloat(document.getElementById('editEventPrice').value),
        maxParticipants: parseInt(document.getElementById('editEventMaxParticipants').value),
        imageUrl: document.getElementById('editEventImageUrl').value
    };

    try {
        const response = await fetch(`${API_BASE}/admin/events/${id}`, {
            method: 'PUT',
            headers: getAuthHeaders(),
            body: JSON.stringify(updatedEvent)
        });

        if (response.ok) {
            showNotification('Событие обновлено', 'success');
            closeEditEventModal();
            await loadAdminEvents();
            if (typeof loadEvents === 'function') await loadEvents();
        } else {
            const data = await response.json();
            showNotification(data.error || 'Ошибка обновления', 'warning');
        }
    } catch (e) {
        showNotification('Ошибка соединения', 'warning');
    }
});

function closeEditEventModal() {
    document.getElementById('editEventModal').style.display = 'none';
}

async function deleteEvent(eventId) {
    if (!confirm('Вы уверены, что хотите удалить это событие?')) return;
    try {
        const response = await fetch(`${API_BASE}/admin/events/${eventId}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        });
        if (response.ok) {
            showNotification('Событие удалено', 'success');
            loadAdminEvents();
            loadStats();
        } else {
            showNotification('Ошибка', 'warning');
        }
    } catch (e) {
        showNotification('Ошибка соединения', 'warning');
    }
}

async function loadUsers() {
    const list = document.getElementById('usersList');
    if (!list) return;
    try {
        const response = await fetch(`${API_BASE}/admin/users`, {
            headers: getAuthHeaders()
        });
        if (!response.ok) throw new Error();
        const users = await response.json();
        if (users.length === 0) {
            list.innerHTML = '<p class="info-text">Нет зарегистрированных пользователей</p>';
            return;
        }
        list.innerHTML = '';
        users.forEach(user => {
            list.innerHTML += `
                <div class="user-item">
                    <div class="user-info">
                        <h4>${user.firstName || ''} ${user.lastName || ''} <span class="user-badge">ID: ${user.id}</span></h4>
                        <p>${user.email}</p>
                        <p>Роль: ${user.role}</p>
                    </div>
                    <div class="user-actions">
                        <button class="btn-delete" onclick="deleteUser(${user.id})">Удалить</button>
                    </div>
                </div>
            `;
        });
    } catch (e) {
        list.innerHTML = '<p class="info-text error">Ошибка загрузки</p>';
    }
}

async function deleteUser(userId) {
    if (!confirm('Удалить пользователя?')) return;
    try {
        const response = await fetch(`${API_BASE}/admin/users/${userId}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        });
        if (response.ok) {
            showNotification('Пользователь удалён', 'success');
            loadUsers();
            loadStats();
        } else {
            showNotification('Ошибка', 'warning');
        }
    } catch (e) {
        showNotification('Ошибка соединения', 'warning');
    }
}

let currentReportData = null;
let currentReportType = null;

async function generateReport(type) {
    try {
        const response = await fetch(`${API_BASE}/admin/reports/${type}`, {
            headers: getAuthHeaders()
        });
        if (!response.ok) throw new Error('Ошибка загрузки отчета');
        const data = await response.json();
        currentReportData = data;
        currentReportType = type;
        downloadCurrentReport();
        showNotification('Отчёт сохранён (таблица CSV)', 'success');
    } catch (e) {
        showNotification(`Ошибка загрузки отчета: ${e.message || 'неизвестно'}`, 'warning');
        console.error(e);
    }
}

function escapeCsvCell(value) {
    const s = String(value ?? '');
    if (/[",;\n\r]/.test(s)) {
        return `"${s.replace(/"/g, '""')}"`;
    }
    return s;
}

/** Табличный отчёт UTF-8 (с BOM) — открывается в Excel с корректной кириллицей. Разделитель «;» для локали RU. */
function downloadCurrentReport() {
    if (!currentReportData) {
        showNotification('Нет данных для скачивания', 'warning');
        return;
    }
    const datePart = new Date().toISOString().slice(0, 10);
    const filename = `report_${currentReportType}_${datePart}.csv`;
    const titleMap = {
        participants: 'Отчет по участникам',
        sports: 'Популярные виды спорта',
        popular: 'Популярные мероприятия',
        revenue: 'Финансовый отчет'
    };
    const title = `${titleMap[currentReportType] || 'Отчет'} (${datePart})`;
    const rows = [];
    if (currentReportType === 'participants' && Array.isArray(currentReportData)) {
        rows.push(['Мероприятие', 'Участников', 'Лимит']);
        currentReportData.forEach(item => rows.push([item.title, String(item.participants), String(item.maxParticipants)]));
    } else if (currentReportType === 'sports' && Array.isArray(currentReportData)) {
        rows.push(['Вид спорта', 'Всего участников']);
        currentReportData.forEach(item => rows.push([item.sport, String(item.totalParticipants)]));
    } else if (currentReportType === 'popular' && Array.isArray(currentReportData)) {
        rows.push(['#', 'Мероприятие', 'Участников']);
        currentReportData.forEach((item, index) => rows.push([String(index + 1), item.title, String(item.participants)]));
    } else if (currentReportType === 'revenue' && currentReportData) {
        rows.push(['Показатель', 'Значение']);
        rows.push(['Общая выручка', `${currentReportData.totalRevenue} BYN`]);
        Object.entries(currentReportData.byEvent || {}).forEach(([name, value]) => rows.push([name, `${value} BYN`]));
    } else {
        rows.push(['Данные']);
        rows.push([JSON.stringify(currentReportData, null, 2)]);
    }
    if (rows.length === 0) {
        rows.push(['Данные']);
        rows.push(['Нет данных для отчета']);
    }
    const bom = '\uFEFF';
    const tableLines = rows.map(row => row.map(escapeCsvCell).join(';'));
    const csv = bom + escapeCsvCell(title) + '\r\n\r\n' + tableLines.join('\r\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = filename;
    link.rel = 'noopener';
    document.body.appendChild(link);
    link.click();
    link.remove();
    setTimeout(() => URL.revokeObjectURL(link.href), 0);
}

function switchAdminTab(tabId) {
    document.querySelectorAll('#adminProfileSection .tab').forEach(btn => btn.classList.remove('active'));
    document.querySelectorAll('#adminProfileSection .tab-content').forEach(content => content.classList.remove('active'));

    const button = document.querySelector(`#adminProfileSection .tab[data-tab="${tabId}"]`);
    const content = document.getElementById(tabId);
    if (button) button.classList.add('active');
    if (content) content.classList.add('active');
    if (tabId === 'chat') initSupportChat();
}