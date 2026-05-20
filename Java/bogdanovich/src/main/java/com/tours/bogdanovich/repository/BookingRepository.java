package com.tours.bogdanovich.repository;

import com.tours.bogdanovich.entity.Booking;
import com.tours.bogdanovich.entity.BookingStatus;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface BookingRepository extends JpaRepository<Booking, Integer> {
    List<Booking> findByUser_IdOrderByIdDesc(Integer userId);

    List<Booking> findByStatusOrderByIdDesc(BookingStatus status);

    Optional<Booking> findByIdAndUser_Id(Integer id, Integer userId);
}
