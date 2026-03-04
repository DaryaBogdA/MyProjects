package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.Country;
import org.springframework.data.jpa.repository.JpaRepository;

public interface CountriesRepository extends JpaRepository<Country, Integer> {
}
