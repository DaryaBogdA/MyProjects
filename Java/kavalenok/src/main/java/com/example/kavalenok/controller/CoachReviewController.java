package com.example.kavalenok.controller;

import com.example.kavalenok.model.User;
import com.example.kavalenok.service.CoachReviewService;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

@Controller
@RequestMapping("/coach")
public class CoachReviewController {

    private final CoachReviewService coachReviewService;

    public CoachReviewController(CoachReviewService coachReviewService) {
        this.coachReviewService = coachReviewService;
    }

    @GetMapping("/{coachId}/reviews")
    public String viewReviews(@PathVariable Long coachId, Model model, HttpSession session) {
        User currentUser = (User) session.getAttribute("user");
        model.addAttribute("reviews", coachReviewService.getReviewsByCoach(coachId));
        model.addAttribute("coachId", coachId);
        model.addAttribute("canLeaveReview", currentUser != null);
        return "coach-reviews";
    }

    @PostMapping("/{coachId}/review")
    public String addReview(@PathVariable Long coachId,
                            @RequestParam Integer rating,
                            @RequestParam(required = false) String comment,
                            HttpSession session,
                            RedirectAttributes redirectAttributes) {
        User currentUser = (User) session.getAttribute("user");
        if (currentUser == null) {
            return "redirect:/login";
        }

        try {
            coachReviewService.addReview(currentUser, coachId, rating, comment);
            redirectAttributes.addFlashAttribute("successMessage", "Отзыв успешно добавлен!");
        } catch (IllegalArgumentException e) {
            redirectAttributes.addFlashAttribute("errorMessage", e.getMessage());
        }
        return "redirect:/coach/" + coachId + "/reviews";
    }

    @PostMapping("/review/{reviewId}/delete")
    public String deleteReview(@PathVariable Long reviewId,
                               HttpSession session,
                               RedirectAttributes redirectAttributes) {
        User currentUser = (User) session.getAttribute("user");
        if (currentUser == null) {
            return "redirect:/login";
        }
        boolean isAdmin = currentUser.getRole() == User.Role.ADMIN;
        try {
            coachReviewService.deleteReview(reviewId, currentUser, isAdmin);
            redirectAttributes.addFlashAttribute("successMessage", "Отзыв удален");
        } catch (IllegalArgumentException e) {
            redirectAttributes.addFlashAttribute("errorMessage", e.getMessage());
        }
        return "redirect:/coaches";
    }
}