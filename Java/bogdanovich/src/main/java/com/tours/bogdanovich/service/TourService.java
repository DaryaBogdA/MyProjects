package com.tours.bogdanovich.service;

import com.tours.bogdanovich.dto.AdminTourRequestDto;
import com.tours.bogdanovich.dto.AttractionResponceDto;
import com.tours.bogdanovich.dto.ExcursionDto;
import com.tours.bogdanovich.dto.TourDetailDto;
import com.tours.bogdanovich.dto.TourDto;
import com.tours.bogdanovich.entity.*;
import com.tours.bogdanovich.repository.AttractionRepository;
import com.tours.bogdanovich.repository.CityRepository;
import com.tours.bogdanovich.repository.ExcursionRepository;
import com.tours.bogdanovich.repository.TourRepository;
import com.tours.bogdanovich.util.MediaUrlUtil;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

@Service
@Transactional(readOnly = true)
public class TourService {

    private final TourRepository tourRepository;
    private final CityRepository cityRepository;
    private final ExcursionRepository excursionRepository;
    private final AttractionRepository attractionRepository;

    public TourService(TourRepository tourRepository,
                       CityRepository cityRepository,
                       ExcursionRepository excursionRepository,
                       AttractionRepository attractionRepository) {
        this.tourRepository = tourRepository;
        this.cityRepository = cityRepository;
        this.excursionRepository = excursionRepository;
        this.attractionRepository = attractionRepository;
    }

    public List<TourDto> listPublishedTours() {
        return tourRepository.findByPublishedTrueOrderByPopularityDesc().stream()
                .map(this::toDto)
                .toList();
    }

    public TourDetailDto getPublishedTourDetail(Integer id) {
        Tour tour = tourRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Tour not found"));
        if (!tour.isPublished()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Tour not found");
        }
        return toDetailDto(tour);
    }

    public List<TourDto> listAllTours() {
        return tourRepository.findAll().stream().map(this::toDto).toList();
    }

    public TourDetailDto getTourDetail(Integer id) {
        Tour tour = tourRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Tour not found"));
        return toDetailDto(tour);
    }

