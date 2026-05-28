package com.example.kavalenok.controller;

import com.example.kavalenok.model.ActionLog;
import com.example.kavalenok.model.CoachNote;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.ActionLogRepository;
import com.example.kavalenok.repository.CoachNoteRepository;
import com.example.kavalenok.repository.FriendRepository;
import com.example.kavalenok.repository.UserRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import java.util.List;

@Controller
public class CoachNoteController {

    private final CoachNoteRepository coachNoteRepository;
    private final FriendRepository friendRepository;
    private final UserRepository userRepository;
    private final ActionLogRepository actionLogRepository;

    public CoachNoteController(CoachNoteRepository coachNoteRepository,
                               FriendRepository friendRepository,
                               UserRepository userRepository,
                               ActionLogRepository actionLogRepository) {
        this.coachNoteRepository = coachNoteRepository;
        this.friendRepository = friendRepository;
        this.userRepository = userRepository;
        this.actionLogRepository = actionLogRepository;
    }

    @GetMapping("/coach/notes")
    public String notesPage(HttpSession session, Model model) {
        User coach = (User) session.getAttribute("user");
        if (coach == null) {
            return "redirect:/login";
        }
        if (coach.getRole() != User.Role.COACH) {
            return "redirect:/user";
        }

        List<User> friendPlayers = friendRepository.findAllFriends(coach.getId()).stream()
                .map(f -> f.getUser1().getId().equals(coach.getId()) ? f.getUser2() : f.getUser1())
                .toList();
        model.addAttribute("friendPlayers", friendPlayers);
        model.addAttribute("notes", coachNoteRepository.findByCoachIdOrderByCreatedAtDesc(coach.getId()));
        return "coach-notes";
    }

    @PostMapping("/coach/notes/save")
    public String saveNote(@RequestParam Long playerId,
                           @RequestParam String note,
                           HttpSession session,
                           RedirectAttributes redirectAttributes) {
        User coach = (User) session.getAttribute("user");
        if (coach == null) {
            return "redirect:/login";
        }
        if (coach.getRole() != User.Role.COACH) {
            return "redirect:/user";
        }
        if (note == null || note.isBlank()) {
            redirectAttributes.addFlashAttribute("errorMessage", "Текст заметки пустой");
            return "redirect:/coach/notes";
        }
        User player = userRepository.findById(playerId).orElse(null);
        if (player == null) {
            redirectAttributes.addFlashAttribute("errorMessage", "Игрок не найден");
            return "redirect:/coach/notes";
        }

        CoachNote coachNote = coachNoteRepository.findByCoachIdAndPlayerId(coach.getId(), playerId)
                .orElseGet(() -> new CoachNote(coach, player, note.trim()));
        coachNote.setNote(note.trim());
        coachNoteRepository.save(coachNote);

        actionLogRepository.save(new ActionLog(
                coach,
                "COACH_NOTE_SAVED",
                "Заметка о пользователе ID=" + playerId
        ));
        redirectAttributes.addFlashAttribute("successMessage", "Заметка сохранена");
        return "redirect:/coach/notes";
    }
}
