

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


async function updateUIForLoggedInUser() {
    const userActions = document.querySelector('.user-actions');
    if (!userActions) return;

    const userName = localStorage.getItem('userName') || 'Пользователь';

    let adminNav = '';
    try {
        const me = await getCurrentUser();
        if (me && me.role === 'admin') {
            adminNav =
                '<a href="/admin.html">Модерация</a>' +
                '<a href="/stats.html">Статистика</a>';
        }
    } catch (_) {
    }

    userActions.innerHTML = `
        <div class="user-menu">
            <button class="btn btn-outline user-menu-btn">
                <i class="fas fa-user"></i> ${userName} <i class="fas fa-chevron-down"></i>
            </button>
            <div class="user-dropdown">
                <a href="/profile.html">Мой профиль</a>
                <a href="/messages.html">Сообщения</a>
                <a href="/bookings.html">Мои бронирования</a>
                <a href="/reviews.html">Отзывы</a>
                ${adminNav}
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

    function clearLoginErrors() {
        document.getElementById('loginIdentifierError').textContent = '';
        document.getElementById('loginPasswordError').textContent = '';
        document.getElementById('loginGeneralError').textContent = '';
        document.getElementById('loginIdentifier').classList.remove('input-error');
        document.getElementById('loginPassword').classList.remove('input-error');
    }

    function clearRegisterErrors() {
        document.getElementById('registerFirstNameError').textContent = '';
        document.getElementById('registerLastNameError').textContent = '';
        document.getElementById('registerIdentifierError').textContent = '';
        document.getElementById('registerPasswordError').textContent = '';
        document.getElementById('registerConfirmPasswordError').textContent = '';
        document.getElementById('registerTermsError').textContent = '';
        document.getElementById('registerGeneralError').textContent = '';

        document.getElementById('registerFirstName').classList.remove('input-error');
        document.getElementById('registerLastName').classList.remove('input-error');
        document.getElementById('registerIdentifier').classList.remove('input-error');
        document.getElementById('registerPassword').classList.remove('input-error');
        document.getElementById('registerConfirmPassword').classList.remove('input-error');
        document.getElementById('registerTerms').classList.remove('input-error');
    }

    function showLoginError(fieldId, message) {
        if (fieldId === 'general') {
            const errorDiv = document.getElementById('loginGeneralError');
            if (errorDiv) errorDiv.textContent = message;
        } else {
            const errorDiv = document.getElementById(`${fieldId}Error`);
            if (errorDiv) errorDiv.textContent = message;
            const input = document.getElementById(fieldId);
            if (input) input.classList.add('input-error');
        }
    }

    function showRegisterError(fieldId, message) {
        if (fieldId === 'general') {
            const errorDiv = document.getElementById('registerGeneralError');
            if (errorDiv) errorDiv.textContent = message;
        } else {
            const errorDiv = document.getElementById(`${fieldId}Error`);
            if (errorDiv) errorDiv.textContent = message;
            const input = document.getElementById(fieldId);
            if (input) input.classList.add('input-error');
        }
    }

    function validateEmail(email) {
        const re = /^[^\s@]+@([^\s@]+\.)+[^\s@]+$/;
        return re.test(email);
    }

    function validatePhone(phone) {
        const re = /^[\+\(]?[0-9]{1,4}[\)\-\s]?[\d\-\s]{7,15}$/;
        return re.test(phone);
    }

    function showLogin() {
        loginTab.classList.add('active');
        registerTab.classList.remove('active');
        loginForm.classList.add('active');
        registerForm.classList.remove('active');
        clearLoginErrors();
        clearRegisterErrors();
    }

    function showRegister() {
        registerTab.classList.add('active');
        loginTab.classList.remove('active');
        registerForm.classList.add('active');
        loginForm.classList.remove('active');
        clearLoginErrors();
        clearRegisterErrors();
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
            clearLoginErrors();

            const identifier = document.getElementById('loginIdentifier').value.trim();
            const password = document.getElementById('loginPassword').value;

            let hasError = false;

            if (!identifier) {
                showLoginError('loginIdentifier', 'Введите email или номер телефона');
                hasError = true;
            }

            if (!password) {
                showLoginError('loginPassword', 'Введите пароль');
                hasError = true;
            }

            if (hasError) return;

            try {
                await login(identifier, password);
                window.location.href = '/profile.html';
            } catch (err) {
                let errorMessage = err.message || 'Ошибка входа';
                if (errorMessage.includes('invalid credentials') || errorMessage.includes('не найден')) {
                    showLoginError('general', 'Неверный email/телефон или пароль');
                } else {
                    showLoginError('general', errorMessage);
                }
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

                document.getElementById('registerPasswordError').textContent = '';
                passwordInput.classList.remove('input-error');
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
                    document.getElementById('registerConfirmPasswordError').textContent = '';
                    confirmInput.classList.remove('input-error');
                } else {
                    passwordMatch.textContent = 'Пароли не совпадают';
                    passwordMatch.className = 'password-match-minimal';
                }
            }

            confirmInput.addEventListener('input', checkPasswordMatch);
            passwordInput.addEventListener('input', checkPasswordMatch);
        }

        const clearInputErrors = () => {
            const fields = ['registerFirstName', 'registerLastName', 'registerIdentifier', 'registerPassword', 'registerConfirmPassword'];
            fields.forEach(field => {
                const input = document.getElementById(field);
                if (input) {
                    input.addEventListener('input', () => {
                        document.getElementById(`${field}Error`).textContent = '';
                        input.classList.remove('input-error');
                    });
                }
            });
            const termsCheckbox = document.getElementById('registerTerms');
            if (termsCheckbox) {
                termsCheckbox.addEventListener('change', () => {
                    document.getElementById('registerTermsError').textContent = '';
                    termsCheckbox.classList.remove('input-error');
                });
            }
        };
        clearInputErrors();

        registerFormElement.addEventListener('submit', async (e) => {
            e.preventDefault();
            clearRegisterErrors();

            const firstName = document.getElementById('registerFirstName').value.trim();
            const lastName = document.getElementById('registerLastName').value.trim();
            const identifier = document.getElementById('registerIdentifier').value.trim();
            const password = document.getElementById('registerPassword').value;
            const confirmPassword = document.getElementById('registerConfirmPassword').value;
            const termsChecked = document.getElementById('registerTerms').checked;

            let hasError = false;

            if (!firstName) {
                showRegisterError('registerFirstName', 'Введите имя');
                hasError = true;
            } else if (firstName.length < 2) {
                showRegisterError('registerFirstName', 'Имя должно содержать минимум 2 символа');
                hasError = true;
            }

            if (!lastName) {
                showRegisterError('registerLastName', 'Введите фамилию');
                hasError = true;
            } else if (lastName.length < 2) {
                showRegisterError('registerLastName', 'Фамилия должна содержать минимум 2 символа');
                hasError = true;
            }

            if (!identifier) {
                showRegisterError('registerIdentifier', 'Введите email или номер телефона');
                hasError = true;
            } else {
                const isEmail = identifier.includes('@');
                if (isEmail && !validateEmail(identifier)) {
                    showRegisterError('registerIdentifier', 'Введите корректный email');
                    hasError = true;
                } else if (!isEmail && !validatePhone(identifier)) {
                    showRegisterError('registerIdentifier', 'Введите корректный номер телефона');
                    hasError = true;
                }
            }

            if (!password) {
                showRegisterError('registerPassword', 'Введите пароль');
                hasError = true;
            } else if (password.length < 6) {
                showRegisterError('registerPassword', 'Пароль должен содержать минимум 6 символов');
                hasError = true;
            } else if (checkPasswordStrength(password) < 3) {
                showRegisterError('registerPassword', 'Пароль слишком слабый. Используйте заглавные буквы, цифры и спецсимволы');
                hasError = true;
            }

            if (!confirmPassword) {
                showRegisterError('registerConfirmPassword', 'Подтвердите пароль');
                hasError = true;
            } else if (password !== confirmPassword) {
                showRegisterError('registerConfirmPassword', 'Пароли не совпадают');
                hasError = true;
            }

            if (!termsChecked) {
                showRegisterError('registerTerms', 'Необходимо согласиться с условиями использования');
                hasError = true;
            }

            if (hasError) return;

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
                let errorMessage = err.message || 'Ошибка регистрации';
                if (errorMessage.includes('already exists')) {
                    showRegisterError('general', 'Пользователь с таким email или телефоном уже зарегистрирован');
                } else {
                    showRegisterError('general', errorMessage);
                }
            }
        });
    }
}
let currentFilters = {
    type: '',
    propertyType: 'all',
    rooms: 'all',
    floor: 'all',
    minPrice: '',
    maxPrice: '',
    minArea: '',
    maxArea: '',
    city: '',
    search: '',
    utilitiesIncluded: false,
    furniture: false,
    children: false,
    pets: false,
    parking: false,
    elevator: false,
    rentPricePeriod: 'all'
};

async function loadListings() {
    const listingsGrid = document.querySelector('.listings-section .listings-grid');
    if (!listingsGrid) return;

    const isSalePage = window.location.pathname.includes('sale.html');
    const listingType = isSalePage ? 'sale' : 'rent';

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
    if (currentFilters.search && currentFilters.search !== '') {
        appendParam('city', currentFilters.search);
    }

    if (currentFilters.minPrice && currentFilters.minPrice !== '') {
        if (isSalePage) appendParam('minPrice', currentFilters.minPrice);
    }
    if (currentFilters.maxPrice && currentFilters.maxPrice !== '') {
        if (isSalePage) appendParam('maxPrice', currentFilters.maxPrice);
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
            filteredListings = filteredListings.filter(
                (l) =>
                    l.property_type === currentFilters.propertyType ||
                    (currentFilters.propertyType === 'apartment' && l.title?.toLowerCase().includes('квартир')) ||
                    (currentFilters.propertyType === 'house' && l.title?.toLowerCase().includes('дом')) ||
                    (currentFilters.propertyType === 'room' && l.title?.toLowerCase().includes('комнат'))
            );
        }

        if (currentFilters.floor && currentFilters.floor !== 'all') {
            if (currentFilters.floor === '10+') {
                filteredListings = filteredListings.filter(l => Number(l.floor || 0) >= 10);
            } else if (currentFilters.floor === 'not_first') {
                filteredListings = filteredListings.filter(l => Number(l.floor || 0) > 1);
            } else if (currentFilters.floor === 'not_last') {
                filteredListings = filteredListings.filter(l => Number(l.floor || 0) > 0 && Number(l.total_floors || 0) > 0 && Number(l.floor || 0) < Number(l.total_floors || 0));
            } else {
                filteredListings = filteredListings.filter(l => String(l.floor || '') === String(currentFilters.floor));
            }
        }

        if (currentFilters.utilitiesIncluded) {
            filteredListings = filteredListings.filter(l => l.utilities_included === true);
        }

        if (!isSalePage) {
            const rp = currentFilters.rentPricePeriod || 'all';
            const minP = currentFilters.minPrice !== '' ? Number(currentFilters.minPrice) : null;
            const maxP = currentFilters.maxPrice !== '' ? Number(currentFilters.maxPrice) : null;
            filteredListings = filteredListings.filter((l) => {
                if (l.listing_type !== 'rent') return true;
                const desc = l.description || '';
                if (rp !== 'all' && listingHasExplicitRentPricePeriod(desc)) {
                    const meta = extractListingMetaFromDescription(desc);
                    if (meta.pricePeriod !== rp) return false;
                }
                const price = Number(l.price || 0);
                if (minP != null && !Number.isNaN(minP) && price < minP) return false;
                if (maxP != null && !Number.isNaN(maxP) && price > maxP) return false;
                return true;
            });
        }

        if (listingsGrid) {
            if (filteredListings.length === 0) {
                listingsGrid.innerHTML = '<p class="no-results">Нет объявлений, соответствующих фильтрам</p>';
                lastVisibleListings = [];
                if (map) updateMapMarkers([]);
                return;
            }

            listingsGrid.innerHTML = filteredListings.map(listing => {
                const badge =
                    listing.listing_type === 'sale'
                        ? '<span class="badge badge-sale">Продажа</span>'
                        : listing.listing_type === 'rent'
                          ? '<span class="badge badge-rent">Аренда</span>'
                          : '';

                return `
                <article class="listing-card">
                    <div class="listing-image">
                        <img src="${listing.photos ? listing.photos.split(',')[0] : 'https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=400&h=250&fit=crop'}" alt="${escapeHtml(listing.title)}">
                        ${badge}
                    </div>
                    <div class="listing-content">
                        <div class="listing-header">
                            <h4 class="listing-title">${escapeHtml(listing.title)}</h4>
                            <div class="listing-price">${listing.price.toLocaleString()} BYN <span>${listingPriceSuffix(listing)}</span></div>
                        </div>
                        <div class="listing-location">
                            <i class="fas fa-map-marker-alt"></i> ${escapeHtml(listing.district || listing.city)}
                        </div>
                        <div class="listing-details">
                            <span><i class="fas fa-vector-square"></i> ${Number(listing.area ?? 0) || 0} м²</span>
                            <span><i class="fas fa-layer-group"></i> ${listingFloorsLabel(listing)}</span>
                            ${listing.listing_type === 'rent' ? '<span><i class="fas fa-bed"></i> ' + (Number(listing.rooms ?? 0) || 0) + ' комн.</span>' : ''}
                            <span class="rating"><i class="fas fa-star"></i> ${listingRatingLabel(listing)}</span>
                        </div>
                        <div class="listing-footer">
                            <span class="available-date"><i class="far fa-calendar-alt"></i> ${listing.available_from ? `Свободно с ${new Date(listing.available_from).toLocaleDateString()}` : 'В продаже'}</span>
                            <div style="display:flex; gap:8px; align-items:center;">
                                <a href="/listing-details.html?id=${listing.id}" class="btn btn-outline btn-sm">Подробнее</a>
                                <button class="btn-like" data-id="${listing.id}">
                                    <i class="far fa-heart"></i>
                                </button>
                            </div>
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
        lastVisibleListings = filteredListings;
        if (map) updateMapMarkers(filteredListings);

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
let lastVisibleListings = [];

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
    setTimeout(() => {
        try {
            map.invalidateSize();
        } catch (_) {}
    }, 200);
}


