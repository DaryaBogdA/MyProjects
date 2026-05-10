package com.event.arena.controller;

import com.event.arena.entity.Role;
import com.event.arena.entity.SupportMessage;
import com.event.arena.entity.User;
import com.event.arena.repository.SupportMessageRepository;
import com.event.arena.repository.UserRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.*;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/chat")
public class ChatController {
    private final SupportMessageRepository supportMessageRepository;
    private final UserRepository userRepository;

    public ChatController(SupportMessageRepository supportMessageRepository, UserRepository userRepository) {
        this.supportMessageRepository = supportMessageRepository;
        this.userRepository = userRepository;
    }

    @GetMapping("/messages")
    public ResponseEntity<?> getMessages(@AuthenticationPrincipal User currentUser,
                                         @RequestParam(required = false) Long userId) {
        if (currentUser == null) {
            return ResponseEntity.status(401).body(Map.of("error", "Требуется авторизация"));
        }
        User other;
        if (currentUser.getRole() == Role.ADMIN) {
            if (userId == null) {
                return ResponseEntity.ok(Collections.emptyList());
            }
            other = userRepository.findById(userId).orElse(null);
        } else {
            other = userRepository.findFirstByRole(Role.ADMIN).orElse(null);
        }
        if (other == null) {
            return ResponseEntity.ok(Collections.emptyList());
        }
        List<SupportMessage> conversation = supportMessageRepository.findConversation(currentUser.getId(), other.getId());
        List<Map<String, Object>> result = conversation.stream().map(m -> {
            Map<String, Object> item = new HashMap<>();
            item.put("id", m.getId());
            item.put("message", m.getMessage());
            item.put("createdAt", m.getCreatedAt());
            item.put("mine", Objects.equals(m.getSender().getId(), currentUser.getId()));
            return item;
        }).collect(Collectors.toList());
        return ResponseEntity.ok(result);
    }

    @GetMapping("/conversations")
    public ResponseEntity<?> getConversationUsers(@AuthenticationPrincipal User currentUser) {
        if (currentUser == null || currentUser.getRole() != Role.ADMIN) {
            return ResponseEntity.status(403).body(Map.of("error", "Доступ только для администратора"));
        }
        List<SupportMessage> all = supportMessageRepository.findBySenderIdOrRecipientIdOrderByCreatedAtAsc(currentUser.getId(), currentUser.getId());
        Map<Long, Map<String, Object>> uniqueUsers = new LinkedHashMap<>();
        for (SupportMessage message : all) {
            User candidate = Objects.equals(message.getSender().getId(), currentUser.getId()) ? message.getRecipient() : message.getSender();
            if (candidate.getRole() == Role.ADMIN) {
                continue;
            }
            uniqueUsers.put(candidate.getId(), Map.of(
                    "id", candidate.getId(),
                    "email", candidate.getEmail(),
                    "fullName", ((candidate.getFirstName() == null ? "" : candidate.getFirstName()) + " " + (candidate.getLastName() == null ? "" : candidate.getLastName())).trim()
            ));
        }
        return ResponseEntity.ok(uniqueUsers.values());
    }

    @PostMapping("/messages")
    public ResponseEntity<?> sendMessage(@AuthenticationPrincipal User currentUser,
                                         @RequestBody Map<String, Object> payload) {
        if (currentUser == null) {
            return ResponseEntity.status(401).body(Map.of("error", "Требуется авторизация"));
        }
        String text = payload.get("message") == null ? "" : payload.get("message").toString().trim();
        if (text.isEmpty()) {
            return ResponseEntity.badRequest().body(Map.of("error", "Сообщение пустое"));
        }
        User recipient;
        if (currentUser.getRole() == Role.ADMIN) {
            Object userId = payload.get("userId");
            if (userId == null) {
                return ResponseEntity.badRequest().body(Map.of("error", "Выберите пользователя"));
            }
            recipient = userRepository.findById(Long.valueOf(userId.toString())).orElse(null);
        } else {
            recipient = userRepository.findFirstByRole(Role.ADMIN).orElse(null);
        }
        if (recipient == null) {
            return ResponseEntity.badRequest().body(Map.of("error", "Получатель не найден"));
        }
        SupportMessage message = new SupportMessage();
        message.setSender(currentUser);
        message.setRecipient(recipient);
        message.setMessage(text);
        message.setCreatedAt(LocalDateTime.now());
        supportMessageRepository.save(message);
        return ResponseEntity.ok(Map.of("message", "Отправлено"));
    }
}
