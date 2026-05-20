package com.tours.bogdanovich.service;

import com.tours.bogdanovich.dto.CreateTripRequestDto;
import com.tours.bogdanovich.dto.TripItemRequestDto;
import com.tours.bogdanovich.dto.TripResponseDto;
import com.tours.bogdanovich.dto.TripSummaryDto;
import com.tours.bogdanovich.entity.*;
import com.tours.bogdanovich.repository.*;
import com.tours.bogdanovich.util.DateValidationUtil;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.util.ArrayList;
import java.util.List;

@Service
public class TripService {

    private final TripRepository tripRepository;
    private final TripItemRepository tripItemRepository;
    private final CountriesRepository countryRepository;
    private final CityRepository cityRepository;
    private final UserService userService;
    private final PricingService pricingService;

    public TripService(TripRepository tripRepository,
                       TripItemRepository tripItemRepository,
                       CountriesRepository countryRepository,
                       CityRepository cityRepository,
                       UserService userService,
                       PricingService pricingService) {
        this.tripRepository = tripRepository;
        this.tripItemRepository = tripItemRepository;
        this.countryRepository = countryRepository;
        this.cityRepository = cityRepository;
        this.userService = userService;
        this.pricingService = pricingService;
    }

    @Transactional
    public TripResponseDto createTrip(CreateTripRequestDto request, String userEmail) {
        Users user = userService.loadUser(userEmail);
        Trip trip = buildTrip(request, user);
        Trip saved = tripRepository.save(trip);

        BigDecimal total = saveItems(saved, request.getItems());
        TripResponseDto response = new TripResponseDto();
        response.setTripId(saved.getId());
        response.setTotalPrice(total);
        return response;
    }

    @Transactional
    public void updateTrip(Integer tripId, CreateTripRequestDto request, String userEmail) {
        Trip trip = loadOwnedTrip(tripId, userEmail);
        applyTripFields(trip, request);

        trip.getItems().clear();
        tripRepository.saveAndFlush(trip);
        saveItems(trip, request.getItems());
        tripRepository.save(trip);
    }

    @Transactional(readOnly = true)
    public List<TripSummaryDto> listMySummaries(String userEmail) {
        Users user = userService.loadUser(userEmail);
        return tripRepository.findByUser_Id(user.getId()).stream().map(this::toSummary).toList();
    }

    public BigDecimal calculateTotal(List<TripItemRequestDto> items) {
        if (items == null || items.isEmpty()) {
            return BigDecimal.ZERO;
        }
        BigDecimal total = BigDecimal.ZERO;
        for (TripItemRequestDto itemReq : items) {
            ItemType type = ItemType.valueOf(itemReq.getItemType());
            int qty = itemReq.getQuantity() != null ? itemReq.getQuantity() : 1;
            BigDecimal unit = pricingService.getPricePerUnit(type, itemReq.getItemId());
            total = total.add(unit.multiply(BigDecimal.valueOf(qty)));
        }
        return total.setScale(2, RoundingMode.HALF_UP);
    }

    private Trip buildTrip(CreateTripRequestDto request, Users user) {
        Trip trip = new Trip();
        trip.setUser(user);
        applyTripFields(trip, request);
        return trip;
    }

    private void applyTripFields(Trip trip, CreateTripRequestDto request) {
        DateValidationUtil.requireTripDates(request.getDateFrom(), request.getDateTo());
        trip.setDateFrom(request.getDateFrom());
        trip.setDateTo(request.getDateTo());
        trip.setPeopleCount(request.getPeopleCount() != null ? request.getPeopleCount() : 1);
        trip.setTitle(request.getTitle());

        if (request.getCountryId() != null) {
            Country country = countryRepository.findById(request.getCountryId())
                    .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Country not found"));
            trip.setCountry(country);
        } else {
            trip.setCountry(null);
        }

        if (request.getCityId() != null) {
            City city = cityRepository.findById(request.getCityId())
                    .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "City not found"));
            trip.setCity(city);
        } else {
            trip.setCity(null);
        }
    }

    private BigDecimal saveItems(Trip trip, List<TripItemRequestDto> itemReqs) {
        if (itemReqs == null || itemReqs.isEmpty()) {
            return BigDecimal.ZERO;
        }
        BigDecimal total = BigDecimal.ZERO;
        List<TripItem> items = new ArrayList<>();
        for (TripItemRequestDto itemReq : itemReqs) {
            ItemType type = ItemType.valueOf(itemReq.getItemType());
            int qty = itemReq.getQuantity() != null ? itemReq.getQuantity() : 1;
            TripItem item = new TripItem();
            item.setTrip(trip);
            item.setItemType(type);
            item.setItemId(itemReq.getItemId());
            item.setQuantity(qty);
            items.add(item);
            total = total.add(pricingService.getPricePerUnit(type, itemReq.getItemId()).multiply(BigDecimal.valueOf(qty)));
        }
        tripItemRepository.saveAll(items);
        return total.setScale(2, RoundingMode.HALF_UP);
    }

    private Trip loadOwnedTrip(Integer tripId, String userEmail) {
        Users user = userService.loadUser(userEmail);
        Trip trip = tripRepository.findById(tripId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Trip not found"));
        if (trip.getUser() == null || trip.getUser().getId() != user.getId()) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN, "Not your trip");
        }
        return trip;
    }

    private TripSummaryDto toSummary(Trip trip) {
        trip.getItems().size();
        TripSummaryDto dto = new TripSummaryDto();
        dto.setId(trip.getId());
        dto.setTitle(trip.getTitle());
        dto.setCountryName(trip.getCountry() != null ? trip.getCountry().getName() : null);
        dto.setCityName(trip.getCity() != null ? trip.getCity().getName() : null);
        dto.setDateFrom(trip.getDateFrom());
        dto.setDateTo(trip.getDateTo());
        dto.setPeopleCount(trip.getPeopleCount());
        dto.setTotalPrice(calculateTotalFromTrip(trip));
        return dto;
    }

    private BigDecimal calculateTotalFromTrip(Trip trip) {
        BigDecimal total = BigDecimal.ZERO;
        for (TripItem it : trip.getItems()) {
            int q = it.getQuantity() != null ? it.getQuantity() : 1;
            total = total.add(pricingService.getPricePerUnit(it.getItemType(), it.getItemId()).multiply(BigDecimal.valueOf(q)));
        }
        return total.setScale(2, RoundingMode.HALF_UP);
    }
}
