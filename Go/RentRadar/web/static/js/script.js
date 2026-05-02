

const API_URL = '/api';


let currentUserId = localStorage.getItem('userId');

function setCurrentUserId(userId) {
    currentUserId = userId ? String(userId) : '';
    if (currentUserId) {
        localStorage.setItem('userId', currentUserId);
    } else {
        localStorage.removeItem('userId');
    }
}


async function apiRequest(endpoint, options = {}) {
    const headers = {
        'Content-Type': 'application/json',
        ...options.headers,
    };

    if (currentUserId) {
        headers['X-User-ID'] = currentUserId;
    }

    const response = await fetch(`${API_URL}${endpoint}`, {
        ...options,
        headers,
    });

    const data = await response.json();

    if (!response.ok) {
        throw new Error(data.error || 'Request failed');
    }

    return data;
}


async function isUserLoggedIn() {
    if (!currentUserId) return false;
    try {
        await apiRequest('/me');
        return true;
    } catch {
        return false;
    }
}

window.isUserLoggedIn = isUserLoggedIn;


async function login(identifier, password) {
    try {
        const data = await apiRequest('/login', {
            method: 'POST',
            body: JSON.stringify({ identifier, password }),
        });

        if (data && data.user_id) {
            setCurrentUserId(data.user_id);
            localStorage.setItem('userName', data.first_name || (data.email ? data.email.split('@')[0] : 'Пользователь'));
            return data;
        } else {
            throw new Error('Пользователь не найден');
        }
    } catch (err) {
        console.error('Login error:', err);
        throw err;
    }
}


async function register(userData) {
    const data = await apiRequest('/register', {
        method: 'POST',
        body: JSON.stringify(userData),
    });

    setCurrentUserId(data.user_id);
    localStorage.setItem('userName', data.first_name || data.email.split('@')[0]);
    return data;
}


function logoutUser() {
    apiRequest('/logout', { method: 'POST' })
        .catch(() => {})
        .finally(() => {
            setCurrentUserId(null);
            localStorage.removeItem('userName');
            updateUIForLoggedOutUser();
            window.location.href = '/index.html';
        });
}


async function getCurrentUser() {
    if (!currentUserId) return null;
    try {
        return await apiRequest('/me');
    } catch {
        return null;
    }
}


async function getListings(type = null, city = null) {
    let url = '/listings';
    const params = [];
    if (type) params.push(`type=${type}`);
    if (city) params.push(`city=${encodeURIComponent(city)}`);
    if (params.length) url += '?' + params.join('&');

    return await apiRequest(url);
}


async function createListing(listingData) {
    return await apiRequest('/listings', {
        method: 'POST',
        body: JSON.stringify(listingData),
    });
}


window.togglePassword = function(inputId, button) {
    const input = document.getElementById(inputId);
    const icon = button.querySelector('i');

    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    } else {
        input.type = 'password';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    }
};


function updateUIForLoggedInUser() {
    const userActions = document.querySelector('.user-actions');
    if (!userActions) return;

    const userName = localStorage.getItem('userName') || 'Пользователь';

    userActions.innerHTML = `
        <div class="user-menu">
            <button class="btn btn-outline user-menu-btn">
                <i class="fas fa-user"></i> ${userName} <i class="fas fa-chevron-down"></i>
            </button>
            <div class="user-dropdown">
                <a href="/profile.html">Мой профиль</a>
                <a href="my-listings.html">Мои объявления</a>
                <a href="favorites.html">Избранное</a>
                <a href="messages.html">Сообщения</a>
                <a href="/reviews.html">Отзывы</a>
                <a href="#" id="logoutBtn">Выйти</a>
            </div>
        </div>
        <a href="/create-listing.html" class="btn btn-primary" id="postAdLink">Разместить объявление</a>
    `;

    setupUserMenu();
}


function updateUIForLoggedOutUser() {
    const userActions = document.querySelector('.user-actions');
    if (!userActions) return;

    userActions.innerHTML = `
        <a href="/login.html" class="btn btn-outline" id="loginBtn">Войти</a>
        <a href="/login.html" class="btn btn-primary" id="postAdLink">Разместить объявление</a>
    `;
}


