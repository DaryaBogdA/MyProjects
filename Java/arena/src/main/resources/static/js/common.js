let currentUser = null;
let currentToken = localStorage.getItem('token');
let currentRating = 0;
let currentEventId = null;
let map = null;
let markers = [];
let events = [];
let pendingEvents = [];
let favorites = [];
let myEvents = [];
let locationMap = null;
let locationMarker = null;
let selectedCoordinates = null;

const API_BASE = 'http://localhost:8079/api';

document.addEventListener('DOMContentLoaded', function() {
    checkAuth();
    loadEvents();
    loadGallery();

    const priceRange = document.getElementById('priceRange');
    if (priceRange) {
        priceRange.addEventListener('input', function(e) {
            document.getElementById('priceValue').textContent = e.target.value + ' BYN';
        });
    }

    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', filterEvents);
    }

    const adminSearchInput = document.getElementById('adminSearchInput');
    if (adminSearchInput) {
        adminSearchInput.addEventListener('input', (e) => filterAdminEvents(e.target.value));
    }

    const logo = document.querySelector('.logo h1');
    if (logo) {
        logo.style.cursor = 'pointer';
        logo.addEventListener('click', () => window.location.href = 'index.html');
    }
});

function checkAuth() {
    const token = localStorage.getItem('token');
    if (token) {
        try {
            const decoded = jwt_decode(token);
            if (decoded.exp * 1000 < Date.now()) {
                logout();
                return;
            }
            currentUser = {
                id: decoded.userId || decoded.sub,
                email: decoded.sub,
                firstName: decoded.firstName || '',
                lastName: decoded.lastName || '',
                role: decoded.role || 'USER',
                isAdmin: decoded.role === 'ADMIN'
            };
            localStorage.setItem('currentUser', JSON.stringify(currentUser));
            loadUserProfileData();
        } catch (e) {
            console.error('Invalid token', e);
            logout();
        }
    } else {
        currentUser = null;
    }
    updateUIBasedOnAuth();
}

function updateUIBasedOnAuth() {
    if (window.location.pathname.includes('auth.html')) return;
    const navButtons = document.querySelector('.nav-buttons');
    if (!navButtons) return;
    if (currentUser) {
        const profileLink = currentUser.isAdmin ? 'admin.html' : 'profile.html';
        navButtons.innerHTML = `
            <a href="${profileLink}" style="color: white; margin-right: 15px; text-decoration: none;">👤 ${currentUser.firstName || currentUser.email}</a>
            <button class="btn btn-outline" onclick="logout()">Выйти</button>
        `;
    } else {
        navButtons.innerHTML = `
            <a href="auth.html" class="btn btn-outline">Войти</a>
        `;
    }
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    currentUser = null;
    showNotification('Вы вышли из системы', 'info');
    window.location.href = 'index.html';
}

async function loadEvents() {
    try {
        const response = await fetch(`${API_BASE}/events`);
        if (!response.ok) throw new Error('Ошибка загрузки событий');
        const data = await response.json();
        events = data.map(e => ({
            ...e,
            sport: e.sport?.name?.toLowerCase() || '',
            sportName: e.sport?.name || '',
            city: e.city?.name?.toLowerCase() || '',
            cityName: e.city?.name || '',
            dateDisplay: new Date(e.date).toLocaleDateString('ru-RU', { year: 'numeric', month: 'long', day: 'numeric' }),
            icon: getSportIcon(e.sport?.name),
            badge: e.sport?.name || '',
            coordinates: [e.latitude, e.longitude]
        }));
        displayEvents(events.filter(e => e.status === 'approved'));
        updateMapMarkers(events.filter(e => e.status === 'approved'));
    } catch (error) {
        console.error(error);
        showNotification('Не удалось загрузить события', 'warning');
    }
}

async function loadGallery() {
    try {
        const response = await fetch(`${API_BASE}/gallery`);
        if (!response.ok) throw new Error('Ошибка загрузки галереи');
        const galleryImages = await response.json();
        displayGallery(galleryImages);
    } catch (error) {
        console.error(error);
    }
}

