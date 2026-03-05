package com.event.arena.repository;

import com.event.arena.entity.Review;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface ReviewRepository extends JpaRepository<Review, Long> {
    List<Review> findByEventId(Long eventId);
    List<Review> findByUserId(Long userId);
    boolean existsByUserIdAndEventId(Long userId, Long eventId);
}