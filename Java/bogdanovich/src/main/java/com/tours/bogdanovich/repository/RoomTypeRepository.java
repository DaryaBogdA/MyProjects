package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.RoomType;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface RoomTypeRepository extends JpaRepository<RoomType, Integer> {
    List<RoomType> findByHotelId(Integer hotelId);

    List<RoomType> findByHotel_City_Id(Integer cityId);
}
