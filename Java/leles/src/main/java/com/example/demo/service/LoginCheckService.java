package com.example.demo.service;

import com.example.demo.model.User;
import com.example.demo.repository.UserRepository;
import org.mindrot.jbcrypt.BCrypt;
import org.springframework.stereotype.Service;

import java.util.Optional;

@Service
public class LoginCheckService {

    private final UserRepository userRepository;

    public LoginCheckService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    public String validateFormat(String login, String password) {
        if (login == null || login.trim().isEmpty()) {
            return "Логин обязателен для заполнения";
        }
        if (password == null || password.isEmpty()) {
            return "Пароль обязателен для заполнения";
        }
        if (login.trim().length() < 3) {
            return "Логин должен содержать минимум 3 символа";
        }
        if (password.length() < 8) {
            return "Пароль должен содержать минимум 8 символов";
        }
        return null;
    }

    public Optional<User> authenticate(String login, String password) {
        if (login == null || password == null || login.trim().isEmpty() || password.isEmpty()) {
            return Optional.empty();
        }
        
        String normalizedLogin = login.trim();
        
        Optional<User> userOpt = userRepository.findByLogin(normalizedLogin);
        if (userOpt.isEmpty()) {
            return Optional.empty();
        }
        
        User user = userOpt.get();
        
        if (user.getPassword() != null) {
            try {
                String storedPassword = user.getPassword();
                if (storedPassword.startsWith("$2a$") || storedPassword.startsWith("$2b$") || storedPassword.startsWith("$2y$")) {
                    if (BCrypt.checkpw(password, storedPassword)) {
                        return Optional.of(user);
                    }
                } else {
                    return Optional.empty();
                }
            } catch (Exception e) {
                return Optional.empty();
            }
        }
        return Optional.empty();
    }
}

