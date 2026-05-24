package com.event.arena.controller;

import com.event.arena.entity.Event;
import com.event.arena.entity.Review;
import com.event.arena.entity.Role;
import com.event.arena.entity.User;
import com.event.arena.repository.EventRepository;
import com.event.arena.repository.ReviewRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/reviews")
public class ReviewController {

    private final ReviewRepository reviewRepository;
    private final EventRepository eventRepository;

    public ReviewController(ReviewRepository reviewRepository, EventRepository eventRepository) {
        this.reviewRepository = reviewRepository;
        this.eventRepository = eventRepository;
    }

    @GetMapping("/event/{eventId}")
    public List<Map<String, Object>> getReviewsForEvent(@PathVariable Long eventId) {
        return reviewRepository.findByEventIdWithUser(eventId).stream()
                .map(this::reviewToMap)
                .collect(Collectors.toList());
    }

    @PostMapping
    public ResponseEntity<?> addReview(@AuthenticationPrincipal User user,
                                       @RequestBody Map<String, Object> payload) {
        if (user == null) {
            return ResponseEntity.status(401).body(Map.of("error", "Необходимо войти в систему"));
        }
        Long eventId = Long.valueOf(payload.get("eventId").toString());
        Integer rating = parseRating(payload.get("rating"));
        String comment = (String) payload.get("comment");

        if (rating == null || rating < 1 || rating > 5) {
            return ResponseEntity.badRequest().body(Map.of("error", "Рейтинг должен быть от 1 до 5"));
        }
        if (comment == null || comment.trim().isEmpty()) {
            return ResponseEntity.badRequest().body(Map.of("error", "Комментарий не может быть пустым"));
        }

        if (reviewRepository.existsByUserIdAndEventId(user.getId(), eventId)) {
            return ResponseEntity.badRequest().body(Map.of("error", "Вы уже оставили отзыв на это мероприятие"));
        }

        Event event = eventRepository.findById(eventId).orElse(null);
        if (event == null) {
            return ResponseEntity.notFound().build();
        }

        Review review = new Review();
        review.setUser(user);
        review.setEvent(event);
        review.setRating(rating);
        review.setComment(comment);
        review.setCreatedAt(LocalDateTime.now());

        reviewRepository.save(review);
        return ResponseEntity.ok(Map.of("message", "Отзыв добавлен", "review", reviewToMap(review)));
    }

    private Integer parseRating(Object ratingObj) {
        if (ratingObj == null) return null;
        if (ratingObj instanceof Number) {
            return ((Number) ratingObj).intValue();
        }
        try {
            return Integer.parseInt(ratingObj.toString());
        } catch (NumberFormatException e) {
            return null;
        }
    }

    private Map<String, Object> reviewToMap(Review review) {
        Map<String, Object> map = new HashMap<>();
        map.put("id", review.getId());
        map.put("rating", review.getRating());
        map.put("comment", review.getComment());
        map.put("createdAt", review.getCreatedAt());
        User user = review.getUser();
        if (user != null) {
            Map<String, Object> userMap = new HashMap<>();
            userMap.put("firstName", user.getFirstName());
            userMap.put("lastName", user.getLastName());
            map.put("user", userMap);
        }
        return map;
    }

    @DeleteMapping("/{reviewId}")
    public ResponseEntity<?> deleteReview(@AuthenticationPrincipal User user,
                                          @PathVariable Long reviewId) {
        Review review = reviewRepository.findById(reviewId).orElse(null);
        if (review == null) {
            return ResponseEntity.notFound().build();
        }
        if (user == null || (!user.getRole().equals(Role.ADMIN) && !review.getUser().getId().equals(user.getId()))) {
            return ResponseEntity.status(403).body(Map.of("error", "Нет прав для удаления"));
        }
        reviewRepository.delete(review);
        return ResponseEntity.ok(Map.of("message", "Отзыв удалён"));
    }
}