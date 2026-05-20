package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.Excursion;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface ExcursionRepository extends JpaRepository<Excursion, Integer> {
    List<Excursion> findByCityId(Integer cityId);

    List<Excursion> findByCity_Country_Id(Integer countryId);

    List<Excursion> findByCityIdIn(List<Integer> cityIds);
}