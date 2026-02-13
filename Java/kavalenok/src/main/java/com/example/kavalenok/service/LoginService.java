package com.example.kavalenok.service;

import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.UserRepository;
import org.mindrot.jbcrypt.BCrypt;
import org.springframework.stereotype.Service;

import java.util.Optional;

@Service
public class LoginService {
    private final UserRepository userRepository;

    public LoginService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }
    public void validateFormat(String email, String password) {
        if (!email.contains("@")) {
            throw new IllegalArgumentException("Введите корректный email адрес");
        }
        if (password == null || password.isEmpty()) {
            throw new IllegalArgumentException ("Пароль обязателен для заполнения");
        }
        if (password.length() < 8) {
            throw new IllegalArgumentException ("Пароль должен содержать минимум 8 символов");
        }
    }

    public User authenticate(String email, String password) {
        validateFormat(email, password);

        Optional<User> userOptional = userRepository.findByEmail(email);
        if (userOptional.isEmpty()) {
            throw new IllegalArgumentException("Неверный email или пароль");
        }

        User user = userOptional.get();

        if (user.getLocked() != null && user.getLocked()) {
            throw new IllegalArgumentException("Ваш аккаунт заблокирован. Обратитесь к администратору.");
        }

        if (!BCrypt.checkpw(password, user.getPasswordHash())) {
            throw new IllegalArgumentException("Неверный email или пароль");
        }

        return user;
    }
}
