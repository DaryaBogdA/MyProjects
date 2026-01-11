package com.example.Bank_REST.repository;

import com.example.Bank_REST.entity.Cards;
import org.springframework.data.domain.Page;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.domain.Pageable;

import java.util.Optional;

public interface CardsRepository extends JpaRepository<Cards, Long> {
    Page<Cards> findByOwnerId(Long ownerId, Pageable pageable);
    Page<Cards> findByOwnerIdAndMaskedNumberContainingIgnoreCase(Long ownerId, String search, Pageable pageable);
    Optional<Cards> findByEncryptedNumber(String encryptedNumber);
}
