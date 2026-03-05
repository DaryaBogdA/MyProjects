package com.event.arena.controller;

import com.event.arena.entity.Event;
import com.event.arena.entity.Registration;
import com.event.arena.entity.RegistrationStatus;
import com.event.arena.entity.User;
import com.event.arena.repository.EventRepository;
import com.event.arena.repository.RegistrationRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Map;

@RestController
@RequestMapping("/registrations")
public class RegistrationController {

    private final RegistrationRepository registrationRepository;
    private final EventRepository eventRepository;

    public RegistrationController(RegistrationRepository registrationRepository, EventRepository eventRepository) {
        this.registrationRepository = registrationRepository;
        this.eventRepository = eventRepository;
    }

    @GetMapping("/my")
    public ResponseEntity<List<Long>> getMyEventIds(@AuthenticationPrincipal User user) {
        if (user == null) {
            return ResponseEntity.status(401).build();
        }
        List<Long> eventIds = registrationRepository.findEventIdsByUserAndStatus(user, RegistrationStatus.registered);
        return ResponseEntity.ok(eventIds);
    }

    @PostMapping("/{eventId}")
    public ResponseEntity<?> register(@AuthenticationPrincipal User user, @PathVariable Long eventId) {
        if (user == null) {
            return ResponseEntity.status(401).body(Map.of("error", "Необходимо войти в систему"));
        }

        Event event = eventRepository.findById(eventId).orElse(null);
        if (event == null) {
            return ResponseEntity.notFound().build();
        }

        if (registrationRepository.existsByUserAndEventAndStatus(user, event, RegistrationStatus.registered)) {
            return ResponseEntity.badRequest().body(Map.of("error", "Вы уже записаны на это мероприятие"));
        }

        int currentCount = (int) registrationRepository.countByEventAndStatus(event, RegistrationStatus.registered);
        if (currentCount >= event.getMaxParticipants()) {
            return ResponseEntity.badRequest().body(Map.of("error", "Все места заняты"));
        }

        Registration reg = new Registration(user, event, LocalDateTime.now());
        reg.setStatus(RegistrationStatus.registered);
        registrationRepository.save(reg);

        event.setCurrentParticipants(event.getCurrentParticipants() + 1);
        eventRepository.save(event);

        return ResponseEntity.ok(Map.of("message", "Вы успешно записались на мероприятие!"));
    }

    @DeleteMapping("/{eventId}")
    public ResponseEntity<?> cancelRegistration(@AuthenticationPrincipal User user, @PathVariable Long eventId) {
        if (user == null) {
            return ResponseEntity.status(401).body(Map.of("error", "Необходимо войти в систему"));
        }

        Event event = eventRepository.findById(eventId).orElse(null);
        if (event == null) {
            return ResponseEntity.notFound().build();
        }

        var opt = registrationRepository.findByUserAndEventAndStatus(user, event, RegistrationStatus.registered);
        if (opt.isEmpty()) {
            return ResponseEntity.badRequest().body(Map.of("error", "Вы не записаны на это мероприятие"));
        }

        opt.get().setStatus(RegistrationStatus.cancelled);
        registrationRepository.save(opt.get());

        event.setCurrentParticipants(Math.max(0, event.getCurrentParticipants() - 1));
        eventRepository.save(event);

        return ResponseEntity.ok(Map.of("message", "Запись отменена"));
    }
}
