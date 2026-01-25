package com.example.Bank_REST.repository;

import com.example.Bank_REST.entity.Users;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

/**
 * Репозиторий для работы с пользователями.
 * 
 * @author darya
 */
public interface UsersRepository extends JpaRepository<Users, Long> {
    /**
     * Находит пользователя по имени пользователя.
     * 
     * @param username имя пользователя
     * @return Optional с пользователем, если найден
     */
    Optional<Users> findByUsername(String username);
    
    /**
     * Находит пользователя по email.
     * 
     * @param email email пользователя
     * @return Optional с пользователем, если найден
     */
    Optional<Users> findByEmail(String email);
}
