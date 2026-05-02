

document.addEventListener('DOMContentLoaded', function() {
    initReviewsPage();
});

function initReviewsPage() {
    
    const tabs = document.querySelectorAll('.reviews-tab');
    const reviewsContainer = document.querySelector('.reviews-container');
    
    
    const originalReview = reviewsContainer.innerHTML;
    
    
    tabs.forEach(tab => {
        tab.addEventListener('click', function() {
            
            tabs.forEach(t => t.classList.remove('active'));
            
            
            this.classList.add('active');
            
            
            const tabText = this.textContent.trim();
            
            
            if (tabText.includes('Все отзывы')) {
                showAllReviews(reviewsContainer, originalReview);
            } else if (tabText.includes('Высокий рейтинг')) {
                showHighRatedReviews(reviewsContainer, originalReview);
            }
        });
    });
}

function showAllReviews(container, originalReview) {
    
    container.innerHTML = originalReview;
}

function showHighRatedReviews(container, originalReview) {
    
    const temp = document.createElement('div');
    temp.innerHTML = originalReview;
    
    
    const reviewCard = temp.querySelector('.review-card');
    
    
    const ratingElement = reviewCard.querySelector('.rating-value');
    const rating = parseFloat(ratingElement.textContent);
    
    
    if (rating >= 4.5) {
        
        container.innerHTML = temp.innerHTML;
    } else {
        
        container.innerHTML = `
            <div class="empty-state">
                <i class="fas fa-star"></i>
                <h3>Нет отзывов с высоким рейтингом</h3>
                <p>У вас пока нет отзывов с рейтингом 4.5 и выше</p>
            </div>
        `;
    }
}