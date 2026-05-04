function getHeaderHtml() {
    return `
        <div class="container header-container">
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
            <div class="user-actions" id="userActions"></div>
            <button class="mobile-menu-btn" id="mobileMenuBtn">
                <i class="fas fa-bars"></i>
            </button>
        </div>
    `;
}

function getFooterHtml() {
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
                        <li><a href="/index.html"><i class="fas fa-home"></i> Аренда</a></li>
                        <li><a href="/sale.html"><i class="fas fa-tag"></i> Продажа</a></li>
                        <li><a href="/news.html"><i class="fas fa-newspaper"></i> Новости</a></li>
                        <li><a href="/about.html"><i class="fas fa-info-circle"></i> О нас</a></li>
                    </ul>
                </div>
                <div class="footer-column">
                    <h4>Помощь</h4>
                    <ul>
                        <li><a href="/help-sell.html"><i class="fas fa-chart-line"></i> Как продать</a></li>
                        <li><a href="/help-buy.html"><i class="fas fa-shopping-cart"></i> Как купить</a></li>
                        <li><a href="/safety.html"><i class="fas fa-shield-alt"></i> Безопасность</a></li>
                        <li><a href="/support.html"><i class="fas fa-headset"></i> Поддержка</a></li>
                    </ul>
                </div>
                <div class="footer-column">
                    <h4>Контакты</h4>
                    <ul>
                        <li><i class="fas fa-phone"></i> +375 29 123-45-67</li>
                        <li><i class="fas fa-envelope"></i> info@rentradar.by</li>
                        <li><i class="fas fa-clock"></i> Пн-Пт: 9:00 - 21:00</li>
                    </ul>
                </div>
                <div class="footer-column">
                    <h4>Документы</h4>
                    <ul>
                        <li><a href="/privacy.html"><i class="fas fa-lock"></i> Политика конфиденциальности</a></li>
                        <li><a href="/terms.html"><i class="fas fa-file-contract"></i> Пользовательское соглашение</a></li>
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
        </div>
    `;
}

function initComponents() {
    const headerContainer = document.querySelector('header.header .header-container');
    if (headerContainer && headerContainer.children.length === 0) {
        headerContainer.innerHTML = getHeaderHtml();
    }

    const footer = document.querySelector('footer.footer');
    if (footer && footer.children.length === 0) {
        footer.innerHTML = getFooterHtml();
    }

    initMobileMenu();
}

function initMobileMenu() {
    const mobileMenuBtn = document.getElementById('mobileMenuBtn');
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