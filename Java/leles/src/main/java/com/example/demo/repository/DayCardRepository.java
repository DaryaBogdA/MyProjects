package com.example.demo.repository;

import com.example.demo.model.DayCard;
import org.springframework.data.jpa.repository.JpaRepository;

import java.time.LocalDate;
import java.util.Optional;

public interface DayCardRepository extends JpaRepository<DayCard, Integer> {
    Optional<DayCard> findByIdUserIdAndIdDate(Integer userId, LocalDate date);
}
