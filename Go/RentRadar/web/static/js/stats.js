document.addEventListener('DOMContentLoaded', async () => {
    const root = document.getElementById('statsRoot');
    const toolbar = document.getElementById('statsSortToolbar');
    if (!root) return;

    const uid = localStorage.getItem('userId');
    if (!uid) {
        window.location.href = '/login.html';
        return;
    }
    try {
        const meRes = await fetch('/api/me', { headers: { 'X-User-ID': uid } });
        const me = await meRes.json().catch(() => ({}));
        if (!meRes.ok || me.role !== 'admin') {
            window.location.href = '/index.html';
            return;
        }
    } catch {
        window.location.href = '/index.html';
        return;
    }

    let currentSort = 'rating';

    const esc =
        typeof escapeHtml === 'function'
            ? escapeHtml
            : function (s) {
                  if (!s) return '';
                  return String(s)
                      .replace(/&/g, '&amp;')
                      .replace(/</g, '&lt;')
                      .replace(/>/g, '&gt;')
                      .replace(/"/g, '&quot;')
                      .replace(/'/g, '&#39;');
              };

    const defaultImg =
        'https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=400&h=250&fit=crop';

    function renderToolbar() {
        if (!toolbar) return;
        toolbar.innerHTML = `
            <div class="stats-sort-bar" role="tablist" aria-label="Сортировка статистики">
                <span class="stats-sort-label">Показать:</span>
                <button type="button" class="stats-sort-btn ${currentSort === 'rating' ? 'active' : ''}" data-sort="rating" role="tab" aria-selected="${currentSort === 'rating'}">По рейтингу</button>
                <button type="button" class="stats-sort-btn ${currentSort === 'views' ? 'active' : ''}" data-sort="views" role="tab" aria-selected="${currentSort === 'views'}">По просмотрам</button>
            </div>`;
        toolbar.querySelectorAll('.stats-sort-btn').forEach((btn) => {
            btn.addEventListener('click', () => {
                const next = btn.getAttribute('data-sort');
                if (!next || next === currentSort) return;
                currentSort = next;
                loadAndRender();
            });
        });
    }

    function renderListingCard(listing, rank) {
        const badge =
            listing.listing_type === 'sale'
                ? '<span class="badge badge-sale">Продажа</span>'
                : listing.listing_type === 'rent'
                  ? '<span class="badge badge-rent">Аренда</span>'
                  : '';
        const imgSrc = listing.photos ? String(listing.photos).split(',')[0].trim() : defaultImg;
        const safeImgAttr = String(imgSrc).replace(/"/g, '&quot;');
        const priceSuffix =
            typeof listingPriceSuffix === 'function' ? listingPriceSuffix(listing) : '';
        const ratingLabel =
            typeof listingRatingLabel === 'function' ? listingRatingLabel(listing) : '—';
        const avail = listing.available_from
            ? `Свободно с ${new Date(listing.available_from).toLocaleDateString()}`
            : 'В продаже';
        const rentRooms =
            listing.listing_type === 'rent'
                ? `<span><i class="fas fa-bed"></i> ${listing.rooms} комн.</span>`
                : '';
        const floorsLabel =
            typeof listingFloorsLabel === 'function'
                ? listingFloorsLabel(listing)
                : `${Number(listing.floor ?? 0)}/${Number(listing.total_floors ?? 0)} эт.`;

        return `
                <article class="listing-card stats-listing-card">
                    <div class="listing-image">
                        <span class="stats-card-rank" aria-label="Место в списке">#${rank}</span>
                        <img src="${safeImgAttr}" alt="${esc(listing.title)}">
                        ${badge}
                    </div>
                    <div class="listing-content">
                        <div class="listing-header">
                            <h4 class="listing-title">${esc(listing.title)}</h4>
                            <div class="listing-price">${Number(listing.price || 0).toLocaleString()} BYN <span>${esc(priceSuffix)}</span></div>
                        </div>
                        <div class="listing-location">
                            <i class="fas fa-map-marker-alt"></i> ${esc(listing.district || listing.city || '')}
                        </div>
                        <div class="listing-details">
                            <span><i class="fas fa-vector-square"></i> ${listing.area || 0} м²</span>
                            <span><i class="fas fa-layer-group"></i> ${floorsLabel}</span>
                            ${rentRooms}
                            <span><i class="fas fa-eye"></i> ${listing.views_count || 0} просм.</span>
                            <span class="rating"><i class="fas fa-star"></i> ${esc(ratingLabel)}</span>
                        </div>
                        <div class="listing-footer">
                            <span class="available-date"><i class="far fa-calendar-alt"></i> ${esc(avail)}</span>
                            <div style="display:flex; gap:8px; align-items:center;">
                                <a href="/listing-details.html?id=${listing.id}" class="btn btn-outline btn-sm">Подробнее</a>
                            </div>
                        </div>
                    </div>
                </article>`;
    }

    async function loadAndRender() {
        root.innerHTML = '<p class="stats-empty">Загрузка…</p>';
        renderToolbar();
        try {
            const res = await fetch(`/api/stats/overview?sort=${encodeURIComponent(currentSort)}`, {
                headers: { 'X-User-ID': uid },
            });
            const data = await res.json().catch(() => ({}));
            if (!res.ok) throw new Error(data.error || 'Не удалось загрузить статистику');

            const items = Array.isArray(data.listings) ? data.listings : [];
            const title =
                currentSort === 'views'
                    ? 'Все объявления по убыванию просмотров'
                    : 'Все объявления по убыванию рейтинга';

            if (items.length === 0) {
                root.innerHTML = `<section class="stats-section"><h2><i class="fas fa-list"></i> ${esc(title)}</h2><p class="stats-empty">В каталоге пока нет опубликованных объявлений.</p></section>`;
                return;
            }

            const cards = items.map((it, idx) => renderListingCard(it, idx + 1)).join('');

            root.innerHTML = `
                <section class="stats-section stats-section-grid">
                    <h2><i class="fas fa-list"></i> ${esc(title)}</h2>
                    <div class="listings-grid">${cards}</div>
                </section>`;
        } catch (e) {
            root.innerHTML = `<p class="stats-empty">${esc(e.message || 'Ошибка')}</p>`;
        }
    }

    await loadAndRender();
});