function displayEvents(eventsToShow) {
    const grid = document.getElementById('eventsGrid');
    if (!grid) return;

    grid.innerHTML = '';

    eventsToShow.forEach(event => {
        const percentFilled = (event.currentParticipants / event.maxParticipants) * 100;
        const placesLeft = event.maxParticipants - event.currentParticipants;
        const isFavorite = favorites.includes(event.id);

        const card = `
            <div class="event-card" data-sport="${event.sport}" data-city="${event.city}" data-price="${event.price}" data-date="${event.date}">
                <div class="event-image" style="background-image: url('${event.imageUrl}')">
                    <span class="event-badge">${event.badge}</span>
                    ${currentUser && !currentUser.isAdmin ? `<button class="favorite-btn ${isFavorite ? 'active' : ''}" onclick="toggleFavorite(${event.id})">${isFavorite ? '❤️' : '🤍'}</button>` : ''}
                </div>
                <div class="event-header">
                    <h3>${event.title}</h3>
                </div>
                <div class="event-body">
                    <div class="event-info">
                        <div class="info-item"><i>📅</i><span>${event.dateDisplay}</span></div>
                        <div class="info-item"><i>⏰</i><span>${event.time}</span></div>
                        <div class="info-item"><i>📍</i><span>${event.location}</span></div>
                        <div class="info-item"><i>👥</i><span>Участников: ${event.currentParticipants} / ${event.maxParticipants}</span></div>
                    </div>
                    <div class="price">${event.price}</div>
                    <div class="participants">
                        <div class="participants-bar"><div class="participants-fill" style="width: ${percentFilled}%"></div></div>
                        <div class="places-left">Осталось ${placesLeft} мест</div>
                    </div>
                </div>
                <div class="event-footer">
                    <span>${event.icon} ${event.cityName}</span>
                    <button class="btn-details" onclick="showEventDetails(${event.id})">Подробнее</button>
                </div>
            </div>
        `;
        grid.innerHTML += card;
    });
}

function displayGallery(images) {
    const gallery = document.getElementById('galleryGrid');
    if (!gallery) return;
    gallery.innerHTML = '';
    images.forEach(image => {
        gallery.innerHTML += `
            <div class="gallery-item">
                <img src="${image.imageUrl}" alt="${image.caption}" loading="lazy">
                <div class="gallery-caption">${image.caption}</div>
            </div>
        `;
    });
    setTimeout(updateScrollIndicators, 100);
}

function getSportIcon(sportName) {
    const map = {
        'Бег': '🏃',
        'Велоспорт': '🚴',
        'Теннис': '🎾',
        'Футбол': '⚽',
        'Плавание': '🏊',
        'Лыжи': '⛷️',
        'Волейбол': '🏐',
        'Баскетбол': '🏀'
    };
    return map[sportName] || '🏃';
}

function filterEvents() {
    const sport = document.getElementById('sportFilter').value;
    const city = document.getElementById('cityFilter').value;
    const maxPrice = parseInt(document.getElementById('priceRange').value);
    const date = document.getElementById('dateFilter').value;
    const searchTerm = document.getElementById('searchInput').value.toLowerCase();

    const filtered = events.filter(event => {
        if (event.status !== 'approved') return false;
        if (sport !== 'all' && event.sport !== sport) return false;
        if (city !== 'all' && event.city !== city) return false;
        if (event.price > maxPrice) return false;
        if (date && event.date !== date) return false;
        if (searchTerm) {
            const searchableText = `${event.price} ${event.title} ${event.sportName} ${event.location} ${event.cityName} ${event.description || ''}`.toLowerCase();
            if (!searchableText.includes(searchTerm)) return false;
        }
        return true;
    });

    displayEvents(filtered);
    updateMapMarkers(filtered);
}

