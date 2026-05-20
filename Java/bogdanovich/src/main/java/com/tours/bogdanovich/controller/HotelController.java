package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.HotelDto;
import com.tours.bogdanovich.dto.RoomTypeDto;
import com.tours.bogdanovich.entity.Hotel;
import com.tours.bogdanovich.repository.HotelRepository;
import com.tours.bogdanovich.repository.RoomTypeRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/hotels")
public class HotelController {

    private final HotelRepository hotelRepository;
    private final RoomTypeRepository roomTypeRepository;

    public HotelController(HotelRepository hotelRepository, RoomTypeRepository roomTypeRepository) {
        this.hotelRepository = hotelRepository;
        this.roomTypeRepository = roomTypeRepository;
    }

    @GetMapping("/byCity/{cityId}")
    public ResponseEntity<List<HotelDto>> getHotelsByCity(@PathVariable Integer cityId) {
        List<Hotel> hotels = hotelRepository.findByCityId(cityId);
        List<HotelDto> dtos = hotels.stream().map(this::toDto).collect(Collectors.toList());
        return ResponseEntity.ok(dtos);
    }

    private HotelDto toDto(Hotel hotel) {
        HotelDto dto = new HotelDto();
        dto.setId(hotel.getId());
        dto.setName(hotel.getName());
        dto.setStars(hotel.getStars());
        dto.setPhotoUrl(hotel.getPhotoUrl());
        dto.setRoomTypes(roomTypeRepository.findByHotelId(hotel.getId()).stream()
                .map(rt -> new RoomTypeDto(rt.getId(), rt.getName(), rt.getPriceNight(), rt.getMaxOccupancy()))
                .collect(Collectors.toList()));
        return dto;
    }
}