package com.example.Bank_REST.service;

import com.example.Bank_REST.dto.RegisterRequestDto;
import com.example.Bank_REST.entity.Users;
import com.example.Bank_REST.entity.Role;
import com.example.Bank_REST.repository.UsersRepository;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import java.time.Instant;

/**
 * Сервис для аутентификации и регистрации пользователей.
 * 
 * <p>Обеспечивает регистрацию новых пользователей с проверкой уникальности
 * имени пользователя и email, а также хешированием паролей.</p>
 * 
 * @author darya
 */
@Service
public class AuthService {

    private final UsersRepository usersRepository;
    private final PasswordEncoder passwordEncoder;

    /**
     * Конструктор сервиса аутентификации.
     * 
     * @param usersRepository репозиторий для работы с пользователями
     * @param passwordEncoder кодировщик паролей
     */
    public AuthService(UsersRepository usersRepository, PasswordEncoder passwordEncoder) {
        this.usersRepository = usersRepository;
        this.passwordEncoder = passwordEncoder;
    }

    /**
     * Регистрирует нового пользователя в системе.
     * 
     * <p>Проверяет уникальность имени пользователя и email,
     * хеширует пароль и создает пользователя с ролью USER.</p>
     * 
     * @param dto данные для регистрации
     * @return созданный пользователь
     * @throws RuntimeException если имя пользователя или email уже существуют
     */
    public Users register(RegisterRequestDto dto) {
        if (usersRepository.findByUsername(dto.getUsername()).isPresent()) {
            throw new RuntimeException("Username already exists");
        }
        if (usersRepository.findByEmail(dto.getEmail()).isPresent()) {
            throw new RuntimeException("Email already exists");
        }

        Users user = new Users();
        user.setUsername(dto.getUsername());
        user.setEmail(dto.getEmail());
        user.setPasswordHash(passwordEncoder.encode(dto.getPassword()));
        user.setRole(Role.USER);
        user.setCreatedAt(Instant.now());

        return usersRepository.save(user);
    }
}
