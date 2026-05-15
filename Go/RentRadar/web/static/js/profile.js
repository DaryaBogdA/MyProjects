let currentUser = null;

document.addEventListener('DOMContentLoaded', async function() {
    if (typeof isUserLoggedIn === 'function' && !await isUserLoggedIn()) {
        window.location.href = '/login.html';
        return;
    }

    await loadUserData();
    initTabs();
    initProfileForm();
    initPasswordForm();
    await loadFavorites();
    await loadMyListings();
    await updateStats();
});

function initTabs() {
    const tabs = document.querySelectorAll('.profile-tab');
    const contents = document.querySelectorAll('.profile-tab-content');

    tabs.forEach(tab => {
        tab.addEventListener('click', function() {
            if (this.tagName === 'A' || !this.dataset.tab) return;
            const tabId = this.dataset.tab;
            tabs.forEach(t => t.classList.remove('active'));
            contents.forEach(c => c.classList.remove('active'));
            this.classList.add('active');
            const content = document.getElementById(`${tabId}-tab`);
            if (content) content.classList.add('active');
        });
    });
}

async function loadUserData() {
    try {
        currentUser = await getCurrentUser();
        if (!currentUser) return;

        document.getElementById('profileFirstName').value = currentUser.first_name || '';
        document.getElementById('profileLastName').value = currentUser.last_name || '';
        document.getElementById('profileEmail').value = currentUser.email || '';
        document.getElementById('profilePhone').value = currentUser.phone || '';
    } catch (err) {
        console.error('Failed to load user data:', err);
    }
}

function initProfileForm() {
    const form = document.getElementById('profileForm');
    if (!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        try {
            await updateProfile({
                first_name: document.getElementById('profileFirstName').value,
                last_name: document.getElementById('profileLastName').value,
                email: document.getElementById('profileEmail').value.trim(),
                phone: document.getElementById('profilePhone').value,
            });
            showNotification('Профиль успешно обновлен', 'success');
            const msg = document.getElementById('profileSaveMessage');
            if (msg) msg.textContent = 'Изменения сохранены.';
            alert('Данные профиля успешно сохранены.');
            await loadUserData();
        } catch (err) {
            showNotification(err.message || 'Ошибка обновления профиля', 'error');
        }
    });
}

function initPasswordForm() {
    const form = document.getElementById('passwordForm');
    if (!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        const currentPassword = document.getElementById('currentPassword').value;
        const newPassword = document.getElementById('newPassword').value;
        const confirmPassword = document.getElementById('confirmPassword').value;

        if (!currentPassword || !newPassword || !confirmPassword) {
            showNotification('Заполните все поля', 'error');
            return;
        }
        if (newPassword !== confirmPassword) {
            showNotification('Пароли не совпадают', 'error');
            return;
        }

        try {
            await changePassword({
                current_password: currentPassword,
                new_password: newPassword,
            });
            showNotification('Пароль изменен', 'success');
            form.reset();
        } catch (err) {
            showNotification(err.message || 'Ошибка смены пароля', 'error');
        }
    });
}

function renderEmptyFavorites(grid) {
    grid.innerHTML = `
        <div class="empty-favorites-state" style=" text-align: center; padding: 60px 20px; background: white; border-radius: 16px; border: 1px dashed var(--border-color); max-width: 760px; margin: 0 auto; width: 100%;">
            <i class="far fa-heart" style="font-size: 4rem; color: var(--text-muted); margin-bottom: 20px; display: inline-block;"></i>
            <h3 style="font-size: 1.3rem; color: var(--text-dark); margin-bottom: 10px;">Вы ещё не добавили ничего в избранное</h3>
            <p style="color: var(--text-muted); margin-bottom: 25px;">Добавляйте понравившиеся объявления, нажимая на сердечко</p>
            <a href="/index.html" class="btn btn-primary" style="display: inline-block; padding: 12px 30px;">
                <i class="fas fa-search"></i> Найти жилье
            </a>
        </div>
    `;
}

async function loadFavorites() {
    const grid = document.getElementById('favoritesGrid');
    if (!grid) return;

    try {
        const favorites = await getFavorites();

        if (!Array.isArray(favorites) || favorites.length === 0) {
            renderEmptyFavorites(grid);
            return;
        }

        grid.innerHTML = favorites.map((listing) => `
            <div class="favorite-card" data-id="${listing.id}">
                <div class="favorite-image">
                    <img src="${listing.photos ? listing.photos.split(',')[0] : 'https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=400&h=250&fit=crop'}" alt="${escapeHtml(listing.title)}">
                </div>
                <div class="favorite-content">
                    <div class="favorite-title">${escapeHtml(listing.title)}</div>
                    <div class="favorite-price">${listing.price.toLocaleString()} BYN</div>
                    <div class="favorite-location">${escapeHtml(listing.city || listing.district || '')}</div>
                    <div class="favorite-actions">
                        <button class="btn btn-danger btn-sm remove-favorite" data-id="${listing.id}">Удалить</button>
                        <a href="/listing-details.html?id=${listing.id}" class="btn btn-outline btn-sm">Подробнее</a>
                    </div>
                </div>
            </div>
        `).join('');

        document.querySelectorAll('.remove-favorite').forEach((btn) => {
            btn.addEventListener('click', async () => {
                try {
                    await removeFromFavorites(Number(btn.dataset.id));
                    await loadFavorites();
                    await updateStats();
                } catch (err) {
                    showNotification(err.message || 'Ошибка удаления', 'error');
                }
            });
        });
    } catch (err) {
        console.error('Error loading favorites:', err);
        renderEmptyFavorites(grid);
    }
}
function listingStatusBadge(listing) {
    if (!listing.is_active) {
        return '<span class="listing-status-badge removed">Снято</span>';
    }
    const m = String(listing.moderation_status || '').toLowerCase();
    if (m === 'pending') {
        return '<span class="listing-status-badge pending">На модерации</span>';
    }
    if (m === 'rejected') {
        return '<span class="listing-status-badge rejected">Отклонено</span>';
    }
    return '<span class="listing-status-badge approved">Опубликовано</span>';
}

