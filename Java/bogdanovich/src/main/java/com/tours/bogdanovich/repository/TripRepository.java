package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.Trip;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface TripRepository extends JpaRepository<Trip, Integer> {
    List<Trip> findByUser_Id(Integer userId);
}
