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
                <div class="ld-review-card">
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
                    <textarea id="reviewComment" rows="4" class="form-control ld-textarea" placeholder="Что понравилось, что стоит учесть…"></textarea>
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
        root.innerHTML = `
            <div style="display:flex;flex-direction:column;gap:20px;">
            <article class="ld-article-grid" style="display:grid; grid-template-columns:1.4fr 1fr; gap:18px;">
                <div>
                    <div class="ld-carousel" data-index="0">
                        <button type="button" class="ld-carousel-arrow prev" aria-label="Предыдущее фото"><i class="fas fa-chevron-left"></i></button>
                        <img id="ldCarouselImage" src="${escapeHtml(images[0])}" alt="${escapeHtml(listing.title || '')}" style="width:100%; max-height:440px; object-fit:cover; border-radius:12px;">
                        <button type="button" class="ld-carousel-arrow next" aria-label="Следующее фото"><i class="fas fa-chevron-right"></i></button>
                        <div class="ld-carousel-counter" id="ldCarouselCounter">1 / ${images.length}</div>
                    </div>
                    <div style="margin-top:14px; background:white; border:1px solid var(--border-color); border-radius:12px; padding:14px;">
                        <h2 style="margin:0 0 10px;">${escapeHtml(listing.title || '')}</h2>
                        <p style="margin:0 0 8px;"><i class="fas fa-map-marker-alt"></i> ${escapeHtml(listing.address || listing.city || '')}</p>
                        <p style="margin:0 0 8px;">${escapeHtml(meta.description || '')}</p>
                        <p style="margin:0; color:var(--text-muted);">${listing.rooms || 0} комн. · ${listing.area || 0} м² · ${listing.floor || 0}/${listing.total_floors || 0} эт.</p>
                    </div>
                </div>
                <div class="ld-sidebar-stack">
                    <div class="ld-sidebar-card">
                        <div class="ld-price-tag">${Number(listing.price || 0).toLocaleString()} <span style="font-size:1rem;font-weight:600;">BYN ${rentSuffix}</span></div>
                        <p class="ld-meta-line">Тип: <strong>${listing.listing_type === 'sale' ? 'Продажа' : 'Аренда'}</strong></p>
                    </div>
                    <div class="ld-sidebar-card ld-contact-card">
                        <div class="ld-contact-head">
                            <span class="ld-contact-icon"><i class="fas fa-comments"></i></span>
                            <div>
                                <h3 class="ld-contact-title">Связь с собственником</h3>
                                <p class="ld-muted">Нажмите на кнопку, чтобы начать диалог</p>
                            </div>
                        </div>
                        <button type="button" id="startChatBtn" class="chat-simple-btn">
                            <i class="fas fa-comment-dots"></i> Открыть чат с собственником
                        </button>
                        <div id="bookingMount" style="margin-top: 16px;"></div>
                        <div id="reportHostMount" style="margin-top: 12px;"></div>
                    </div>
                </div>
            </article>
            <section id="reviewsMount" class="ld-reviews-section">Загрузка отзывов…</section>
            </div>`;

        const carousel = root.querySelector('.ld-carousel');
        const carouselImage = document.getElementById('ldCarouselImage');
        const carouselCounter = document.getElementById('ldCarouselCounter');
        function setCarouselIndex(nextIdx) {
            if (!carousel || !carouselImage || images.length === 0) return;
            let i = Number(nextIdx) || 0;
            if (i < 0) i = images.length - 1;
            if (i >= images.length) i = 0;
            carousel.dataset.index = String(i);
            carouselImage.src = images[i];
            if (carouselCounter) carouselCounter.textContent = `${i + 1} / ${images.length}`;
        }
        carousel?.querySelector('.prev')?.addEventListener('click', () => {
            setCarouselIndex(Number(carousel.dataset.index || 0) - 1);
        });
        carousel?.querySelector('.next')?.addEventListener('click', () => {
            setCarouselIndex(Number(carousel.dataset.index || 0) + 1);
        });

        document.getElementById('startChatBtn')?.addEventListener('click', async () => {
            const userId = localStorage.getItem('userId');
            if (!userId) {
                window.location.href = '/login.html';
                return;
            }
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
                alert(e.message || 'Ошибка');
            }
        });

        const bookingMount = document.getElementById('bookingMount');
        if (bookingMount) {
            if (listing.listing_type !== 'rent') {
                bookingMount.innerHTML = '<div class="booking-info"><i class="fas fa-info-circle"></i> Бронирование доступно только для аренды.</div>';
            } else if (!uid) {
                bookingMount.innerHTML = '<div class="booking-info"><i class="fas fa-lock"></i> <a href="/login.html">Войдите</a>, чтобы забронировать.</div>';
            } else if (String(uid) === String(listing.user_id)) {
                bookingMount.innerHTML = '';
            } else {
                const today = new Date();
                const minDate = today.toISOString().slice(0, 10);
                const maxDateObj = new Date();
                maxDateObj.setMonth(maxDateObj.getMonth() + 6);
                const maxDate = maxDateObj.toISOString().slice(0, 10);

                bookingMount.innerHTML = `
            <div class="ld-booking-card">
                <h4><i class="fas fa-calendar-check"></i> Забронировать даты</h4>
                <div class="booking-info">
                    <i class="fas fa-clock"></i> Выберите даты заезда и выезда
                </div>
                <div class="booking-date-picker">
                    <div class="booking-date-group">
                        <label><i class="fas fa-calendar-alt"></i> Заезд</label>
                        <input type="date" id="bookingCheckIn" class="form-control" min="${minDate}" max="${maxDate}">
                    </div>
                    <div class="booking-date-group">
                        <label><i class="fas fa-calendar-check"></i> Выезд</label>
                        <input type="date" id="bookingCheckOut" class="form-control" min="${minDate}" max="${maxDate}">
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
