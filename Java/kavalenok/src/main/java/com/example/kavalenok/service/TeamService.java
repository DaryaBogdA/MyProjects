package com.example.kavalenok.service;

import com.example.kavalenok.model.*;
import com.example.kavalenok.repository.TeamMemberRepository;
import com.example.kavalenok.repository.TeamRepository;
import com.example.kavalenok.repository.UserRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@Service
public class TeamService {

    private final TeamRepository teamRepository;
    private final TeamMemberRepository teamMemberRepository;
    private final UserRepository userRepository;

    public static final Map<TeamRole, Integer> ROLE_LIMITS = new HashMap<>();
    static {
        ROLE_LIMITS.put(TeamRole.SETTER, 1);
        ROLE_LIMITS.put(TeamRole.LIBERO, 1);
        ROLE_LIMITS.put(TeamRole.MIDDLE_BLOCKER, 2);
        ROLE_LIMITS.put(TeamRole.OUTSIDE_HITTER, 2);
        ROLE_LIMITS.put(TeamRole.OPPOSITE, 1);
    }

    public TeamService(TeamRepository teamRepository,
                       TeamMemberRepository teamMemberRepository,
                       UserRepository userRepository) {
        this.teamRepository = teamRepository;
        this.teamMemberRepository = teamMemberRepository;
        this.userRepository = userRepository;
    }

    @Transactional
    public Team createTeam(String name, String description, User coach) {
        Team team = new Team(name, coach);
        team.setDescription(description);
        return teamRepository.save(team);
    }

    public boolean isRoleAvailable(Long teamId, TeamRole role) {
        long current = teamMemberRepository.countApprovedByTeamAndRole(teamId, role);
        return current < ROLE_LIMITS.get(role);
    }

    @Transactional
    public void applyToTeam(Long teamId, User applicant, TeamRole role) {
        Team team = teamRepository.findById(teamId)
                .orElseThrow(() -> new IllegalArgumentException("Команда не найдена"));

        if (teamMemberRepository.existsByTeamIdAndUserIdAndStatus(teamId, applicant.getId(), TeamMemberStatus.PENDING) ||
                teamMemberRepository.existsByTeamIdAndUserIdAndStatus(teamId, applicant.getId(), TeamMemberStatus.APPROVED)) {
            throw new IllegalArgumentException("Вы уже отправили заявку или уже в команде");
        }

        TeamMember application = new TeamMember(team, applicant, role);
        application.setStatus(TeamMemberStatus.PENDING);
        teamMemberRepository.save(application);
    }

    @Transactional
    public void approveRequest(Long teamId, Long userId, User captain) {
        Team team = teamRepository.findById(teamId)
                .orElseThrow(() -> new IllegalArgumentException("Команда не найдена"));
        if (!team.getCaptain().getId().equals(captain.getId())) {
            throw new IllegalArgumentException("Только капитан может одобрять заявки");
        }

        TeamMember request = teamMemberRepository.findByTeamIdAndUserId(teamId, userId)
                .orElseThrow(() -> new IllegalArgumentException("Заявка не найдена"));

        if (request.getStatus() != TeamMemberStatus.PENDING) {
            throw new IllegalArgumentException("Заявка уже обработана");
        }

        if (!isRoleAvailable(teamId, request.getRole())) {
            throw new IllegalArgumentException("Достигнут лимит для позиции " + request.getRole());
        }

        request.setStatus(TeamMemberStatus.APPROVED);
        request.setRespondedAt(java.time.LocalDateTime.now());
        teamMemberRepository.save(request);
    }

    @Transactional
    public void rejectRequest(Long teamId, Long userId, User captain) {
        Team team = teamRepository.findById(teamId)
                .orElseThrow(() -> new IllegalArgumentException("Команда не найдена"));
        if (!team.getCaptain().getId().equals(captain.getId())) {
            throw new IllegalArgumentException("Только капитан может отклонять заявки");
        }

        TeamMember request = teamMemberRepository.findByTeamIdAndUserId(teamId, userId)
                .orElseThrow(() -> new IllegalArgumentException("Заявка не найдена"));

        if (request.getStatus() != TeamMemberStatus.PENDING) {
            throw new IllegalArgumentException("Заявка уже обработана");
        }

        request.setStatus(TeamMemberStatus.REJECTED);
        request.setRespondedAt(java.time.LocalDateTime.now());
        teamMemberRepository.save(request);
    }

    @Transactional
    public void assignCoach(Long teamId, Long coachId, User captain) {
        Team team = teamRepository.findById(teamId)
                .orElseThrow(() -> new IllegalArgumentException("Команда не найдена"));
        if (!team.getCaptain().getId().equals(captain.getId())) {
            throw new IllegalArgumentException("Только капитан может назначить тренера");
        }

        User coach = userRepository.findById(coachId)
                .orElseThrow(() -> new IllegalArgumentException("Пользователь не найден"));
        if (coach.getRole() != User.Role.COACH) {
            throw new IllegalArgumentException("Пользователь не является тренером");
        }

        team.setCoach(coach);
        teamRepository.save(team);
    }

    @Transactional
    public void removeMember(Long teamId, Long userId, User captain) {
        Team team = teamRepository.findById(teamId)
                .orElseThrow(() -> new IllegalArgumentException("Команда не найдена"));
        if (!team.getCaptain().getId().equals(captain.getId())) {
            throw new IllegalArgumentException("Только капитан может удалять участников");
        }
        if (captain.getId().equals(userId)) {
            throw new IllegalArgumentException("Капитан не может удалить сам себя (передайте капитана)");
        }

        TeamMember member = teamMemberRepository.findByTeamIdAndUserId(teamId, userId)
                .orElseThrow(() -> new IllegalArgumentException("Участник не найден"));
        teamMemberRepository.delete(member);
    }

    public TeamWithMembers getTeamWithMembers(Long teamId) {
        Team team = teamRepository.findById(teamId)
                .orElseThrow(() -> new IllegalArgumentException("Команда не найдена"));
        List<TeamMember> approved = teamMemberRepository.findByTeamIdAndStatusOrderByRoleAsc(teamId, TeamMemberStatus.APPROVED);
        List<TeamMember> pending = teamMemberRepository.findByTeamIdAndStatus(teamId, TeamMemberStatus.PENDING);

        return new TeamWithMembers(team, approved, pending);
    }

    public static class TeamWithMembers {
        private final Team team;
        private final List<TeamMember> approvedMembers;
        private final List<TeamMember> pendingRequests;

        public TeamWithMembers(Team team, List<TeamMember> approvedMembers, List<TeamMember> pendingRequests) {
            this.team = team;
            this.approvedMembers = approvedMembers;
            this.pendingRequests = pendingRequests;
        }

        public Team getTeam() { return team; }
        public List<TeamMember> getApprovedMembers() { return approvedMembers; }
        public List<TeamMember> getPendingRequests() { return pendingRequests; }

        public Map<TeamRole, Integer> getRoleCounts() {
            Map<TeamRole, Integer> counts = new HashMap<>();
            for (TeamRole role : TeamRole.values()) counts.put(role, 0);
            for (TeamMember m : approvedMembers) {
                counts.merge(m.getRole(), 1, Integer::sum);
            }
            return counts;
        }

        public boolean isRoleAvailable(TeamRole role) {
            int current = getRoleCounts().get(role);
            return current < ROLE_LIMITS.get(role);
        }
    }
}