package com.example.kavalenok.controller;

import com.example.kavalenok.model.User;
import com.example.kavalenok.service.MessageService;
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

@WebMvcTest(controllers = MessageController.class)
class MessageControllerWebMvcTest {

    @Autowired
    private MockMvc mockMvc;

    @MockitoBean
    private MessageService messageService;

    @Test
    void messagesPageRendersConversationPreview() throws Exception {
        User currentUser = new User();
        currentUser.setId(1L);
        currentUser.setFirstName("Текущий");
        currentUser.setLastName("Пользователь");

        User friend = new User();
        friend.setId(2L);
        friend.setFirstName("Мария");
        friend.setLastName("Смирнова");
        friend.setAvatarUrl("/img/avatars/2.png");

        MessageService.ConversationPreview preview =
                new MessageService.ConversationPreview(friend, null, 0);

        when(messageService.getConversations(currentUser)).thenReturn(List.of(preview));
        when(messageService.getUnreadCount(1L)).thenReturn(0);

        mockMvc.perform(get("/messages").sessionAttr("user", currentUser))
                .andExpect(status().isOk())
                .andExpect(content().string(org.hamcrest.Matchers.containsString("Мария")));
    }
}
