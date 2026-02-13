package com.example.kavalenok.controller;

import com.example.kavalenok.model.User;
import com.example.kavalenok.service.MessageService;
import jakarta.servlet.http.HttpSession;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ModelAttribute;

@ControllerAdvice
public class GlobalControllerAdvice {

    private final MessageService messageService;

    public GlobalControllerAdvice(MessageService messageService) {
        this.messageService = messageService;
    }

    @ModelAttribute("unreadMessages")
    public Integer addUnreadCount(HttpSession session) {
        User user = (User) session.getAttribute("user");
        if (user != null) {
            return messageService.getUnreadCount(user.getId());
        }
        return 0;
    }
}