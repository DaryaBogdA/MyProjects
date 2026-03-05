document.addEventListener('DOMContentLoaded', function() {
    displayEvents(events.filter(e => e.status === 'approved'));

    const sportFilter = document.getElementById('sportFilter');
    const cityFilter = document.getElementById('cityFilter');
    const priceRange = document.getElementById('priceRange');
    const dateFilter = document.getElementById('dateFilter');
    const searchInput = document.getElementById('searchInput');

    if (sportFilter) sportFilter.addEventListener('change', filterEvents);
    if (cityFilter) cityFilter.addEventListener('change', filterEvents);
    if (priceRange) priceRange.addEventListener('input', filterEvents);
    if (dateFilter) dateFilter.addEventListener('change', filterEvents);
    if (searchInput) searchInput.addEventListener('input', filterEvents);
});

function filterEvents() {
    const sport = document.getElementById('sportFilter').value;
    const city = document.getElementById('cityFilter').value;
    const maxPrice = parseInt(document.getElementById('priceRange').value);
    const date = document.getElementById('dateFilter').value;
    const searchTerm = document.getElementById('searchInput').value.toLowerCase();

    const filtered = events.filter(event => {
        if (event.status !== 'approved') return false;
        if (sport !== 'all' && event.sport !== sport) return false;
        if (city !== 'all' && event.city !== city) return false;
        if (event.price > maxPrice) return false;
        if (date && event.date !== date) return false;
        if (searchTerm) {
            const searchableText = `${event.price} ${event.title} ${event.sportName} ${event.location} ${event.cityName} ${event.description || ''}`.toLowerCase();
            if (!searchableText.includes(searchTerm)) return false;
        }
        return true;
    });

    displayEvents(filtered);
    updateMapMarkers(filtered);
}

function applyFilters() {
    filterEvents();
    showNotification('Фильтры применены', 'success');
}

function filterBySport(sport) {
    document.getElementById('sportFilter').value = sport;
    filterEvents();
}