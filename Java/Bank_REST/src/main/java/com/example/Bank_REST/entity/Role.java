package com.example.Bank_REST.entity;

import org.springframework.security.core.authority.SimpleGrantedAuthority;

import java.util.List;

/**
 * Роли пользователей в системе.
 * 
 * <p>Определяет роли пользователей и предоставляет метод для получения
 * authorities для Spring Security.</p>
 * 
 * @author darya
 */
public enum Role {
    /** Администратор с полным доступом */
    ADMIN,
    /** Обычный пользователь с ограниченным доступом */
    USER;

    /**
     * Преобразует роль в список authorities для Spring Security.
     * 
     * @return список authorities с префиксом "ROLE_"
     */
    public List<SimpleGrantedAuthority> getAuthorities() {
        return List.of(new SimpleGrantedAuthority("ROLE_" + this.name()));
    }
}