    @Transactional
    public TourDto createTour(AdminTourRequestDto request) {
        if (request.getName() == null || request.getName().isBlank()) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Укажите название тура");
        }
        if (request.getPriceTotal() == null) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Укажите цену тура");
        }
        if (request.getCityId() == null) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Выберите страну и город");
        }
        Tour tour = applyRequest(new Tour(), request);
        return toDto(tourRepository.save(tour));
    }

    @Transactional
    public TourDto updateTour(Integer id, AdminTourRequestDto request) {
        Tour tour = tourRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Tour not found"));
        applyRequest(tour, request);
        return toDto(tourRepository.save(tour));
    }

    @Transactional
    public TourDto setPublished(Integer id, boolean published) {
        Tour tour = tourRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Tour not found"));
        tour.setPublished(published);
        return toDto(tourRepository.save(tour));
    }

    @Transactional
    public void deleteTour(Integer id) {
        if (!tourRepository.existsById(id)) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Tour not found");
        }
        tourRepository.deleteById(id);
    }

    private Tour applyRequest(Tour tour, AdminTourRequestDto request) {
        if (request.getName() != null) {
            tour.setName(request.getName());
        }
        if (request.getDescription() != null) {
            tour.setDescription(request.getDescription());
        }
        if (request.getType() != null) {
            tour.setType(TourType.valueOf(request.getType()));
        } else if (tour.getType() == null) {
            tour.setType(TourType.SINGLE_COUNTRY);
        }
        if (request.getDurationDays() != null) {
            tour.setDurationDays(request.getDurationDays());
        } else if (tour.getDurationDays() == null) {
            tour.setDurationDays(3);
        }
        if (request.getPriceTotal() != null) {
            tour.setPriceTotal(request.getPriceTotal());
        }
        if (request.getPhotoUrl() != null) {
            tour.setPhotoUrl(request.getPhotoUrl());
        }
        if (request.getPopularity() != null) {
            tour.setPopularity(request.getPopularity());
        }
        if (request.getProgramInfo() != null) {
            tour.setProgramInfo(request.getProgramInfo());
        }
        if (request.getCityId() != null) {
            City city = cityRepository.findById(request.getCityId())
                    .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "City not found"));
            tour.setCity(city);
        }
        if (request.getPublished() != null) {
            tour.setPublished(request.getPublished());
        }
        return tour;
    }

    private TourDetailDto toDetailDto(Tour tour) {
        TourDetailDto dto = new TourDetailDto();
        fillDetail(dto, tour);
        if (tour.getCity() != null) {
            dto.setCityId(tour.getCity().getId());
            dto.setCityName(tour.getCity().getName());
            if (tour.getCity().getCountry() != null) {
                dto.setCountryName(tour.getCity().getCountry().getName());
            }
            List<Integer> cityIds = resolveCatalogCityIds(tour);
            if (cityIds.size() == 1) {
                dto.setExcursions(excursionRepository.findByCityId(cityIds.get(0)).stream().map(this::toExcursionDto).toList());
                dto.setAttractions(attractionRepository.findByCityId(cityIds.get(0)).stream().map(this::toAttractionDto).toList());
            } else {
                dto.setExcursions(excursionRepository.findByCityIdIn(cityIds).stream().map(this::toExcursionDto).toList());
                dto.setAttractions(attractionRepository.findByCityIdIn(cityIds).stream().map(this::toAttractionDto).toList());
            }
        } else {
            dto.setExcursions(List.of());
            dto.setAttractions(List.of());
        }
        return dto;
    }

    private List<Integer> resolveCatalogCityIds(Tour tour) {
        City primary = tour.getCity();
        if (primary == null) {
            return List.of();
        }
        if (primary.getCountry() == null || !isMultiCityTourName(tour.getName())) {
            return List.of(primary.getId());
        }
        List<City> countryCities = cityRepository.findByCountry_Id(primary.getCountry().getId());
        List<City> matched = matchCitiesFromTourName(tour.getName(), countryCities);
        if (matched.isEmpty()) {
            return List.of(primary.getId());
        }
        return matched.stream().map(City::getId).toList();
    }

    private boolean isMultiCityTourName(String name) {
        if (name == null || name.isBlank()) {
            return false;
        }
        String n = name.toLowerCase(Locale.ROOT);
        return n.contains("+") || n.contains(" и ") || n.contains(",");
    }

    private List<City> matchCitiesFromTourName(String tourName, List<City> cities) {
        String lower = tourName.toLowerCase(Locale.ROOT);
        List<City> matched = new ArrayList<>();
        for (City city : cities) {
            String cityLower = city.getName().toLowerCase(Locale.ROOT);
            if (lower.contains(cityLower)) {
                matched.add(city);
            }
        }
        if (matched.size() >= 2) {
            return matched;
        }
        return List.of();
    }

    private void fillBase(TourDto dto, Tour tour) {
        dto.setId(tour.getId());
        dto.setName(tour.getName());
        dto.setDescription(tour.getDescription());
        dto.setType(tour.getType() != null ? tour.getType().name() : null);
        dto.setDurationDays(tour.getDurationDays());
        dto.setPriceTotal(tour.getPriceTotal());
        dto.setPhotoUrl(MediaUrlUtil.normalizePhotoUrl(tour.getPhotoUrl()));
        dto.setPopularity(tour.getPopularity());
        dto.setPublished(tour.isPublished());
    }

    private void fillDetail(TourDetailDto dto, Tour tour) {
        dto.setId(tour.getId());
        dto.setName(tour.getName());
        dto.setDescription(tour.getDescription());
        dto.setType(tour.getType() != null ? tour.getType().name() : null);
        dto.setDurationDays(tour.getDurationDays());
        dto.setPriceTotal(tour.getPriceTotal());
        dto.setPhotoUrl(MediaUrlUtil.normalizePhotoUrl(tour.getPhotoUrl()));
        dto.setPopularity(tour.getPopularity());
        dto.setProgramInfo(tour.getProgramInfo());
    }

    private TourDto toDto(Tour tour) {
        TourDto dto = new TourDto();
        fillBase(dto, tour);
        return dto;
    }

    private ExcursionDto toExcursionDto(Excursion e) {
        ExcursionDto dto = new ExcursionDto();
        dto.setId(e.getId());
        dto.setName(e.getName());
        dto.setDescription(e.getDescription());
        dto.setDurationHours(e.getDurationHours());
        dto.setMeetingPoint(e.getMeetingPoint());
        dto.setPrice(e.getPrice());
        dto.setPhotoUrl(MediaUrlUtil.normalizePhotoUrl(e.getPhotoUrl()));
        if (e.getCity() != null) {
            dto.setCityName(e.getCity().getName());
        }
        return dto;
    }

    private AttractionResponceDto toAttractionDto(Attraction a) {
        AttractionResponceDto dto = new AttractionResponceDto();
        dto.setId(a.getId());
        dto.setCityId(a.getCity() != null ? a.getCity().getId() : null);
        dto.setName(a.getName());
        dto.setPopularity(a.getPopularity());
        dto.setDescription(a.getDescription());
        dto.setPhoto_url(MediaUrlUtil.normalizePhotoUrl(a.getPhoto_url()));
        return dto;
    }
}