function setupUserMenu() {
    const userMenu = document.querySelector('.user-menu');
    if (!userMenu) return;

    const menuBtn = userMenu.querySelector('.user-menu-btn');
    const dropdown = userMenu.querySelector('.user-dropdown');

    if (menuBtn && dropdown) {
        menuBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            dropdown.classList.toggle('show');
        });

        document.addEventListener('click', function(e) {
            if (!userMenu.contains(e.target)) {
                dropdown.classList.remove('show');
            }
        });
    }

    const logoutBtn = document.getElementById('logoutBtn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function(e) {
            e.preventDefault();
            logoutUser();
        });
    }
}


function initAuthPage() {
    const loginTab = document.getElementById('loginTab');
    if (!loginTab) return;

    
    (async () => {
        if (await isUserLoggedIn()) {
            window.location.href = '/profile.html';
        }
    })();

    const registerTab = document.getElementById('registerTab');
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');
    const switchToRegister = document.getElementById('switchToRegister');
    const switchToLogin = document.getElementById('switchToLogin');

    function showLogin() {
        loginTab.classList.add('active');
        registerTab.classList.remove('active');
        loginForm.classList.add('active');
        registerForm.classList.remove('active');
    }

    function showRegister() {
        registerTab.classList.add('active');
        loginTab.classList.remove('active');
        registerForm.classList.add('active');
        loginForm.classList.remove('active');
    }

    loginTab.addEventListener('click', showLogin);
    registerTab.addEventListener('click', showRegister);

    if (switchToRegister) {
        switchToRegister.addEventListener('click', (e) => {
            e.preventDefault();
            showRegister();
        });
    }

    if (switchToLogin) {
        switchToLogin.addEventListener('click', (e) => {
            e.preventDefault();
            showLogin();
        });
    }

    
    const loginFormElement = document.getElementById('loginFormElement');
    if (loginFormElement) {
        loginFormElement.addEventListener('submit', async (e) => {
            e.preventDefault();

            const identifier = document.getElementById('loginIdentifier').value;
            const password = document.getElementById('loginPassword').value;

            if (!identifier || !password) {
                alert('Заполните все поля');
                return;
            }

            try {
                await login(identifier, password);
                window.location.href = '/profile.html';
            } catch (err) {
                alert(err.message || 'Ошибка входа');
            }
        });
    }

    
    const registerFormElement = document.getElementById('registerFormElement');
    if (registerFormElement) {
        const passwordInput = document.getElementById('registerPassword');
        const confirmInput = document.getElementById('registerConfirmPassword');
        const passwordStrength = document.getElementById('passwordStrength');
        const passwordMatch = document.getElementById('passwordMatch');

        function checkPasswordStrength(password) {
            let strength = 0;
            if (password.length >= 8) strength++;
            if (/[a-z]/.test(password)) strength++;
            if (/[A-Z]/.test(password)) strength++;
            if (/[0-9]/.test(password)) strength++;
            if (/[!@#$%^&*(),.?":{}|<>]/.test(password)) strength++;
            return strength;
        }

        if (passwordInput) {
            passwordInput.addEventListener('input', function() {
                const password = this.value;

                if (password.length === 0) {
                    passwordStrength.textContent = '';
                    passwordStrength.className = 'password-strength-minimal';
                    return;
                }

                const strength = checkPasswordStrength(password);

                if (strength <= 2) {
                    passwordStrength.textContent = 'Слабый пароль';
                    passwordStrength.className = 'password-strength-minimal weak';
                } else if (strength <= 4) {
                    passwordStrength.textContent = 'Средний пароль';
                    passwordStrength.className = 'password-strength-minimal medium';
                } else {
                    passwordStrength.textContent = 'Надёжный пароль';
                    passwordStrength.className = 'password-strength-minimal strong';
                }
            });
        }

        if (confirmInput && passwordMatch) {
            function checkPasswordMatch() {
                const password = passwordInput.value;
                const confirm = confirmInput.value;

                if (confirm.length === 0) {
                    passwordMatch.textContent = '';
                    return;
                }

                if (password === confirm) {
                    passwordMatch.textContent = 'Пароли совпадают';
                    passwordMatch.className = 'password-match-minimal match';
                } else {
                    passwordMatch.textContent = 'Пароли не совпадают';
                    passwordMatch.className = 'password-match-minimal';
                }
            }

            confirmInput.addEventListener('input', checkPasswordMatch);
            passwordInput.addEventListener('input', checkPasswordMatch);
        }

        registerFormElement.addEventListener('submit', async (e) => {
            e.preventDefault();

            const firstName = document.getElementById('registerFirstName').value;
            const lastName = document.getElementById('registerLastName').value;
            const identifier = document.getElementById('registerIdentifier').value;
            const password = document.getElementById('registerPassword').value;
            const confirmPassword = document.getElementById('registerConfirmPassword').value;

            if (password !== confirmPassword) {
                alert('Пароли не совпадают!');
                return;
            }

            if (checkPasswordStrength(password) < 3) {
                alert('Пароль слишком слабый. Используйте минимум 8 символов, заглавные и строчные буквы, цифры');
                return;
            }

            const isEmail = identifier.includes('@');

            try {
                await register({
                    email: isEmail ? identifier : '',
                    phone: !isEmail ? identifier : '',
                    password: password,
                    first_name: firstName,
                    last_name: lastName,
                });
                window.location.href = '/profile.html';
            } catch (err) {
                alert(err.message || 'Ошибка регистрации');
            }
        });
    }
}

let currentFilters = {
    type: '',
    propertyType: 'all',
    rooms: 'all',
    minPrice: '',
    maxPrice: '',
    minArea: '',
    maxArea: '',
    city: '',
    utilitiesIncluded: false,
    furniture: false,
    children: false,
    pets: false,
    parking: false,
    elevator: false
};

async function loadListings() {
    const listingsGrid = document.querySelector('.listings-grid');
    if (!listingsGrid) return;

    const isSalePage = window.location.pathname.includes('sale.html');
    const listingType = isSalePage ? 'sale' : currentFilters.type;

    let url = '/listings';
    if (listingType) {
        url += `?type=${listingType}`;
    }

    const appendParam = (key, value) => {
        url += (url.includes('?') ? '&' : '?') + `${key}=${encodeURIComponent(value)}`;
    };

    if (currentFilters.city && currentFilters.city !== '') {
        appendParam('city', currentFilters.city);
    }

    if (currentFilters.minPrice && currentFilters.minPrice !== '') {
        appendParam('minPrice', currentFilters.minPrice);
    }
    if (currentFilters.maxPrice && currentFilters.maxPrice !== '') {
        appendParam('maxPrice', currentFilters.maxPrice);
    }

    if (currentFilters.minArea && currentFilters.minArea !== '') {
        appendParam('minArea', currentFilters.minArea);
    }
    if (currentFilters.maxArea && currentFilters.maxArea !== '') {
        appendParam('maxArea', currentFilters.maxArea);
    }

    if (currentFilters.rooms && currentFilters.rooms !== 'all') {
        appendParam('rooms', currentFilters.rooms);
    }

    try {
        const listingsRaw = await apiRequest(url);
        const listings = Array.isArray(listingsRaw) ? listingsRaw : [];
        const favoriteIds = await getFavoriteIds();

        let filteredListings = listings;

        if (currentFilters.propertyType && currentFilters.propertyType !== 'all') {
            filteredListings = filteredListings.filter(l =>
                l.property_type === currentFilters.propertyType ||
                (currentFilters.propertyType === 'apartment' && l.title?.toLowerCase().includes('квартир')) ||
                (currentFilters.propertyType === 'house' && l.title?.toLowerCase().includes('дом')) ||
                (currentFilters.propertyType === 'studio' && l.title?.toLowerCase().includes('студи'))
            );
        }

        if (currentFilters.utilitiesIncluded) {
            filteredListings = filteredListings.filter(l => l.utilities_included === true);
        }

        if (listingsGrid) {
            if (filteredListings.length === 0) {
                listingsGrid.innerHTML = '<p class="no-results">Нет объявлений, соответствующих фильтрам</p>';
                return;
            }

            listingsGrid.innerHTML = filteredListings.map(listing => {
                const badge = listing.listing_type === 'sale' ? '<span class="badge">Продажа</span>' : '';

                return `
                <article class="listing-card">
                    <div class="listing-image">
                        <img src="${listing.photos ? listing.photos.split(',')[0] : 'https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=400&h=250&fit=crop'}" alt="${escapeHtml(listing.title)}">
                        ${badge}
                    </div>
                    <div class="listing-content">
                        <div class="listing-header">
                            <h4 class="listing-title">${escapeHtml(listing.title)}</h4>
                            <div class="listing-price">${listing.price.toLocaleString()} BYN <span>${listing.listing_type === 'rent' ? '/ месяц' : ''}</span></div>
                        </div>
                        <div class="listing-location">
                            <i class="fas fa-map-marker-alt"></i> ${escapeHtml(listing.district || listing.city)}
                        </div>
                        <div class="listing-details">
                            <span><i class="fas fa-vector-square"></i> ${listing.area} м²</span>
                            <span><i class="fas fa-layer-group"></i> ${listing.floor}/${listing.total_floors} эт.</span>
                            ${listing.listing_type === 'rent' ? '<span><i class="fas fa-bed"></i> ' + listing.rooms + ' комн.</span>' : ''}
                            <span class="rating"><i class="fas fa-star"></i> 4.8 (15)</span>
                        </div>
                        <div class="listing-footer">
                            <span class="available-date"><i class="far fa-calendar-alt"></i> ${listing.available_from ? `Свободно с ${new Date(listing.available_from).toLocaleDateString()}` : 'В продаже'}</span>
                                <button class="btn-like" data-id="${listing.id}">
                                    <i class="far fa-heart"></i>
                                </button>
                        </div>
                    </div>
                </article>
            `}).join('');
        }

        document.querySelectorAll('.btn-like').forEach(btn => {
            btn.removeEventListener('click', handleLikeClick);
            btn.addEventListener('click', handleLikeClick);
        });
        applyFavoriteStateForCards(favoriteIds);

    } catch (err) {
        console.error('Failed to load listings:', err);
        if (listingsGrid) {
            listingsGrid.innerHTML = `<p class="error">Ошибка загрузки объявлений: ${escapeHtml(err.message || 'unknown error')}</p>`;
        }
    }
}

function handleLikeClick(e) {
    e.preventDefault();
    toggleFavorite(this);
}

async function toggleFavorite(button) {
    const listingId = Number(button.dataset.id);
    if (!listingId) return;
    const icon = button.querySelector('i');
    if (!icon) return;

    if (!await isUserLoggedIn()) {
        window.location.href = '/login.html';
        return;
    }

    const isFavorite = icon.classList.contains('fas');
    try {
        if (isFavorite) {
            await removeFromFavorites(listingId);
            icon.classList.remove('fas');
            icon.classList.add('far');
            icon.style.color = '';
        } else {
            await addToFavorites(listingId);
            icon.classList.remove('far');
            icon.classList.add('fas');
            icon.style.color = '#ff5a5f';
        }
    } catch (err) {
        console.error('Failed to toggle favorite:', err);
    }
}

async function getFavoriteIds() {
    if (!await isUserLoggedIn()) return new Set();
    try {
        const favorites = await getFavorites();
        return new Set((favorites || []).map(item => item.id));
    } catch {
        return new Set();
    }
}

function applyFavoriteStateForCards(favoriteIds) {
    document.querySelectorAll('.btn-like[data-id]').forEach(btn => {
        const listingId = Number(btn.dataset.id);
        const icon = btn.querySelector('i');
        if (!listingId || !icon) return;

        if (favoriteIds.has(listingId)) {
            icon.classList.remove('far');
            icon.classList.add('fas');
            icon.style.color = '#ff5a5f';
        } else {
            icon.classList.remove('fas');
            icon.classList.add('far');
            icon.style.color = '';
        }
    });
}

let map = null;
let mapMarkers = [];

async function initMap() {
    const mapContainer = document.getElementById('map');
    if (!mapContainer) return;

    map = L.map('map').setView([53.9045, 27.5615], 12);

    L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        subdomains: 'abcd',
        maxZoom: 19,
        minZoom: 3
    }).addTo(map);

    
    await loadMapMarkers();
}


