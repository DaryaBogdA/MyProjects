function localDateISO(d) {
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${y}-${m}-${day}`;
}

function mapBookingError(msg) {
    const s = String(msg || '').trim();
    const ru = {
        'check_out must be after check_in': 'Дата выезда должна быть позже даты заезда.',
        'invalid check_in date': 'Укажите корректную дату заезда.',
        'invalid check_out date': 'Укажите корректную дату выезда.',
        'check_in cannot be in the past': 'Дата заезда не может быть в прошлом.',
        'selected dates are already booked': 'На эти даты уже есть бронирование. Выберите другие даты.',
    };
    return ru[s] || s || 'Не удалось оформить бронирование. Проверьте даты.';
}

function mapChatStartError(msg) {
    const s = String(msg || '').trim().toLowerCase();
    if (s === 'cannot chat with yourself') {
        return 'Вы не можете написать сами себе. Чтобы связаться с гостем по бронированию, откройте раздел «Мои бронирования» и нажмите «Перейти в чат» в карточке заявки.';
    }
    if (s === 'no booking with this guest for this listing') {
        return 'Нет подтверждённой заявки на бронирование с этим пользователем по этому объявлению.';
    }
    if (s === 'listing not found') {
        return 'Объявление не найдено или снято с публикации.';
    }
    if (s === 'unauthorized') {
        return 'Войдите в аккаунт, чтобы написать в чат.';
    }
    return String(msg || '').trim() || 'Не удалось открыть чат. Попробуйте позже.';
}

function initReviewStarPicker(mount) {
    const picker = mount.querySelector('.ld-star-picker');
    const hidden = mount.querySelector('#reviewRating');
    if (!picker || !hidden) return;
    const setRating = (r) => {
        const v = Math.max(1, Math.min(5, r));
        hidden.value = String(v);
        picker.querySelectorAll('.ld-star-btn').forEach((b) => {
            const n = Number(b.dataset.rating);
            b.classList.toggle('active', n <= v);
        });
    };
    picker.querySelectorAll('.ld-star-btn').forEach((btn) => {
        btn.addEventListener('click', () => setRating(Number(btn.dataset.rating)));
    });
    setRating(Number(hidden.value) || 5);
}

function extractPricePeriodMeta(rawDescription) {
    const raw = String(rawDescription || '');
    const m = raw.match(/^\[\[RR_PRICE_PERIOD:(day|month)\]\]\s*/i);
    const period = m ? (String(m[1]).toLowerCase() === 'day' ? 'day' : 'month') : 'month';
    const description = raw.replace(/^\[\[RR_PRICE_PERIOD:(day|month)\]\]\s*/i, '');
    return { period, description };
}

// Функция для отображения удобств
function renderAmenities(amenitiesArray) {
    const amenityMap = {
        'parking': { icon: 'fa-parking', label: 'Парковка' },
        'elevator': { icon: 'fa-arrow-up', label: 'Лифт' },
        'furniture': { icon: 'fa-couch', label: 'Мебель' },
        'children': { icon: 'fa-child', label: 'Можно с детьми' },
        'pets': { icon: 'fa-paw', label: 'Можно с животными' },
        'wifi': { icon: 'fa-wifi', label: 'Wi-Fi' },
        'renovation': { icon: 'fa-paint-roller', label: 'Евроремонт' },
        'new_building': { icon: 'fa-building', label: 'Новостройка' }
    };

    if (!amenitiesArray || amenitiesArray.length === 0) {
        return '<p class="ld-muted" style="margin: 10px 0 0;">Нет указанных удобств</p>';
    }

    let html = '<div class="ld-amenities-grid">';
    for (const amenity of amenitiesArray) {
        const info = amenityMap[amenity] || { icon: 'fa-check', label: amenity };
        html += `<div class="ld-amenity-item"><i class="fas ${info.icon}"></i> ${info.label}</div>`;
    }
    html += '</div>';
    return html;
}

let lightboxImages = [];
let lightboxCurrentIndex = 0;
let touchStartX = 0;

function initLightbox(images) {
    lightboxImages = images;
    if (lightboxImages.length === 0) return;

    let lb = document.getElementById('lightboxModal');
    if (!lb) {
        lb = document.createElement('div');
        lb.id = 'lightboxModal';
        lb.className = 'lightbox-modal';
        lb.innerHTML = `
            <button class="lightbox-close" id="lightboxCloseBtn">&times;</button>
            <button class="lightbox-prev" id="lightboxPrevBtn"><i class="fas fa-chevron-left"></i></button>
            <button class="lightbox-next" id="lightboxNextBtn"><i class="fas fa-chevron-right"></i></button>
            <div class="lightbox-content">
                <img id="lightboxImg" src="" alt="Просмотр фото">
                <div class="lightbox-counter" id="lightboxCounter"></div>
            </div>
            <button class="lightbox-fullscreen" id="lightboxFullscreenBtn"><i class="fas fa-expand"></i></button>
        `;
        document.body.appendChild(lb);

        const style = document.createElement('style');
        style.textContent = `
            .lightbox-modal {
                display: none;
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0,0,0,0.95);
                z-index: 10000;
                justify-content: center;
                align-items: center;
                cursor: pointer;
            }
            .lightbox-modal.show {
                display: flex;
            }
            .lightbox-content {
                position: relative;
                max-width: 90vw;
                max-height: 90vh;
                display: flex;
                justify-content: center;
                align-items: center;
            }
            .lightbox-content img {
                max-width: 90vw;
                max-height: 90vh;
                object-fit: contain;
                border-radius: 8px;
                box-shadow: 0 0 30px rgba(0,0,0,0.5);
                cursor: default;
            }
            .lightbox-close {
                position: absolute;
                top: 20px;
                right: 30px;
                font-size: 40px;
                color: white;
                background: none;
                border: none;
                cursor: pointer;
                z-index: 10001;
                transition: opacity 0.2s;
            }
            .lightbox-close:hover {
                opacity: 0.7;
            }
            .lightbox-prev, .lightbox-next {
                position: absolute;
                top: 50%;
                transform: translateY(-50%);
                background: rgba(0,0,0,0.6);
                color: white;
                border: none;
                font-size: 32px;
                width: 50px;
                height: 50px;
                border-radius: 50%;
                cursor: pointer;
                display: flex;
                align-items: center;
                justify-content: center;
                transition: background 0.2s;
                z-index: 10001;
            }
            .lightbox-prev { left: 20px; }
            .lightbox-next { right: 20px; }
            .lightbox-prev:hover, .lightbox-next:hover {
                background: rgba(0,0,0,0.8);
            }
            .lightbox-counter {
                position: absolute;
                bottom: -40px;
                left: 0;
                right: 0;
                text-align: center;
                color: white;
                font-size: 14px;
                background: rgba(0,0,0,0.6);
                padding: 5px 12px;
                border-radius: 20px;
                width: fit-content;
                margin: 0 auto;
            }
            .lightbox-fullscreen {
                position: absolute;
                bottom: 20px;
                right: 20px;
                background: rgba(0,0,0,0.6);
                color: white;
                border: none;
                font-size: 20px;
                width: 40px;
                height: 40px;
                border-radius: 50%;
                cursor: pointer;
                display: flex;
                align-items: center;
                justify-content: center;
                transition: background 0.2s;
                z-index: 10001;
            }
            .lightbox-fullscreen:hover {
                background: rgba(0,0,0,0.8);
            }
            @media (max-width: 768px) {
                .lightbox-prev, .lightbox-next {
                    width: 40px;
                    height: 40px;
                    font-size: 24px;
                }
                .lightbox-prev { left: 10px; }
                .lightbox-next { right: 10px; }
                .lightbox-close { top: 10px; right: 15px; font-size: 32px; }
                .lightbox-fullscreen { bottom: 10px; right: 10px; width: 36px; height: 36px; }
            }
        `;
        document.head.appendChild(style);

        lb.addEventListener('click', (e) => {
            if (e.target === lb) {
                lb.classList.remove('show');
            }
        });

        document.getElementById('lightboxCloseBtn').addEventListener('click', () => {
            lb.classList.remove('show');
        });
        document.getElementById('lightboxPrevBtn').addEventListener('click', (e) => {
            e.stopPropagation();
            lightboxCurrentIndex = (lightboxCurrentIndex - 1 + lightboxImages.length) % lightboxImages.length;
            updateLightboxImage();
        });
        document.getElementById('lightboxNextBtn').addEventListener('click', (e) => {
            e.stopPropagation();
            lightboxCurrentIndex = (lightboxCurrentIndex + 1) % lightboxImages.length;
            updateLightboxImage();
        });
        document.getElementById('lightboxFullscreenBtn').addEventListener('click', () => {
            const img = document.getElementById('lightboxImg');
            if (!img) return;
            if (document.fullscreenElement) {
                document.exitFullscreen();
            } else {
                img.requestFullscreen().catch(() => {});
            }
        });

        document.addEventListener('keydown', (e) => {
            if (!lb.classList.contains('show')) return;
            if (e.key === 'Escape') {
                lb.classList.remove('show');
            } else if (e.key === 'ArrowLeft') {
                lightboxCurrentIndex = (lightboxCurrentIndex - 1 + lightboxImages.length) % lightboxImages.length;
                updateLightboxImage();
            } else if (e.key === 'ArrowRight') {
                lightboxCurrentIndex = (lightboxCurrentIndex + 1) % lightboxImages.length;
                updateLightboxImage();
            }
        });

        let touchStartX = 0;
        lb.addEventListener('touchstart', (e) => {
            touchStartX = e.changedTouches[0].screenX;
        });
        lb.addEventListener('touchend', (e) => {
            const diff = e.changedTouches[0].screenX - touchStartX;
            if (Math.abs(diff) > 50) {
                if (diff > 0) {
                    lightboxCurrentIndex = (lightboxCurrentIndex - 1 + lightboxImages.length) % lightboxImages.length;
                } else {
                    lightboxCurrentIndex = (lightboxCurrentIndex + 1) % lightboxImages.length;
                }
                updateLightboxImage();
            }
        });
    }

    function updateLightboxImage() {
        const img = document.getElementById('lightboxImg');
        const counter = document.getElementById('lightboxCounter');
        if (img && lightboxImages[lightboxCurrentIndex]) {
            img.src = lightboxImages[lightboxCurrentIndex];
        }
        if (counter) {
            counter.textContent = `${lightboxCurrentIndex + 1} / ${lightboxImages.length}`;
        }
    }

    const imgElement = document.getElementById('ldCarouselImage');
    if (imgElement) {
        imgElement.style.cursor = 'pointer';
        imgElement.addEventListener('click', () => {
            lightboxCurrentIndex = 0;
            updateLightboxImage();
            lb.classList.add('show');
        });
    }

    window.openLightboxAtIndex = (index) => {
        lightboxCurrentIndex = Math.max(0, Math.min(index, lightboxImages.length - 1));
        updateLightboxImage();
        lb.classList.add('show');
    };
}

document.addEventListener('DOMContentLoaded', async () => {
    const root = document.getElementById('detailsRoot');
    const id = new URLSearchParams(window.location.search).get('id');
    if (!id) {
        root.innerHTML = '<p class="error">Не найден id объявления</p>';
        return;
    }

    const uid = localStorage.getItem('userId');

    async function sendUserReport(reportedUserId, listingIdOpt) {
        if (!uid) {
            window.location.href = '/login.html';
            return;
        }
        const reason = window.prompt('Опишите причину жалобы (будет рассмотрена модератором):');
        if (!reason || !String(reason).trim()) return;
        try {
            const body = { reported_user_id: reportedUserId, reason: String(reason).trim() };
            if (listingIdOpt) body.listing_id = listingIdOpt;
            const resp = await fetch('/api/reports', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-User-ID': uid },
                body: JSON.stringify(body),
            });
            const data = await resp.json().catch(() => ({}));
            if (!resp.ok) throw new Error(data.error || 'Не удалось отправить жалобу');
            alert('Жалоба отправлена модераторам.');
        } catch (e) {
            alert(e.message || 'Ошибка');
        }
    }

    function starsHtml(n) {
        let s = '';
        for (let i = 1; i <= 5; i++) {
            s += `<i class="fas fa-star" style="color:${i <= n ? '#f5a623' : '#ddd'}"></i>`;
        }
        return s;
    }

    function formatReviewDate(iso) {
        if (!iso) return '';
        const d = new Date(iso);
        if (Number.isNaN(d.getTime())) return String(iso);
        return d.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' });
    }

    async function loadReviews(listing, mount) {
        const headers = {};
        if (uid) headers['X-User-ID'] = uid;
        const revRes = await fetch(`/api/listings/${id}/reviews`, { headers });
        const revPayload = await revRes.json().catch(() => ({}));
        if (!revRes.ok) {
            mount.innerHTML = `<p class="error" style="font-size:0.9rem;">${escapeHtml(revPayload.error || 'Не удалось загрузить отзывы')}</p>`;
            return;
        }

        const avg = revPayload.count > 0 ? Number(revPayload.average_rating).toFixed(1) : '—';

        let formHtml = '';
        if (!uid) {
            formHtml = `<p style="color:var(--text-muted); margin:0 0 12px;"><a href="/login.html">Войдите</a>, чтобы оставить отзыв.</p>`;
        } else if (String(listing.user_id) === String(uid)) {
            formHtml = '';
        } else if (revPayload.viewer_has_reviewed) {
            formHtml = '<p class="ld-muted" style="margin:0 0 16px;">Вы уже оставили отзыв к этому объявлению.</p>';
        } else {
            const starsBtns = [1, 2, 3, 4, 5]
                .map(
                    (n) =>
                        `<button type="button" class="ld-star-btn" data-rating="${n}" aria-label="${n} из 5"><i class="fas fa-star"></i></button>`
                )
                .join('');
            formHtml = `
                <div class="ld-review-card ld-review-form-card">
                    <div class="ld-review-card-head">
                        <i class="fas fa-star"></i>
                        <div>
                            <h4 style="margin:0 0 4px;">Оставить отзыв</h4>
                            <p class="ld-muted">Оценка и комментарий — после модерации отзыв появится на странице.</p>
                        </div>
                    </div>
                    <span class="ld-label">Оценка</span>
                    <div class="ld-star-picker" role="group" aria-label="Оценка из пяти">${starsBtns}</div>
                    <input type="hidden" id="reviewRating" value="5">
                    <label class="ld-label" for="reviewComment">Комментарий</label>
                    <div class="ld-review-textarea-wrap">
                        <textarea id="reviewComment" rows="5" class="form-control ld-textarea ld-review-textarea" placeholder="Что понравилось, что стоит учесть…"></textarea>
                    </div>
                    <button type="button" id="submitReviewBtn" class="btn btn-primary ld-submit-review"><i class="fas fa-paper-plane"></i> Отправить отзыв</button>
                </div>`;
        }

        const items = Array.isArray(revPayload.items) ? revPayload.items : [];
        const listHtml =
            items.length === 0
                ? '<p style="color:var(--text-muted);">Пока нет отзывов.</p>'
                : items
                    .map(
                        (r) => `
                <div class="ld-review-item">
                    <div class="ld-review-item-head">
                        <div><strong>${escapeHtml(r.author_name || 'Пользователь')}</strong>
                            <div style="font-size:0.85rem; color:var(--text-muted);">${escapeHtml(formatReviewDate(r.created_at))}</div></div>
                        <div>${starsHtml(r.rating)} <span style="margin-left:6px;">${r.rating}/5</span></div>
                    </div>
                    ${r.comment ? `<p style="margin:10px 0 0;">${escapeHtml(r.comment)}</p>` : ''}
                    ${
                            uid && String(r.user_id) !== String(uid)
                                ? `<button type="button" class="btn btn-outline btn-sm report-review-btn ld-report-review" data-user-id="${r.user_id}">Пожаловаться на автора</button>`
                                : ''
                        }
                </div>`
                    )
                    .join('');

        mount.innerHTML = `
            <div class="ld-reviews-heading">
                <h3>Отзывы</h3>
                <span style="color:var(--text-muted);">Средняя: <strong>${avg}</strong> · ${revPayload.count || 0} шт.</span>
            </div>
            ${formHtml}
            <div id="reviewsListInner">${listHtml}</div>`;

        initReviewStarPicker(mount);

        mount.querySelector('#reviewsListInner')?.addEventListener('click', (ev) => {
            const btn = ev.target.closest('.report-review-btn');
            if (!btn) return;
            sendUserReport(Number(btn.dataset.userId), Number(id));
        });

        document.getElementById('submitReviewBtn')?.addEventListener('click', async () => {
            const rating = Number(document.getElementById('reviewRating')?.value || 0);
            const comment = (document.getElementById('reviewComment')?.value || '').trim();
            try {
                const resp = await fetch(`/api/listings/${id}/reviews`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json', 'X-User-ID': uid },
                    body: JSON.stringify({ rating, comment }),
                });
                const data = await resp.json().catch(() => ({}));
                if (!resp.ok) throw new Error(data.error || 'Не удалось отправить');
                const pending = data.moderation_status === 'pending';
                await loadReviews(listing, mount);
                if (pending) {
                    alert('Отзыв отправлен на проверку модератору. Он появится на странице после одобрения.');
                }
            } catch (e) {
                alert(e.message || 'Ошибка');
            }
        });
    }

    try {
        const headers = {};
        if (uid) headers['X-User-ID'] = uid;
        const res = await fetch(`/api/listings/${id}`, { headers });
        const listing = await res.json();
        if (!res.ok) throw new Error(listing.error || 'Ошибка загрузки');

        const photos = (listing.photos || '').split(',').map((s) => s.trim()).filter(Boolean);
        const images = photos.length > 0 ? photos : ['https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=900&h=500&fit=crop'];
        const meta = extractPricePeriodMeta(listing.description || '');
        const rentSuffix = listing.listing_type === 'rent' ? (meta.period === 'day' ? '/ сутки' : '/ месяц') : '';

        // Парсим удобства из JSON
        let amenitiesArray = [];
        if (listing.amenities) {
            try {
                amenitiesArray = JSON.parse(listing.amenities);
            } catch (e) {
                console.error('Failed to parse amenities:', e);
            }
        }
        const amenitiesHtml = renderAmenities(amenitiesArray);

        // Контактная информация
        const contactName = listing.contact_name || 'Собственник';
        const contactPhone = listing.contact_phone || 'Не указан';

        initLightbox(images);

        root.innerHTML = `
            <div style="display:flex;flex-direction:column;gap:20px;">
            <article class="ld-article-grid" style="display:grid; grid-template-columns:1.4fr 1fr; gap:18px;">
                <div>
                    <div class="ld-carousel" data-index="0">
                        <button type="button" class="ld-carousel-arrow prev" aria-label="Предыдущее фото"><i class="fas fa-chevron-left"></i></button>
                        ${
            listing.listing_type === 'sale'
                ? '<span class="badge badge-sale" style="position:absolute;top:12px;left:12px;z-index:3;">Продажа</span>'
                : '<span class="badge badge-rent" style="position:absolute;top:12px;left:12px;z-index:3;">Аренда</span>'
        }
                        <div id="ldCarouselFrame" class="photo-blur-frame ld-carousel-frame">
                            <div class="photo-blur-bg" style="background-image:url('${escapeHtml(images[0])}')"></div>
                            <img id="ldCarouselImage" class="photo-blur-main" src="${escapeHtml(images[0])}" alt="${escapeHtml(listing.title || '')}">
                        </div>
                        <button type="button" class="ld-carousel-arrow next" aria-label="Следующее фото"><i class="fas fa-chevron-right"></i></button>
                        <div class="ld-carousel-counter" id="ldCarouselCounter">1 / ${images.length}</div>
                    </div>
                    <div style="margin-top:14px; background:white; border:1px solid var(--border-color); border-radius:12px; padding:14px;">
                        <h2 style="margin:0 0 10px;">${escapeHtml(listing.title || '')}</h2>
                        <p style="margin:0 0 8px;"><i class="fas fa-map-marker-alt"></i> ${escapeHtml(listing.address || listing.city || '')}</p>
                        <p style="margin:0 0 8px;">${escapeHtml(meta.description || '')}</p>
                        <p style="margin:0; color:var(--text-muted);">${listing.rooms || 0} комн. · ${listing.area || 0} м² · ${Number(listing.floor ?? 0)}/${Number(listing.total_floors ?? 0)} эт.</p>
                    </div>
                </div>
                <div class="ld-sidebar-stack">
                    <div class="ld-sidebar-card">
                        <div class="ld-price-tag">${Number(listing.price || 0).toLocaleString()} <span style="font-size:1rem;font-weight:600;">BYN ${rentSuffix}</span></div>
                        <p class="ld-meta-line">Тип: <strong>${listing.listing_type === 'sale' ? 'Продажа' : 'Аренда'}</strong></p>
                    </div>
                    <div class="ld-sidebar-card ld-contact-card">
                        <div class="ld-contact-head">
                            <span class="ld-contact-icon"><i class="fas fa-user"></i></span>
                            <div>
                                <h3 class="ld-contact-title">Контактная информация</h3>
                                <p class="ld-muted"><strong>${escapeHtml(contactName)}</strong></p>
                                <p class="ld-muted"><i class="fas fa-phone"></i> ${escapeHtml(contactPhone)}</p>
                            </div>
                        </div>
                    </div>
                    <div class="ld-sidebar-card">
                        <div class="ld-contact-head">
                            <span class="ld-contact-icon"><i class="fas fa-couch"></i></span>
                            <div>
                                <h3 class="ld-contact-title">Удобства и особенности</h3>
                                ${amenitiesHtml}
                            </div>
                        </div>
                    </div>
                    <div class="ld-sidebar-card ld-contact-card">
                        <div class="ld-contact-head">
                            <span class="ld-contact-icon"><i class="fas fa-comments"></i></span>
                            <div>
                                <h3 class="ld-contact-title">Связь с собственником</h3>
                                <p class="ld-muted">Напишите сообщение, чтобы задать вопрос</p>
                            </div>
                        </div>
                        <p id="chatStartMsg" class="ld-booking-msg" role="alert" style="display:none;margin:0 0 10px;"></p>
                        <button type="button" id="startChatBtn" class="chat-simple-btn">
                            <i class="fas fa-comment-dots"></i> Написать сообщение
                        </button>
                        <div id="bookingMount" style="margin-top: 16px;"></div>
                        <div id="reportHostMount" style="margin-top: 12px;"></div>
                    </div>
                </div>
            </article>
            <section id="reviewsMount" class="ld-reviews-section">Загрузка отзывов…</section>
            </div>`;

        const carousel = root.querySelector('.ld-carousel');
        const carouselFrame = document.getElementById('ldCarouselFrame');
        const carouselImage = document.getElementById('ldCarouselImage');
        const carouselCounter = document.getElementById('ldCarouselCounter');

        if (typeof initPhotoBlurFrame === 'function' && carouselFrame) {
            initPhotoBlurFrame(carouselFrame);
        } else if (typeof initPhotoBlurFrames === 'function') {
            initPhotoBlurFrames(carousel);
        }

        function setCarouselIndex(nextIdx) {
            if (!carousel || !carouselImage || images.length === 0) return;
            let i = Number(nextIdx) || 0;
            if (i < 0) i = images.length - 1;
            if (i >= images.length) i = 0;
            carousel.dataset.index = String(i);

            const frame = document.getElementById('ldCarouselFrame');
            if (frame) {
                const blurBg = frame.querySelector('.photo-blur-bg');
                if (blurBg) blurBg.style.backgroundImage = `url('${escapeHtml(images[i])}')`;
                const mainImg = frame.querySelector('.photo-blur-main');
                if (mainImg) mainImg.src = images[i];
            } else {
                carouselImage.src = images[i];
            }
            if (carouselCounter) carouselCounter.textContent = `${i + 1} / ${images.length}`;
        }

        (carouselFrame || carouselImage).addEventListener('click', () => {
            const currentIdx = Number(carousel.dataset.index || 0);
            if (typeof window.openLightboxAtIndex === 'function') {
                window.openLightboxAtIndex(currentIdx);
            }
        });

        carousel?.querySelector('.prev')?.addEventListener('click', () => {
            setCarouselIndex(Number(carousel.dataset.index || 0) - 1);
        });
        carousel?.querySelector('.next')?.addEventListener('click', () => {
            setCarouselIndex(Number(carousel.dataset.index || 0) + 1);
        });

        document.getElementById('startChatBtn')?.addEventListener('click', async () => {
            const userId = localStorage.getItem('userId');
            const chatStartMsg = document.getElementById('chatStartMsg');
            const showChatErr = (text) => {
                if (!chatStartMsg) {
                    alert(text);
                    return;
                }
                chatStartMsg.textContent = text;
                chatStartMsg.style.display = 'block';
                chatStartMsg.className = 'ld-booking-msg ld-booking-err';
                chatStartMsg.setAttribute('role', 'alert');
            };
            const clearChatErr = () => {
                if (!chatStartMsg) return;
                chatStartMsg.style.display = 'none';
                chatStartMsg.textContent = '';
                chatStartMsg.className = 'ld-booking-msg';
            };
            if (!userId) {
                window.location.href = '/login.html';
                return;
            }
            clearChatErr();
            try {
                const resp = await fetch('/api/messages/start', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-User-ID': userId,
                    },
                    body: JSON.stringify({ listing_id: Number(id) }),
                });
                const data = await resp.json();
                if (!resp.ok) throw new Error(data.error || 'Не удалось начать диалог');
                window.location.href = `/messages.html?conversation=${data.conversation_id}`;
            } catch (e) {
                showChatErr(mapChatStartError(e.message));
            }
        });

        const bookingMount = document.getElementById('bookingMount');
        if (bookingMount) {
            if (listing.listing_type !== 'rent') {
                bookingMount.innerHTML = '<div class="booking-info"><i class="fas fa-info-circle"></i> Бронирование доступно только для аренды.</div>';
            } else if (!uid) {
                bookingMount.innerHTML = '<div class="booking-info"><i class="fas fa-lock"></i> <a href="/login.html">Войдите</a>, чтобы забронировать.</div>';
            } else if (String(uid) === String(listing.user_id)) {
                bookingMount.innerHTML = `
                    <div class="ld-booking-card ld-owner-booking-hint">
                        <p class="ld-muted" style="margin:0 0 10px;line-height:1.45;">
                            <i class="fas fa-calendar-check"></i> Заявки на бронирование этого жилья смотрите в разделе «Мои бронирования» — там можно подтвердить заявку и <strong>написать гостю в чат</strong>.
                        </p>
                        <a href="/bookings.html" class="btn btn-outline" style="width:100%;text-align:center;">Открыть бронирования</a>
                    </div>`;
            } else {
                const today = new Date();
                let minBookDate = localDateISO(today);
                let availFromLabel = '';
                if (listing.available_from) {
                    const avail = new Date(listing.available_from);
                    if (!Number.isNaN(avail.getTime())) {
                        const availIso = localDateISO(avail);
                        if (availIso > minBookDate) {
                            minBookDate = availIso;
                        }
                        availFromLabel = avail.toLocaleDateString('ru-RU', {
                            day: 'numeric',
                            month: 'long',
                            year: 'numeric',
                        });
                    }
                }
                const maxDateObj = new Date();
                maxDateObj.setMonth(maxDateObj.getMonth() + 6);
                const maxDate = localDateISO(maxDateObj);
                const availHint = availFromLabel
                    ? `<div class="booking-info"><i class="far fa-calendar-alt"></i> Свободно с <strong>${availFromLabel}</strong> — заезд не раньше этой даты.</div>`
                    : '';

                bookingMount.innerHTML = `
            <div class="ld-booking-card">
                <h4><i class="fas fa-calendar-check"></i> Забронировать даты</h4>
                ${availHint}
                <div class="booking-info">
                    <i class="fas fa-clock"></i> Выберите даты заезда и выезда
                </div>
                <div class="booking-date-picker">
                    <div class="booking-date-group">
                        <label><i class="fas fa-calendar-alt"></i> Заезд</label>
                        <input type="date" id="bookingCheckIn" class="form-control" min="${minBookDate}" max="${maxDate}">
                    </div>
                    <div class="booking-date-group">
                        <label><i class="fas fa-calendar-check"></i> Выезд</label>
                        <input type="date" id="bookingCheckOut" class="form-control" min="${minBookDate}" max="${maxDate}">
                    </div>
                </div>
                <button type="button" id="createBookingBtn" class="ld-booking-btn">
                    <i class="fas fa-paper-plane"></i> Отправить заявку на бронирование
                </button>
                <p id="bookingMsg" class="ld-booking-msg" role="status"></p>
            </div>
        `;

                const checkInInput = document.getElementById('bookingCheckIn');
                const checkOutInput = document.getElementById('bookingCheckOut');

                if (checkInInput) {
                    checkInInput.addEventListener('change', () => {
                        const checkIn = checkInInput.value;
                        if (checkIn && checkOutInput) {
                            checkOutInput.min = checkIn;
                        }
                    });
                }

                document.getElementById('createBookingBtn')?.addEventListener('click', async () => {
                    const checkIn = document.getElementById('bookingCheckIn')?.value || '';
                    const checkOut = document.getElementById('bookingCheckOut')?.value || '';
                    const msg = document.getElementById('bookingMsg');
                    if (msg) {
                        msg.textContent = '';
                        msg.className = 'ld-booking-msg';
                    }
                    if (!checkIn || !checkOut) {
                        if (msg) {
                            msg.textContent = 'Укажите даты заезда и выезда.';
                            msg.classList.add('ld-booking-err');
                        }
                        return;
                    }
                    if (checkOut <= checkIn) {
                        if (msg) {
                            msg.textContent = 'Дата выезда должна быть позже даты заезда.';
                            msg.classList.add('ld-booking-err');
                        }
                        return;
                    }
                    if (checkIn < minBookDate) {
                        if (msg) {
                            msg.textContent = availFromLabel
                                ? `Дата заезда не может быть раньше ${availFromLabel} (дата «Свободно с» в объявлении).`
                                : 'Дата заезда не может быть в прошлом.';
                            msg.classList.add('ld-booking-err');
                        }
                        return;
                    }
                    try {
                        const resp = await fetch(`/api/listings/${id}/bookings`, {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json', 'X-User-ID': uid },
                            body: JSON.stringify({ check_in: checkIn, check_out: checkOut }),
                        });
                        const data = await resp.json().catch(() => ({}));
                        if (!resp.ok) throw new Error(mapBookingError(data.error));
                        if (msg) {
                            msg.innerHTML = '<i class="fas fa-check-circle"></i> Заявка отправлена! Собственник получит уведомление.';
                            msg.classList.add('ld-booking-ok');
                        }
                        document.getElementById('bookingCheckIn').value = '';
                        document.getElementById('bookingCheckOut').value = '';
                    } catch (e) {
                        if (msg) {
                            msg.innerHTML = '<i class="fas fa-exclamation-triangle"></i> ' + mapBookingError(e.message);
                            msg.classList.add('ld-booking-err');
                        }
                    }
                });
            }
        }

        const reportHostMount = document.getElementById('reportHostMount');
        if (reportHostMount && uid && String(uid) !== String(listing.user_id)) {
            reportHostMount.innerHTML =
                '<button type="button" id="reportHostBtn" class="btn btn-outline" style="width:100%;font-size:0.85rem;">Пожаловаться на собственника</button>';
            document.getElementById('reportHostBtn')?.addEventListener('click', () => {
                sendUserReport(Number(listing.user_id), Number(id));
            });
        } else if (reportHostMount) {
            reportHostMount.innerHTML = '';
        }

        const rm = document.getElementById('reviewsMount');
        if (rm) await loadReviews(listing, rm);
    } catch (e) {
        root.innerHTML = `<p class="error">${escapeHtml(e.message || 'Ошибка')}</p>`;
    }
});