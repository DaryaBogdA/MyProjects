package com.example.demo.repository;

import com.example.demo.model.MatrixHistory;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface MatrixHistoryRepository extends JpaRepository<MatrixHistory, Integer> {
    List<MatrixHistory> findByUserIdOrderByCreatedAtDesc(Integer userId);
}