async function loadMapMarkers() {
    if (!map) return;

    
    mapMarkers.forEach(marker => map.removeLayer(marker));
    mapMarkers = [];

    
    const isSalePage = window.location.pathname.includes('sale.html');
    const listingType = isSalePage ? 'sale' : 'rent';

    
    const mapTitle = isSalePage ? 'Продажа недвижимости' : 'Аренда недвижимости';

    console.log(`Loading map markers for: ${mapTitle}, type: ${listingType}`);

    try {
        const listings = await getListings(listingType);

        console.log('Loaded listings for map:', listings);

        
        const listingsWithCoords = listings.filter(l => l.latitude && l.longitude);

        console.log('Listings with coordinates:', listingsWithCoords);

        if (listingsWithCoords.length === 0) {
            console.log('Нет объявлений с координатами');
            
            const testMarker = L.marker([53.9045, 27.5615]).addTo(map)
                .bindPopup('Тестовый маркер<br>Координаты работают!');
            mapMarkers.push(testMarker);
            return;
        }

        
        if (listingsWithCoords.length > 0) {
            map.setView([listingsWithCoords[0].latitude, listingsWithCoords[0].longitude], 12);
        }

        listingsWithCoords.forEach(listing => {
            console.log('Adding marker for:', listing.title, listing.latitude, listing.longitude);

            
            const markerHtml = `
                <div style="background-color: #2c7873; color: white; border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; border: 2px solid white; box-shadow: 0 2px 5px rgba(0,0,0,0.3); font-size: 12px; font-weight: bold;">
                    ${Math.round(listing.price).toLocaleString().split(' ')[0]}
                </div>
            `;

            const markerIcon = L.divIcon({
                html: markerHtml,
                iconSize: [36, 36],
                popupAnchor: [0, -18],
                className: 'custom-marker'
            });

            const marker = L.marker([listing.latitude, listing.longitude], { icon: markerIcon }).addTo(map);

            const popupContent = `
                <div class="map-popup-listing" style="min-width: 220px;">
                    <img src="${listing.photos ? listing.photos.split(',')[0] : 'https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=200&h=100&fit=crop'}" style="width: 100%; height: 100px; object-fit: cover; border-radius: 8px; margin-bottom: 8px;">
                    <h4 style="font-size: 14px; margin-bottom: 4px;">${escapeHtml(listing.title)}</h4>
                    <div style="color: var(--primary-teal); font-weight: bold;">${listing.price.toLocaleString()} BYN</div>
                    <div style="font-size: 12px; color: #666;">${escapeHtml(listing.address)}</div>
                    <a href="/listing?id=${listing.id}" style="color: #2c7873; text-decoration: none; font-size: 12px; margin-top: 5px; display: inline-block;">Подробнее →</a>
                </div>
            `;

            marker.bindPopup(popupContent);
            mapMarkers.push(marker);
        });

    } catch (err) {
        console.error('Failed to load map markers:', err);
    }
}


