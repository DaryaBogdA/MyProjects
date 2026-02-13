package com.example.kavalenok.controller;

import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.UserRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

@Controller
@RequestMapping("/admin")
public class AdminController {

    @Autowired
    private UserRepository userRepository;

    private boolean isAdmin(HttpSession session) {
        User user = (User) session.getAttribute("user");
        return user != null && user.getRole() == User.Role.ADMIN;
    }

    @GetMapping
    public String adminPanel(HttpSession session, Model model) {
        if (!isAdmin(session)) {
            return "redirect:/";
        }
        model.addAttribute("users", userRepository.findAllByOrderByIdAsc());
        return "admin";
    }

    @PostMapping("/user/{userId}/role")
    public String changeRole(@PathVariable Long userId,
                             @RequestParam User.Role role,
                             HttpSession session,
                             RedirectAttributes redirectAttributes) {
        if (!isAdmin(session)) {
            return "redirect:/";
        }
        User user = userRepository.findById(userId).orElse(null);
        if (user != null) {
            user.setRole(role);
            userRepository.save(user);
            redirectAttributes.addFlashAttribute("successMessage", "Роль пользователя обновлена");
        }
        return "redirect:/admin";
    }

    @PostMapping("/user/{userId}/lock")
    public String lockUser(@PathVariable Long userId,
                           HttpSession session,
                           RedirectAttributes redirectAttributes) {
        if (!isAdmin(session)) {
            return "redirect:/";
        }
        User currentUser = (User) session.getAttribute("user");
        if (currentUser.getId().equals(userId)) {
            redirectAttributes.addFlashAttribute("errorMessage", "Нельзя заблокировать самого себя");
            return "redirect:/admin";
        }
        User user = userRepository.findById(userId).orElse(null);
        if (user != null) {
            user.setLocked(true);
            userRepository.save(user);
            redirectAttributes.addFlashAttribute("successMessage", "Пользователь заблокирован");
        }
        return "redirect:/admin";
    }

    @PostMapping("/user/{userId}/unlock")
    public String unlockUser(@PathVariable Long userId,
                             HttpSession session,
                             RedirectAttributes redirectAttributes) {
        if (!isAdmin(session)) {
            return "redirect:/";
        }
        User user = userRepository.findById(userId).orElse(null);
        if (user != null) {
            user.setLocked(false);
            userRepository.save(user);
            redirectAttributes.addFlashAttribute("successMessage", "Пользователь разблокирован");
        }
        return "redirect:/admin";
    }
}