function mapLatLngForListing(listing) {
    const lat = listing.latitude != null ? Number(listing.latitude) : NaN;
    const lng = listing.longitude != null ? Number(listing.longitude) : NaN;
    if (Number.isFinite(lat) && Number.isFinite(lng) && Math.abs(lat) > 0.01 && Math.abs(lng) > 0.01) {
        return { lat, lng, approx: false };
    }
    const baseLat = 53.9045;
    const baseLng = 27.5615;
    const id = Number(listing.id) || 0;
    const h = ((id * 9301 + 49297) % 100000) / 100000 - 0.5;
    const h2 = ((id * 11003 + 30011) % 100000) / 100000 - 0.5;
    return { lat: baseLat + h * 0.12, lng: baseLng + h2 * 0.12, approx: true };
}

async function loadMapMarkers(listingsOverride = null) {
    if (!map) return;

    mapMarkers.forEach((marker) => map.removeLayer(marker));
    mapMarkers = [];

    try {
        let arr = Array.isArray(listingsOverride) ? listingsOverride : null;
        if (!arr) {
            const isSalePage = window.location.pathname.includes('sale.html');
            const listingType = isSalePage ? 'sale' : 'rent';
            const listings = await getListings(listingType);
            arr = Array.isArray(listings) ? listings : [];
        }
        if (arr.length === 0) {
            return;
        }

        const first = mapLatLngForListing(arr[0]);
        map.setView([first.lat, first.lng], 12);

        arr.forEach((listing) => {
            const { lat, lng, approx } = mapLatLngForListing(listing);
            const markerHtml = `
                <div style="background-color: #2c7873; color: white; border-radius: 50%; width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; border: 2px solid white; box-shadow: 0 2px 5px rgba(0,0,0,0.3); font-size: 11px; font-weight: bold;">
                    ${Math.round(Number(listing.price) || 0).toLocaleString().split(' ')[0]}
                </div>
            `;

            const markerIcon = L.divIcon({
                html: markerHtml,
                iconSize: [36, 36],
                iconAnchor: [18, 36],
                popupAnchor: [0, -34],
                className: 'custom-marker',
            });

            const marker = L.marker([lat, lng], { icon: markerIcon }).addTo(map);

            const popupContent = `
                <div class="map-popup-listing" style="min-width: 220px;">
                    <img src="${listing.photos ? listing.photos.split(',')[0] : 'https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=200&h=100&fit=crop'}" style="width: 100%; height: 100px; object-fit: cover; border-radius: 8px; margin-bottom: 8px;">
                    <h4 style="font-size: 14px; margin-bottom: 4px;">${escapeHtml(listing.title)}</h4>
                    <div style="color: var(--primary-teal); font-weight: bold;">${Number(listing.price || 0).toLocaleString()} BYN</div>
                    <div style="font-size: 12px; color: #666;">${escapeHtml(listing.address || listing.city || '')}</div>
                    ${approx ? '<div style="font-size:11px;color:#b45309;margin-top:4px;">Приблизительное положение</div>' : ''}
                    <a href="/listing-details.html?id=${listing.id}" style="color: #2c7873; text-decoration: none; font-size: 12px; margin-top: 5px; display: inline-block;">Подробнее →</a>
                </div>
            `;

            marker.bindPopup(popupContent);
            mapMarkers.push(marker);
        });
    } catch (err) {
        console.error('Failed to load map markers:', err);
    }
}


