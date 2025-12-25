package com.example.demo.check;

import com.example.demo.model.User;
import com.example.demo.repository.UserRepository;
import org.springframework.stereotype.Service;
import org.springframework.util.DigestUtils;

import java.util.Optional;

@Service
public class LoginCheckService {

    private final UserRepository userRepository;
    private final String salt = "n;fa43p9n8b2-83460243mv098=9;fkdjSDG";

    public LoginCheckService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    public String validateFormat(String login, String password) {
        if (login == null || login.length() < 2) return "Логин должен содержать минимум 2 символа";
        if (password == null || password.length() < 8) return "Пароль должен содержать минимум 8 символов";
        return null;
    }

    public Optional<User> authenticate(String login, String password) {
        if (login == null || password == null) return Optional.empty();
        String hashed = DigestUtils.md5DigestAsHex((salt + password).getBytes());
        return userRepository.findByLoginAndPassword(login, hashed);
    }
}

