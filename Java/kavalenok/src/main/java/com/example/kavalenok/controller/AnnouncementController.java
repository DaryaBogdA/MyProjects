package com.example.kavalenok.controller;

import com.example.kavalenok.model.Announcement;
import com.example.kavalenok.repository.AnnouncementRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;

import java.util.List;

@Controller
public class AnnouncementController {

    @Autowired
    private AnnouncementRepository announcementRepository;

    @GetMapping("/announcements")
    public String announcements(Model model) {
        List<Announcement> announcements = announcementRepository.findAllByIsActiveTrueOrderByCreatedAtDesc();
        model.addAttribute("announcements", announcements);
        return "announcements";
    }
}