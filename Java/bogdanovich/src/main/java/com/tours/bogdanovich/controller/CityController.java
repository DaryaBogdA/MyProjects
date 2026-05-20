package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.CityDto;
import com.tours.bogdanovich.entity.City;
import com.tours.bogdanovich.repository.CityRepository;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/cities")
public class CityController {

    private final CityRepository cityRepository;

    public CityController(CityRepository cityRepository) {
        this.cityRepository = cityRepository;
    }

    @GetMapping
    public ResponseEntity<List<CityDto>> getAllCities() {
        List<City> cities = cityRepository.findAll();
        List<CityDto> dtos = cities.stream().map(this::toDto).collect(Collectors.toList());
        return ResponseEntity.ok(dtos);
    }

    @GetMapping("/search")
    public ResponseEntity<CityDto> getCityByName(@RequestParam String name) {
        City city = cityRepository.findByName(name)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "City not found"));
        return ResponseEntity.ok(toDto(city));
    }

    @GetMapping("/byCountry/{countryId}")
    public ResponseEntity<List<CityDto>> getByCountry(@PathVariable Integer countryId) {
        return ResponseEntity.ok(cityRepository.findByCountry_Id(countryId).stream().map(this::toDto).toList());
    }

    private CityDto toDto(City city) {
        CityDto dto = new CityDto();
        dto.setId(city.getId());
        dto.setName(city.getName());
        if (city.getCountry() != null) {
            dto.setCountryId(city.getCountry().getId());
            dto.setCountryName(city.getCountry().getName());
        }
        return dto;
    }
}