async function updateMapMarkers() {
    await loadMapMarkers();
}


function applyFilters() {
    
    const activeChip = document.querySelector('.filter-chip.active');
    if (activeChip && activeChip.textContent !== 'Квартира') {
        const propertyText = activeChip.textContent;
        if (propertyText === 'Дом') currentFilters.propertyType = 'house';
        else if (propertyText === 'Студия') currentFilters.propertyType = 'studio';
        else if (propertyText === 'Комната') currentFilters.propertyType = 'room';
        else if (propertyText === 'Апартаменты') currentFilters.propertyType = 'apartments';
        else if (propertyText === 'Дача') currentFilters.propertyType = 'cottage';
        else if (propertyText === 'Участок') currentFilters.propertyType = 'land';
        else if (propertyText === 'Коммерческая') currentFilters.propertyType = 'commercial';
        else currentFilters.propertyType = 'all';
    } else {
        currentFilters.propertyType = 'all'; 
    }

    
    const activeRoom = document.querySelector('.room-btn.active');
    if (activeRoom && !activeRoom.classList.contains('active-all')) {
        const roomText = activeRoom.textContent;
        currentFilters.rooms = roomText === '5+' ? '5' : roomText;
    } else {
        currentFilters.rooms = 'all';
    }

    
    const minPrice = document.getElementById('min-price');
    const maxPrice = document.getElementById('max-price');
    currentFilters.minPrice = minPrice ? minPrice.value : '';
    currentFilters.maxPrice = maxPrice ? maxPrice.value : '';

    
    const minArea = document.getElementById('min-area');
    const maxArea = document.getElementById('max-area');
    currentFilters.minArea = minArea ? minArea.value : '';
    currentFilters.maxArea = maxArea ? maxArea.value : '';

    
    const regionSelect = document.getElementById('region-select');
    if (regionSelect && regionSelect.value) {
        const regionMap = {
            'minsk-city': 'Минск',
            'minsk': 'Минская область',
            'brest': 'Брестская область',
            'vitebsk': 'Витебская область',
            'gomel': 'Гомельская область',
            'grodno': 'Гродненская область',
            'mogilev': 'Могилёвская область'
        };
        currentFilters.city = regionMap[regionSelect.value] || '';
    } else {
        currentFilters.city = '';
    }

    
    currentFilters.utilitiesIncluded = document.getElementById('filter-wifi')?.checked || false;
    currentFilters.furniture = document.getElementById('filter-pets')?.checked || false;
    currentFilters.children = document.getElementById('filter-children')?.checked || false;
    currentFilters.pets = document.getElementById('filter-washer')?.checked || false;
    currentFilters.parking = document.getElementById('filter-parking')?.checked || false;
    currentFilters.elevator = document.getElementById('filter-elevator')?.checked || false;

    
    loadListings();
    if (map) updateMapMarkers();
}


