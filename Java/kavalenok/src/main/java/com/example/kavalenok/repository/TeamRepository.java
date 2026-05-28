package com.example.kavalenok.repository;

import com.example.kavalenok.model.Team;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import java.util.List;
import java.util.Optional;

public interface TeamRepository extends JpaRepository<Team, Long> {
    @Query("""
            SELECT t
            FROM Team t
            JOIN FETCH t.captain c
            LEFT JOIN FETCH t.coach coach
            ORDER BY t.createdAt DESC
            """)
    List<Team> findAllWithCaptainOrderByCreatedAtDesc();

    @Query("""
            SELECT t
            FROM Team t
            JOIN FETCH t.captain c
            LEFT JOIN FETCH t.coach coach
            WHERE t.id = :id
            """)
    Optional<Team> findByIdWithCaptain(Long id);
}