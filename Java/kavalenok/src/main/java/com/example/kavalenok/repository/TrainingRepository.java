package com.example.kavalenok.repository;

import com.example.kavalenok.model.Training;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface TrainingRepository extends JpaRepository<Training, Long> {
    @Query("SELECT t FROM Training t WHERE t.dateTime > CURRENT_TIMESTAMP " +
            "AND (:title IS NULL OR LOWER(t.title) LIKE LOWER(CONCAT('%', :title, '%'))) " +
            "AND (:location IS NULL OR LOWER(t.location) LIKE LOWER(CONCAT('%', :location, '%'))) " +
            "AND (:coachId IS NULL OR t.coach.id = :coachId) " +
            "ORDER BY t.dateTime ASC")
    List<Training> findTrainingsWithFilters(@Param("title") String title,
                                            @Param("location") String location,
                                            @Param("coachId") Long coachId);

}