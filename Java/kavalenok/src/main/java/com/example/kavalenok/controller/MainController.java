package com.example.kavalenok.controller;

import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;

@Controller
public class MainController {

    @GetMapping("/")
    public String index(HttpSession session, Model model) {
        model.addAttribute("login", session.getAttribute("login"));
        return "main";
    }
}
