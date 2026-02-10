package com.example.kavalenok.service;

import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.UserRepository;
import org.mindrot.jbcrypt.BCrypt;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.Optional;

@Service
public class RegisterService {

    private final UserRepository userRepository;

    public RegisterService(UserRepository userRepository) {this.userRepository = userRepository;}

    public void validateRegistration(String email, String password, String confirmPassword, boolean agree) {
        if (email == null || email.trim().isEmpty()) {
            throw new IllegalArgumentException("Email обязателен");
        }

        if (!email.contains("@")) {
            throw new IllegalArgumentException("Неверный формат email");
        }

        if (password == null || password.trim().isEmpty()) {
            throw new IllegalArgumentException("Пароль обязателен");
        }

        if (password.length() < 8) {
            throw new IllegalArgumentException("Пароль должен содержать минимум 8 символов");
        }

        if (confirmPassword == null || confirmPassword.trim().isEmpty()) {
            throw new IllegalArgumentException("Подтверждение пароля обязательно");
        }

        if (!password.equals(confirmPassword)) {
            throw new IllegalArgumentException("Пароли не совпадают");
        }

        if (!agree) {
            throw new IllegalArgumentException("Вы должны согласиться с условиями");
        }

        Optional<User> existingEmail = userRepository.findByEmail(email);
        if (existingEmail.isPresent()) {
            throw new IllegalArgumentException("Пользователь с таким email уже существует");
        }
    }

    public User register(String email, String password, String confirmPassword, boolean agree) {
        validateRegistration(email, password, confirmPassword, agree);

        String hashedPassword = BCrypt.hashpw(password, BCrypt.gensalt(12));
        User user = new User();
        user.setEmail(email.toLowerCase().trim());
        user.setPasswordHash(hashedPassword);
        user.setCreatedAt(LocalDateTime.now());
        user.setRole(User.Role.USER);

        return userRepository.save(user);
    }
}
