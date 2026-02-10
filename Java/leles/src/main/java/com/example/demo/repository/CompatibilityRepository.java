package com.example.demo.repository;

import com.example.demo.model.Compatibility;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface CompatibilityRepository extends JpaRepository<Compatibility, Integer> {
}