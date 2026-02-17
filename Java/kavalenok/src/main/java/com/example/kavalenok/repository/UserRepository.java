package com.example.kavalenok.repository;

import com.example.kavalenok.model.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;
import java.util.Optional;

public interface UserRepository extends JpaRepository<User, Long> {

    Optional<User> findByEmail(String email);
    List<User> findAllByOrderByIdAsc();

    @Query("SELECT u FROM User u LEFT JOIN FETCH u.coachProfile WHERE u.role = 'COACH' ORDER BY u.rating DESC")
    List<User> findAllCoaches();

    @Query("SELECT u FROM User u WHERE u.id != :excludeId AND (" +
            "LOWER(u.firstName) LIKE LOWER(CONCAT('%', :query, '%')) OR " +
            "LOWER(u.lastName) LIKE LOWER(CONCAT('%', :query, '%')) OR " +
            "LOWER(u.email) LIKE LOWER(CONCAT('%', :query, '%')))")
    List<User> searchUsers(@Param("query") String query, @Param("excludeId") Long excludeId);

}