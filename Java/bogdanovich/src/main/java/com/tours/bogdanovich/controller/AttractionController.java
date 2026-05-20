package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.AttractionResponceDto;
import com.tours.bogdanovich.entity.Attraction;
import com.tours.bogdanovich.repository.AttractionRepository;
import com.tours.bogdanovich.util.MediaUrlUtil;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/attractions")
public class AttractionController {

    private final AttractionRepository attractionRepository;

    public AttractionController(AttractionRepository attractionRepository) {
        this.attractionRepository = attractionRepository;
    }

    @GetMapping
    public ResponseEntity<List<AttractionResponceDto>> getAllAttractions() {
        List<Attraction> attractions = attractionRepository.findAll();
        List<AttractionResponceDto> dtos = attractions.stream().map(this::toDto).toList();
        return ResponseEntity.ok(dtos);
    }

    @GetMapping("/byCity/{cityId}")
    public ResponseEntity<List<AttractionResponceDto>> getByCity(@PathVariable Integer cityId) {
        List<Attraction> attractions = attractionRepository.findByCityId(cityId);
        List<AttractionResponceDto> dtos = attractions.stream().map(this::toDto).toList();
        return ResponseEntity.ok(dtos);
    }

    private AttractionResponceDto toDto(Attraction a) {
        AttractionResponceDto dto = new AttractionResponceDto();
        dto.setId(a.getId());
        dto.setCityId(a.getCity() != null ? a.getCity().getId() : null);
        dto.setName(a.getName());
        dto.setPopularity(a.getPopularity());
        dto.setDescription(a.getDescription());
        dto.setPhoto_url(MediaUrlUtil.normalizePhotoUrl(a.getPhoto_url()));
        return dto;
    }
}

