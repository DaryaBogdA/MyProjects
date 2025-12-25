package com.example.demo.repository;

import com.example.demo.model.CardTaro;
import com.example.demo.model.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import java.util.Optional;

public interface CardTaroRepository extends JpaRepository<CardTaro, Integer> {
    Optional<CardTaro> findById(int id);
    @Query(value = "SELECT * FROM cards_taro ORDER BY RAND() LIMIT 1", nativeQuery = true)
    CardTaro findRandomCard();
}
