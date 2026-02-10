package com.example.demo.controller;

import com.example.demo.service.DayCardCheckService;
import com.example.demo.model.CardTaro;
import com.example.demo.model.DayCard;
import com.example.demo.model.User;
import com.example.demo.repository.CardTaroRepository;
import com.example.demo.repository.UserRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import java.time.LocalDate;
import java.util.Optional;

@Controller
public class UserController {

    private final UserRepository userRepository;
    private final CardTaroRepository cardRepo;
    private final DayCardCheckService dayCardCheckService;

    public UserController(UserRepository userRepository,
                          CardTaroRepository cardRepo,
                          DayCardCheckService dayCardCheckService) {
        this.userRepository = userRepository;
        this.cardRepo = cardRepo;
        this.dayCardCheckService = dayCardCheckService;
    }

    @GetMapping("/user")
    public String user(HttpSession session, Model model,
                       @RequestParam(value = "error", required = false) String error,
                       @RequestParam(value = "message", required = false) String message) {

        String login = (String) session.getAttribute("login");
        if (login == null) {
            return "redirect:/login";
        }

        User user = userRepository.findByLogin(login).orElse(null);
        if (user == null) {
            session.removeAttribute("login");
            return "redirect:/login";
        }

        model.addAttribute("user", user);
        model.addAttribute("isVip", user.getIsVip());
        model.addAttribute("madeCount", user.getRequestCount());
        model.addAttribute("maxFree", 3);
        model.addAttribute("remaining", Math.max(0, 3 - user.getRequestCount()));

        if (error != null || message != null) {
            model.addAttribute("error", error != null ? Integer.parseInt(error) : 0);
            model.addAttribute("message", message);
        }

        dayCardCheckService.findByUserIdAndDate(user.getId(), LocalDate.now())
                .ifPresent(dayCard -> model.addAttribute("card", dayCard.getCard()));

        return "user";
    }

    @PostMapping("/card/day")
    public String cardOfDay(HttpSession session, RedirectAttributes redirectAttributes) {
        String login = (String) session.getAttribute("login");
        if (login == null) return "redirect:/login";

        User user = userRepository.findByLogin(login).orElse(null);
        if (user == null) {
            session.removeAttribute("login");
            return "redirect:/login";
        }

        LocalDate today = LocalDate.now();
        Optional<DayCard> existing = dayCardCheckService.findByUserIdAndDate(user.getId(), today);

        if (existing.isPresent()) {
            redirectAttributes.addFlashAttribute("message", "Вы уже вытянули карту на сегодня");
            redirectAttributes.addFlashAttribute("error", 1);
            return "redirect:/user";
        }

        CardTaro randomCard = cardRepo.findRandomCard();
        dayCardCheckService.getOrCreateCard(user, randomCard);

        redirectAttributes.addFlashAttribute("message", "Ваша карта дня готова");
        redirectAttributes.addFlashAttribute("error", 0);
        return "redirect:/user";
    }

}
