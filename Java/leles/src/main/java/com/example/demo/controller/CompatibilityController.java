package com.example.demo.controller;

import com.example.demo.service.CompatibilityService;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;

@Controller
public class CompatibilityController {

    private final CompatibilityService service;

    public CompatibilityController(CompatibilityService service) {
        this.service = service;
    }

    @GetMapping("/compatibility")
    public String showForm() {
        return "compatibility";
    }

    @PostMapping("/compatibility")
    public String calculate(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate birthDate1,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate birthDate2,
            Model model) {

        int number = service.calculateNumber(birthDate1, birthDate2);
        String description = service.calculateCompatibility(birthDate1, birthDate2);

        model.addAttribute("number", number);
        model.addAttribute("description", description);
        model.addAttribute("date1", birthDate1);
        model.addAttribute("date2", birthDate2);

        return "compatibility";
    }
}