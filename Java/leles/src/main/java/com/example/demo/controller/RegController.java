package com.example.demo.controller;

import com.example.demo.service.RegCheckService;
import com.example.demo.model.User;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

@Controller
public class RegController {

    private final RegCheckService regCheckService;

    public RegController(RegCheckService regCheckService) {
        this.regCheckService = regCheckService;
    }

    @GetMapping("/reg")
    public String regForm(HttpSession session, Model model) {
        if (session.getAttribute("login") != null) {
            return "redirect:/user";
        }
        
        model.addAttribute("user", new User());
        return "reg";
    }

    @PostMapping("/check/reg")
    public String register(@RequestParam(required = false) String login,
                          @RequestParam(required = false) String name,
                          @RequestParam(required = false) String email,
                          @RequestParam(required = false) String password,
                          Model model, RedirectAttributes ra) {
        
        User user = new User();
        user.setLogin(login);
        user.setName(name);
        user.setEmail(email);
        user.setPassword(password);

        if (login == null || login.trim().isEmpty() ||
            email == null || email.trim().isEmpty() || 
            password == null || password.isEmpty()) {
            model.addAttribute("error", "Заполните все обязательные поля");
            User userForForm = new User();
            userForForm.setLogin(login);
            userForForm.setName(name);
            userForForm.setEmail(email);
            model.addAttribute("user", userForForm);
            return "reg";
        }

        String error = regCheckService.validateReg(user);
        if (error != null) {
            model.addAttribute("error", error);
            User userForForm = new User();
            userForForm.setLogin(user.getLogin());
            userForForm.setName(user.getName());
            userForForm.setEmail(user.getEmail());
            model.addAttribute("user", userForForm);
            return "reg";
        }

        ra.addFlashAttribute("message", "Регистрация прошла успешно! Войдите в систему, используя свой логин и пароль.");
        return "redirect:/login";
    }
}