function applyFilters() {
    filterEvents();
    showNotification('Фильтры применены', 'success');
}

function filterBySport(sport) {
    document.getElementById('sportFilter').value = sport;
    filterEvents();
}

function initMap() {
    if (!document.getElementById('belarus-map')) return;
    map = L.map('belarus-map', { attributionControl: false }).setView([53.7098, 27.9534], 7);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { attribution: false }).addTo(map);
    updateMapMarkers(events.filter(e => e.status === 'approved'));
}

function updateMapMarkers(filteredEvents) {
    if (!map) return;
    markers.forEach(marker => map.removeLayer(marker));
    markers = [];

    filteredEvents.forEach(event => {
        if (!event.coordinates || !event.coordinates[0]) return;
        const marker = L.marker(event.coordinates, {
            icon: L.divIcon({
                className: 'custom-marker',
                html: `<div style="display: flex; align-items: center; justify-content: center; width: 100%; height: 100%;">${event.icon}</div>`,
                iconSize: [30, 30]
            })
        }).addTo(map);

        marker.bindPopup(`
            <b>${event.title}</b><br>
            📅 ${event.dateDisplay}<br>
            ⏰ ${event.time}<br>
            💰 ${event.price} BYN<br>
            <button onclick="showEventDetails(${event.id})" style="margin-top:10px; padding:5px 10px; background:#4169E1; color:white; border:none; border-radius:4px; cursor:pointer;">Подробнее</button>
        `);

        markers.push(marker);
    });
}

function getAuthHeaders() {
    const token = localStorage.getItem('token');
    return token ? {
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
    } : {};
}

async function loadUserFavorites() {
    if (!currentUser) return;
    try {
        const response = await fetch(`${API_BASE}/favorites/my`, {
            headers: getAuthHeaders()
        });
        if (response.ok) {
            favorites = await response.json();
        } else {
            favorites = [];
        }
    } catch (e) {
        console.error('Ошибка загрузки избранного', e);
        favorites = [];
    }
    updateFavoritesCount();
    if (window.location.pathname.includes('profile.html') && typeof displayFavorites === 'function') {
        displayFavorites();
    }
}

async function loadUserRegistrations() {
    if (!currentUser) return;
    try {
        const response = await fetch(`${API_BASE}/registrations/my`, {
            headers: getAuthHeaders()
        });
        if (response.ok) {
            myEvents = await response.json();
        } else {
            myEvents = [];
        }
    } catch (e) {
        console.error('Ошибка загрузки регистраций', e);
        myEvents = [];
    }
    if (window.location.pathname.includes('profile.html') && typeof displayMyEvents === 'function') {
        displayMyEvents();
    }
}

async function loadUserProfileData() {
    await Promise.all([loadUserFavorites(), loadUserRegistrations()]);
}

function updateFavoritesCount() {
    const countEl = document.getElementById('favoritesCount');
    if (countEl) countEl.textContent = favorites.length;
}

async function toggleFavorite(eventId) {
    if (!currentUser || currentUser.isAdmin) {
        showNotification('Войдите в систему', 'warning');
        return;
    }
    const isFav = favorites.includes(eventId);
    const method = isFav ? 'DELETE' : 'POST';
    try {
        const response = await fetch(`${API_BASE}/favorites/${eventId}`, {
            method: method,
            headers: getAuthHeaders()
        });
        const data = await response.json().catch(() => ({}));
        if (!response.ok) {
            showNotification(data.error || 'Ошибка', 'warning');
            return;
        }
        if (isFav) {
            favorites = favorites.filter(id => id !== eventId);
            showNotification('Удалено из избранного', 'info');
        } else {
            favorites.push(eventId);
            showNotification('Добавлено в избранное', 'success');
        }
        updateFavoritesCount();
        displayEvents(events.filter(e => e.status === 'approved'));
        if (window.location.pathname.includes('profile.html') && typeof displayFavorites === 'function') {
            displayFavorites();
        }
    } catch (e) {
        showNotification('Ошибка соединения', 'warning');
    }
}