function resetFilters() {
    
    document.querySelectorAll('.filter-chip').forEach(chip => {
        chip.classList.remove('active');
    });
    

    
    document.querySelectorAll('.room-btn').forEach(btn => {
        btn.classList.remove('active');
    });

    
    const minPrice = document.getElementById('min-price');
    const maxPrice = document.getElementById('max-price');
    const minArea = document.getElementById('min-area');
    const maxArea = document.getElementById('max-area');
    const moveInDate = document.getElementById('move-in-date');
    const regionSelect = document.getElementById('region-select');

    if (minPrice) minPrice.value = '';
    if (maxPrice) maxPrice.value = '';
    if (minArea) minArea.value = '';
    if (maxArea) maxArea.value = '';
    if (moveInDate) moveInDate.value = '2026-03-15';
    if (regionSelect) regionSelect.value = '';

    
    document.querySelectorAll('.filter-checkboxes input[type="checkbox"]').forEach(cb => {
        cb.checked = false;
    });

    
    currentFilters = {
        type: window.location.pathname.includes('sale.html') ? 'sale' : '',
        propertyType: 'all',
        rooms: 'all',
        minPrice: '',
        maxPrice: '',
        minArea: '',
        maxArea: '',
        city: '',
        utilitiesIncluded: false,
        furniture: false,
        children: false,
        pets: false,
        parking: false,
        elevator: false
    };

    
    loadListings();
    if (map) updateMapMarkers();
}


