package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.Country;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface CountriesRepository extends JpaRepository<Country, Integer> {
    Optional<Country> findByName(String name);
}