async function registerForEvent() {
    if (!currentUser) {
        closeEventModal();
        showAuthModal();
        showNotification('Войдите в систему', 'warning');
        return;
    }
    if (currentUser.isAdmin) {
        showNotification('Администратор не может записываться на мероприятия', 'warning');
        return;
    }
    const event = events.find(e => e.id === currentEventId);
    if (!event) return;
    if (event.currentParticipants >= event.maxParticipants) {
        showNotification('Все места заняты', 'warning');
        return;
    }
    if (myEvents.includes(event.id)) {
        showNotification('Вы уже записаны на это мероприятие', 'info');
        closeEventModal();
        return;
    }
    try {
        const response = await fetch(`${API_BASE}/registrations/${event.id}`, {
            method: 'POST',
            headers: getAuthHeaders()
        });
        const data = await response.json().catch(() => ({}));
        if (!response.ok) {
            showNotification(data.error || 'Ошибка записи', 'warning');
            return;
        }
        event.currentParticipants += 1;
        myEvents.push(event.id);
        showNotification('Вы успешно записались на мероприятие!', 'success');
        closeEventModal();
        displayEvents(events.filter(e => e.status === 'approved'));
        if (typeof updateMapMarkers === 'function') updateMapMarkers(events.filter(e => e.status === 'approved'));
        if (window.location.pathname.includes('profile.html') && typeof displayMyEvents === 'function') {
            displayMyEvents();
        }
    } catch (e) {
        showNotification('Ошибка соединения', 'warning');
    }
}

