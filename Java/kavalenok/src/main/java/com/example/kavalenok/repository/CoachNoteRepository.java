package com.example.kavalenok.repository;

import com.example.kavalenok.model.CoachNote;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;
import java.util.Optional;

public interface CoachNoteRepository extends JpaRepository<CoachNote, Long> {
    @Query("SELECT n FROM CoachNote n JOIN FETCH n.player p WHERE n.coach.id = :coachId ORDER BY n.createdAt DESC")
    List<CoachNote> findByCoachIdOrderByCreatedAtDesc(@Param("coachId") Long coachId);

    Optional<CoachNote> findByCoachIdAndPlayerId(Long coachId, Long playerId);
}
