package com.example.Bank_REST.security;

import com.example.Bank_REST.entity.Users;
import com.example.Bank_REST.repository.UsersRepository;
import org.springframework.security.core.userdetails.*;
import org.springframework.stereotype.Service;

/**
 * Реализация сервиса для загрузки данных пользователя Spring Security.
 * 
 * <p>Загружает пользователя из базы данных по имени пользователя
 * и преобразует его в объект UserDetails для Spring Security.</p>
 * 
 * @author darya
 */
@Service
public class UserDetailsServiceImpl implements UserDetailsService {

    private final UsersRepository usersRepository;

    /**
     * Конструктор сервиса загрузки пользователей.
     * 
     * @param usersRepository репозиторий для работы с пользователями
     */
    public UserDetailsServiceImpl(UsersRepository usersRepository) {
        this.usersRepository = usersRepository;
    }

    /**
     * Загружает пользователя по имени пользователя.
     * 
     * @param username имя пользователя
     * @return объект UserDetails с данными пользователя
     * @throws UsernameNotFoundException если пользователь не найден
     */
    @Override
    public UserDetails loadUserByUsername(String username) throws UsernameNotFoundException {
        Users user = usersRepository.findByUsername(username)
                .orElseThrow(() -> new UsernameNotFoundException("User not found: " + username));

        return new CustomUserDetails(user);
    }
}
