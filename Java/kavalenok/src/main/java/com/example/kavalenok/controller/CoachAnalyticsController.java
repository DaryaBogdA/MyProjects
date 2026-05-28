package com.example.kavalenok.controller;

import com.example.kavalenok.model.Training;
import com.example.kavalenok.model.TrainingParticipant;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.TrainingParticipantRepository;
import com.example.kavalenok.repository.TrainingRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;

import java.time.LocalDateTime;
import java.util.List;

@Controller
public class CoachAnalyticsController {

    private final TrainingRepository trainingRepository;
    private final TrainingParticipantRepository trainingParticipantRepository;

    public CoachAnalyticsController(TrainingRepository trainingRepository,
                                    TrainingParticipantRepository trainingParticipantRepository) {
        this.trainingRepository = trainingRepository;
        this.trainingParticipantRepository = trainingParticipantRepository;
    }

    @GetMapping("/coach/analytics")
    public String analytics(HttpSession session, Model model) {
        User coach = (User) session.getAttribute("user");
        if (coach == null) {
            return "redirect:/login";
        }
        if (coach.getRole() != User.Role.COACH) {
            return "redirect:/user";
        }

        List<Training> trainings = trainingRepository.findByCoachIdOrderByDateTimeDesc(coach.getId());
        long totalTrainings = trainings.size();
        long upcomingTrainings = trainings.stream()
                .filter(t -> t.getDateTime() != null && t.getDateTime().isAfter(LocalDateTime.now()))
                .count();
        long totalApplications = trainingParticipantRepository.countAllByCoachId(coach.getId());
        long approved = trainingParticipantRepository.countAllByCoachIdAndStatus(coach.getId(), TrainingParticipant.Status.APPROVED);
        long attended = trainingParticipantRepository.countAllByCoachIdAndStatus(coach.getId(), TrainingParticipant.Status.ATTENDED);

        model.addAttribute("totalTrainings", totalTrainings);
        model.addAttribute("upcomingTrainings", upcomingTrainings);
        model.addAttribute("totalApplications", totalApplications);
        model.addAttribute("approvedApplications", approved);
        model.addAttribute("attendedApplications", attended);
        model.addAttribute("trainings", trainings.stream().limit(10).toList());
        return "coach-analytics";
    }
}
