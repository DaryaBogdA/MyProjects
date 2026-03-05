document.addEventListener('DOMContentLoaded', async function() {
    if (!currentUser || currentUser.isAdmin) {
        window.location.href = 'index.html';
        return;
    }
    await loadUserProfileData();
    document.getElementById('userProfileSection').style.display = 'block';
    document.getElementById('userName').textContent = `${currentUser.firstName || currentUser.name || ''} ${currentUser.lastName || ''}`.trim() || currentUser.email;
    switchUserTab('favorites');
});

function switchUserTab(tabId) {
    document.querySelectorAll('#userProfileSection .tab').forEach(tab => tab.classList.remove('active'));
    document.querySelectorAll('#userProfileSection .tab-content').forEach(content => content.classList.remove('active'));

    const tabs = document.querySelectorAll('#userProfileSection .tab');
    tabs.forEach(tab => {
        if (tab.textContent.trim().toLowerCase().includes(tabId.replace('-', ' '))) {
            tab.classList.add('active');
        }
    });

    const selectedContent = document.getElementById(`${tabId}-tab`);
    if (selectedContent) selectedContent.classList.add('active');

    switch(tabId) {
        case 'favorites':
            displayFavorites();
            break;
        case 'my-events':
            displayMyEvents();
            break;
        case 'add-event':
            setTimeout(() => initLocationMap(), 100);
            break;
    }
}

function displayFavorites() {
    const list = document.getElementById('favoritesList');
    if (!list) return;
    const favoriteEvents = events.filter(e => favorites.includes(e.id));
    if (favoriteEvents.length === 0) {
        list.innerHTML = '<p class="info-text">У вас пока нет избранных мероприятий</p>';
        return;
    }
    list.innerHTML = '';
    favoriteEvents.forEach(event => {
        list.innerHTML += `
            <div class="profile-event-item">
                <div>
                    <strong>${event.title}</strong>
                    <p>📅 ${event.dateDisplay} • ${event.time}</p>
                    <p>📍 ${event.location}</p>
                    <span class="event-status status-upcoming">${event.price} BYN</span>
                </div>
                <div class="profile-actions">
                    <button class="btn-review" onclick="showEventDetails(${event.id})">Подробнее</button>
                    <button class="btn-delete" onclick="toggleFavorite(${event.id})">Удалить</button>
                </div>
            </div>
        `;
    });
}

function displayMyEvents() {
    const list = document.getElementById('myEventsList');
    if (!list) return;
    const myRegisteredEvents = events.filter(e => myEvents.includes(e.id));
    if (myRegisteredEvents.length === 0) {
        list.innerHTML = '<p class="info-text">У вас пока нет записей на мероприятия</p>';
        return;
    }
    list.innerHTML = '';
    myRegisteredEvents.forEach(event => {
        const eventDate = new Date(event.date);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        const isPast = eventDate < today;
        list.innerHTML += `
            <div class="profile-event-item">
                <div>
                    <strong>${event.title}</strong>
                    <p>📅 ${event.dateDisplay} • ${event.time}</p>
                    <p>📍 ${event.location}</p>
                    <span class="event-status ${isPast ? 'status-past' : 'status-upcoming'}">
                        ${isPast ? 'Прошло' : 'Предстоит'}
                    </span>
                </div>
                <div class="profile-actions">
                    <button class="btn-review" onclick="showEventDetails(${event.id})">Подробнее</button>
                    ${!isPast ? `<button class="btn-delete" onclick="unregisterFromEvent(${event.id})">Отменить запись</button>` : ''}
                </div>
            </div>
        `;
    });
}