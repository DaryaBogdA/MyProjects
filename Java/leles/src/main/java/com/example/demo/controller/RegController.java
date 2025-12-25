package com.example.demo.controller;

import com.example.demo.check.RegCheckService;
import com.example.demo.model.User;
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
    public String regForm(Model model) {
        model.addAttribute("user", new User());
        return "reg";
    }

    @PostMapping("/check/reg")
    public String register(@ModelAttribute User user, Model model, RedirectAttributes ra) {
        if (user == null || user.getLogin() == null || user.getEmail() == null || user.getPassword() == null) {
            model.addAttribute("error", "Заполните форму регистрации");
            return "reg";
        }

        System.out.println("DEBUG register user: login=" + user.getLogin()
                + " email=" + user.getEmail()
                + " name=" + user.getName()
                + " pwdLen=" + (user.getPassword() == null ? 0 : user.getPassword().length()));

        String error = regCheckService.validateReg(user);
        if (error != null) {
            model.addAttribute("error", error);
            return "reg";
        }

        ra.addFlashAttribute("message", "Регистрация прошла успешно. Войдите в систему.");
        return "redirect:/login";
    }


}