async function updateMapMarkers(listingsOverride = null) {
    await loadMapMarkers(listingsOverride);
}


function applyFilters() {
    const isSalePage = window.location.pathname.includes('sale.html');
    const propertyGroup = Array.from(document.querySelectorAll('.filter-group')).find(
        (group) => group.querySelector('label')?.textContent.trim() === 'Тип жилья'
    );
    const activeChip = propertyGroup ? propertyGroup.querySelector('.filter-chip.active') : null;
    if (activeChip && activeChip.textContent.trim() !== 'Квартира') {
        const propertyText = activeChip.textContent.trim();
        if (propertyText === 'Дом') currentFilters.propertyType = 'house';
        else if (propertyText === 'Комната') currentFilters.propertyType = 'room';
        else currentFilters.propertyType = 'all';
    } else {
        currentFilters.propertyType = 'all';
    }

    
    const activeRoom = document.querySelector('.room-buttons .room-btn.active');
    if (activeRoom && !activeRoom.classList.contains('active-all')) {
        const roomText = activeRoom.textContent.trim();
        currentFilters.rooms = roomText === '5+' ? '5' : roomText;
    } else {
        currentFilters.rooms = 'all';
    }

    const searchInput = document.querySelector('.search-input');
    currentFilters.search = searchInput ? searchInput.value.trim() : '';

    
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

    currentFilters.floor = 'all';
    if (isSalePage) {
        const floorGroup = Array.from(document.querySelectorAll('.filter-group')).find(
            (group) => group.querySelector('label')?.textContent.trim() === 'Этаж'
        );
        const activeFloor = floorGroup ? floorGroup.querySelector('.filter-chip.active') : null;
        if (activeFloor) {
            currentFilters.floor = activeFloor.textContent.trim();
            if (currentFilters.floor === 'Не первый') currentFilters.floor = 'not_first';
            if (currentFilters.floor === 'Не последний') currentFilters.floor = 'not_last';
        }
    }

    currentFilters.utilitiesIncluded = isSalePage
        ? (document.getElementById('filter-renovation')?.checked || false)
        : (document.getElementById('filter-wifi')?.checked || false);
    currentFilters.furniture = document.getElementById('filter-furniture')?.checked || false;
    currentFilters.children = document.getElementById('filter-children')?.checked || false;
    currentFilters.pets = document.getElementById('filter-pets')?.checked || false;
    currentFilters.parking = document.getElementById('filter-parking')?.checked || false;
    currentFilters.elevator = document.getElementById('filter-elevator')?.checked || false;

    const rentPeriodGroup = document.getElementById('rent-price-period-group');
    if (rentPeriodGroup) {
        const activeRP = rentPeriodGroup.querySelector('.filter-chip.active');
        const dp = activeRP?.getAttribute('data-rent-price-period');
        if (dp === 'day') currentFilters.rentPricePeriod = 'day';
        else if (dp === 'month') currentFilters.rentPricePeriod = 'month';
        else currentFilters.rentPricePeriod = 'all';
    } else {
        currentFilters.rentPricePeriod = 'all';
    }

    loadListings();
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
    const searchInput = document.querySelector('.search-input');
    if (searchInput) searchInput.value = '';

    
    document.querySelectorAll('.filter-checkboxes input[type="checkbox"]').forEach(cb => {
        cb.checked = false;
    });

    
    currentFilters = {
        type: window.location.pathname.includes('sale.html') ? 'sale' : '',
        propertyType: 'all',
        rooms: 'all',
        floor: 'all',
        minPrice: '',
        maxPrice: '',
        minArea: '',
        maxArea: '',
        city: '',
        search: '',
        utilitiesIncluded: false,
        furniture: false,
        children: false,
        pets: false,
        parking: false,
        elevator: false,
        rentPricePeriod: 'all'
    };

    activateDefaultFilterChips();

    loadListings();
}


