

document.addEventListener('DOMContentLoaded', function() {
    
    const menuLinks = document.querySelectorAll('.main-nav a');
    menuLinks.forEach(link => {
        link.classList.remove('active');
        if (link.getAttribute('href') === 'sale.html') {
            link.classList.add('active');
        }
    });
    
    
    const resetFiltersBtn = document.getElementById('resetFiltersBtn');
    if (resetFiltersBtn) {
        resetFiltersBtn.addEventListener('click', function() {
            
            document.querySelectorAll('.filter-chip').forEach(chip => {
                chip.classList.remove('active');
            });
            document.querySelector('.filter-chip')?.classList.add('active');
            
            
            document.querySelectorAll('.room-btn').forEach(btn => {
                btn.classList.remove('active');
            });
            document.querySelector('.room-btn:nth-child(2)')?.classList.add('active');
            
            
            document.getElementById('min-price').value = '';
            document.getElementById('max-price').value = '';
            document.getElementById('min-area').value = '';
            document.getElementById('max-area').value = '';
            
            
            const regionSelect = document.getElementById('region-select');
            if (regionSelect) regionSelect.value = '';
            
            
            document.querySelectorAll('.filter-checkboxes input[type="checkbox"]').forEach(cb => {
                cb.checked = false;
            });
            
            
            document.querySelectorAll('.filter-group:has(label:contains("Этаж")) .filter-chip').forEach(chip => {
                chip.classList.remove('active');
            });
            
            
            document.querySelectorAll('.filter-group:has(label:contains("Тип дома")) .filter-chip').forEach(chip => {
                chip.classList.remove('active');
            });
        });
    }
    
    
    const checkPostAdAuth = function() {
        const postAdLinks = document.querySelectorAll('#postAdLink, .btn-primary:contains("Разместить объявление")');
        postAdLinks.forEach(link => {
            link.addEventListener('click', function(e) {
                
                if (typeof window.isUserLoggedIn === 'function' && !window.isUserLoggedIn()) {
                    e.preventDefault();
                    window.location.href = 'login.html';
                }
            });
        });
    };
    
    
    setTimeout(checkPostAdAuth, 100);
    
    
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
        applyFiltersBtn.addEventListener('click', function() {
            alert('Фильтры применены (демонстрация для раздела Продажа)');
        });
    }

    
    const searchBtn = document.querySelector('.search-btn');
    const searchInput = document.querySelector('.search-input');
    if (searchBtn && searchInput) {
        searchBtn.addEventListener('click', function() {
            const query = searchInput.value.trim();
            if (query) {
                alert(`Поиск: "${query}" (демонстрация)`);
            } else {
                alert('Введите запрос для поиска');
            }
        });

        searchInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                searchBtn.click();
            }
        });
    }

    
    const mapBtn = document.querySelector('.btn-map');
    if (mapBtn) {
        mapBtn.addEventListener('click', function() {
            alert('Здесь будет открываться карта с объявлениями о продаже.');
        });
    }

    
    const likeButtons = document.querySelectorAll('.btn-like');
    likeButtons.forEach(btn => {
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
});