package com.event.arena.repository;

import com.event.arena.entity.Event;
import com.event.arena.entity.EventStatus;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface EventRepository extends JpaRepository<Event, Long> {

    @Query("SELECT e FROM Event e LEFT JOIN FETCH e.sport LEFT JOIN FETCH e.city WHERE e.status = :status")
    List<Event> findByStatusWithSportAndCity(@Param("status") EventStatus status);
    List<Event> findByStatus(EventStatus status);
    @Query("SELECT COUNT(e) FROM Event e WHERE e.status = :status")
    long countByStatus(@Param("status") EventStatus status);}
