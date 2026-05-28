package com.example.kavalenok.controller;

import com.example.kavalenok.model.Team;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.TeamMemberRepository;
import com.example.kavalenok.repository.TeamRepository;
import com.example.kavalenok.repository.UserRepository;
import com.example.kavalenok.service.MessageService;
import com.example.kavalenok.service.TeamService;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.webmvc.test.autoconfigure.WebMvcTest;
import org.springframework.test.context.bean.override.mockito.MockitoBean;
import org.springframework.test.web.servlet.MockMvc;

import java.util.List;

import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.content;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@WebMvcTest(controllers = TeamController.class)
class TeamControllerWebMvcTest {

    @Autowired
    private MockMvc mockMvc;

    @MockitoBean
    private TeamService teamService;
    @MockitoBean
    private TeamRepository teamRepository;
    @MockitoBean
    private TeamMemberRepository teamMemberRepository;
    @MockitoBean
    private UserRepository userRepository;
    @MockitoBean
    private MessageService messageService;

    @Test
    void teamsPageLoadsAndShowsTeamName() throws Exception {
        User captain = new User();
        captain.setId(1L);
        captain.setFirstName("Иван");
        captain.setLastName("Петров");

        Team team = new Team();
        team.setId(10L);
        team.setName("Волики");
        team.setCaptain(captain);
        team.setDescription("Описание");

        when(teamRepository.findAllWithCaptainOrderByCreatedAtDesc()).thenReturn(List.of(team));
        when(messageService.getUnreadCount(1L)).thenReturn(0);

        mockMvc.perform(get("/teams"))
                .andExpect(status().isOk())
                .andExpect(content().string(org.hamcrest.Matchers.containsString("Волики")));
    }
}
