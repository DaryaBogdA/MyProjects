package com.example.kavalenok.repository;

import com.example.kavalenok.model.ActionLog;
import org.springframework.data.jpa.repository.EntityGraph;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface ActionLogRepository extends JpaRepository<ActionLog, Long> {
    @EntityGraph(attributePaths = "actor")
    List<ActionLog> findTop100ByOrderByCreatedAtDesc();
}
