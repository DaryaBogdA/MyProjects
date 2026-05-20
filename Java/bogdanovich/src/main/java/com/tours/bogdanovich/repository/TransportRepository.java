package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.Transport;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface TransportRepository extends JpaRepository<Transport, Integer> {
    List<Transport> findByFromCityIdAndToCityId(Integer fromId, Integer toId);
}