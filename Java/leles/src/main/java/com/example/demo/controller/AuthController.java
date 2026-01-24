package com.example.demo.controller;

import com.example.demo.service.LoginCheckService;
import com.example.demo.model.User;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import java.util.Optional;

@Controller
public class AuthController {

    private final LoginCheckService loginCheckService;

    public AuthController(LoginCheckService loginCheckService) {
        this.loginCheckService = loginCheckService;
    }

    @GetMapping("/login")
    public String login(HttpSession session, Model model) {
        if (session.getAttribute("login") != null) {
            return "redirect:/user";
        }
        
        model.addAttribute("user", new User());
        return "login";
    }

    @PostMapping("/check/login")
    public String checkLogin(@RequestParam(required = false) String login,
                             @RequestParam(required = false) String password,
                             RedirectAttributes redirectAttributes,
                             HttpSession session) {

        if (login == null || password == null || login.trim().isEmpty() || password.isEmpty()) {
            redirectAttributes.addFlashAttribute("error", "Заполните логин и пароль");
            return "redirect:/login";
        }

        String formatError = loginCheckService.validateFormat(login, password);
        if (formatError != null) {
            redirectAttributes.addFlashAttribute("error", formatError);
            return "redirect:/login";
        }

        Optional<User> userOpt = loginCheckService.authenticate(login, password);
        if (userOpt.isEmpty()) {
            redirectAttributes.addFlashAttribute("error", "Неверный логин или пароль");
            return "redirect:/login";
        }

        User authenticatedUser = userOpt.get();
        session.setAttribute("login", authenticatedUser.getLogin());
        session.setAttribute("userId", authenticatedUser.getId());

        redirectAttributes.addFlashAttribute("message", "Добро пожаловать, " + (authenticatedUser.getName() != null && !authenticatedUser.getName().isEmpty() ? authenticatedUser.getName() : authenticatedUser.getLogin()) + "!");
        return "redirect:/user";
    }

    @GetMapping("/logout")
    public String logout(HttpSession session, RedirectAttributes redirectAttributes) {
        session.invalidate();
        redirectAttributes.addFlashAttribute("message", "Вы успешно вышли из системы");
        return "redirect:/login";
    }
}