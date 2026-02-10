package com.example.demo.service;

import com.example.demo.model.User;
import com.example.demo.repository.UserRepository;
import org.mindrot.jbcrypt.BCrypt;
import org.springframework.stereotype.Service;

import java.util.Optional;
import java.util.regex.Pattern;

@Service
public class RegCheckService {

    private final UserRepository userRepository;
    
    private static final Pattern EMAIL_PATTERN = Pattern.compile(
        "^[A-Za-z0-9+_.-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$"
    );
    
    private static final Pattern LOGIN_PATTERN = Pattern.compile("^[a-zA-Z0-9_-]+$");

    public RegCheckService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    public String validateReg(User user) {
        if (user == null) {
            return "Неверные данные формы";
        }

        if (user.getLogin() == null || user.getLogin().trim().isEmpty()) {
            return "Логин обязателен для заполнения";
        }
        
        String login = user.getLogin().trim();
        if (login.length() < 3 || login.length() > 30) {
            return "Логин должен содержать от 3 до 30 символов";
        }
        
        if (!LOGIN_PATTERN.matcher(login).matches()) {
            return "Логин может содержать только буквы, цифры, подчеркивание и дефис";
        }
        
        user.setLogin(login);

        if (user.getName() == null || user.getName().trim().isEmpty()) {
            user.setName(login);
        } else {
            String name = user.getName().trim();
            if (name.length() < 2 || name.length() > 50) {
                return "Имя должно содержать от 2 до 50 символов";
            }
            user.setName(name);
        }

        if (user.getEmail() == null || user.getEmail().trim().isEmpty()) {
            return "Email обязателен для заполнения";
        }
        
        String email = user.getEmail().trim().toLowerCase();
        if (email.length() < 5 || email.length() > 100) {
            return "Email должен содержать от 5 до 100 символов";
        }
        
        if (!EMAIL_PATTERN.matcher(email).matches()) {
            return "Введите корректный email адрес";
        }
        
        user.setEmail(email);

        if (user.getPassword() == null || user.getPassword().isEmpty()) {
            return "Пароль обязателен для заполнения";
        }
        
        String password = user.getPassword();
        if (password.length() < 8) {
            return "Пароль должен содержать минимум 8 символов";
        }
        
        if (password.length() > 100) {
            return "Пароль не должен превышать 100 символов";
        }

        Optional<User> existingLogin = userRepository.findByLogin(user.getLogin());
        if (existingLogin.isPresent()) {
            return "Такой логин уже существует";
        }

        Optional<User> existingEmail = userRepository.findByEmail(user.getEmail());
        if (existingEmail.isPresent()) {
            return "Такой email уже зарегистрирован";
        }

        String hashedPassword = BCrypt.hashpw(password, BCrypt.gensalt(12));
        user.setPassword(hashedPassword);

        try {
            userRepository.save(user);
        } catch (Exception ex) {
            ex.printStackTrace();
            return "Ошибка при сохранении пользователя. Попробуйте позже.";
        }

        return null;
    }
}
