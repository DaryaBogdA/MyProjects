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
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.http.ContentDisposition;
import java.nio.charset.StandardCharsets;
import java.time.format.DateTimeFormatter;
import java.time.LocalDateTime;
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

    @GetMapping("/profile/export-trainings")
    public ResponseEntity<byte[]> exportUserTrainings(HttpSession session) {
        User user = (User) session.getAttribute("user");
        if (user == null) {
            return ResponseEntity.status(HttpStatus.FOUND)
                    .header("Location", "/login")
                    .build();
        }

        List<TrainingParticipant> participations = trainingParticipantRepository
                .findByUserIdOrderByAppliedAtDesc(user.getId());

        StringBuilder csv = new StringBuilder();
        csv.append("Название,Дата,Время,Тренер,Статус\n");

        DateTimeFormatter dateFormatter = DateTimeFormatter.ofPattern("dd.MM.yyyy");
        DateTimeFormatter timeFormatter = DateTimeFormatter.ofPattern("HH:mm");

        for (TrainingParticipant tp : participations) {
            Training t = tp.getTraining();
            String title = escapeCsv(t.getTitle());
            String date = t.getDateTime() != null ? t.getDateTime().format(dateFormatter) : "";
            String time = t.getDateTime() != null ? t.getDateTime().format(timeFormatter) : "";
            String coachName = escapeCsv(t.getCoach().getFirstName() + " " + t.getCoach().getLastName());
            String status = tp.getStatus() != null ? tp.getStatus().name() : "";

            csv.append(String.format("%s,%s,%s,%s,%s\n", title, date, time, coachName, status));
        }

        byte[] bytes = csv.toString().getBytes(StandardCharsets.UTF_8);

        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(new MediaType("text", "plain", StandardCharsets.UTF_8));
        headers.setContentDisposition(ContentDisposition.builder("attachment")
                .filename("my_trainings.csv", StandardCharsets.UTF_8)
                .build());

        return new ResponseEntity<>(bytes, headers, HttpStatus.OK);
    }

    private String escapeCsv(String value) {
        if (value == null) return "";
        if (value.contains(",") || value.contains("\"") || value.contains("\n")) {
            value = value.replace("\"", "\"\"");
            return "\"" + value + "\"";
        }
        return value;
    }



}
