package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.ExcursionDto;
import com.tours.bogdanovich.dto.ExcursionListDto;
import com.tours.bogdanovich.entity.Excursion;
import com.tours.bogdanovich.repository.ExcursionRepository;
import com.tours.bogdanovich.util.MediaUrlUtil;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/excursions")
public class ExcursionController {

    private final ExcursionRepository excursionRepository;

    public ExcursionController(ExcursionRepository excursionRepository) {
        this.excursionRepository = excursionRepository;
    }

    @GetMapping
    public ResponseEntity<List<ExcursionListDto>> getAllExcursions() {
        List<Excursion> excursions = excursionRepository.findAll();
        List<ExcursionListDto> dtos = excursions.stream().map(this::toListDto).collect(Collectors.toList());
        return ResponseEntity.ok(dtos);
    }

    @GetMapping("/byCity/{cityId}")
    public ResponseEntity<List<ExcursionDto>> getExcursionsByCity(@PathVariable Integer cityId) {
        List<Excursion> excursions = excursionRepository.findByCityId(cityId);
        List<ExcursionDto> dtos = excursions.stream().map(this::toDto).collect(Collectors.toList());
        return ResponseEntity.ok(dtos);
    }

    private ExcursionListDto toListDto(Excursion e) {
        ExcursionListDto dto = new ExcursionListDto();
        dto.setId(e.getId());
        if (e.getCity() != null) {
            dto.setCityId(e.getCity().getId());
            dto.setCityName(e.getCity().getName());
        }
        dto.setName(e.getName());
        dto.setDescription(e.getDescription());
        dto.setDurationHours(e.getDurationHours());
        dto.setMeetingPoint(e.getMeetingPoint());
        dto.setPrice(e.getPrice());
        dto.setPhotoUrl(e.getPhotoUrl());
        return dto;
    }

    private ExcursionDto toDto(Excursion e) {
        ExcursionDto dto = new ExcursionDto();
        dto.setId(e.getId());
        dto.setName(e.getName());
        dto.setDescription(e.getDescription());
        dto.setDurationHours(e.getDurationHours());
        dto.setMeetingPoint(e.getMeetingPoint());
        dto.setPrice(e.getPrice());
        dto.setPhotoUrl(MediaUrlUtil.normalizePhotoUrl(e.getPhotoUrl()));
        return dto;
    }
}