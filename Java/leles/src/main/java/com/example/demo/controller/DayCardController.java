package com.example.demo.controller;

import com.example.demo.model.CardTaro;
import com.example.demo.model.DayCard;
import com.example.demo.model.User;
import com.example.demo.repository.UserRepository;
import org.springframework.ui.Model;
import com.example.demo.service.DayCardCheckService;
import com.example.demo.repository.CardTaroRepository;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.time.LocalDate;
import java.util.Optional;

@Controller
@RequestMapping("/check")
public class DayCardController {
    private final DayCardCheckService dayCardCheckService;
    private final CardTaroRepository cardTaroRepository;
    private final UserRepository userRepository;

    public DayCardController(DayCardCheckService dayCardCheckService,
                             CardTaroRepository cardTaroRepository,
                             UserRepository userRepository) {
        this.dayCardCheckService = dayCardCheckService;
        this.cardTaroRepository = cardTaroRepository;
        this.userRepository = userRepository;
    }

    @PostMapping("/DayCardCheckService")
    public String checkCard(@RequestParam Integer userId, Model model) {
        User user = userRepository.findById(userId).orElseThrow();
        CardTaro randomCard = cardTaroRepository.findRandomCard();
        DayCard card = dayCardCheckService.getOrCreateCard(user, randomCard);

        model.addAttribute("card", card.getCard());
        return "user";
    }

    @GetMapping("/DayCardCheckService")
    public String showCard(@RequestParam Integer userId, Model model) {
        LocalDate today = LocalDate.now();
        Optional<DayCard> existing = dayCardCheckService.findByUserIdAndDate(userId, today);

        existing.ifPresent(dayCard -> model.addAttribute("card", dayCard.getCard()));
        return "user";
    }
}
