package com.event.arena.repository;

import com.event.arena.entity.Review;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface ReviewRepository extends JpaRepository<Review, Long> {
    List<Review> findByEventId(Long eventId);

    @Query("SELECT r FROM Review r JOIN FETCH r.user WHERE r.event.id = :eventId ORDER BY r.createdAt DESC")
    List<Review> findByEventIdWithUser(@Param("eventId") Long eventId);
    List<Review> findByUserId(Long userId);
    boolean existsByUserIdAndEventId(Long userId, Long eventId);
}