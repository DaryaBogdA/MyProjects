package com.example.kavalenok.controller;

import com.example.kavalenok.model.Training;
import com.example.kavalenok.model.TrainingParticipant;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.TrainingParticipantRepository;
import com.example.kavalenok.repository.TrainingRepository;
import com.example.kavalenok.repository.UserRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

@Controller
public class TrainingController {

    @Autowired
    private TrainingRepository trainingRepository;

    @Autowired
    private TrainingParticipantRepository trainingParticipantRepository;

    @Autowired
    private UserRepository userRepository;

    @GetMapping("/trainings")
    public String trainings(HttpSession session,
                            Model model,
                            @RequestParam(required = false) String title,
                            @RequestParam(required = false) String location,
                            @RequestParam(required = false) Long coachId) {

        List<Training> trainings = trainingRepository.findTrainingsWithFilters(title, location, coachId);

        User user = (User) session.getAttribute("user");

        Set<Long> registeredTrainingIds = new HashSet<>();
        try {
            if (user != null) {
                for (Training t : trainings) {
                    if (trainingParticipantRepository.existsByTrainingIdAndUserId(t.getId(), user.getId())) {
                        registeredTrainingIds.add(t.getId());
                    }
                }
            }
            for (Training t : trainings) {
                long count = trainingParticipantRepository.countByTrainingId(t.getId());
                t.setCurrentParticipants((int) count);
            }
        } catch (Exception e) {
            for (Training t : trainings) {
                if (t.getCurrentParticipants() == null) t.setCurrentParticipants(0);
            }
        }

        List<User> coaches = userRepository.findAllCoaches();

        model.addAttribute("trainings", trainings);
        model.addAttribute("user", user);
        model.addAttribute("registeredTrainingIds", registeredTrainingIds);
        model.addAttribute("coaches", coaches);

        model.addAttribute("filterTitle", title);
        model.addAttribute("filterLocation", location);
        model.addAttribute("filterCoachId", coachId);

        return "trainings";
    }

    @GetMapping("/trainings/create")
    public String createForm(HttpSession session, Model model) {
        User user = (User) session.getAttribute("user");
        if (user == null || user.getRole() != User.Role.COACH) {
            return "redirect:/trainings";
        }
        model.addAttribute("user", user);
        return "trainingCreate";
    }

    @PostMapping("/trainings/create")
    public String createTraining(HttpSession session,
                                 @RequestParam String title,
                                 @RequestParam String dateTime,
                                 @RequestParam(required = false) String location,
                                 @RequestParam(required = false) Integer maxParticipants,
                                 @RequestParam(required = false) Integer durationMinutes,
                                 RedirectAttributes redirectAttributes) {
        User user = (User) session.getAttribute("user");
        if (user == null || user.getRole() != User.Role.COACH) {
            return "redirect:/trainings";
        }

        try {
            LocalDateTime parsedDateTime = LocalDateTime.parse(dateTime,
                DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm"));
            Training training = new Training(user, title, parsedDateTime);
            training.setLocation(location != null && !location.isBlank() ? location.trim() : null);
            training.setMaxParticipants(maxParticipants != null && maxParticipants > 0 ? maxParticipants : null);
            training.setDurationMinutes(durationMinutes != null && durationMinutes > 0 ? durationMinutes : 60);

            trainingRepository.save(training);
            redirectAttributes.addFlashAttribute("successMessage", "Тренировка успешно добавлена!");
        } catch (Exception e) {
            redirectAttributes.addFlashAttribute("errorMessage", "Ошибка при создании тренировки: " + e.getMessage());
        }
        return "redirect:/trainings";
    }

    @PostMapping("/trainings/register")
    public String register(HttpSession session, @RequestParam Long trainingId, RedirectAttributes redirectAttributes) {
        User user = (User) session.getAttribute("user");
        if (user == null) {
            redirectAttributes.addFlashAttribute("errorMessage", "Войдите, чтобы записаться на тренировку");
            return "redirect:/login";
        }

        var trainingOpt = trainingRepository.findById(trainingId);
        if (trainingOpt.isEmpty()) {
            redirectAttributes.addFlashAttribute("errorMessage", "Тренировка не найдена");
            return "redirect:/trainings";
        }

        Training training = trainingOpt.get();
        if (trainingParticipantRepository.existsByTrainingIdAndUserId(trainingId, user.getId())) {
            redirectAttributes.addFlashAttribute("errorMessage", "Вы уже записаны на эту тренировку");
            return "redirect:/trainings";
        }

        if (training.getMaxParticipants() != null) {
            long count = trainingParticipantRepository.countByTrainingId(trainingId);
            if (count >= training.getMaxParticipants()) {
                redirectAttributes.addFlashAttribute("errorMessage", "Тренировка заполнена");
                return "redirect:/trainings";
            }
        }

        TrainingParticipant participant = new TrainingParticipant(training, user);
        participant.setStatus(TrainingParticipant.Status.APPROVED);
        trainingParticipantRepository.save(participant);

        training.setCurrentParticipants((int) trainingParticipantRepository.countByTrainingId(trainingId));
        trainingRepository.save(training);

        redirectAttributes.addFlashAttribute("successMessage", "Вы успешно записались на тренировку!");
        return "redirect:/trainings";
    }

    @PostMapping("/trainings/unregister")
    public String unregister(HttpSession session, @RequestParam Long trainingId, RedirectAttributes redirectAttributes) {
        User user = (User) session.getAttribute("user");
        if (user == null) {
            return "redirect:/login";
        }

        var participantOpt = trainingParticipantRepository.findByTrainingIdAndUserId(trainingId, user.getId());
        if (participantOpt.isPresent()) {
            trainingParticipantRepository.delete(participantOpt.get());
            var trainingOpt = trainingRepository.findById(trainingId);
            if (trainingOpt.isPresent()) {
                Training t = trainingOpt.get();
                t.setCurrentParticipants((int) trainingParticipantRepository.countByTrainingId(trainingId));
                trainingRepository.save(t);
            }
            redirectAttributes.addFlashAttribute("successMessage", "Вы отписались от тренировки");
        }

        return "redirect:/user";
    }
}
