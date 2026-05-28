package com.example.kavalenok.controller;

import com.example.kavalenok.model.ActionLog;
import com.example.kavalenok.model.Report;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.ActionLogRepository;
import com.example.kavalenok.repository.ReportRepository;
import com.example.kavalenok.repository.UserRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

@Controller
public class ReportController {

    private final ReportRepository reportRepository;
    private final UserRepository userRepository;
    private final ActionLogRepository actionLogRepository;

    public ReportController(ReportRepository reportRepository,
                            UserRepository userRepository,
                            ActionLogRepository actionLogRepository) {
        this.reportRepository = reportRepository;
        this.userRepository = userRepository;
        this.actionLogRepository = actionLogRepository;
    }

    @PostMapping("/reports/user/{reportedUserId}")
    public String reportUser(@PathVariable Long reportedUserId,
                             @RequestParam String reason,
                             HttpSession session,
                             RedirectAttributes redirectAttributes) {
        User reporter = (User) session.getAttribute("user");
        if (reporter == null) {
            return "redirect:/login";
        }
        if (reason == null || reason.isBlank()) {
            redirectAttributes.addFlashAttribute("errorMessage", "Укажите причину жалобы");
            return "redirect:/profile/" + reportedUserId;
        }
        User reportedUser = userRepository.findById(reportedUserId).orElse(null);
        if (reportedUser == null) {
            redirectAttributes.addFlashAttribute("errorMessage", "Пользователь не найден");
            return "redirect:/friends";
        }
        Report report = new Report(reporter, reason.trim());
        report.setReportedUser(reportedUser);
        reportRepository.save(report);

        actionLogRepository.save(new ActionLog(
                reporter,
                "REPORT_CREATED",
                "Жалоба на пользователя ID=" + reportedUserId
        ));

        redirectAttributes.addFlashAttribute("successMessage", "Жалоба отправлена администратору");
        return "redirect:/profile/" + reportedUserId;
    }
}
