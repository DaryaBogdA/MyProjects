package com.example.kavalenok.repository;

import com.example.kavalenok.model.UserAchievement;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface UserAchievementRepository extends JpaRepository<UserAchievement, Long> {
    List<UserAchievement> findByUserIdOrderByAwardedAtDesc(Long userId);

    Optional<UserAchievement> findByUserIdAndBadgeName(Long userId, String badgeName);
}
