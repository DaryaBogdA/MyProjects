package com.example.Bank_REST.security;

import com.example.Bank_REST.entity.Users;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.userdetails.UserDetails;

import java.util.Collection;

/**
 * Кастомная реализация UserDetails для Spring Security.
 * 
 * <p>Обертка над сущностью Users, предоставляющая интерфейс UserDetails
 * для интеграции с Spring Security.</p>
 * 
 * @author darya
 */
public class CustomUserDetails implements UserDetails {
    private final Users user;

    /**
     * Конструктор кастомного UserDetails.
     * 
     * @param user сущность пользователя
     */
    public CustomUserDetails(Users user) {
        this.user = user;
    }

    /**
     * Получает идентификатор пользователя.
     * 
     * @return идентификатор пользователя
     */
    public Long getId() {
        return user.getId();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Collection<? extends GrantedAuthority> getAuthorities() {
        return user.getRole().getAuthorities();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public String getPassword() {
        return user.getPasswordHash();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public String getUsername() {
        return user.getUsername();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean isAccountNonExpired() {
        return true;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean isAccountNonLocked() {
        return user.getRole() != null;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean isCredentialsNonExpired() {
        return true;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean isEnabled() {
        return true;
    }
}
