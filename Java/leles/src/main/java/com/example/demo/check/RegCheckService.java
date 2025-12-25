package com.example.demo.check;

import com.example.demo.model.User;
import com.example.demo.repository.UserRepository;
import org.springframework.stereotype.Service;
import org.springframework.util.DigestUtils;

import java.util.Optional;

@Service
public class RegCheckService {

    private final UserRepository userRepository;

    public RegCheckService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    public String validateReg(User user) {
        if (user == null) {
            return "Неверные данные формы";
        }

        if (user.getLogin() == null || user.getLogin().trim().length() < 2) {
            return "Логин должен содержать минимум 2 символа";
        }

        if (user.getName() != null && user.getName().trim().length() > 0 && user.getName().trim().length() < 2) {
            return "Имя должно содержать минимум 2 символа";
        }

        if (user.getEmail() == null || user.getEmail().trim().length() < 5 || !user.getEmail().contains("@")) {
            return "Введите корректный email (минимум 5 символов)";
        }

        if (user.getPassword() == null || user.getPassword().length() < 8) {
            return "Пароль должен содержать минимум 8 символов";
        }

        Optional<User> existingLogin = userRepository.findByLogin(user.getLogin());
        if (existingLogin.isPresent()) {
            return "Такой логин уже существует";
        }

        Optional<User> existingEmail = userRepository.findByEmail(user.getEmail());
        if (existingEmail.isPresent()) {
            return "Такой email уже существует";
        }

        String salt = "n;fa43p9n8b2-83460243mv098=9;fkdjSDG";
        String hashed = DigestUtils.md5DigestAsHex((salt + user.getPassword()).getBytes());
        user.setPassword(hashed);

        try {
            userRepository.save(user);
        } catch (Exception ex) {
            ex.printStackTrace();
            return "Ошибка при сохранении пользователя: " + ex.getClass().getSimpleName();
        }

        return null;
    }
}
