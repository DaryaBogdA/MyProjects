package com.tours.bogdanovich.dto;

import java.math.BigDecimal;
import java.util.List;
import java.util.Map;

public class DashboardStatsDto {
    private Long totalUsers;
    private Long totalBookings;
    private BigDecimal totalRevenue;
    private Map<String, Long> bookingStatusCounts;
    private List<TourPopularityDto> topTours;
    private List<MonthlyBookingDto> monthlyBookings;

    public Long getTotalUsers() { return totalUsers; }
    public void setTotalUsers(Long totalUsers) { this.totalUsers = totalUsers; }
    public Long getTotalBookings() { return totalBookings; }
    public void setTotalBookings(Long totalBookings) { this.totalBookings = totalBookings; }
    public BigDecimal getTotalRevenue() { return totalRevenue; }
    public void setTotalRevenue(BigDecimal totalRevenue) { this.totalRevenue = totalRevenue; }
    public Map<String, Long> getBookingStatusCounts() { return bookingStatusCounts; }
    public void setBookingStatusCounts(Map<String, Long> bookingStatusCounts) { this.bookingStatusCounts = bookingStatusCounts; }
    public List<TourPopularityDto> getTopTours() { return topTours; }
    public void setTopTours(List<TourPopularityDto> topTours) { this.topTours = topTours; }
    public List<MonthlyBookingDto> getMonthlyBookings() { return monthlyBookings; }
    public void setMonthlyBookings(List<MonthlyBookingDto> monthlyBookings) { this.monthlyBookings = monthlyBookings; }

    public static class TourPopularityDto {
        private String name;
        private Long bookingsCount;
        public TourPopularityDto(String name, Long bookingsCount) { this.name = name; this.bookingsCount = bookingsCount; }
        public String getName() { return name; }
        public Long getBookingsCount() { return bookingsCount; }
    }

    public static class MonthlyBookingDto {
        private String month;
        private Long count;
        public MonthlyBookingDto(String month, Long count) { this.month = month; this.count = count; }
        public String getMonth() { return month; }
        public Long getCount() { return count; }
    }
}