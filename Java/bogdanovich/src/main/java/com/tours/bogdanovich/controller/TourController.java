package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.TourDetailDto;
import com.tours.bogdanovich.dto.TourDto;
import com.tours.bogdanovich.service.TourService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/tours")
public class TourController {

    private final TourService tourService;

    public TourController(TourService tourService) {
        this.tourService = tourService;
    }

    @GetMapping
    public ResponseEntity<List<TourDto>> listTours() {
        return ResponseEntity.ok(tourService.listPublishedTours());
    }

    @GetMapping("/{id}")
    public ResponseEntity<TourDetailDto> getTour(@PathVariable Integer id) {
        return ResponseEntity.ok(tourService.getPublishedTourDetail(id));
    }
}
