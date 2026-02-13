package com.example.kavalenok.controller;

import com.example.kavalenok.model.Message;
import com.example.kavalenok.model.User;
import com.example.kavalenok.service.MessageService;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import java.util.List;

@Controller
public class MessageController {

    private final MessageService messageService;

    public MessageController(MessageService messageService) {
        this.messageService = messageService;
    }

    @GetMapping("/messages")
    public String conversations(HttpSession session, Model model) {
        User user = (User) session.getAttribute("user");
        if (user == null) {
            return "redirect:/login";
        }
        model.addAttribute("conversations", messageService.getConversations(user));
        return "messages";
    }

    @GetMapping("/messages/{friendId}")
    public String conversation(@PathVariable Long friendId,
                               HttpSession session,
                               Model model,
                               RedirectAttributes redirectAttributes) {
        User user = (User) session.getAttribute("user");
        if (user == null) {
            return "redirect:/login";
        }

        try {
            List<Message> messages = messageService.getConversation(user, friendId);
            messageService.markConversationAsRead(user, friendId);
            model.addAttribute("messages", messages);
            model.addAttribute("friendId", friendId);
            model.addAttribute("friendName", getFriendName(messages, user, friendId));
            return "conversation";
        } catch (Exception e) {
            redirectAttributes.addFlashAttribute("errorMessage", "Не удалось загрузить переписку");
            return "redirect:/messages";
        }
    }

    @PostMapping("/messages/send")
    public String sendMessage(@RequestParam Long receiverId,
                              @RequestParam String content,
                              HttpSession session,
                              RedirectAttributes redirectAttributes) {
        User sender = (User) session.getAttribute("user");
        if (sender == null) {
            return "redirect:/login";
        }

        if (content == null || content.trim().isEmpty()) {
            redirectAttributes.addFlashAttribute("errorMessage", "Сообщение не может быть пустым");
            return "redirect:/messages/" + receiverId;
        }

        try {
            messageService.sendMessage(sender, receiverId, content.trim());
        } catch (IllegalArgumentException e) {
            redirectAttributes.addFlashAttribute("errorMessage", e.getMessage());
        }
        return "redirect:/messages/" + receiverId;
    }

    private String getFriendName(List<Message> messages, User currentUser, Long friendId) {
        if (!messages.isEmpty()) {
            Message first = messages.get(0);
            if (first.getSender().getId().equals(currentUser.getId())) {
                return first.getReceiver().getFirstName() + " " + first.getReceiver().getLastName();
            } else {
                return first.getSender().getFirstName() + " " + first.getSender().getLastName();
            }
        }
        return "Пользователь";
    }
}