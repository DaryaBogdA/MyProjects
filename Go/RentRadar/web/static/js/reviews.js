document.addEventListener('DOMContentLoaded', () => {
    initReviewsPage();
});

function starsRow(n) {
    let s = '';
    for (let i = 1; i <= 5; i++) {
        s += `<i class="fas fa-star" style="color:${i <= n ? '#f5a623' : '#ddd'}"></i>`;
    }
    return s;
}

function listingThumb(photos) {
    const first = (photos || '').split(',')[0].trim();
    return first || 'https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=400&h=300&fit=crop';
}

function formatWhen(iso) {
    if (!iso) return '';
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return String(iso);
    return d.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' });
}

function priceLabel(price, listingType) {
    const p = Number(price || 0).toLocaleString();
    return listingType === 'sale' ? `${p} BYN` : `${p} BYN/мес`;
}

function reviewModBadge(st) {
    const s = String(st || 'approved').toLowerCase();
    if (s === 'pending') return '<span style="font-size:0.75rem;padding:2px 8px;border-radius:999px;background:#fff4e6;color:#b45309;font-weight:600;">На модерации</span>';
    if (s === 'rejected') return '<span style="font-size:0.75rem;padding:2px 8px;border-radius:999px;background:#fee2e2;color:#b91c1c;font-weight:600;">Скрыт</span>';
    return '<span style="font-size:0.75rem;padding:2px 8px;border-radius:999px;background:#e8f3f2;color:#2c7873;font-weight:600;">Опубликован</span>';
}

function cardWritten(r) {
    return `
        <div class="review-card">
            <div class="review-header">
                <div class="review-rating">
                    <div class="stars">${starsRow(r.rating)}</div>
                    <span class="rating-value">${r.rating}.0</span>
                </div>
                <div style="display:flex;flex-direction:column;align-items:flex-end;gap:6px;">
                    ${reviewModBadge(r.moderation_status)}
                <div class="review-date" style="color:var(--text-muted); font-size:0.9rem;">
                    <i class="far fa-calendar-alt"></i> ${escapeHtml(formatWhen(r.created_at))}
                </div>
                </div>
            </div>
            <div class="review-listing-info">
                <div class="review-listing-image">
                    <img src="${escapeHtml(listingThumb(r.listing_photos))}" alt="">
                </div>
                <div class="review-listing-details">
                    <div class="review-listing-title">
                        <a href="/listing-details.html?id=${r.listing_id}">${escapeHtml(r.listing_title || 'Объявление')}</a>
                    </div>
                    <div class="review-listing-meta">
                        <span><i class="fas fa-tag"></i> ${escapeHtml(priceLabel(r.listing_price, r.listing_type))}</span>
                        <span><i class="fas fa-home"></i> ${r.listing_type === 'sale' ? 'Продажа' : 'Аренда'}</span>
                    </div>
                </div>
            </div>
            ${r.comment ? `<div class="review-content"><div class="review-text">${escapeHtml(r.comment)}</div></div>` : ''}
        </div>`;
}

function cardAbout(r) {
    return `
        <div class="review-card">
            <div class="review-header">
                <div class="review-user">
                    <div class="review-user-info">
                        <h4>${escapeHtml(r.reviewer_name || 'Пользователь')}</h4>
                        <div class="review-date">
                            <i class="far fa-calendar-alt"></i> ${escapeHtml(formatWhen(r.created_at))}
                        </div>
                    </div>
                </div>
                <div style="display:flex;flex-direction:column;align-items:flex-end;gap:6px;">
                    ${reviewModBadge(r.moderation_status)}
                    <div class="review-rating">
                        <div class="stars">${starsRow(r.rating)}</div>
                        <span class="rating-value">${r.rating}.0</span>
                    </div>
                </div>
            </div>
            <div class="review-listing-info">
                <div class="review-listing-image">
                    <img src="${escapeHtml(listingThumb(r.listing_photos))}" alt="">
                </div>
                <div class="review-listing-details">
                    <div class="review-listing-title">
                        <a href="/listing-details.html?id=${r.listing_id}">${escapeHtml(r.listing_title || 'Объявление')}</a>
                    </div>
                    <div class="review-listing-meta">
                        <span><i class="fas fa-tag"></i> ${escapeHtml(priceLabel(r.listing_price, r.listing_type))}</span>
                    </div>
                </div>
            </div>
            ${r.comment ? `<div class="review-content"><div class="review-text">${escapeHtml(r.comment)}</div></div>` : ''}
        </div>`;
}

async function initReviewsPage() {
    const uid = localStorage.getItem('userId');
    if (!uid) {
        window.location.href = '/login.html';
        return;
    }

    const container = document.getElementById('reviewsContainer');
    const tabs = document.querySelectorAll('.reviews-tab');
    let written = [];
    let about = [];
    let activeTab = 'written';

    try {
        const [wRes, aRes] = await Promise.all([
            fetch('/api/reviews/by-me', { headers: { 'X-User-ID': uid } }),
            fetch('/api/reviews/about-my-listings', { headers: { 'X-User-ID': uid } }),
        ]);
        const wBody = await wRes.json().catch(() => []);
        const aBody = await aRes.json().catch(() => []);
        if (!wRes.ok) throw new Error((wBody && wBody.error) || 'Не удалось загрузить отзывы');
        written = Array.isArray(wBody) ? wBody : [];
        about = aRes.ok && Array.isArray(aBody) ? aBody : [];
    } catch (e) {
        container.innerHTML = `<p class="error">${escapeHtml(e.message || 'Ошибка')}</p>`;
        return;
    }

    const statWritten = document.getElementById('statWritten');
    const statAbout = document.getElementById('statAbout');
    const statAvg = document.getElementById('statAvg');
    if (statWritten) statWritten.textContent = String(written.length);
    if (statAbout) statAbout.textContent = String(about.length);
    if (statAvg) {
        if (about.length === 0) statAvg.textContent = '—';
        else {
            const sum = about.reduce((s, r) => s + Number(r.rating || 0), 0);
            statAvg.textContent = (Math.round((sum / about.length) * 10) / 10).toFixed(1);
        }
    }

    function render() {
        const list = activeTab === 'written' ? written : about;
        if (!list.length) {
            container.innerHTML = `
                <div class="empty-state">
                    <i class="fas fa-star"></i>
                    <h3>Пока пусто</h3>
                    <p>${activeTab === 'written' ? 'Вы ещё не оставляли отзывов.' : 'На ваши объявления пока никто не оставил отзывов.'}</p>
                </div>`;
            return;
        }
        container.innerHTML = list.map((r) => (activeTab === 'written' ? cardWritten(r) : cardAbout(r))).join('');
    }

    tabs.forEach((tab) => {
        tab.addEventListener('click', () => {
            tabs.forEach((t) => t.classList.remove('active'));
            tab.classList.add('active');
            activeTab = tab.dataset.tab === 'about' ? 'about' : 'written';
            render();
        });
    });

    render();
}
