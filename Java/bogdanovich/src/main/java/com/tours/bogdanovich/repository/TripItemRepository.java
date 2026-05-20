package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.TripItem;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface TripItemRepository extends JpaRepository<TripItem, Integer> {
    List<TripItem> findByTripId(Integer tripId);
}
