package com.tours.bogdanovich.service;

import com.tours.bogdanovich.dto.DashboardStatsDto;
import com.tours.bogdanovich.entity.BookingStatus;
import com.tours.bogdanovich.repository.*;
import org.springframework.stereotype.Service;
import java.math.BigDecimal;
import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.util.*;
import java.util.stream.Collectors;

@Service
public class StatisticsService {
    private final BookingRepository bookingRepository;
    private final UsersRepository usersRepository;
    private final TourRepository tourRepository;

    public StatisticsService(BookingRepository bookingRepository, UsersRepository usersRepository, TourRepository tourRepository) {
        this.bookingRepository = bookingRepository;
        this.usersRepository = usersRepository;
        this.tourRepository = tourRepository;
    }

    public DashboardStatsDto getDashboardStats() {
        DashboardStatsDto dto = new DashboardStatsDto();

        dto.setTotalUsers(usersRepository.count());

        long totalBookings = bookingRepository.count();
        dto.setTotalBookings(totalBookings);

        BigDecimal revenue = bookingRepository.findAll().stream()
                .filter(b -> b.getStatus() == BookingStatus.CONFIRMED || b.getStatus() == BookingStatus.PAID || b.getStatus() == BookingStatus.COMPLETED)
                .map(b -> b.getTotalPrice() != null ? b.getTotalPrice() : BigDecimal.ZERO)
                .reduce(BigDecimal.ZERO, BigDecimal::add);
        dto.setTotalRevenue(revenue);

        Map<String, Long> statusCounts = bookingRepository.findAll().stream()
                .collect(Collectors.groupingBy(b -> b.getStatus().name(), Collectors.counting()));
        dto.setBookingStatusCounts(statusCounts);

        var topTours = bookingRepository.findAll().stream()
                .filter(b -> b.getTour() != null)
                .collect(Collectors.groupingBy(b -> b.getTour().getName(), Collectors.counting()))
                .entrySet().stream()
                .sorted(Map.Entry.<String, Long>comparingByValue().reversed())
                .limit(5)
                .map(e -> new DashboardStatsDto.TourPopularityDto(e.getKey(), e.getValue()))
                .toList();
        dto.setTopTours(topTours);

        List<DashboardStatsDto.MonthlyBookingDto> monthly = new ArrayList<>();
        LocalDate now = LocalDate.now();
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("MMM yyyy", Locale.forLanguageTag("ru"));
        for (int i = 5; i >= 0; i--) {
            LocalDate monthStart = now.minusMonths(i).withDayOfMonth(1);
            LocalDate monthEnd = monthStart.plusMonths(1).minusDays(1);
            final LocalDate start = monthStart;
            final LocalDate end = monthEnd;
            long count = bookingRepository.findAll().stream()
                    .filter(b -> b.getTravelDate() != null && !b.getTravelDate().isBefore(start) && !b.getTravelDate().isAfter(end))
                    .count();
            monthly.add(new DashboardStatsDto.MonthlyBookingDto(start.format(formatter), count));
        }
        dto.setMonthlyBookings(monthly);

        return dto;
    }
}