package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.BookTourRequestDto;
import com.tours.bogdanovich.dto.BookingDetailDto;
import com.tours.bogdanovich.dto.BookingDto;
import com.tours.bogdanovich.service.BookingDocumentService;
import com.tours.bogdanovich.service.BookingService;
import org.springframework.http.ContentDisposition;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;

import java.nio.charset.StandardCharsets;
import java.util.List;

@RestController
@RequestMapping("/bookings")
public class BookingController {

    private final BookingService bookingService;
    private final BookingDocumentService bookingDocumentService;

    public BookingController(BookingService bookingService,
                             BookingDocumentService bookingDocumentService) {
        this.bookingService = bookingService;
        this.bookingDocumentService = bookingDocumentService;
    }

    @PostMapping("/tours/{tourId}")
    public ResponseEntity<BookingDto> bookTour(@PathVariable Integer tourId,
                                               @RequestBody(required = false) BookTourRequestDto request,
                                               Authentication authentication) {
        var travelDate = request != null ? request.getTravelDate() : null;
        return ResponseEntity.ok(bookingService.bookTour(tourId, authentication.getName(), travelDate));
    }

    @PostMapping("/trips/{tripId}")
    public ResponseEntity<BookingDto> bookTrip(@PathVariable Integer tripId, Authentication authentication) {
        return ResponseEntity.ok(bookingService.bookTrip(tripId, authentication.getName()));
    }

    @GetMapping("/me")
    public ResponseEntity<List<BookingDto>> myBookings(Authentication authentication) {
        return ResponseEntity.ok(bookingService.listMyBookings(authentication.getName()));
    }

    @GetMapping("/{id}")
    public ResponseEntity<BookingDetailDto> getBooking(@PathVariable Integer id, Authentication authentication) {
        return ResponseEntity.ok(bookingService.getMyBookingDetail(id, authentication.getName()));
    }

    @PostMapping("/{id}/cancel")
    public ResponseEntity<BookingDto> cancel(@PathVariable Integer id, Authentication authentication) {
        return ResponseEntity.ok(bookingService.cancelBooking(id, authentication.getName()));
    }

    @GetMapping("/{id}/document")
    public ResponseEntity<byte[]> downloadDocument(@PathVariable Integer id, Authentication authentication) {
        byte[] body = bookingDocumentService.buildDocument(id, authentication.getName());
        String filename = "travelcanvas-booking-" + id + ".txt";
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(new MediaType("text", "plain", StandardCharsets.UTF_8));
        headers.setContentDisposition(ContentDisposition.attachment()
                .filename(filename, StandardCharsets.UTF_8)
                .build());
        return ResponseEntity.ok().headers(headers).body(body);
    }
}
