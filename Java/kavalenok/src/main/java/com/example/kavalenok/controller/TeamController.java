package com.example.kavalenok.controller;

import com.example.kavalenok.model.*;
import com.example.kavalenok.repository.TeamMemberRepository;
import com.example.kavalenok.repository.TeamRepository;
import com.example.kavalenok.repository.UserRepository;
import com.example.kavalenok.service.TeamService;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import java.util.List;

@Controller
@RequestMapping("/teams")
public class TeamController {

    private final TeamService teamService;
    private final TeamRepository teamRepository;
    private final TeamMemberRepository teamMemberRepository;
    private final UserRepository userRepository;

    public TeamController(TeamService teamService,
                          TeamRepository teamRepository,
                          TeamMemberRepository teamMemberRepository,
                          UserRepository userRepository) {
        this.teamService = teamService;
        this.teamRepository = teamRepository;
        this.teamMemberRepository = teamMemberRepository;
        this.userRepository = userRepository;
    }

    @GetMapping
    public String listTeams(Model model) {
        List<Team> teams = teamRepository.findAllByOrderByCreatedAtDesc();
        model.addAttribute("teams", teams);
        return "teams";
    }

    @GetMapping("/create")
    public String createForm(HttpSession session, Model model) {
        User user = (User) session.getAttribute("user");
        if (user == null) {
            return "redirect:/login";
        }
        if (user.getRole() != User.Role.COACH) {
            return "redirect:/teams?error=onlyCoach";
        }
        return "teamCreate";
    }

    @PostMapping("/create")
    public String createTeam(@RequestParam String name,
                             @RequestParam(required = false) String description,
                             HttpSession session,
                             RedirectAttributes redirectAttributes) {
        User coach = (User) session.getAttribute("user");
        if (coach == null) {
            return "redirect:/login";
        }
        if (coach.getRole() != User.Role.COACH) {
            redirectAttributes.addFlashAttribute("errorMessage", "Только тренер может создать команду");
            return "redirect:/teams";
        }
        try {
            Team team = teamService.createTeam(name, description, coach);
            redirectAttributes.addFlashAttribute("successMessage", "Команда успешно создана!");
            return "redirect:/teams/" + team.getId();
        } catch (Exception e) {
            redirectAttributes.addFlashAttribute("errorMessage", "Ошибка: " + e.getMessage());
            return "redirect:/teams/create";
        }
    }

    @GetMapping("/{id}")
    public String teamDetail(@PathVariable Long id,
                             HttpSession session,
                             Model model,
                             RedirectAttributes redirectAttributes) {
        User currentUser = (User) session.getAttribute("user");
        try {
            TeamService.TeamWithMembers teamData = teamService.getTeamWithMembers(id);
            model.addAttribute("teamData", teamData);
            model.addAttribute("currentUser", currentUser);
            model.addAttribute("roleLimits", TeamService.ROLE_LIMITS);

            boolean isCaptain = currentUser != null &&
                    teamData.getTeam().getCaptain().getId().equals(currentUser.getId());
            model.addAttribute("isCaptain", isCaptain);

            if (currentUser != null) {
                var existing = teamMemberRepository.findByTeamIdAndUserId(id, currentUser.getId());
                model.addAttribute("existingMembership", existing.orElse(null));
            }

            if (isCaptain) {
                List<User> coaches = userRepository.findAllCoaches();
                model.addAttribute("coaches", coaches);
            }

            return "teamDetail";
        } catch (Exception e) {
            redirectAttributes.addFlashAttribute("errorMessage", e.getMessage());
            return "redirect:/teams";
        }
    }

    @PostMapping("/{id}/apply")
    public String applyToTeam(@PathVariable Long id,
                              @RequestParam TeamRole role,
                              HttpSession session,
                              RedirectAttributes redirectAttributes) {
        User user = (User) session.getAttribute("user");
        if (user == null) {
            return "redirect:/login";
        }
        try {
            teamService.applyToTeam(id, user, role);
            redirectAttributes.addFlashAttribute("successMessage", "Заявка отправлена!");
        } catch (Exception e) {
            redirectAttributes.addFlashAttribute("errorMessage", e.getMessage());
        }
        return "redirect:/teams/" + id;
    }

    @PostMapping("/{teamId}/approve/{userId}")
    public String approveRequest(@PathVariable Long teamId,
                                 @PathVariable Long userId,
                                 HttpSession session,
                                 RedirectAttributes redirectAttributes) {
        User captain = (User) session.getAttribute("user");
        if (captain == null) return "redirect:/login";
        try {
            teamService.approveRequest(teamId, userId, captain);
            redirectAttributes.addFlashAttribute("successMessage", "Заявка одобрена");
        } catch (Exception e) {
            redirectAttributes.addFlashAttribute("errorMessage", e.getMessage());
        }
        return "redirect:/teams/" + teamId;
    }

    @PostMapping("/{teamId}/reject/{userId}")
    public String rejectRequest(@PathVariable Long teamId,
                                @PathVariable Long userId,
                                HttpSession session,
                                RedirectAttributes redirectAttributes) {
        User captain = (User) session.getAttribute("user");
        if (captain == null) return "redirect:/login";
        try {
            teamService.rejectRequest(teamId, userId, captain);
            redirectAttributes.addFlashAttribute("successMessage", "Заявка отклонена");
        } catch (Exception e) {
            redirectAttributes.addFlashAttribute("errorMessage", e.getMessage());
        }
        return "redirect:/teams/" + teamId;
    }

    @PostMapping("/{teamId}/assign-coach")
    public String assignCoach(@PathVariable Long teamId,
                              @RequestParam Long coachId,
                              HttpSession session,
                              RedirectAttributes redirectAttributes) {
        User captain = (User) session.getAttribute("user");
        if (captain == null) return "redirect:/login";
        try {
            teamService.assignCoach(teamId, coachId, captain);
            redirectAttributes.addFlashAttribute("successMessage", "Тренер назначен");
        } catch (Exception e) {
            redirectAttributes.addFlashAttribute("errorMessage", e.getMessage());
        }
        return "redirect:/teams/" + teamId;
    }

    @PostMapping("/{teamId}/remove/{userId}")
    public String removeMember(@PathVariable Long teamId,
                               @PathVariable Long userId,
                               HttpSession session,
                               RedirectAttributes redirectAttributes) {
        User captain = (User) session.getAttribute("user");
        if (captain == null) return "redirect:/login";
        try {
            teamService.removeMember(teamId, userId, captain);
            redirectAttributes.addFlashAttribute("successMessage", "Участник удалён");
        } catch (Exception e) {
            redirectAttributes.addFlashAttribute("errorMessage", e.getMessage());
        }
        return "redirect:/teams/" + teamId;
    }
}