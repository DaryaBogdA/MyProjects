package com.event.arena.repository;

import com.event.arena.entity.Favorite;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface FavoriteRepository extends JpaRepository<Favorite, Long> {
    List<Favorite> findByUserId(Long userId);

    @Query("SELECT CASE WHEN COUNT(f) > 0 THEN true ELSE false END FROM Favorite f WHERE f.user.id = :userId AND f.event.id = :eventId")
    boolean existsByUserIdAndEventId(@Param("userId") Long userId, @Param("eventId") Long eventId);

    @Modifying
    @Query("DELETE FROM Favorite f WHERE f.user.id = :userId AND f.event.id = :eventId")
    void deleteByUserIdAndEventId(@Param("userId") Long userId, @Param("eventId") Long eventId);
}