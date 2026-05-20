package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.Attraction;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface AttractionRepository extends JpaRepository<Attraction, Integer> {
    List<Attraction> findByCityId(Integer cityId);

    List<Attraction> findByCity_Country_Id(Integer countryId);

    List<Attraction> findByCityIdIn(List<Integer> cityIds);
}
