package com.event.arena.repository;

import com.event.arena.entity.City;
import org.springframework.data.jpa.repository.JpaRepository;


public interface CityRepository extends JpaRepository<City, Long> {
}
