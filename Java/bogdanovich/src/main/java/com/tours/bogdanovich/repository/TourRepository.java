package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.Tour;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface TourRepository extends JpaRepository<Tour, Integer> {

    Optional<Tour> findByName(String name);

    List<Tour> findByNameContainingIgnoreCase(String part);
    List<Tour> findByPublishedTrueOrderByPopularityDesc();
}