async function loadMyListings() {
    const grid = document.getElementById('myListingsGrid');
    if (!grid) return;

    try {
        const raw = await getMyListings();
        const listings = Array.isArray(raw) ? raw : [];
        if (listings.length === 0) {
            grid.innerHTML = `
                <div class="empty-favorites-state" style=" text-align: center; padding: 60px 20px; background: white; border-radius: 16px; border: 1px dashed var(--border-color); max-width: 760px; margin: 0 auto; width: 100%;">
                    <i class="fas fa-home" style="font-size: 4rem; color: var(--text-muted); margin-bottom: 20px; display: inline-block;"></i>
                    <h3 style="font-size: 1.3rem; color: var(--text-dark); margin-bottom: 10px;">У вас нет объявлений</h3>
                    <p style="color: var(--text-muted); margin-bottom: 25px;">Разместите первое объявление</p>
                    <a href="/create-listing.html" class="btn btn-primary" style="display: inline-block; padding: 12px 30px;">Разместить объявление</a>
                </div>
            `;
            return;
        }

        grid.innerHTML = listings.map((listing) => {
            const isRent = String(listing.listing_type || '').toLowerCase() === 'rent';
            const typeLabel = isRent ? 'Аренда' : 'Продажа';
            const typeClass = isRent ? 'badge-rent' : 'badge-sale';
            return `
            <div class="my-listing-card" data-id="${listing.id}">
                <div class="my-listing-image">
                    <span class="my-listing-type-badge ${typeClass}">${typeLabel}</span>
                    <img src="${listing.photos ? listing.photos.split(',')[0] : 'https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=120&h=120&fit=crop'}" alt="${escapeHtml(listing.title)}">
                </div>
                <div class="my-listing-content">
                    <div class="my-listing-header">
                        <div>
                            <div class="favorite-title">${escapeHtml(listing.title)}</div>
                            <div class="favorite-price">${listing.price.toLocaleString()} BYN</div>
                            <div class="listing-status-row">${listingStatusBadge(listing)}</div>
                        </div>
                    </div>
                    <div class="listing-details">
                        <span><i class="fas fa-vector-square"></i> ${listing.area || 0} м²</span>
                        <span><i class="fas fa-eye"></i> ${listing.views_count || 0} просмотров</span>
                    </div>
                    <div class="my-listing-actions">
                        <a href="/create-listing.html?id=${listing.id}" class="btn btn-outline btn-sm">Редактировать</a>
                        <button type="button" class="btn btn-danger btn-sm delete-listing" data-id="${listing.id}">Удалить</button>
                    </div>
                </div>
            </div>
        `;
        }).join('');

        document.querySelectorAll('.delete-listing').forEach((btn) => {
            btn.addEventListener('click', async () => {
                if (!confirm('Удалить объявление?')) return;
                try {
                    await deleteListing(Number(btn.dataset.id));
                    await loadMyListings();
                    await loadFavorites();
                    await updateStats();
                } catch (err) {
                    showNotification(err.message || 'Ошибка удаления', 'error');
                }
            });
        });
    } catch (err) {
        grid.innerHTML = '<p class="error">Ошибка загрузки объявлений</p>';
    }
}

async function deleteListing(listingId) {
    return await apiRequest(`/listings/${listingId}`, { method: 'DELETE' });
}

async function updateStats() {
    try {
        const [favorites, listings] = await Promise.all([getFavorites(), getMyListings()]);
        const totalViews = listings.reduce((sum, l) => sum + (l.views_count || 0), 0);
        document.getElementById('statsFavorites').textContent = String(favorites.length);
        document.getElementById('statsListings').textContent = String(listings.length);
        document.getElementById('statsViews').textContent = String(totalViews);
    } catch {
        
    }
}

function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `settings-notification ${type}`;
    notification.innerHTML = `<span>${message}</span>`;
    document.body.appendChild(notification);
    setTimeout(() => notification.classList.add('show'), 10);
    setTimeout(() => {
        notification.classList.remove('show');
        setTimeout(() => notification.remove(), 300);
    }, 2500);
}