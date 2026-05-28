package com.example.kavalenok.controller;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;

@Controller
public class InfoController {

    @GetMapping("/rules")
    public String rules() {
        return "rules";
    }
}