async function unregisterFromEvent(eventId) {
    if (!currentUser) {
        showNotification('Войдите в систему', 'warning');
        return;
    }
    if (!confirm('Вы уверены, что хотите отменить запись?')) return;
    try {
        const res = await fetch(`${API_BASE}/registrations/${eventId}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        });
        const data = await res.json().catch(() => ({}));
        if (!res.ok) {
            showNotification(data.error || 'Ошибка отмены записи', 'warning');
            return;
        }
        const index = myEvents.indexOf(eventId);
        if (index > -1) myEvents.splice(index, 1);
        const event = events.find(e => e.id === eventId);
        if (event) event.currentParticipants = Math.max(0, event.currentParticipants - 1);
        showNotification('Запись отменена', 'success');
        displayEvents(events.filter(e => e.status === 'approved'));
        if (typeof updateMapMarkers === 'function') updateMapMarkers(events.filter(e => e.status === 'approved'));
        if (document.getElementById('myEventsList')) displayMyEvents && displayMyEvents();
    } catch (e) {
        showNotification('Ошибка соединения', 'warning');
    }
}

async function loadReviewsForEvent(eventId) {
    try {
        const response = await fetch(`${API_BASE}/reviews/event/${eventId}`);
        if (!response.ok) return '<p>Не удалось загрузить отзывы</p>';
        const reviews = await response.json();
        if (reviews.length === 0) return '<p>Пока нет отзывов</p>';
        let html = '';
        reviews.forEach(r => {
            html += `
                <div style="border-bottom:1px solid #ccc; padding:10px;">
                    <strong>${r.user?.firstName || 'Пользователь'}</strong> (${r.rating}★)
                    <p>${r.comment}</p>
                    <small>${new Date(r.createdAt).toLocaleDateString()}</small>
                </div>
            `;
        });
        return html;
    } catch (e) {
        return '<p>Ошибка загрузки отзывов</p>';
    }
}

function showEventDetails(eventId) {
    const modal = document.getElementById('eventModal');
    const details = document.getElementById('eventDetails');
    const favBtn = document.getElementById('favBtn');
    const registerBtn = document.getElementById('registerBtn');
    const reviewBtn = document.getElementById('reviewBtn');
    currentEventId = eventId;
    const event = events.find(e => e.id === eventId);
    if (!event) return;

    loadReviewsForEvent(eventId).then(reviewsHtml => {
        const isRegistered = myEvents.includes(eventId);
        const isFull = event.currentParticipants >= event.maxParticipants;
        details.innerHTML = `
            <div style="margin:15px 0;">
                <img src="${event.imageUrl}" alt="${event.title}" style="width:100%; height:200px; object-fit:cover; border-radius:8px; margin-bottom:15px;">
                <h3>${event.title}</h3>
                <p><strong>📅 Дата:</strong> ${event.dateDisplay}</p>
                <p><strong>⏰ Время:</strong> ${event.time}</p>
                <p><strong>📍 Место:</strong> ${event.location}</p>
                <p><strong>💰 Стоимость:</strong> ${event.price} BYN</p>
                <p><strong>👥 Участников:</strong> ${event.currentParticipants} / ${event.maxParticipants}</p>
                <p><strong>📝 Описание:</strong> ${event.description}</p>
                <hr>
                <h4>Отзывы</h4>
                <div id="reviewsContainer">${reviewsHtml}</div>
            </div>
        `;
        if (currentUser && !currentUser.isAdmin) {
            favBtn.style.display = 'block';
            favBtn.textContent = favorites.includes(eventId) ? '❤️ В избранном' : '🤍 В избранное';
            if (isRegistered) {
                registerBtn.style.display = 'block';
                registerBtn.textContent = '✅ Вы записаны';
                registerBtn.disabled = true;
            } else if (isFull) {
                registerBtn.style.display = 'block';
                registerBtn.textContent = '❌ Все места заняты';
                registerBtn.disabled = true;
            } else {
                registerBtn.style.display = 'block';
                registerBtn.textContent = '✅ Записаться';
                registerBtn.disabled = false;
            }
            reviewBtn.style.display = 'block';
        } else {
            favBtn.style.display = 'none';
            registerBtn.style.display = 'none';
            reviewBtn.style.display = 'none';
        }
        modal.style.display = 'block';
    });
}

function closeEventModal() {
    document.getElementById('eventModal').style.display = 'none';
}

function addToFavorites() {
    if (!currentUser) {
        closeEventModal();
        showAuthModal();
        showNotification('Войдите в систему', 'warning');
        return;
    }
    toggleFavorite(currentEventId);
    closeEventModal();
}

function showReviewForm(eventId) {
    document.getElementById('reviewModal').style.display = 'block';
}

function closeReviewModal() {
    document.getElementById('reviewModal').style.display = 'none';
}

function setRating(rating) {
    currentRating = rating;
    document.querySelectorAll('.star').forEach((star, index) => {
        star.classList.toggle('active', index < rating);
    });
}

async function submitReview() {
    if (!currentUser) {
        showNotification('Войдите в систему', 'warning');
        closeReviewModal();
        return;
    }
    const rating = currentRating;
    const comment = document.getElementById('reviewText').value.trim();

    if (rating === 0) {
        showNotification('Выберите оценку', 'warning');
        return;
    }
    if (!comment) {
        showNotification('Напишите отзыв', 'warning');
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/reviews`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify({
                eventId: currentEventId,
                rating: rating,
                comment: comment
            })
        });
        const data = await response.json();
        if (!response.ok) {
            showNotification(data.error || 'Ошибка', 'warning');
            return;
        }
        showNotification('Спасибо за отзыв!', 'success');
        closeReviewModal();
        showEventDetails(currentEventId);
    } catch (e) {
        showNotification('Ошибка соединения', 'warning');
    }
}

