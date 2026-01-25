package com.example.Bank_REST.service;

import org.springframework.security.core.userdetails.UserDetails;

/**
 * Интерфейс сервиса для работы с JWT токенами.
 * 
 * <p>Предоставляет методы для генерации, извлечения данных и валидации JWT токенов.</p>
 * 
 * @author darya
 */
public interface JwtService {
    /**
     * Генерирует JWT токен для пользователя.
     * 
     * @param userDetails данные пользователя
     * @return JWT токен
     */
    String generateToken(UserDetails userDetails);
    
    /**
     * Извлекает имя пользователя из JWT токена.
     * 
     * @param token JWT токен
     * @return имя пользователя
     */
    String extractUsername(String token);
    
    /**
     * Проверяет валидность JWT токена для указанного пользователя.
     * 
     * @param token JWT токен
     * @param userDetails данные пользователя
     * @return true, если токен валиден, иначе false
     */
    boolean validateToken(String token, UserDetails userDetails);
}
