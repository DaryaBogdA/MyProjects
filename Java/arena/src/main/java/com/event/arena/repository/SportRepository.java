package com.event.arena.repository;

import com.event.arena.entity.Sport;
import org.springframework.data.jpa.repository.JpaRepository;

public interface SportRepository extends JpaRepository<Sport, Long> {
}
