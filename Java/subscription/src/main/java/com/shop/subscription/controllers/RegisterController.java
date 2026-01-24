package com.shop.subscription.controllers;

import com.shop.subscription.entity.Users;
import com.shop.subscription.repository.UsersRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

@Controller
public class RegisterController {
    @Autowired
    private UsersRepository usersRepository;

    @GetMapping("/register")
    public String main(Model model) {
        return "register";
    }

    private void addAttr(Model model, String name, String login, String email, String password){
        model.addAttribute("name", name);
        model.addAttribute("login", login);
        model.addAttribute("email", email);
        model.addAttribute("password", password);
    }

    @PostMapping("/register")
    public String register(@RequestParam String name, @RequestParam String login, @RequestParam String email, @RequestParam String password, @RequestParam String password_return, Model model) {

        if (name.length() < 3) {
            model.addAttribute("error", "В имени должно быть больше букв");
            addAttr(model, name, login, email, password);
            return "register";
        }

        if (login.length() < 3) {
            model.addAttribute("error", "В логине должно быть больше букв");
            addAttr(model, name, login, email, password);
            return "register";
        }

        if (password.length() < 8) {
            model.addAttribute("error", "Пароль должен содержать минимум 8 символов");
            addAttr(model, name, login, email, password);
            return "register";
        }

        if (!password.equals(password_return)) {
            model.addAttribute("error", "Пароли не совпадают");
            addAttr(model, name, login, email, password);
            return "register";
        }

        if (!email.matches("^[A-Za-z0-9+_.-]+@[A-Za-z0-9.-]+$")) {
            model.addAttribute("error", "Email имеет неверный формат");
            addAttr(model, name, login, email, password);
            return "register";
        }


        Users user = new Users(name, login, email, password);
        usersRepository.save(user);
        return "redirect:/";
    }
}