function activateDefaultFilterChips() {
    const isSalePage = window.location.pathname.includes('sale.html');
    document.querySelectorAll('.filter-chip').forEach((chip) => chip.classList.remove('active'));
    if (isSalePage) {
        const first = document.querySelector('.filter-chip');
        if (first) first.classList.add('active');
    } else {
        document.querySelectorAll('.filter-group').forEach((g) => {
            const lab = g.querySelector('label')?.textContent?.trim();
            if (lab === 'Тип жилья' || lab === 'Период цены') {
                const fc = g.querySelector('.filter-chip');
                if (fc) fc.classList.add('active');
            }
        });
    }
}


function initMainPage() {
    if (!document.querySelector('.filter-chip')) return;

    
    const isSalePage = window.location.pathname.includes('sale.html');
    if (isSalePage) {
        currentFilters.type = 'sale';
    } else {
        currentFilters.type = '';
    }

    
    activateDefaultFilterChips();

    
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
        currentFilters.search = query;
        loadListings();
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
    const listingsGrid = document.querySelector('.listings-section .listings-grid');
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
                    <span class="badge badge-sale">Продажа</span>
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
                        <span><i class="fas fa-vector-square"></i> ${Number(listing.area ?? 0) || 0} м²</span>
                        <span><i class="fas fa-layer-group"></i> ${listingFloorsLabel(listing)}</span>
                        <span><i class="fas fa-building"></i> Панельный</span>
                    </div>
                    <div class="listing-footer">
                        <span class="available-date"><i class="far fa-calendar-alt"></i> В продаже</span>
                        <div style="display:flex; gap:8px; align-items:center;">
                            <a href="/listing-details.html?id=${listing.id}" class="btn btn-outline btn-sm">Подробнее</a>
                            <button class="btn-like"><i class="far fa-heart"></i></button>
                        </div>
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
    if (!await isUserLoggedIn()) return [];
    try {
        const response = await apiRequest('/favorites');
        return Array.isArray(response) ? response : [];
    } catch (err) {
        console.error('Error fetching favorites:', err);
        return [];
    }
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
    const data = await apiRequest('/my-listings');
    return Array.isArray(data) ? data : [];
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

function standardFooterHtml() {
    return `
    <div class="container">
        <div class="footer-top">
            <div class="footer-column">
                <div class="footer-logo">
                    <i class="fas fa-search-location"></i>
                    <span>RentRadar</span>
                </div>
                <p class="footer-description">
                    Крупнейшая площадка недвижимости в Беларуси. Тысячи объявлений от собственников и агентств.
                </p>
            </div>
            <div class="footer-column">
                <h4>Разделы</h4>
                <ul>
                    <li><a href="/index.html">Аренда</a></li>
                    <li><a href="/sale.html">Продажа</a></li>
                    <li><a href="/news.html">Новости</a></li>
                    <li><a href="/about.html">О нас</a></li>
                </ul>
            </div>
            <div class="footer-column">
                <h4>Помощь</h4>
                <ul>
                    <li><a href="/help-sell.html">Как продать</a></li>
                    <li><a href="/help-buy.html">Как купить</a></li>
                    <li><a href="/safety.html">Безопасность</a></li>
                    <li><a href="/support.html">Поддержка</a></li>
                </ul>
            </div>
            <div class="footer-column">
                <h4>Контакты</h4>
                <ul>
                    <li class="footer-contact-line"><i class="fas fa-phone"></i> +375 29 123-45-67</li>
                    <li><i class="fas fa-envelope"></i> info@rentradar.by</li>
                    <li><i class="fas fa-clock"></i> Пн-Пт: 9:00 - 21:00</li>
                </ul>
            </div>
        </div>
        <div class="footer-bottom">
            <div class="footer-bottom-left">
                <p>&copy; 2026 RentRadar. Все права защищены.</p>
            </div>
            <div class="footer-bottom-right">
                <a href="/privacy.html">Политика конфиденциальности</a>
                <a href="/terms.html">Пользовательское соглашение</a>
            </div>
        </div>
    </div>`;
}

function standardHeaderHtml() {
    return `
    <a href="/index.html" class="logo-link">
        <div class="logo">
            <span class="logo-icon"><i class="fas fa-search-location"></i></span>
            <span class="logo-text">RentRadar</span>
        </div>
    </a>
    <nav class="main-nav">
        <ul>
            <li><a href="/index.html">Аренда</a></li>
            <li><a href="/sale.html">Продажа</a></li>
            <li><a href="/news.html">Новости</a></li>
            <li><a href="/about.html">О нас</a></li>
        </ul>
    </nav>
    <div class="user-actions"></div>
    <button class="mobile-menu-btn">
        <i class="fas fa-bars"></i>
    </button>`;
}

function injectStandardHeader() {
    const headerContainer = document.querySelector('header.header .header-container');
    if (!headerContainer) return;
    headerContainer.innerHTML = standardHeaderHtml();
}

function injectStandardFooter() {
    const path = window.location.pathname || '';
    if (path.endsWith('/login.html')) return;
    const footer = document.querySelector('footer.footer');
    if (!footer) return;
    footer.innerHTML = standardFooterHtml();
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

function extractListingMetaFromDescription(rawDescription) {
    const raw = String(rawDescription || '');
    const m = raw.match(/^\[\[RR_PRICE_PERIOD:(day|month)\]\]\s*/i);
    const pricePeriod = m ? (String(m[1]).toLowerCase() === 'day' ? 'day' : 'month') : 'month';
    const description = raw.replace(/^\[\[RR_PRICE_PERIOD:(day|month)\]\]\s*/i, '');
    return { description, pricePeriod };
}

function listingPriceSuffix(listing) {
    if (listing.listing_type !== 'rent') return '';
    const meta = extractListingMetaFromDescription(listing.description || '');
    return meta.pricePeriod === 'day' ? '/ сутки' : '/ месяц';
}

function listingRatingLabel(listing) {
    const avg = Number(listing.average_rating || 0);
    const cnt = Number(listing.reviews_count || 0);
    if (cnt <= 0) return 'нет отзывов';
    return `${avg.toFixed(1)} (${cnt})`;
}

function listingHasExplicitRentPricePeriod(rawDescription) {
    return /^\[\[RR_PRICE_PERIOD:(day|month)\]\]/i.test(String(rawDescription || '').trim());
}

function listingFloorsLabel(listing) {
    const f = Number(listing?.floor ?? 0);
    const t = Number(listing?.total_floors ?? 0);
    const fv = Number.isFinite(f) ? f : 0;
    const tv = Number.isFinite(t) ? t : 0;
    if (fv === 0 && tv === 0) return '—';
    return `${fv}/${tv} эт.`;
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
    injectStandardHeader();
    injectStandardFooter();
    if (typeof initComponents === 'function') {
        initComponents();
    }
    if (await isUserLoggedIn()) {
        if (!localStorage.getItem('userName')) {
            const me = await getCurrentUser();
            if (me) {
                localStorage.setItem('userName', me.first_name || me.email || me.phone || 'Пользователь');
            }
        }
        await updateUIForLoggedInUser();
    } else {
        updateUIForLoggedOutUser();
    }

    initAuthPage();
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
window.deleteListing = deleteListing;
window.listingFloorsLabel = listingFloorsLabel;
