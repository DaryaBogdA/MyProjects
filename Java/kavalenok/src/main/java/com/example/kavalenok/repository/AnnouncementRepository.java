package com.example.kavalenok.repository;

import com.example.kavalenok.model.Announcement;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface AnnouncementRepository extends JpaRepository<Announcement, Long> {
    List<Announcement> findAllByIsActiveTrueOrderByCreatedAtDesc();
}