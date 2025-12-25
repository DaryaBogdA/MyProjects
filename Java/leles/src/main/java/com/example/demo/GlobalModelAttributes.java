package com.example.demo;

import com.example.demo.model.User;
import com.example.demo.repository.UserRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ModelAttribute;

@ControllerAdvice
public class GlobalModelAttributes {

    private final UserRepository userRepository;

    public GlobalModelAttributes(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    @ModelAttribute("login")
    public String login(HttpSession session) {
        Object login = session.getAttribute("login");
        return login != null ? login.toString() : null;
    }

    @ModelAttribute("user")
    public User currentUser(HttpSession session) {
        Object login = session.getAttribute("login");
        if (login instanceof String) {
            return userRepository.findByLogin((String) login).orElse(null);
        }
        return null;
    }
}
