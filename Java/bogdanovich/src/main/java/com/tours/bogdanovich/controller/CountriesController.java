package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.AttractionResponceDto;
import com.tours.bogdanovich.dto.CityResponseDto;
import com.tours.bogdanovich.dto.CountryResponseDto;
import com.tours.bogdanovich.entity.Attraction;
import com.tours.bogdanovich.entity.City;
import com.tours.bogdanovich.entity.Country;
import com.tours.bogdanovich.service.CountryService;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/countries")
public class CountriesController {

    private final CountryService countryService;

    public CountriesController(CountryService countryService) {
        this.countryService = countryService;
    }

    @GetMapping
    public ResponseEntity<Page<CountryResponseDto>> getAllCountries(Pageable pageable) {
        Page<Country> countries = countryService.getAllCountries(pageable);
        return ResponseEntity.ok(countries.map(this::toCountryDto));
    }

    @GetMapping("/{id}")
    public ResponseEntity<CountryResponseDto> getCountryById(@PathVariable Integer id) {
        Country country = countryService.getCountryById(id);
        return ResponseEntity.ok(toDtoWithCities(country));
    }

    private CountryResponseDto toCountryDto(Country country) {
        CountryResponseDto dto = new CountryResponseDto();
        dto.setId(country.getId());
        dto.setName(country.getName());
        dto.setPopularity(country.getPopularity());
        dto.setPath_url(country.getPath_url());
        dto.setDescription(country.getDescription());
        return dto;
    }

    private CountryResponseDto toDtoWithCities(Country country) {
        CountryResponseDto dto = toCountryDto(country);
        List<CityResponseDto> cities = country.getCities().stream()
                .map(this::cityToDto)
                .collect(Collectors.toList());
        dto.setCities(cities);
        return dto;
    }

    private CityResponseDto cityToDto(City city) {
        CityResponseDto dto = new CityResponseDto();
        dto.setId(city.getId());
        dto.setCountryId(city.getCountry().getId());
        dto.setName(city.getName());
        dto.setPopularity(city.getPopularity());
        dto.setDescription(city.getDescription());
        List<AttractionResponceDto> attractions = city.getAttractions().stream()
                .map(this::attractionToDto)
                .collect(Collectors.toList());
        dto.setAttractions(attractions);
        return dto;
    }

    private AttractionResponceDto attractionToDto(Attraction attraction) {
        AttractionResponceDto dto = new AttractionResponceDto();
        dto.setId(attraction.getId());
        dto.setCityId(attraction.getCity().getId());
        dto.setName(attraction.getName());
        dto.setPopularity(attraction.getPopularity());
        dto.setDescription(attraction.getDescription());
        dto.setPhoto_url(attraction.getPhoto_url());
        return dto;
    }
}
