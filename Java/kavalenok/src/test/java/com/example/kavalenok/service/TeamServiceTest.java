package com.example.kavalenok.service;

import com.example.kavalenok.model.Team;
import com.example.kavalenok.model.TeamMember;
import com.example.kavalenok.model.TeamMemberStatus;
import com.example.kavalenok.model.TeamRole;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.TeamMemberRepository;
import com.example.kavalenok.repository.TeamRepository;
import com.example.kavalenok.repository.UserRepository;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.util.Optional;

import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

@ExtendWith(MockitoExtension.class)
class TeamServiceTest {

    @Mock
    private TeamRepository teamRepository;
    @Mock
    private TeamMemberRepository teamMemberRepository;
    @Mock
    private UserRepository userRepository;

    @InjectMocks
    private TeamService teamService;

    @Test
    void applyToTeamThrowsWhenUserAlreadyPending() {
        User user = new User();
        user.setId(1L);

        Team team = new Team();
        team.setId(10L);

        when(teamRepository.findById(10L)).thenReturn(Optional.of(team));
        when(teamMemberRepository.existsByTeamIdAndUserIdAndStatus(10L, 1L, TeamMemberStatus.PENDING)).thenReturn(true);

        IllegalArgumentException exception = assertThrows(IllegalArgumentException.class,
                () -> teamService.applyToTeam(10L, user, TeamRole.SETTER));

        assertEquals("Вы уже отправили заявку или уже в команде", exception.getMessage());
    }

    @Test
    void approveRequestWorksWhenCaptainAndRoleAvailable() {
        User captain = new User();
        captain.setId(5L);

        Team team = new Team();
        team.setId(10L);
        team.setCaptain(captain);

        User candidate = new User();
        candidate.setId(11L);

        TeamMember request = new TeamMember();
        request.setTeam(team);
        request.setUser(candidate);
        request.setRole(TeamRole.SETTER);
        request.setStatus(TeamMemberStatus.PENDING);

        when(teamRepository.findById(10L)).thenReturn(Optional.of(team));
        when(teamMemberRepository.findByTeamIdAndUserId(10L, 11L)).thenReturn(Optional.of(request));
        when(teamMemberRepository.countApprovedByTeamAndRole(10L, TeamRole.SETTER)).thenReturn(0L);
        when(teamMemberRepository.save(any(TeamMember.class))).thenAnswer(invocation -> invocation.getArgument(0));

        assertDoesNotThrow(() -> teamService.approveRequest(10L, 11L, captain));
        assertEquals(TeamMemberStatus.APPROVED, request.getStatus());
    }
}