function initMainPage() {
    if (!document.querySelector('.filter-chip')) return;

    
    const isSalePage = window.location.pathname.includes('sale.html');
    if (isSalePage) {
        currentFilters.type = 'sale';
    } else {
        currentFilters.type = '';
    }

    
    document.querySelectorAll('.filter-chip').forEach(chip => {
        chip.classList.remove('active');
    });

    
    if (isSalePage) {
        const firstChip = document.querySelector('.filter-chip');
        if (firstChip) firstChip.classList.add('active');
    }

    
    document.querySelectorAll('.room-btn').forEach(btn => {
        btn.classList.remove('active');
    });

    
    const filterChips = document.querySelectorAll('.filter-chip');
    filterChips.forEach(chip => {
        chip.addEventListener('click', function() {
            const parentGroup = this.closest('.filter-group');
            if (parentGroup) {
                const chipsInGroup = parentGroup.querySelectorAll('.filter-chip');
                chipsInGroup.forEach(c => c.classList.remove('active'));
            }
            this.classList.add('active');
        });
    });

    const roomBtns = document.querySelectorAll('.room-btn');
    roomBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            roomBtns.forEach(b => b.classList.remove('active'));
            this.classList.add('active');
        });
    });

    const applyFiltersBtn = document.querySelector('.btn-apply-filters');
    if (applyFiltersBtn) {
        applyFiltersBtn.removeEventListener('click', applyFilters);
        applyFiltersBtn.addEventListener('click', applyFilters);
    }

    const resetFiltersBtn = document.getElementById('resetFiltersBtn');
    if (resetFiltersBtn) {
        resetFiltersBtn.removeEventListener('click', resetFilters);
        resetFiltersBtn.addEventListener('click', resetFilters);
    }

    const searchBtn = document.querySelector('.search-btn');
    const searchInput = document.querySelector('.search-input');
    if (searchBtn && searchInput) {
        searchBtn.removeEventListener('click', handleSearch);
        searchBtn.addEventListener('click', handleSearch);
        searchInput.removeEventListener('keypress', handleSearchKeypress);
        searchInput.addEventListener('keypress', handleSearchKeypress);
    }

    const mapBtn = document.querySelector('.btn-map');
    if (mapBtn) {
        mapBtn.removeEventListener('click', showMap);
        mapBtn.addEventListener('click', showMap);
    }
}


function handleSearch() {
    const searchInput = document.querySelector('.search-input');
    if (searchInput) {
        const query = searchInput.value.trim();
        if (query) {
            currentFilters.city = query;
            loadListings();
        } else {
            currentFilters.city = '';
            loadListings();
        }
    }
}

