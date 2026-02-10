package com.example.kavalenok.controller;

import com.example.kavalenok.model.User;
import com.example.kavalenok.service.RegisterService;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

@Controller
public class RegisterController {

    private final RegisterService registerService;

    public RegisterController(RegisterService registerService) {this.registerService = registerService;}

    @GetMapping("/register")
    public String registerForm(HttpSession session, Model model){
        if (session.getAttribute("user") != null) {
            return "redirect:/";
        }

        return "register";
    }

    @PostMapping("/register")
    public String register(@RequestParam String email,
                           @RequestParam String password,
                           @RequestParam String confirmPassword,
                           @RequestParam(required = false, defaultValue = "false") Boolean agree,
                           HttpSession session,
                           Model model){
        try {
            User user = registerService.register(email, password, confirmPassword, agree);

            session.setAttribute("user", user);

            return "/moreinfo";

        } catch (IllegalArgumentException e) {
            model.addAttribute("errorMessage", e.getMessage());
            model.addAttribute("email", email);
            model.addAttribute("password", password);
            model.addAttribute("confirmPassword", confirmPassword);
            return "register";
        } catch (Exception e) {
            model.addAttribute("errorMessage", "Ошибка регистрации: " + e.getMessage());
            model.addAttribute("email", email);
            model.addAttribute("password", password);
            model.addAttribute("confirmPassword", confirmPassword);
            return "register";
        }
    }
}
