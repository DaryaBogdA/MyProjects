package com.tours.bogdanovich.service;

import com.tours.bogdanovich.dto.BookingDetailDto;
import com.tours.bogdanovich.dto.BookingDto;
import com.tours.bogdanovich.entity.*;
import com.tours.bogdanovich.repository.BookingRepository;
import com.tours.bogdanovich.repository.TourRepository;
import com.tours.bogdanovich.repository.TripRepository;
import com.tours.bogdanovich.util.DateValidationUtil;
import com.tours.bogdanovich.util.MediaUrlUtil;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

@Service
public class BookingService {

    private final BookingRepository bookingRepository;
    private final TourRepository tourRepository;
    private final TripRepository tripRepository;
    private final UserService userService;
    private final PricingService pricingService;
    private final NotificationService notificationService;

    public BookingService(BookingRepository bookingRepository,
                          TourRepository tourRepository,
                          TripRepository tripRepository,
                          UserService userService,
                          PricingService pricingService,
                          NotificationService notificationService) {
        this.bookingRepository = bookingRepository;
        this.tourRepository = tourRepository;
        this.tripRepository = tripRepository;
        this.userService = userService;
        this.pricingService = pricingService;
        this.notificationService = notificationService;
    }

    @Transactional
    public BookingDto bookTour(Integer tourId, String userEmail, LocalDate travelDate) {
        DateValidationUtil.requireTravelDate(travelDate);
        Users user = userService.loadUser(userEmail);
        Tour tour = tourRepository.findById(tourId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Tour not found"));
        if (!tour.isPublished()) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Тур недоступен для бронирования");
        }

        Booking booking = new Booking();
        booking.setUser(user);
        booking.setTour(tour);
        booking.setTrip(null);
        booking.setTravelDate(travelDate);
        booking.setStatus(BookingStatus.PENDING);
        booking.setTotalPrice(tour.getPriceTotal() != null ? tour.getPriceTotal() : BigDecimal.ZERO);
        return toDto(bookingRepository.save(booking));
    }

    @Transactional
    public BookingDto bookTrip(Integer tripId, String userEmail) {
        Users user = userService.loadUser(userEmail);
        Trip trip = tripRepository.findById(tripId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Trip not found"));
        if (trip.getUser() == null || trip.getUser().getId() != user.getId()) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN, "Not your trip");
        }
        DateValidationUtil.requireTripDates(trip.getDateFrom(), trip.getDateTo());
        trip.getItems().size();

        BigDecimal total = BigDecimal.ZERO;
        for (TripItem it : trip.getItems()) {
            int q = it.getQuantity() != null ? it.getQuantity() : 1;
            total = total.add(pricingService.getPricePerUnit(it.getItemType(), it.getItemId()).multiply(BigDecimal.valueOf(q)));
        }