function handleSearchKeypress(e) {
    if (e.key === 'Enter') {
        e.preventDefault();
        handleSearch();
    }
}

function showMap() {
    
    
    const listingsSection = document.querySelector('.listings-section');
    if (listingsSection) {
        listingsSection.scrollIntoView({ behavior: 'smooth' });
    }
}


async function loadSaleListings() {
    const listingsGrid = document.querySelector('.listings-grid');
    if (!listingsGrid) return;

    
    if (!window.location.pathname.includes('sale.html')) return;

    try {
        const listings = await getListings('sale');

        if (listings.length === 0) {
            listingsGrid.innerHTML = '<p>Нет объявлений о продаже</p>';
            return;
        }

        listingsGrid.innerHTML = listings.map(listing => `
            <article class="listing-card">
                <div class="listing-image">
                    <img src="${listing.photos ? listing.photos.split(',')[0] : 'https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=400&h=250&fit=crop'}" alt="${listing.title}">
                    <span class="badge">Продажа</span>
                </div>
                <div class="listing-content">
                    <div class="listing-header">
                        <h4 class="listing-title">${escapeHtml(listing.title)}</h4>
                        <div class="listing-price">${listing.price.toLocaleString()} BYN</div>
                    </div>
                    <div class="listing-location">
                        <i class="fas fa-map-marker-alt"></i> ${escapeHtml(listing.district || listing.city)}
                    </div>
                    <div class="listing-details">
                        <span><i class="fas fa-vector-square"></i> ${listing.area} м²</span>
                        <span><i class="fas fa-layer-group"></i> ${listing.floor}/${listing.total_floors} эт.</span>
                        <span><i class="fas fa-building"></i> Панельный</span>
                    </div>
                    <div class="listing-footer">
                        <span class="available-date"><i class="far fa-calendar-alt"></i> В продаже</span>
                        <button class="btn-like"><i class="far fa-heart"></i></button>
                    </div>
                </div>
            </article>
        `).join('');

        
        document.querySelectorAll('.btn-like').forEach(btn => {
            btn.addEventListener('click', function(e) {
                e.preventDefault();
                const icon = this.querySelector('i');
                if (icon.classList.contains('far')) {
                    icon.classList.remove('far');
                    icon.classList.add('fas');
                    icon.style.color = '#ff5a5f';
                } else {
                    icon.classList.remove('fas');
                    icon.classList.add('far');
                    icon.style.color = '';
                }
            });
        });

    } catch (err) {
        console.error('Failed to load sale listings:', err);
    }
}

async function getFavorites() {
    return await apiRequest('/favorites');
}


async function addToFavorites(listingId) {
    return await apiRequest('/favorites', {
        method: 'POST',
        body: JSON.stringify({ listing_id: listingId }),
    });
}


async function removeFromFavorites(listingId) {
    return await apiRequest(`/favorites/${listingId}`, {
        method: 'DELETE',
    });
}


async function getMyListings() {
    return await apiRequest('/my-listings');
}


async function updateProfile(userData) {
    return await apiRequest('/profile', {
        method: 'PUT',
        body: JSON.stringify(userData),
    });
}


async function changePassword(passwordData) {
    return await apiRequest('/change-password', {
        method: 'POST',
        body: JSON.stringify(passwordData),
    });
}

