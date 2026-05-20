package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.Hotel;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface HotelRepository extends JpaRepository<Hotel, Integer> {
    List<Hotel> findByCityId(Integer cityId);
}