async function addNewEvent(event) {
    event.preventDefault();
    if (!currentUser || currentUser.isAdmin) {
        showNotification('Войдите в систему', 'warning');
        return;
    }
    if (!selectedCoordinates) {
        showNotification('Выберите место на карте', 'warning');
        return;
    }
    const sportSelect = document.getElementById('newEventSport');
    const sportId = sportSelect.value;
    const sportName = sportSelect.options[sportSelect.selectedIndex].text;
    const citySelect = document.getElementById('newEventCity');
    const cityId = citySelect.value;
    const cityName = citySelect.options[citySelect.selectedIndex].text;
    const dateValue = document.getElementById('newEventDate').value;
    if (!dateValue) {
        showNotification('Выберите дату', 'warning');
        return;
    }
    const timeValue = document.getElementById('newEventTime').value || '10:00';
    const location = document.getElementById('newEventLocation').value;
    const price = parseFloat(document.getElementById('newEventPrice').value) || 0;
    const maxParticipants = parseInt(document.getElementById('newEventMaxParticipants').value) || 50;
    const description = document.getElementById('newEventDescription').value;
    const imageFile = document.getElementById('newEventImage').files[0];
    let imageBase64 = '';
    if (imageFile) {
        imageBase64 = await fileToBase64(imageFile);
    } else {
        imageBase64 = 'https://images.unsplash.com/photo-1461896836934-ffe607ba8211?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80';
    }

    const newEventData = {
        title: document.getElementById('newEventTitle').value,
        description: description,
        sportId: sportId,
        cityId: cityId,
        date: dateValue,
        time: timeValue,
        location: location,
        price: price,
        maxParticipants: maxParticipants,
        imageUrl: imageBase64,
        latitude: selectedCoordinates[0],
        longitude: selectedCoordinates[1]
    };

    try {
        const response = await fetch(`${API_BASE}/events`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify(newEventData)
        });
        const data = await response.json();
        if (!response.ok) {
            showNotification(data.error || 'Ошибка создания события', 'warning');
            return;
        }
        showNotification('Событие отправлено на модерацию', 'success');
        document.getElementById('addEventForm').reset();
        document.getElementById('imagePreview').innerHTML = '';
        document.getElementById('imagePreview').classList.remove('active');
        if (locationMarker) {
            locationMap.removeLayer(locationMarker);
            locationMarker = null;
        }
        selectedCoordinates = null;
        document.getElementById('selectedCoordinates').innerHTML = 'Координаты не выбраны';
        if (typeof switchUserTab === 'function') {
            switchUserTab('my-events');
        }
    } catch (e) {
        showNotification('Ошибка соединения', 'warning');
    }
}

function previewImage(event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        const preview = document.getElementById('imagePreview');
        reader.onload = function(e) {
            preview.innerHTML = `<img src="${e.target.result}" alt="Preview">`;
            preview.classList.add('active');
        };
        reader.readAsDataURL(file);
    }
}

function fileToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = reject;
    });
}

function getSportIcon(sport) {
    const icons = { running: '🏃', football: '⚽', basketball: '🏀', volleyball: '🏐', tennis: '🎾', swimming: '🏊', cycling: '🚴', skiing: '⛷️' };
    return icons[sport] || '🏃';
}

function initLocationMap() {
    const mapContainer = document.getElementById('eventLocationMap');
    if (!mapContainer) return;
    if (locationMap) locationMap.remove();
    locationMap = L.map('eventLocationMap', { attributionControl: false }).setView([53.7098, 27.9534], 7);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { attribution: false }).addTo(locationMap);
    locationMap.on('click', function(e) {
        const { lat, lng } = e.latlng;
        selectedCoordinates = [lat, lng];
        if (locationMarker) {
            locationMarker.setLatLng([lat, lng]);
        } else {
            locationMarker = L.marker([lat, lng], { draggable: true }).addTo(locationMap);
            locationMarker.on('dragend', function(e) {
                selectedCoordinates = [e.target.getLatLng().lat, e.target.getLatLng().lng];
                updateCoordinatesDisplay();
            });
        }
        updateCoordinatesDisplay();
    });
}

function updateCoordinatesDisplay() {
    const display = document.getElementById('selectedCoordinates');
    if (display && selectedCoordinates) {
        display.innerHTML = `<strong>Выбраны координаты:</strong><br>Широта: ${selectedCoordinates[0].toFixed(4)}<br>Долгота: ${selectedCoordinates[1].toFixed(4)}`;
    }
}

