package com.example.kavalenok.controller;

import com.example.kavalenok.model.ActionLog;
import com.example.kavalenok.model.Report;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.ActionLogRepository;
import com.example.kavalenok.repository.ReportRepository;
import com.example.kavalenok.repository.UserRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import java.util.ArrayList;
import java.util.List;

@Controller
@RequestMapping("/admin")
public class AdminController {

    @Autowired
    private UserRepository userRepository;
    @Autowired
    private ReportRepository reportRepository;
    @Autowired
    private ActionLogRepository actionLogRepository;

    private boolean isAdmin(HttpSession session) {
        User user = (User) session.getAttribute("user");
        return user != null && user.getRole() == User.Role.ADMIN;
    }

    @GetMapping
    public String adminPanel(HttpSession session, Model model) {
        if (!isAdmin(session)) {
            return "redirect:/";
        }
        List<Report> reports = reportRepository.findAllByOrderByCreatedAtDesc();
        List<ReportView> reportViews = new ArrayList<>();
        for (Report report : reports) {
            String reporterEmail = report.getReporter() != null ? report.getReporter().getEmail() : "-";
            String reportedEmail = report.getReportedUser() != null ? report.getReportedUser().getEmail() : "-";
            reportViews.add(new ReportView(report, reporterEmail, reportedEmail));
        }
        model.addAttribute("users", userRepository.findAllByOrderByIdAsc());
        model.addAttribute("reportViews", reportViews);
        model.addAttribute("logs", actionLogRepository.findTop100ByOrderByCreatedAtDesc());
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
            User admin = (User) session.getAttribute("user");
            actionLogRepository.save(new ActionLog(admin, "USER_ROLE_CHANGED",
                    "ID=" + userId + ", роль=" + role.name()));
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
            actionLogRepository.save(new ActionLog(currentUser, "USER_LOCKED", "ID=" + userId));
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
            User admin = (User) session.getAttribute("user");
            actionLogRepository.save(new ActionLog(admin, "USER_UNLOCKED", "ID=" + userId));
            redirectAttributes.addFlashAttribute("successMessage", "Пользователь разблокирован");
        }
        return "redirect:/admin";
    }

    @PostMapping("/report/{reportId}/status")
    public String updateReportStatus(@PathVariable Long reportId,
                                     @RequestParam Report.Status status,
                                     HttpSession session,
                                     RedirectAttributes redirectAttributes) {
        if (!isAdmin(session)) {
            return "redirect:/";
        }
        Report report = reportRepository.findById(reportId).orElse(null);
        if (report == null) {
            redirectAttributes.addFlashAttribute("errorMessage", "Жалоба не найдена");
            return "redirect:/admin";
        }
        report.setStatus(status);
        reportRepository.save(report);
        User admin = (User) session.getAttribute("user");
        actionLogRepository.save(new ActionLog(
                admin,
                "REPORT_STATUS_CHANGED",
                "ID=" + reportId + ", статус=" + status.name()
        ));
        redirectAttributes.addFlashAttribute("successMessage", "Статус жалобы обновлён");
        return "redirect:/admin";
    }

    @PostMapping("/user/{userId}/verify-coach")
    public String verifyCoach(@PathVariable Long userId,
                              HttpSession session,
                              RedirectAttributes redirectAttributes) {
        if (!isAdmin(session)) {
            return "redirect:/";
        }
        User user = userRepository.findById(userId).orElse(null);
        if (user == null || user.getRole() != User.Role.COACH) {
            redirectAttributes.addFlashAttribute("errorMessage", "Тренер не найден");
            return "redirect:/admin";
        }
        user.setCoachVerified(true);
        userRepository.save(user);
        User admin = (User) session.getAttribute("user");
        actionLogRepository.save(new ActionLog(admin, "COACH_VERIFIED", "ID=" + userId));
        redirectAttributes.addFlashAttribute("successMessage", "Сертификат тренера подтверждён");
        return "redirect:/admin";
    }

    @PostMapping("/user/{userId}/unverify-coach")
    public String unverifyCoach(@PathVariable Long userId,
                                HttpSession session,
                                RedirectAttributes redirectAttributes) {
        if (!isAdmin(session)) {
            return "redirect:/";
        }
        User user = userRepository.findById(userId).orElse(null);
        if (user == null || user.getRole() != User.Role.COACH) {
            redirectAttributes.addFlashAttribute("errorMessage", "Тренер не найден");
            return "redirect:/admin";
        }
        user.setCoachVerified(false);
        userRepository.save(user);
        User admin = (User) session.getAttribute("user");
        actionLogRepository.save(new ActionLog(admin, "COACH_UNVERIFIED", "ID=" + userId));
        redirectAttributes.addFlashAttribute("successMessage", "Верификация тренера снята");
        return "redirect:/admin";
    }

    public record ReportView(
            Report report,
            String reporterEmail,
            String reportedUserEmail
    ) {
    }
}