function initCreateListing() {
    const form = document.getElementById('createListingForm');
    if (!form) return;

    
    (async () => {
        if (!await isUserLoggedIn()) {
            window.location.href = '/login.html';
            return;
        }
    })();

    
    const rentBtn = document.getElementById('rentTypeBtn');
    const saleBtn = document.getElementById('saleTypeBtn');
    const rentOnlyFields = document.querySelectorAll('.rent-only');
    const saleOnlyFields = document.querySelectorAll('.sale-only');
    const pricePeriodLabel = document.getElementById('pricePeriodLabel');
    const priceLabel = document.getElementById('priceLabel');

    function updateFormForType(type) {
        if (type === 'rent') {
            rentOnlyFields.forEach(field => field.classList.remove('hidden'));
            saleOnlyFields.forEach(field => field.classList.add('hidden'));
            if (pricePeriodLabel) pricePeriodLabel.textContent = '(за месяц)';
            if (priceLabel) priceLabel.textContent = 'Цена за месяц (BYN) *';
        } else {
            rentOnlyFields.forEach(field => field.classList.add('hidden'));
            saleOnlyFields.forEach(field => field.classList.remove('hidden'));
            if (pricePeriodLabel) pricePeriodLabel.textContent = '(за всё)';
            if (priceLabel) priceLabel.textContent = 'Цена (BYN) *';
        }

        if (rentBtn && saleBtn) {
            rentBtn.classList.toggle('active', type === 'rent');
            saleBtn.classList.toggle('active', type === 'sale');
        }
    }

    if (rentBtn && saleBtn) {
        rentBtn.addEventListener('click', () => updateFormForType('rent'));
        saleBtn.addEventListener('click', () => updateFormForType('sale'));
    }

    
    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        if (!document.getElementById('agreeTerms')?.checked) {
            alert('Необходимо подтвердить право на сдачу/продажу');
            return;
        }

        const listingType = rentBtn?.classList.contains('active') ? 'rent' : 'sale';

        const listingData = {
            title: document.getElementById('listingTitle').value,
            description: document.getElementById('description').value,
            listing_type: listingType,
            price: parseFloat(document.getElementById('price').value),
            area: parseFloat(document.getElementById('area').value),
            rooms: parseInt(document.getElementById('rooms').value),
            floor: parseInt(document.getElementById('floor').value),
            total_floors: parseInt(document.getElementById('totalFloors').value),
            address: document.getElementById('address').value,
            city: document.getElementById('city').value,
            district: document.getElementById('district').value,
            available_from: document.getElementById('availableFrom')?.value || '',
            deposit: document.querySelector('input[name="deposit"]:checked')?.value || 'none',
            utilities_included: document.getElementById('utilitiesIncluded')?.checked || false,
            photos: '', 
        };

        try {
            await createListing(listingData);
            alert('Объявление опубликовано!');
            window.location.href = '/index.html';
        } catch (err) {
            alert(err.message || 'Ошибка при публикации');
        }
    });
}


function escapeHtml(str) {
    if (!str) return '';
    return str
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

function setupMobileMenu() {
    const mobileMenuBtn = document.querySelector('.mobile-menu-btn');
    if (mobileMenuBtn) {
        mobileMenuBtn.addEventListener('click', function() {
            const mainNav = document.querySelector('.main-nav');
            if (mainNav) {
                if (mainNav.style.display === 'flex') {
                    mainNav.style.display = 'none';
                } else {
                    mainNav.style.display = 'flex';
                    mainNav.style.flexDirection = 'column';
                    mainNav.style.position = 'absolute';
                    mainNav.style.top = '80px';
                    mainNav.style.left = '0';
                    mainNav.style.right = '0';
                    mainNav.style.background = 'white';
                    mainNav.style.padding = '20px';
                    mainNav.style.boxShadow = '0 4px 10px rgba(0,0,0,0.1)';
                    mainNav.style.zIndex = '1000';
                }
            }
        });
    }
}

function setActiveMenuItem() {
    const currentPage = window.location.pathname.split('/').pop() || 'index.html';
    const menuLinks = document.querySelectorAll('.main-nav a');

    menuLinks.forEach(link => {
        link.classList.remove('active');
        const href = link.getAttribute('href');

        if (currentPage === 'index.html' && (href === 'index.html' || href === '#')) {
            link.classList.add('active');
        } else if (currentPage === 'sale.html' && href === 'sale.html') {
            link.classList.add('active');
        } else if (href === currentPage) {
            link.classList.add('active');
        }
    });
}
async function deleteListing(listingId) {
    return await apiRequest(`/listings/${listingId}`, {
        method: 'DELETE',
    });
}
document.addEventListener('DOMContentLoaded', async () => {
    if (await isUserLoggedIn()) {
        if (!localStorage.getItem('userName')) {
            const me = await getCurrentUser();
            if (me) {
                localStorage.setItem('userName', me.first_name || me.email || me.phone || 'Пользователь');
            }
        }
        updateUIForLoggedInUser();
    } else {
        updateUIForLoggedOutUser();
    }

    initAuthPage();
    initCreateListing();
    initMainPage();
    setupMobileMenu();
    setActiveMenuItem();

    await initMap();

    await loadListings();
});


window.isUserLoggedIn = isUserLoggedIn;
window.logoutUser = logoutUser;
window.getCurrentUser = getCurrentUser;
window.getFavorites = getFavorites;
window.getMyListings = getMyListings;
window.updateProfile = updateProfile;
window.changePassword = changePassword;
window.addToFavorites = addToFavorites;
window.removeFromFavorites = removeFromFavorites;
window.deleteListing = deleteListing
