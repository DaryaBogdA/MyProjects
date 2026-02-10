package com.example.kavalenok.repository;

import com.example.kavalenok.model.TrainingParticipant;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;
import java.util.Optional;

public interface TrainingParticipantRepository extends JpaRepository<TrainingParticipant, Long> {

    Optional<TrainingParticipant> findByTrainingIdAndUserId(Long trainingId, Long userId);

    boolean existsByTrainingIdAndUserId(Long trainingId, Long userId);

    @Query("SELECT tp FROM TrainingParticipant tp JOIN FETCH tp.training t WHERE tp.user.id = :userId ORDER BY tp.appliedAt DESC")
    List<TrainingParticipant> findByUserIdOrderByAppliedAtDesc(@Param("userId") Long userId);

    @Query("SELECT COUNT(tp) FROM TrainingParticipant tp WHERE tp.training.id = :trainingId AND tp.status IN ('PENDING', 'APPROVED', 'ATTENDED')")
    long countByTrainingId(Long trainingId);
}