        Booking booking = new Booking();
        booking.setUser(user);
        booking.setTrip(trip);
        booking.setTour(null);
        booking.setStatus(BookingStatus.PENDING);
        booking.setTotalPrice(total.setScale(2, RoundingMode.HALF_UP));
        return toDto(bookingRepository.save(booking));
    }

    @Transactional
    public BookingDto cancelBooking(Integer bookingId, String userEmail) {
        Users user = userService.loadUser(userEmail);
        Booking booking = bookingRepository.findById(bookingId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Booking not found"));
        if (booking.getUser() == null || booking.getUser().getId() != user.getId()) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN, "Not your booking");
        }
        if (booking.getStatus() != BookingStatus.CANCELLED) {
            booking.setStatus(BookingStatus.CANCELLED);
            booking = bookingRepository.save(booking);
        }
        return toDto(booking);
    }

    @Transactional(readOnly = true)
    public List<BookingDto> listMyBookings(String userEmail) {
        Users user = userService.loadUser(userEmail);
        return bookingRepository.findByUser_IdOrderByIdDesc(user.getId()).stream().map(this::toDto).toList();
    }

    @Transactional(readOnly = true)
    public BookingDetailDto getMyBookingDetail(Integer bookingId, String userEmail) {
        Users user = userService.loadUser(userEmail);
        Booking booking = bookingRepository.findByIdAndUser_Id(bookingId, user.getId())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Booking not found"));
        return toDetailDto(booking);
    }

    @Transactional(readOnly = true)
    public List<BookingDto> listPendingBookings() {
        return bookingRepository.findByStatusOrderByIdDesc(BookingStatus.PENDING).stream().map(this::toAdminDto).toList();
    }

    @Transactional(readOnly = true)
    public BookingDetailDto getBookingDetailForAdmin(Integer bookingId) {
        Booking booking = bookingRepository.findById(bookingId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Booking not found"));
        BookingDetailDto dto = toDetailDto(booking);
        if (booking.getUser() != null) {
            dto.setUserEmail(booking.getUser().getEmail());
        }
        return dto;
    }

    @Transactional
    public BookingDto approveBooking(Integer bookingId) {
        Booking booking = bookingRepository.findById(bookingId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Booking not found"));
        if (booking.getStatus() != BookingStatus.PENDING) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Заявка уже обработана");
        }
        booking.setStatus(BookingStatus.CONFIRMED);
        Booking saved = bookingRepository.save(booking);

        notificationService.sendBookingApprovedEmail(saved.getUser().getEmail(), saved.getId());

        return toAdminDto(saved);
    }

    @Transactional
    public BookingDto rejectBooking(Integer bookingId) {
        Booking booking = bookingRepository.findById(bookingId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Booking not found"));
        if (booking.getStatus() != BookingStatus.PENDING) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Заявка уже обработана");
        }
        booking.setStatus(BookingStatus.CANCELLED);
        Booking saved = bookingRepository.save(booking);
        notificationService.sendBookingRejectedEmail(saved.getUser().getEmail(), saved.getId());
        return toAdminDto(saved);
    }

    private BookingDto toDto(Booking booking) {
        BookingDto dto = new BookingDto();
        dto.setId(booking.getId());
        dto.setStatus(booking.getStatus() != null ? booking.getStatus().name() : null);
        dto.setTotalPrice(booking.getTotalPrice());
        dto.setTravelDate(booking.getTravelDate());
        dto.setTripId(booking.getTrip() != null ? booking.getTrip().getId() : null);
        dto.setTourId(booking.getTour() != null ? booking.getTour().getId() : null);
        dto.setTourName(booking.getTour() != null ? booking.getTour().getName() : null);
        if (booking.getTrip() != null) {
            dto.setTripTitle(booking.getTrip().getTitle());
        }
        return dto;
    }

    private BookingDto toAdminDto(Booking booking) {
        BookingDto dto = toDto(booking);
        if (booking.getUser() != null) {
            dto.setUserEmail(booking.getUser().getEmail());
        }
        return dto;
    }

    private BookingDetailDto toDetailDto(Booking booking) {
        BookingDetailDto dto = new BookingDetailDto();
        dto.setId(booking.getId());
        dto.setStatus(booking.getStatus() != null ? booking.getStatus().name() : null);
        dto.setTotalPrice(booking.getTotalPrice());

        dto.setTravelDate(booking.getTravelDate());
        if (booking.getTour() != null) {
            dto.setKind("TOUR");
            Tour tour = booking.getTour();
            dto.setTourId(tour.getId());
            dto.setTourName(tour.getName());
            dto.setTourDescription(tour.getDescription());
            dto.setTourPhotoUrl(MediaUrlUtil.normalizePhotoUrl(tour.getPhotoUrl()));
            if (booking.getTravelDate() != null) {
                dto.setDateFrom(booking.getTravelDate());
                if (tour.getDurationDays() != null && tour.getDurationDays() > 0) {
                    dto.setDateTo(booking.getTravelDate().plusDays(tour.getDurationDays() - 1L));
                }
            }
            if (tour.getCity() != null) {
                dto.setCityName(tour.getCity().getName());
                if (tour.getCity().getCountry() != null) {
                    dto.setCountryName(tour.getCity().getCountry().getName());
                }
            }
        } else if (booking.getTrip() != null) {
            dto.setKind("TRIP");
            Trip trip = booking.getTrip();
            dto.setTripId(trip.getId());
            dto.setTripTitle(trip.getTitle());
            dto.setDateFrom(trip.getDateFrom());
            dto.setDateTo(trip.getDateTo());
            dto.setPeopleCount(trip.getPeopleCount());
            if (trip.getCountry() != null) {
                dto.setCountryName(trip.getCountry().getName());
            }
            if (trip.getCity() != null) {
                dto.setCityName(trip.getCity().getName());
            }
            List<String> items = new ArrayList<>();
            for (TripItem it : trip.getItems()) {
                int q = it.getQuantity() != null ? it.getQuantity() : 1;
                items.add(pricingService.describeItem(it.getItemType(), it.getItemId()) + " × " + q);
            }
            dto.setTripItems(items);
        }
        return dto;
    }
}
