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
import java.util.List;
import java.util.Map;

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
    public List<Review> getReviewsForEvent(@PathVariable Long eventId) {
        return reviewRepository.findByEventId(eventId);
    }

    @PostMapping
    public ResponseEntity<?> addReview(@AuthenticationPrincipal User user,
                                       @RequestBody Map<String, Object> payload) {
        if (user == null) {
            return ResponseEntity.status(401).body(Map.of("error", "Необходимо войти в систему"));
        }
        Long eventId = Long.valueOf(payload.get("eventId").toString());
        Integer rating = (Integer) payload.get("rating");
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
        return ResponseEntity.ok(Map.of("message", "Отзыв добавлен"));
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