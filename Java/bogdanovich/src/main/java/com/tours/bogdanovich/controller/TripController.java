package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.CreateTripRequestDto;
import com.tours.bogdanovich.dto.TripSummaryDto;
import com.tours.bogdanovich.dto.TripResponseDto;
import com.tours.bogdanovich.service.TripService;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/trips")
public class TripController {

    private final TripService tripService;

    public TripController(TripService tripService) {
        this.tripService = tripService;
    }

    @PostMapping
    public ResponseEntity<TripResponseDto> createTrip(@RequestBody CreateTripRequestDto request, Authentication authentication) {
        return ResponseEntity.ok(tripService.createTrip(request, authentication.getName()));
    }

    @PutMapping("/{id}")
    public ResponseEntity<Void> updateTrip(@PathVariable Integer id,
                                           @RequestBody CreateTripRequestDto request,
                                           Authentication authentication) {
        tripService.updateTrip(id, request, authentication.getName());
        return ResponseEntity.ok().build();
    }

    @GetMapping("/me")
    public ResponseEntity<List<TripSummaryDto>> getMyTrips(Authentication authentication) {
        return ResponseEntity.ok(tripService.listMySummaries(authentication.getName()));
    }
}
