package com.example.kavalenok.repository;

import com.example.kavalenok.model.CoachReview;
import com.example.kavalenok.model.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface CoachReviewRepository extends JpaRepository<CoachReview, Long> {
    List<CoachReview> findByCoachOrderByCreatedAtDesc(User coach);

    @Query("SELECT AVG(r.rating) FROM CoachReview r WHERE r.coach.id = :coachId")
    Double getAverageRatingByCoachId(@Param("coachId") Long coachId);
}