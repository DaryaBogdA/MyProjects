package com.event.arena.repository;

import com.event.arena.entity.Event;
import com.event.arena.entity.Registration;
import com.event.arena.entity.RegistrationStatus;
import com.event.arena.entity.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;
import java.util.Optional;

public interface RegistrationRepository extends JpaRepository<Registration, Long> {

    @Query("SELECT r.event.id FROM Registration r WHERE r.user = :user AND r.status = :status")
    List<Long> findEventIdsByUserAndStatus(@Param("user") User user, @Param("status") RegistrationStatus status);

    List<Registration> findByUserAndStatus(User user, RegistrationStatus status);

    Optional<Registration> findByUserAndEventAndStatus(User user, Event event, RegistrationStatus status);

    boolean existsByUserAndEventAndStatus(User user, Event event, RegistrationStatus status);

    long countByEventAndStatus(Event event, RegistrationStatus status);
}
