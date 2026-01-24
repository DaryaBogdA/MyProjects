package com.shop.subscription.controllers;

import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

@Controller
public class LoginController {
    @GetMapping("/login")
    public String main(Model model) {
        return "login";
    }

    @PostMapping("/login")
    public String login(Model model, @RequestParam String login, @RequestParam String password){
        if (login == null || password == null || login.isBlank() || password.isBlank()) {
            model.addAttribute("error", "Заполните логин и пароль");
            return "login";
        }
    return "redirect: /";
    }
}
