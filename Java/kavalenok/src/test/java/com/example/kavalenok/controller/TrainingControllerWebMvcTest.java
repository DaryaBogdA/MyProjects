package com.example.kavalenok.controller;

import com.example.kavalenok.model.Training;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.TrainingParticipantRepository;
import com.example.kavalenok.repository.TrainingRepository;
import com.example.kavalenok.repository.UserRepository;
import com.example.kavalenok.service.MessageService;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.webmvc.test.autoconfigure.WebMvcTest;
import org.springframework.test.context.bean.override.mockito.MockitoBean;
import org.springframework.test.web.servlet.MockMvc;

import java.time.LocalDateTime;
import java.util.List;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyLong;
import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.content;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@WebMvcTest(controllers = TrainingController.class)
class TrainingControllerWebMvcTest {

    @Autowired
    private MockMvc mockMvc;

    @MockitoBean
    private TrainingRepository trainingRepository;
    @MockitoBean
    private TrainingParticipantRepository trainingParticipantRepository;
    @MockitoBean
    private UserRepository userRepository;
    @MockitoBean
    private MessageService messageService;

    @Test
    void trainingsPageShowsPastTrainingsToo() throws Exception {
        User coach = new User();
        coach.setId(3L);
        coach.setFirstName("Иван");
        coach.setLastName("Петров");

        Training pastTraining = new Training();
        pastTraining.setId(1L);
        pastTraining.setTitle("Тренировка прошлого месяца");
        pastTraining.setDateTime(LocalDateTime.now().minusMonths(1));
        pastTraining.setCoach(coach);
        pastTraining.setCurrentParticipants(0);

        when(trainingRepository.findTrainingsWithFilters(any(), any(), any(), any(), any())).thenReturn(List.of(pastTraining));
        when(trainingParticipantRepository.countByTrainingId(anyLong())).thenReturn(0L);
        when(userRepository.findAllCoaches()).thenReturn(List.of(coach));

        mockMvc.perform(get("/trainings"))
                .andExpect(status().isOk())
                .andExpect(content().string(org.hamcrest.Matchers.containsString("Тренировка прошлого месяца")));
    }
}
