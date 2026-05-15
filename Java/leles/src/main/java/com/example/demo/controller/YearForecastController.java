package com.example.demo.controller;

import com.example.demo.model.User;
import com.example.demo.repository.MatrixRepository;
import com.example.demo.repository.UserRepository;
import com.example.demo.service.YearForecastService;
import jakarta.servlet.http.HttpSession;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@Controller
public class YearForecastController {

    private final YearForecastService yearForecastService;
    private final UserRepository userRepository;
    private final MatrixRepository matrixRepository;

    public YearForecastController(YearForecastService yearForecastService,
                                  UserRepository userRepository,
                                  MatrixRepository matrixRepository) {
        this.yearForecastService = yearForecastService;
        this.userRepository = userRepository;
        this.matrixRepository = matrixRepository;
    }

    @GetMapping("/forecast/year")
    public String showForm(HttpSession session, Model model) {
        User user = requireLoggedInUser(session);
        if (user == null) {
            return "redirect:/login";
        }

        model.addAttribute("isVip", Boolean.TRUE.equals(user.getIsVip()));
        model.addAttribute("forecastYear", LocalDate.now().getYear());
        return "yearForecast";
    }

    @PostMapping("/forecast/year")
    public String calculate(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate birthDate,
            @RequestParam int forecastYear,
            HttpSession session,
            Model model) {

        User user = requireLoggedInUser(session);
        if (user == null) {
            return "redirect:/login";
        }

        model.addAttribute("isVip", Boolean.TRUE.equals(user.getIsVip()));
        model.addAttribute("birthDate", birthDate);
        model.addAttribute("forecastYear", forecastYear);

        if (!Boolean.TRUE.equals(user.getIsVip())) {
            model.addAttribute("vipRequired", true);
            return "yearForecast";
        }

        YearForecastService.YearForecastResult result =
                yearForecastService.calculate(birthDate, forecastYear);

        model.addAttribute("personalYear", result.personalYear());
        model.addAttribute("personalYearMatrix", matrixRepository.findById(result.personalYear()).orElse(null));

        List<Map<String, Object>> monthItems = new ArrayList<>();
        for (YearForecastService.MonthForecast month : result.months()) {
            Map<String, Object> item = new HashMap<>();
            item.put("month", month.month());
            item.put("monthName", month.monthName());
            item.put("energy", month.energy());
            matrixRepository.findById(month.energy()).ifPresent(m -> item.put("matrix", m));
            monthItems.add(item);
        }
        model.addAttribute("monthItems", monthItems);

        return "yearForecast";
    }

    private User requireLoggedInUser(HttpSession session) {
        String login = (String) session.getAttribute("login");
        if (login == null) {
            return null;
        }
        return userRepository.findByLogin(login).orElse(null);
    }
}
