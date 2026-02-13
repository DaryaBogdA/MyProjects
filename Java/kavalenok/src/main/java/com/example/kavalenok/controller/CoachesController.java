package com.example.kavalenok.controller;

import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.UserRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;

import java.util.List;

@Controller
public class CoachesController {

    @Autowired
    private UserRepository userRepository;

    @GetMapping("/coaches")
    public String user(HttpSession session, Model model) {
        List<User> coaches = userRepository.findAllCoaches();
        model.addAttribute("coaches", coaches);

        return "coaches";
    }
}