function showAuthModal() {
    document.getElementById('authModal').style.display = 'block';
    switchAuthTab('login');
}
function closeAuthModal() {
    document.getElementById('authModal').style.display = 'none';
}
function switchAuthTab(tab) {
    const loginFields = document.getElementById('registerFields');
    const loginFields2 = document.getElementById('registerFields2');
    const submitBtn = document.getElementById('authSubmitBtn');
    document.querySelectorAll('.auth-tab').forEach(t => t.classList.remove('active'));
    const activeTab = document.querySelector(tab === 'login' ? '#loginTab' : '#registerTab');
    if (activeTab) activeTab.classList.add('active');
    if (tab === 'login') {
        loginFields.style.display = 'none';
        loginFields2.style.display = 'none';
        submitBtn.textContent = 'Войти';
        document.getElementById('authModalTitle').textContent = 'Вход';
    } else {
        loginFields.style.display = 'block';
        loginFields2.style.display = 'block';
        submitBtn.textContent = 'Зарегистрироваться';
        document.getElementById('authModalTitle').textContent = 'Регистрация';
    }
}
function handleAuth(event) {
    event.preventDefault();
    const email = document.getElementById('authEmail').value;
    const password = document.getElementById('authPassword').value;
    const activeTab = document.querySelector('.auth-tab.active');
    const isLogin = activeTab ? activeTab.textContent === 'Вход' : true;
    if (email === 'admin@eventarena.by' && password === 'admin123') {
        currentUser = { id: 'admin', name: 'Администратор', email, isAdmin: true };
        localStorage.setItem('currentUser', JSON.stringify(currentUser));
        showNotification('Добро пожаловать, Администратор!', 'success');
        closeAuthModal();
        window.location.href = 'admin.html';
        return;
    }
    if (isLogin) {
        const users = JSON.parse(localStorage.getItem('users') || '[]');
        const user = users.find(u => u.email === email && u.password === password);
        if (user) {
            currentUser = { id: user.id, name: user.name, lastName: user.lastName || '', email, isAdmin: false };
            localStorage.setItem('currentUser', JSON.stringify(currentUser));
            showNotification('Добро пожаловать!', 'success');
            closeAuthModal();
            window.location.href = 'profile.html';
        } else {
            showNotification('Неверный email или пароль', 'warning');
        }
    } else {
        const name = document.getElementById('regName').value;
        const lastName = document.getElementById('regLastName').value;
        if (!name || !lastName) {
            showNotification('Заполните все поля', 'warning');
            return;
        }
        const users = JSON.parse(localStorage.getItem('users') || '[]');
        if (users.find(u => u.email === email)) {
            showNotification('Пользователь с таким email уже существует', 'warning');
            return;
        }
        const newUser = { id: Date.now(), name, lastName, email, password };
        users.push(newUser);
        localStorage.setItem('users', JSON.stringify(users));
        currentUser = { id: newUser.id, name, lastName, email, isAdmin: false };
        localStorage.setItem('currentUser', JSON.stringify(currentUser));
        showNotification('Регистрация прошла успешно!', 'success');
        closeAuthModal();
        window.location.href = 'profile.html';
    }
}

function showNotification(message, type = 'info') {
    const notification = document.getElementById('notification');
    const messageEl = document.getElementById('notificationMessage');
    messageEl.textContent = message;
    notification.style.borderLeftColor = type === 'success' ? '#28a745' : type === 'warning' ? '#ffc107' : '#9370DB';
    notification.style.display = 'block';
    setTimeout(() => notification.style.display = 'none', 3000);
}

window.onclick = function(event) {
    const eventModal = document.getElementById('eventModal');
    const reviewModal = document.getElementById('reviewModal');
    if (event.target === eventModal) eventModal.style.display = 'none';
    if (event.target === reviewModal) reviewModal.style.display = 'none';
};