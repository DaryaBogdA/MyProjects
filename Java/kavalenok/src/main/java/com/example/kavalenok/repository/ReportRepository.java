package com.example.kavalenok.repository;

import com.example.kavalenok.model.Report;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface ReportRepository extends JpaRepository<Report, Long> {
    @Query("""
            SELECT r
            FROM Report r
            JOIN FETCH r.reporter reporter
            LEFT JOIN FETCH r.reportedUser reportedUser
            ORDER BY r.createdAt DESC
            """)
    List<Report> findAllByOrderByCreatedAtDesc();

    @Query("""
            SELECT r
            FROM Report r
            JOIN FETCH r.reporter reporter
            LEFT JOIN FETCH r.reportedUser reportedUser
            WHERE r.status = :status
            ORDER BY r.createdAt DESC
            """)
    List<Report> findByStatusOrderByCreatedAtDesc(@Param("status") Report.Status status);
}
