package com.example.kavalenok.controller;

import com.example.kavalenok.model.CoachReview;
import com.example.kavalenok.model.User;
import com.example.kavalenok.service.CoachReviewService;
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

@WebMvcTest(controllers = CoachReviewController.class)
class CoachReviewControllerWebMvcTest {

    @Autowired
    private MockMvc mockMvc;

    @MockitoBean
    private CoachReviewService coachReviewService;
    @MockitoBean
    private MessageService messageService;

    @Test
    void coachReviewsPageRendersWithReviewerData() throws Exception {
        User reviewer = new User();
        reviewer.setId(8L);
        reviewer.setFirstName("Анна");
        reviewer.setLastName("Иванова");
        reviewer.setAvatarUrl("/img/avatars/1.png");

        User coach = new User();
        coach.setId(3L);

        CoachReview review = new CoachReview();
        review.setId(100L);
        review.setCoach(coach);
        review.setUser(reviewer);
        review.setRating(5);
        review.setComment("Отличный тренер");
        review.setCreatedAt(LocalDateTime.now());

        when(coachReviewService.getReviewsByCoach(3L)).thenReturn(List.of(review));

        mockMvc.perform(get("/coach/3/reviews"))
                .andExpect(status().isOk())
                .andExpect(content().string(org.hamcrest.Matchers.containsString("Отличный тренер")))
                .andExpect(content().string(org.hamcrest.Matchers.containsString("Анна")));
    }
}
