package com.example.kavalenok.controller;

import com.example.kavalenok.model.Announcement;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.AnnouncementRepository;
import com.example.kavalenok.service.MessageService;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.webmvc.test.autoconfigure.WebMvcTest;
import org.springframework.test.context.bean.override.mockito.MockitoBean;
import org.springframework.test.web.servlet.MockMvc;

import java.time.LocalDateTime;
import java.util.List;

import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.content;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@WebMvcTest(controllers = AnnouncementController.class)
class AnnouncementControllerWebMvcTest {

    @Autowired
    private MockMvc mockMvc;

    @MockitoBean
    private AnnouncementRepository announcementRepository;
    @MockitoBean
    private MessageService messageService;

    @Test
    void announcementsPageRendersAuthorName() throws Exception {
        User author = new User();
        author.setId(2L);
        author.setFirstName("Павел");
        author.setLastName("Сидоров");

        Announcement announcement = new Announcement();
        announcement.setId(1L);
        announcement.setAuthor(author);
        announcement.setTitle("Ищем игрока");
        announcement.setContent("На пятницу нужен либеро");
        announcement.setCreatedAt(LocalDateTime.now());
        announcement.setIsActive(true);

        when(announcementRepository.findAllByIsActiveTrueOrderByCreatedAtDesc()).thenReturn(List.of(announcement));

        mockMvc.perform(get("/announcements"))
                .andExpect(status().isOk())
                .andExpect(content().string(org.hamcrest.Matchers.containsString("Ищем игрока")))
                .andExpect(content().string(org.hamcrest.Matchers.containsString("Павел")));
    }
}
