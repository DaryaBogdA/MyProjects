package com.event.arena.controller;

import com.event.arena.entity.Event;
import com.event.arena.entity.Favorite;
import com.event.arena.entity.User;
import com.event.arena.repository.EventRepository;
import com.event.arena.repository.FavoriteRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/favorites")
public class FavoriteController {

    private final FavoriteRepository favoriteRepository;
    private final EventRepository eventRepository;

    public FavoriteController(FavoriteRepository favoriteRepository, EventRepository eventRepository) {
        this.favoriteRepository = favoriteRepository;
        this.eventRepository = eventRepository;
    }

    @GetMapping("/my")
    public ResponseEntity<List<Long>> getMyFavoriteEventIds(@AuthenticationPrincipal User user) {
        if (user == null) {
            return ResponseEntity.status(401).build();
        }
        List<Long> eventIds = favoriteRepository.findByUserId(user.getId()).stream()
                .map(fav -> fav.getEvent().getId())
                .collect(Collectors.toList());
        return ResponseEntity.ok(eventIds);
    }

    @PostMapping("/{eventId}")
    public ResponseEntity<?> addFavorite(@AuthenticationPrincipal User user, @PathVariable Long eventId) {
        if (user == null) {
            return ResponseEntity.status(401).body(Map.of("error", "Необходимо войти в систему"));
        }

        if (!eventRepository.existsById(eventId)) {
            return ResponseEntity.notFound().build();
        }

        if (favoriteRepository.existsByUserIdAndEventId(user.getId(), eventId)) {
            return ResponseEntity.badRequest().body(Map.of("error", "Уже в избранном"));
        }

        Event event = eventRepository.findById(eventId)
                .orElseThrow(() -> new RuntimeException("Событие не найдено"));        Favorite fav = new Favorite(user, event);
        favoriteRepository.save(fav);

        return ResponseEntity.ok(Map.of("message", "Добавлено в избранное"));
    }

    @DeleteMapping("/{eventId}")
    public ResponseEntity<?> removeFavorite(@AuthenticationPrincipal User user, @PathVariable Long eventId) {
        if (user == null) {
            return ResponseEntity.status(401).body(Map.of("error", "Необходимо войти в систему"));
        }

        if (!eventRepository.existsById(eventId)) {
            return ResponseEntity.notFound().build();
        }

        favoriteRepository.deleteByUserIdAndEventId(user.getId(), eventId);

        return ResponseEntity.ok(Map.of("message", "Удалено из избранного"));
    }
}