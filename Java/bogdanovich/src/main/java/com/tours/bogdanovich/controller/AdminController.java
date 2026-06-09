package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.*;
import com.tours.bogdanovich.service.BookingService;
import com.tours.bogdanovich.service.StatisticsService;
import com.tours.bogdanovich.service.TourService;
import com.tours.bogdanovich.service.UserService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Map;

@RestController
@RequestMapping("/admin")
public class AdminController {

    private final TourService tourService;
    private final BookingService bookingService;
    private final StatisticsService statisticsService;

    public AdminController(TourService tourService,StatisticsService statisticsService, BookingService bookingService) {
        this.tourService = tourService;
        this.bookingService = bookingService;
        this.statisticsService = statisticsService;
    }

    @GetMapping("/tours")
    public ResponseEntity<List<TourDto>> listAllTours() {
        return ResponseEntity.ok(tourService.listAllTours());
    }

    @GetMapping("/tours/{id}")
    public ResponseEntity<TourDetailDto> getTour(@PathVariable Integer id) {
        return ResponseEntity.ok(tourService.getTourDetail(id));
    }
    @GetMapping("/stats/dashboard")
    public ResponseEntity<DashboardStatsDto> dashboardStats() {
        return ResponseEntity.ok(statisticsService.getDashboardStats());
    }
    @PostMapping("/tours")
    public ResponseEntity<TourDto> createTour(@RequestBody AdminTourRequestDto request) {
        return ResponseEntity.ok(tourService.createTour(request));
    }

    @PutMapping("/tours/{id}")
    public ResponseEntity<TourDto> updateTour(@PathVariable Integer id, @RequestBody AdminTourRequestDto request) {
        return ResponseEntity.ok(tourService.updateTour(id, request));
    }

    @PostMapping("/tours/{id}/publish")
    public ResponseEntity<TourDto> publish(@PathVariable Integer id) {
        return ResponseEntity.ok(tourService.setPublished(id, true));
    }

    @PostMapping("/tours/{id}/unpublish")
    public ResponseEntity<TourDto> unpublish(@PathVariable Integer id) {
        return ResponseEntity.ok(tourService.setPublished(id, false));
    }

    @DeleteMapping("/tours/{id}")
    public ResponseEntity<Void> deleteTour(@PathVariable Integer id) {
        tourService.deleteTour(id);
        return ResponseEntity.noContent().build();
    }

    @GetMapping("/bookings/pending")
    public ResponseEntity<List<BookingDto>> pendingBookings() {
        return ResponseEntity.ok(bookingService.listPendingBookings());
    }

    @GetMapping("/bookings/{id}")
    public ResponseEntity<BookingDetailDto> getBooking(@PathVariable Integer id) {
        return ResponseEntity.ok(bookingService.getBookingDetailForAdmin(id));
    }

    @PostMapping("/bookings/{id}/approve")
    public ResponseEntity<BookingDto> approve(@PathVariable Integer id) {
        return ResponseEntity.ok(bookingService.approveBooking(id));
    }

    @PostMapping("/bookings/{id}/reject")
    public ResponseEntity<BookingDto> reject(@PathVariable Integer id) {
        return ResponseEntity.ok(bookingService.rejectBooking(id));
    }

    @GetMapping("/stats")
    public ResponseEntity<Map<String, Long>> stats() {
        return ResponseEntity.ok(Map.of(
                "pendingBookings", (long) bookingService.listPendingBookings().size()
        ));
    }
}
