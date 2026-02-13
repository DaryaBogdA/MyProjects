package com.example.kavalenok.controller;

import com.example.kavalenok.model.User;
import com.example.kavalenok.service.MoreInfoService;
import jakarta.servlet.http.HttpSession;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.time.LocalDate;

@Controller
public class MoreInfoController {

    private final MoreInfoService moreInfoService;

    public MoreInfoController(MoreInfoService moreInfoService) {
        this.moreInfoService = moreInfoService;
    }

    @GetMapping("/moreinfo")
    public String moreinfo(HttpSession session, Model model) {
        User user = (User) session.getAttribute("user");
        if (user == null) {
            return "redirect:/login";
        }
        model.addAttribute("firstName", user.getFirstName());
        model.addAttribute("lastName", user.getLastName());
        model.addAttribute("middleName", user.getSurName());
        model.addAttribute("phone", user.getPhone());
        model.addAttribute("birthDate", user.getDate());

        return "moreInfo";
    }

    @PostMapping("/moreinfo")
    public String processMoreInfo(
            @RequestParam String firstName,
            @RequestParam String lastName,
            @RequestParam(required = false) String middleName,
            @RequestParam String phone,
            @RequestParam @DateTimeFormat(pattern = "yyyy-MM-dd") LocalDate birthDate,
            @RequestParam(required = false) MultipartFile avatarFile,
            @RequestParam(required = false) String defaultAvatar,
            HttpSession session,
            Model model) {

        User user = (User) session.getAttribute("user");

        if (user == null) {
            return "redirect:/login";
        }

        try {
            moreInfoService.validateMoreInfo(firstName, lastName, phone, birthDate);

            String avatarPath = moreInfoService.saveAvatar(avatarFile, defaultAvatar, user.getId());

            User updatedUser = moreInfoService.updateUserMoreInfo(
                    user, firstName, lastName, middleName, phone, birthDate, avatarPath
            );

            session.setAttribute("user", updatedUser);

            return "redirect:/";

        } catch (IllegalArgumentException e) {
            model.addAttribute("errorMessage", e.getMessage());
            model.addAttribute("firstName", firstName);
            model.addAttribute("lastName", lastName);
            model.addAttribute("middleName", middleName);
            model.addAttribute("phone", phone);
            model.addAttribute("birthDate", birthDate);
            return "moreInfo";
        } catch (IOException e) {
            model.addAttribute("errorMessage", "Ошибка при загрузке файла: " + e.getMessage());
            model.addAttribute("firstName", firstName);
            model.addAttribute("lastName", lastName);
            model.addAttribute("middleName", middleName);
            model.addAttribute("phone", phone);
            model.addAttribute("birthDate", birthDate);
            return "moreInfo";
        } catch (Exception e) {
            model.addAttribute("errorMessage", "Ошибка при сохранении профиля: " + e.getMessage());
            model.addAttribute("firstName", firstName);
            model.addAttribute("lastName", lastName);
            model.addAttribute("middleName", middleName);
            model.addAttribute("phone", phone);
            model.addAttribute("birthDate", birthDate);
            return "moreInfo";
        }
    }

    @PostMapping("/skipmoreinfo")
    public String skipMoreInfo(HttpSession session) {
        User user = (User) session.getAttribute("user");

        if (user != null) {
            session.setAttribute("user", user);
        }

        return "redirect:/";
    }

}
