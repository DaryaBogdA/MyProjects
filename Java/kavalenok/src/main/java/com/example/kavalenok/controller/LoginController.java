package com.example.kavalenok.controller;

import com.example.kavalenok.model.User;
import com.example.kavalenok.service.LoginService;
import org.springframework.ui.Model;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

@Controller
public class LoginController {
    private final LoginService loginService;

    public LoginController(LoginService loginService) {this.loginService = loginService;}

    @GetMapping("/login")
    public String login(HttpSession session, Model model) {
        if (session.getAttribute("user") != null) {
            return "redirect:/";
        }
        return "login";
    }

    @PostMapping("/login")
    public String login(HttpSession session,
                        Model model,
                        @RequestParam String email,
                        @RequestParam String password) {
        try {
            User user = loginService.authenticate(email, password);

            session.setAttribute("user", user);

            return "redirect:/";

        } catch (IllegalArgumentException e) {
            model.addAttribute("errorMessage", e.getMessage());
            model.addAttribute("email", email);
            return "login";
        } catch (Exception e) {
            model.addAttribute("errorMessage", "Ошибка при входе: " + e.getMessage());
            model.addAttribute("email", email);
            return "login";
        }
    }

    @GetMapping("/logout")
    public String logout(HttpSession session) {
        session.invalidate();
        return "redirect:/";
    }
}
