

document.addEventListener('DOMContentLoaded', function() {
    initCreateListing();
});

function initCreateListing() {
    
    if (typeof isUserLoggedIn === 'function' && !isUserLoggedIn()) {
        window.location.href = 'login.html';
        return;
    }

    
    const rentBtn = document.getElementById('rentTypeBtn');
    const saleBtn = document.getElementById('saleTypeBtn');
    
    
    const rentOnlyFields = document.querySelectorAll('.rent-only');
    const saleOnlyFields = document.querySelectorAll('.sale-only');
    const saleOnlyOptions = document.querySelectorAll('.sale-only-option');
    
    
    const pricePeriodLabel = document.getElementById('pricePeriodLabel');
    const priceLabel = document.getElementById('priceLabel');
    const utilitiesText = document.getElementById('utilitiesText');
    
    
    let currentType = 'rent';
    
    
    function updateFormForType(type) {
        if (type === 'rent') {
            
            
            saleOnlyFields.forEach(field => field.classList.add('hidden'));
            saleOnlyOptions.forEach(option => option.classList.add('hidden'));
            
            
            rentOnlyFields.forEach(field => field.classList.remove('hidden'));
            
            
            if (pricePeriodLabel) pricePeriodLabel.textContent = '(за месяц)';
            if (priceLabel) priceLabel.textContent = 'Цена за месяц (BYN) *';
            if (utilitiesText) utilitiesText.textContent = 'Включены в стоимость';
            
            
            document.getElementById('houseType')?.removeAttribute('required');
            document.getElementById('condition')?.removeAttribute('required');
            document.getElementById('buildingYear')?.removeAttribute('required');
            
            
            document.getElementById('availableFrom')?.setAttribute('required', 'required');
            
        } else {
            
            
            rentOnlyFields.forEach(field => field.classList.add('hidden'));
            
            
            saleOnlyFields.forEach(field => field.classList.remove('hidden'));
            saleOnlyOptions.forEach(option => option.classList.remove('hidden'));
            
            
            if (pricePeriodLabel) pricePeriodLabel.textContent = '(за всё)';
            if (priceLabel) priceLabel.textContent = 'Цена (BYN) *';
            if (utilitiesText) utilitiesText.textContent = 'Коммунальные платежи не включены';
            
            
            document.getElementById('availableFrom')?.removeAttribute('required');
            
            
            
        }
        
        
        rentBtn.classList.toggle('active', type === 'rent');
        saleBtn.classList.toggle('active', type === 'sale');
    }
    
    
    if (rentBtn && saleBtn) {
        rentBtn.addEventListener('click', () => {
            currentType = 'rent';
            updateFormForType('rent');
        });
        
        saleBtn.addEventListener('click', () => {
            currentType = 'sale';
            updateFormForType('sale');
        });
        
        
        updateFormForType('rent');
    }
    
    
    const photoInput = document.getElementById('photoInput');
    const selectPhotosBtn = document.getElementById('selectPhotosBtn');
    const photoPreview = document.getElementById('photoPreview');
    
    if (selectPhotosBtn && photoInput) {
        selectPhotosBtn.addEventListener('click', () => photoInput.click());
        
        photoInput.addEventListener('change', function(e) {
            const files = Array.from(e.target.files);
            photoPreview.innerHTML = '';
            
            files.forEach((file, index) => {
                const reader = new FileReader();
                reader.onload = function(e) {
                    const previewItem = document.createElement('div');
                    previewItem.className = 'photo-preview-item';
                    previewItem.innerHTML = `
                        <img src="${e.target.result}" alt="Фото">
                        <button type="button" class="photo-remove">&times;</button>
                        ${index === 0 ? '<span class="photo-cover">Обложка</span>' : ''}
                    `;
                    photoPreview.appendChild(previewItem);
                    
                    previewItem.querySelector('.photo-remove').addEventListener('click', () => {
                        previewItem.remove();
                    });
                };
                reader.readAsDataURL(file);
            });
        });
    }
    
    
    const description = document.getElementById('description');
    const counter = document.querySelector('.textarea-counter');
    
    if (description && counter) {
        description.addEventListener('input', function() {
            const len = this.value.length;
            counter.textContent = `${len}/1000`;
            if (len > 1000) {
                this.value = this.value.substring(0, 1000);
                counter.textContent = '1000/1000';
            }
        });
    }
    
    
    const form = document.getElementById('createListingForm');
    if (form) {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            
            if (!document.getElementById('agreeTerms').checked) {
                alert('Необходимо подтвердить право на сдачу/продажу');
                return;
            }
            
            const type = currentType === 'rent' ? 'аренду' : 'продажу';
            alert(`Демо: объявление на ${type} опубликовано!`);
            window.location.href = 'index.html';
        });
    }
}