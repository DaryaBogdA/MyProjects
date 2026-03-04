package com.tours.bogdanovich.service;

import com.tours.bogdanovich.entity.Country;
import com.tours.bogdanovich.repository.CountriesRepository;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ResponseStatusException;

@Service
public class CountryService {
    private final CountriesRepository countriesRepository;

    public CountryService(CountriesRepository countriesRepository) {
        this.countriesRepository = countriesRepository;
    }

    public Page<Country> getAllCountries(Pageable pageable) {
        return countriesRepository.findAll(pageable);
    }

    public Country getCountryById(Integer id) {
        return countriesRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Country not found"));
    }
}
