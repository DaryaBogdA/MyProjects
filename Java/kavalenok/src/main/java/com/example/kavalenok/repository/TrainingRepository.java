package com.example.kavalenok.repository;

import com.example.kavalenok.model.Training;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import java.util.List;

public interface TrainingRepository extends JpaRepository<Training, Long> {
    @Query("SELECT t FROM Training t WHERE t.dateTime > CURRENT_TIMESTAMP ORDER BY t.dateTime ASC")
    List<Training> findNewTrainings();
}
