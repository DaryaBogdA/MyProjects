package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.City;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;
import java.util.Optional;

public interface CityRepository extends JpaRepository<City, Integer> {
    Optional<City> findByName(String name);

    List<City> findByCountry_Id(Integer countryId);

    Optional<City> findByNameAndCountry_Id(String name, Integer countryId);
}