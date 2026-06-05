document.addEventListener('DOMContentLoaded', async function() {
    await initCreateListing();
});

async function initCreateListing() {
    if (window.__RR_CREATE_LISTING_INIT_DONE) {
        return;
    }
    window.__RR_CREATE_LISTING_INIT_DONE = true;

    if (typeof isUserLoggedIn === 'function' && !await isUserLoggedIn()) {
        window.location.href = '/login.html';
        return;
    }

    let selectedFiles = [];
    let existingPhotos = [];
    let photosToDelete = [];
    let currentType = 'rent';
    let currentRentPricePeriod = 'month';
    let editListingId = null;
    let existingPhotoCount = 0;
    const params = new URLSearchParams(window.location.search);
    const editIdParam = params.get('id');
    if (editIdParam && /^\d+$/.test(editIdParam)) {
        editListingId = editIdParam;
    }

    const rentBtn = document.getElementById('rentTypeBtn');
    const saleBtn = document.getElementById('saleTypeBtn');
    const form = document.getElementById('createListingForm');
    const submitBtn = document.getElementById('submitBtn');
    const citySelect = document.getElementById('citySelect');
    const cityCustomWrap = document.getElementById('cityCustomWrap');
    const cityCustom = document.getElementById('cityCustom');
    const existingPhotosNote = document.getElementById('existingPhotosNote');
    const floorFormGroup = document.getElementById('floorFormGroup');
    const floorInput = document.getElementById('floor');
    const plotAreaFormGroup = document.getElementById('plotAreaFormGroup');
    const plotAreaInput = document.getElementById('plotArea');
    const propertyTypeSelect = document.getElementById('propertyType');

    function fillCitySelect() {
        if (!citySelect || !window.BELARUS_CITIES) return;
        citySelect.innerHTML = '<option value="" selected disabled hidden>Выберите город</option>';
        for (const name of BELARUS_CITIES) {
            const opt = document.createElement('option');
            opt.value = name;
            opt.textContent = name;
            citySelect.appendChild(opt);
        }
        const other = document.createElement('option');
        other.value = '__other__';
        other.textContent = 'Другой (ввести вручную)';
        citySelect.appendChild(other);
    }

    function toggleCityCustom() {
        if (!citySelect || !cityCustomWrap) return;
        if (citySelect.value === '__other__') {
            cityCustomWrap.classList.remove('hidden');
        } else {
            cityCustomWrap.classList.add('hidden');
            if (cityCustom) cityCustom.value = '';
        }
    }

    function getResolvedCity() {
        if (!citySelect) return '';
        if (citySelect.value === '__other__') {
            return (cityCustom && cityCustom.value.trim()) ? cityCustom.value.trim() : '';
        }
        return citySelect.value.trim();
    }

    function parsePricePeriodFromDescription(raw) {
        const txt = String(raw || '');
        const m = txt.match(/^\[\[RR_PRICE_PERIOD:(day|month)\]\]\s*/i);
        if (!m) return { period: 'month', description: txt };
        return {
            period: String(m[1] || 'month').toLowerCase() === 'day' ? 'day' : 'month',
            description: txt.replace(/^\[\[RR_PRICE_PERIOD:(day|month)\]\]\s*/i, ''),
        };
    }

    function withPricePeriodMarker(description) {
        const clean = String(description || '').replace(/^\[\[RR_PRICE_PERIOD:(day|month)\]\]\s*/i, '');
        if (currentType !== 'rent') return clean;
        const p = currentRentPricePeriod === 'day' ? 'day' : 'month';
        return `[[RR_PRICE_PERIOD:${p}]] ${clean}`;
    }

    fillCitySelect();
    citySelect?.addEventListener('change', () => {
        toggleCityCustom();
        document.getElementById('cityError') && (document.getElementById('cityError').textContent = '');
        citySelect?.classList.remove('input-error');
        cityCustom?.classList.remove('input-error');
    });
    cityCustom?.addEventListener('input', () => {
        document.getElementById('cityError') && (document.getElementById('cityError').textContent = '');
        cityCustom?.classList.remove('input-error');
    });
    toggleCityCustom();

    function initListingLocationMap() {
        const mapEl = document.getElementById('listingLocationMap');
        if (!mapEl || typeof L === 'undefined') {
            window.__rrSetListingMapPos = null;
            return;
        }
        const latInput = document.getElementById('listingLat');
        const lngInput = document.getElementById('listingLng');
        const hint = document.getElementById('locationHint');
        const m = L.map(mapEl, { scrollWheelZoom: false }).setView([53.9045, 27.5615], 11);
        L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', {
            attribution: '&copy; OpenStreetMap',
            subdomains: 'abcd',
            maxZoom: 19,
        }).addTo(m);
        let marker = null;
        function setPos(lat, lng, zoom) {
            if (!Number.isFinite(lat) || !Number.isFinite(lng)) return;
            if (marker) m.removeLayer(marker);
            marker = L.marker([lat, lng], { draggable: true }).addTo(m);
            marker.on('dragend', () => {
                const p = marker.getLatLng();
                if (latInput) latInput.value = p.lat.toFixed(6);
                if (lngInput) lngInput.value = p.lng.toFixed(6);
            });
            if (latInput) latInput.value = lat.toFixed(6);
            if (lngInput) lngInput.value = lng.toFixed(6);
            const z = zoom != null ? zoom : Math.max(m.getZoom(), 14);
            m.setView([lat, lng], z);
            if (hint) hint.textContent = 'Точка выбрана. Маркер можно перетащить.';
            const errEl = document.getElementById('locationMapError');
            if (errEl) errEl.textContent = '';
        }
        window.__rrSetListingMapPos = setPos;
        m.on('click', (e) => {
            setPos(e.latlng.lat, e.latlng.lng);
        });
        const geoBtn = document.getElementById('geocodeAddressBtn');
        geoBtn?.addEventListener('click', async () => {
            const addr = document.getElementById('address')?.value?.trim() || '';
            const city = getResolvedCity();
            const q = [city, addr].filter(Boolean).join(', ');
            if (!city || addr.length < 3) {
                alert('Выберите город и введите адрес не короче 3 символов для поиска.');
                return;
            }
            geoBtn.disabled = true;
            try {
                const uid = localStorage.getItem('userId');
                const res = await fetch(`/api/geocode?q=${encodeURIComponent(q)}`, {
                    headers: uid ? { 'X-User-ID': uid } : {},
                });
                const data = await res.json().catch(() => ({}));
                if (!res.ok) throw new Error(data.error || 'Ошибка поиска');
                setPos(Number(data.lat), Number(data.lon));
                if (hint && data.label) hint.textContent = String(data.label);
            } catch (ex) {
                alert(ex.message || 'Не удалось найти адрес');
            } finally {
                geoBtn.disabled = false;
            }
        });
        requestAnimationFrame(() => {
            try {
                m.invalidateSize();
            } catch (_) {}
        });
        setTimeout(() => {
            try {
                m.invalidateSize();
            } catch (_) {}
        }, 450);
    }

    initListingLocationMap();

    function renderAllPhotos() {
        const photoPreview = document.getElementById('photoPreview');
        if (!photoPreview) return;

        photoPreview.innerHTML = '';
        let allPhotos = [];

        const visibleExisting = existingPhotos.filter(url => !photosToDelete.includes(url));
        for (let i = 0; i < visibleExisting.length; i++) {
            allPhotos.push({ type: 'existing', url: visibleExisting[i], index: i });
        }
        for (let i = 0; i < selectedFiles.length; i++) {
            allPhotos.push({ type: 'new', file: selectedFiles[i], index: i });
        }

        if (allPhotos.length === 0) {
            return;
        }

        allPhotos.forEach((photo, idx) => {
            const previewItem = document.createElement('div');
            previewItem.className = 'photo-preview-item';

            if (photo.type === 'existing') {
                previewItem.innerHTML = `
                    ${photoBlurFrameHtml(photo.url, 'Фото')}
                    <button type="button" class="photo-remove" data-type="existing" data-url="${escapeHtml(photo.url)}" data-index="${photo.index}">&times;</button>
                    ${idx === 0 ? '<span class="photo-cover">Обложка</span>' : ''}
                `;
            } else {
                const reader = new FileReader();
                previewItem.innerHTML = `
                    ${photoBlurFrameHtml('', 'Фото')}
                    <button type="button" class="photo-remove" data-type="new" data-index="${photo.index}">&times;</button>
                    ${idx === 0 ? '<span class="photo-cover">Обложка</span>' : ''}
                `;
                reader.onload = (e) => {
                    const frame = previewItem.querySelector('.photo-blur-frame');
                    if (frame) updatePhotoBlurFrameSrc(frame, e.target.result);
                };
                reader.readAsDataURL(photo.file);
            }
            photoPreview.appendChild(previewItem);
        });

        initPhotoBlurFrames(photoPreview);

        document.querySelectorAll('.photo-remove').forEach(btn => {
            btn.removeEventListener('click', handlePhotoRemove);
            btn.addEventListener('click', handlePhotoRemove);
        });
    }

    function handlePhotoRemove(e) {
        const btn = e.currentTarget;
        const type = btn.dataset.type;

        if (type === 'existing') {
            const url = btn.dataset.url;
            if (!photosToDelete.includes(url)) {
                photosToDelete.push(url);
            }
            renderAllPhotos();
        } else if (type === 'new') {
            const idx = parseInt(btn.dataset.index);
            if (!isNaN(idx) && idx >= 0 && idx < selectedFiles.length) {
                selectedFiles.splice(idx, 1);
                renderAllPhotos();
            }
        }

        updateExistingPhotosNote();
    }

    function updateExistingPhotosNote() {
        const visibleExistingCount = existingPhotos.filter(url => !photosToDelete.includes(url)).length;
        const existingPhotosNote = document.getElementById('existingPhotosNote');
        if (existingPhotosNote) {
            if (visibleExistingCount > 0) {
                existingPhotosNote.style.display = 'block';
                existingPhotosNote.textContent = `Уже загружено фото: ${visibleExistingCount}. Можно добавить ещё или удалить.`;
            } else {
                existingPhotosNote.style.display = 'none';
            }
        }
    }

    async function loadListingForEdit(id) {
        const uid = localStorage.getItem('userId');
        const res = await fetch(`/api/listings/${id}`, {
            headers: { 'X-User-ID': uid || '' },
        });
        const L = await res.json().catch(() => ({}));
        if (!res.ok) {
            alert(L.error || 'Не удалось загрузить объявление');
            window.location.href = '/profile.html';
            return false;
        }
        if (String(L.user_id) !== String(uid)) {
            alert('Это не ваше объявление');
            window.location.href = '/profile.html';
            return false;
        }

        const titleEl = document.getElementById('createPageTitle');
        const subEl = document.getElementById('createPageSubtitle');
        if (titleEl) titleEl.textContent = 'Редактировать объявление';
        if (subEl) subEl.textContent = 'Внесите изменения и сохраните. Объявление снова уйдёт на модерацию.';

        document.getElementById('listingTitle').value = L.title || '';
        const parsedPeriod = parsePricePeriodFromDescription(L.description || '');
        currentRentPricePeriod = parsedPeriod.period;
        document.getElementById('description').value = parsedPeriod.description;
        if (L.property_type) {
            document.getElementById('propertyType').value = L.property_type;
        }
        if (L.rooms != null) document.getElementById('rooms').value = String(L.rooms);
        if (L.area != null) document.getElementById('area').value = String(L.area);
        if (L.plot_area != null && plotAreaInput) plotAreaInput.value = String(L.plot_area);

        updateFloorFieldByPropertyType();

        if (L.floor != null) document.getElementById('floor').value = String(L.floor);
        if (L.total_floors != null) document.getElementById('totalFloors').value = String(L.total_floors);
        document.getElementById('address').value = L.address || '';
        document.getElementById('district').value = L.district || '';
        document.getElementById('price').value = L.price != null ? String(L.price) : '';

        if (L.latitude != null && L.longitude != null) {
            const la = Number(L.latitude);
            const lo = Number(L.longitude);
            if (Number.isFinite(la) && Number.isFinite(lo)) {
                const latIn = document.getElementById('listingLat');
                const lngIn = document.getElementById('listingLng');
                if (latIn) latIn.value = String(la);
                if (lngIn) lngIn.value = String(lo);
                if (typeof window.__rrSetListingMapPos === 'function') {
                    window.__rrSetListingMapPos(la, lo, 15);
                }
            }
        }

        const cityVal = (L.city || '').trim();
        const inList = window.BELARUS_CITIES && BELARUS_CITIES.includes(cityVal);
        if (inList) {
            citySelect.value = cityVal;
        } else if (cityVal) {
            citySelect.value = '__other__';
            if (cityCustom) cityCustom.value = cityVal;
        }
        toggleCityCustom();

        const listingType = (L.listing_type || '').toLowerCase();
        if (listingType === 'sale') {
            updateFormForType('sale');
        } else {
            updateFormForType('rent');
        }

        if (L.available_from) {
            const d = new Date(L.available_from);
            const input = document.getElementById('availableFrom');
            if (input && !isNaN(d.getTime())) {
                input.value = d.toISOString().slice(0, 10);
            }
        }
        const util = document.getElementById('utilitiesIncluded');
        if (util) util.checked = !!L.utilities_included;

        if (L.photos) {
            existingPhotos = L.photos.split(',').map(s => s.trim()).filter(Boolean);
        } else {
            existingPhotos = [];
        }
        photosToDelete = [];

        renderAllPhotos();
        updateExistingPhotosNote();

        if (submitBtn) {
            submitBtn.innerHTML = 'Сохранить изменения';
        }
        return true;
    }

    if (editListingId) {
        const ok = await loadListingForEdit(editListingId);
        if (!ok) return;
    }

    function clearErrors() {
        document.querySelectorAll('.error-message').forEach(el => el.textContent = '');
        document.querySelectorAll('.input-error').forEach(el => el.classList.remove('input-error'));
        citySelect?.classList.remove('input-error');
        cityCustom?.classList.remove('input-error');
    }

    function showError(fieldId, message) {
        const errorDiv = document.getElementById(`${fieldId}Error`);
        if (errorDiv) {
            errorDiv.textContent = message;
        }
        const input = document.getElementById(fieldId);
        if (input) {
            input.classList.add('input-error');
        }
    }

    function showSuccess(message) {
        const messagesDiv = document.getElementById('formMessages');
        messagesDiv.innerHTML = `
            <div class="success-message">
                <i class="fas fa-check-circle"></i>
                <span>${message}</span>
            </div>
        `;
        setTimeout(() => {
            messagesDiv.innerHTML = '';
        }, 5000);
    }

    function updateFormForType(type) {
        const rentOnlyFields = document.querySelectorAll('.rent-only');
        const saleOnlyFields = document.querySelectorAll('.sale-only');
        const pricePeriodLabel = document.getElementById('pricePeriodLabel');
        const priceLabel = document.getElementById('priceLabel');
        const utilitiesText = document.getElementById('utilitiesText');

        if (type === 'rent') {
            rentOnlyFields.forEach(field => field.classList.remove('hidden'));
            saleOnlyFields.forEach(field => field.classList.add('hidden'));
            if (pricePeriodLabel) pricePeriodLabel.textContent = currentRentPricePeriod === 'day' ? '(за сутки)' : '(за месяц)';
            if (priceLabel) priceLabel.textContent = currentRentPricePeriod === 'day' ? 'Цена за сутки (BYN) *' : 'Цена за месяц (BYN) *';
            if (utilitiesText) utilitiesText.textContent = 'Включены в стоимость';
            document.getElementById('availableFrom')?.setAttribute('required', 'required');
        } else {
            rentOnlyFields.forEach(field => field.classList.add('hidden'));
            saleOnlyFields.forEach(field => field.classList.remove('hidden'));
            if (pricePeriodLabel) pricePeriodLabel.textContent = '(за всё)';
            if (priceLabel) priceLabel.textContent = 'Цена (BYN) *';
            if (utilitiesText) utilitiesText.textContent = 'Коммунальные платежи не включены';
            document.getElementById('availableFrom')?.removeAttribute('required');
        }

        if (rentBtn && saleBtn) {
            rentBtn.classList.toggle('active', type === 'rent');
            saleBtn.classList.toggle('active', type === 'sale');
        }
        currentType = type;
        updateFloorFieldByPropertyType();
        updateRentPricePeriodButtons();
        updateHouseSensitiveAmenities();
    }

    function updateRentPricePeriodButtons() {
        document.querySelectorAll('#pricePeriodSwitch .price-period-btn').forEach((btn) => {
            btn.classList.toggle('active', btn.dataset.period === currentRentPricePeriod);
        });
        const pricePeriodLabel = document.getElementById('pricePeriodLabel');
        const priceLabel = document.getElementById('priceLabel');
        if (currentType === 'rent') {
            if (pricePeriodLabel) pricePeriodLabel.textContent = currentRentPricePeriod === 'day' ? '(за сутки)' : '(за месяц)';
            if (priceLabel) priceLabel.textContent = currentRentPricePeriod === 'day' ? 'Цена за сутки (BYN) *' : 'Цена за месяц (BYN) *';
        }
    }

    function updateHouseSensitiveAmenities() {
        const isHouse = propertyTypeSelect?.value === 'house';
        const elevatorWrap = document.getElementById('amenityElevatorWrap');
        const elevator = document.getElementById('amenityElevator');
        if (elevatorWrap) {
            elevatorWrap.classList.toggle('hidden', !!isHouse);
        }
        if (elevator) {
            if (isHouse) {
                elevator.checked = false;
                elevator.disabled = true;
            } else {
                elevator.disabled = false;
            }
        }
        const nbWrap = document.getElementById('amenityNewBuildingWrap');
        const nb = document.getElementById('amenityNewBuilding');
        if (nbWrap && currentType === 'sale') {
            if (isHouse) {
                nbWrap.classList.add('hidden');
                if (nb) nb.checked = false;
            } else {
                nbWrap.classList.remove('hidden');
            }
        }
        if (nb && currentType === 'sale' && isHouse) {
            nb.disabled = true;
        } else if (nb) {
            nb.disabled = false;
        }
    }

    function updateFloorFieldByPropertyType() {
        const isHouse = propertyTypeSelect?.value === 'house';
        if (floorFormGroup) {
            floorFormGroup.classList.toggle('hidden', !!isHouse);
            floorFormGroup.style.display = isHouse ? 'none' : '';
        }
        if (floorInput) {
            if (isHouse) {
                floorInput.removeAttribute('required');
                floorInput.disabled = true;
                floorInput.value = '';
            } else {
                floorInput.setAttribute('required', 'required');
                floorInput.disabled = false;
            }
        }
        if (plotAreaFormGroup) {
            plotAreaFormGroup.classList.toggle('hidden', !isHouse);
            plotAreaFormGroup.style.display = isHouse ? '' : 'none';
        }
        if (plotAreaInput) {
            if (isHouse) {
                plotAreaInput.setAttribute('required', 'required');
                plotAreaInput.disabled = false;
            } else {
                plotAreaInput.removeAttribute('required');
                plotAreaInput.disabled = true;
                plotAreaInput.value = '';
            }
        }
        updateHouseSensitiveAmenities();
    }

    if (rentBtn && saleBtn) {
        rentBtn.addEventListener('click', () => updateFormForType('rent'));
        saleBtn.addEventListener('click', () => updateFormForType('sale'));
    }
    document.querySelectorAll('#pricePeriodSwitch .price-period-btn').forEach((btn) => {
        btn.addEventListener('click', (e) => {
            e.preventDefault();
            currentRentPricePeriod = btn.dataset.period === 'day' ? 'day' : 'month';
            updateRentPricePeriodButtons();
        });
    });
    propertyTypeSelect?.addEventListener('change', updateFloorFieldByPropertyType);
    propertyTypeSelect?.addEventListener('input', updateFloorFieldByPropertyType);
    updateRentPricePeriodButtons();
    updateFloorFieldByPropertyType();
    updateFormForType(currentType);

    const photoInput = document.getElementById('photoInput');
    const selectPhotosBtn = document.getElementById('selectPhotosBtn');
    const photoPreview = document.getElementById('photoPreview');
    const photoUploadArea = document.getElementById('photoUploadArea');

    if (selectPhotosBtn) {
        selectPhotosBtn.addEventListener('click', () => photoInput.click());
    }

    if (photoUploadArea) {
        photoUploadArea.addEventListener('dragover', (e) => {
            e.preventDefault();
            photoUploadArea.classList.add('drag-over');
        });

        photoUploadArea.addEventListener('dragleave', () => {
            photoUploadArea.classList.remove('drag-over');
        });

        photoUploadArea.addEventListener('drop', (e) => {
            e.preventDefault();
            photoUploadArea.classList.remove('drag-over');
            const files = Array.from(e.dataTransfer.files);
            handleFiles(files);
        });
    }

    if (photoInput) {
        photoInput.addEventListener('change', (e) => {
            const files = Array.from(e.target.files);
            handleFiles(files);
        });
    }

    function handleFiles(files) {
        const visibleExistingCount = existingPhotos.filter(url => !photosToDelete.includes(url)).length;
        if (visibleExistingCount + selectedFiles.length + files.length > 50) {
            showError('photos', 'Можно загрузить не более 50 фотографий');
            return;
        }

        const imageFiles = files.filter(file => file.type.startsWith('image/'));
        selectedFiles.push(...imageFiles);
        renderAllPhotos();
        document.getElementById('photosError').textContent = '';
    }

    const description = document.getElementById('description');
    const counter = document.querySelector('.textarea-counter');
    if (description && counter) {
        function syncDescriptionCounter() {
            let len = description.value.length;
            if (len > 5000) {
                description.value = description.value.substring(0, 5000);
                len = 5000;
            }
            counter.textContent = `${len}/5000`;
        }
        description.addEventListener('input', syncDescriptionCounter);
        syncDescriptionCounter();
    }

    function validateForm() {
        clearErrors();
        let isValid = true;

        const title = document.getElementById('listingTitle')?.value.trim();
        if (!title) {
            showError('title', 'Введите название объявления');
            isValid = false;
        } else if (title.length < 5) {
            showError('title', 'Название должно содержать минимум 5 символов');
            isValid = false;
        }

        const propertyType = document.getElementById('propertyType')?.value;
        if (!propertyType) {
            showError('propertyType', 'Выберите тип жилья');
            isValid = false;
        }

        const rooms = document.getElementById('rooms')?.value;
        if (!rooms) {
            showError('rooms', 'Выберите количество комнат');
            isValid = false;
        }

        const area = document.getElementById('area')?.value;
        if (!area) {
            showError('area', 'Введите площадь');
            isValid = false;
        } else if (parseFloat(area) < 1) {
            showError('area', 'Площадь должна быть больше 0');
            isValid = false;
        }

        const isHouse = propertyTypeSelect?.value === 'house';
        if (isHouse) {
            const plotArea = plotAreaInput?.value;
            if (!plotArea) {
                showError('plotArea', 'Укажите размер участка');
                isValid = false;
            } else if (parseFloat(plotArea) < 0.1) {
                showError('plotArea', 'Размер участка должен быть больше 0');
                isValid = false;
            }
        }

        const floor = document.getElementById('floor')?.value;
        if (!isHouse) {
            if (!floor) {
                showError('floor', 'Введите этаж');
                isValid = false;
            } else if (parseInt(floor) < 1) {
                showError('floor', 'Этаж должен быть больше 0');
                isValid = false;
            }
        }

        const totalFloors = document.getElementById('totalFloors')?.value;
        if (!totalFloors) {
            showError('totalFloors', 'Введите количество этажей в доме');
            isValid = false;
        } else if (!isHouse && parseInt(totalFloors) < parseInt(floor || 1)) {
            showError('totalFloors', 'Этажей в доме не может быть меньше текущего этажа');
            isValid = false;
        }

        const address = document.getElementById('address')?.value.trim();
        if (!address) {
            showError('address', 'Введите адрес');
            isValid = false;
        }

        const cityVal = getResolvedCity();
        if (!citySelect || !citySelect.value) {
            document.getElementById('cityError').textContent = 'Выберите город из списка';
            citySelect?.classList.add('input-error');
            isValid = false;
        } else if (citySelect.value === '__other__' && !cityVal) {
            document.getElementById('cityError').textContent = 'Введите название города';
            cityCustom?.classList.add('input-error');
            isValid = false;
        }

        if (typeof L !== 'undefined' && document.getElementById('listingLocationMap')) {
            const lat = document.getElementById('listingLat')?.value?.trim();
            const lng = document.getElementById('listingLng')?.value?.trim();
            if (!lat || !lng) {
                const le = document.getElementById('locationMapError');
                if (le) le.textContent = 'Укажите точку на карте или нажмите «Найти по адресу».';
                isValid = false;
            }
        }

        const price = document.getElementById('price')?.value;
        if (!price) {
            showError('price', 'Введите цену');
            isValid = false;
        } else if (parseFloat(price) < 1) {
            showError('price', 'Цена должна быть больше 0');
            isValid = false;
        }

        if (currentType === 'rent') {
            const availableFrom = document.getElementById('availableFrom')?.value;
            if (!availableFrom) {
                showError('availableFrom', 'Выберите дату, с которой доступно жилье');
                isValid = false;
            }
        }

        const descriptionText = document.getElementById('description')?.value.trim();
        if (!descriptionText) {
            showError('description', 'Введите описание объявления');
            isValid = false;
        } else if (descriptionText.length < 20) {
            showError('description', 'Описание должно содержать минимум 20 символов');
            isValid = false;
        } else if (descriptionText.length > 5000) {
            showError('description', 'Описание не длиннее 1000 символов');
            isValid = false;
        }

        if (!editListingId) {
            const contactName = document.getElementById('contactName')?.value.trim();
            if (!contactName) {
                showError('contactName', 'Введите контактное лицо');
                isValid = false;
            }
            const contactPhone = document.getElementById('contactPhone')?.value.trim();
            if (!contactPhone) {
                showError('contactPhone', 'Введите номер телефона');
                isValid = false;
            } else if (contactPhone.length < 7) {
                showError('contactPhone', 'Введите корректный номер телефона');
                isValid = false;
            }
        }

        const visibleExistingCount = existingPhotos.filter(url => !photosToDelete.includes(url)).length;
        if (selectedFiles.length + visibleExistingCount < 1) {
            showError('photos', 'Нужна хотя бы одна фотография');
            isValid = false;
        }

        const agreeTerms = document.getElementById('agreeTerms')?.checked;
        if (!editListingId && !agreeTerms) {
            showError('terms', 'Необходимо подтвердить право на сдачу/продажу');
            isValid = false;
        }

        return isValid;
    }

    if (form) {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            if (!validateForm()) {
                const firstError = document.querySelector('.input-error');
                if (firstError) {
                    firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }
                return;
            }

            submitBtn.disabled = true;
            submitBtn.innerHTML = '<i class="fas fa-spinner fa-pulse"></i> ' + (editListingId ? 'Сохранение...' : 'Публикация...');

            const formData = new FormData();

            formData.append('title', document.getElementById('listingTitle').value.trim());
            formData.append('property_type', document.getElementById('propertyType').value);
            formData.append('rooms', document.getElementById('rooms').value);
            formData.append('area', document.getElementById('area').value);
            formData.append('floor', propertyTypeSelect?.value === 'house' ? '0' : document.getElementById('floor').value);
            if (propertyTypeSelect?.value === 'house' && plotAreaInput?.value) {
                formData.append('plot_area', plotAreaInput.value);
            }
            formData.append('total_floors', document.getElementById('totalFloors').value);
            formData.append('address', document.getElementById('address').value.trim());
            formData.append('city', getResolvedCity());
            formData.append('district', document.getElementById('district').value.trim());
            formData.append('price', document.getElementById('price').value);
            formData.append('description', withPricePeriodMarker(document.getElementById('description').value.trim()));
            formData.append('listing_type', currentType);
            formData.append('contact_name', document.getElementById('contactName')?.value.trim() || '');
            formData.append('contact_phone', document.getElementById('contactPhone')?.value.trim() || '');

            if (currentType === 'rent') {
                formData.append('available_from', document.getElementById('availableFrom').value);
                formData.append('utilities_included', document.getElementById('utilitiesIncluded')?.checked ? '1' : '0');
            } else {
                formData.append('condition', document.getElementById('condition')?.value || '');
                formData.append('building_year', document.getElementById('buildingYear')?.value || '');
                formData.append('house_type', document.getElementById('houseType')?.value || '');
            }

            const amenities = [];
            if (document.getElementById('amenityParking')?.checked) amenities.push('parking');
            if (document.getElementById('amenityElevator')?.checked) amenities.push('elevator');
            if (document.getElementById('amenityFurniture')?.checked) amenities.push('furniture');
            if (document.getElementById('amenityChildren')?.checked) amenities.push('children');
            if (document.getElementById('amenityPets')?.checked) amenities.push('pets');
            if (document.getElementById('amenityWifi')?.checked) amenities.push('wifi');
            if (document.getElementById('amenityRenovation')?.checked) amenities.push('renovation');
            if (document.getElementById('amenityNewBuilding')?.checked) amenities.push('new_building');

            formData.append('amenities', JSON.stringify(amenities));

            const latv = document.getElementById('listingLat')?.value?.trim();
            const lngv = document.getElementById('listingLng')?.value?.trim();
            if (latv && lngv) {
                formData.append('latitude', latv);
                formData.append('longitude', lngv);
            }

            selectedFiles.forEach(file => {
                formData.append('photos', file);
            });

            if (photosToDelete.length > 0) {
                formData.append('delete_photos', JSON.stringify(photosToDelete));
            }

            try {
                const url = editListingId ? `/api/listings/${editListingId}` : '/api/listings';
                const method = editListingId ? 'PUT' : 'POST';
                const response = await fetch(url, {
                    method,
                    headers: {
                        'X-User-ID': localStorage.getItem('userId')
                    },
                    body: formData
                });

                const result = await response.json().catch(() => ({}));
                if (response.ok) {
                    let text;
                    if (editListingId) {
                        text = 'Изменения сохранены. Объявление снова отправлено на модерацию.';
                    } else {
                        text =
                            result.moderation_status === 'pending'
                                ? 'Объявление отправлено на модерацию. После проверки администратором оно появится в каталоге.'
                                : (result.message || 'Объявление сохранено.');
                    }
                    showSuccess(text);
                    setTimeout(() => {
                        window.location.href = '/profile.html';
                    }, 2600);
                } else {
                    throw new Error(result.error || 'Ошибка при публикации');
                }
            } catch (err) {
                const messagesDiv = document.getElementById('formMessages');
                if (messagesDiv) {
                    messagesDiv.innerHTML = `<div class="error-message" style="padding:12px;border-radius:8px;background:#fee2e2;color:#991b1b;">${(err.message || 'Ошибка').replace(/</g, '&lt;')}</div>`;
                }
                submitBtn.disabled = false;
                submitBtn.innerHTML = editListingId
                    ? '<i class="fas fa-save"></i> Сохранить изменения'
                    : '<i class="fas fa-paper-plane"></i> Опубликовать';
            }
        });
    }
}