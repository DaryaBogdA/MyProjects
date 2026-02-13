package com.example.kavalenok.service;

import com.example.kavalenok.model.CoachReview;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.CoachReviewRepository;
import com.example.kavalenok.repository.UserRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
public class CoachReviewService {

    private final CoachReviewRepository coachReviewRepository;
    private final UserRepository userRepository;

    public CoachReviewService(CoachReviewRepository coachReviewRepository,
                              UserRepository userRepository) {
        this.coachReviewRepository = coachReviewRepository;
        this.userRepository = userRepository;
    }

    @Transactional(readOnly = true)
    public List<CoachReview> getReviewsByCoach(Long coachId) {
        User coach = userRepository.findById(coachId)
                .orElseThrow(() -> new IllegalArgumentException("Тренер не найден"));
        return coachReviewRepository.findByCoachOrderByCreatedAtDesc(coach);
    }

    @Transactional
    public CoachReview addReview(User currentUser, Long coachId, Integer rating, String comment) {
        User coach = userRepository.findById(coachId)
                .orElseThrow(() -> new IllegalArgumentException("Тренер не найден"));
        if (!coach.getRole().equals(User.Role.COACH)) {
            throw new IllegalArgumentException("Пользователь не является тренером");
        }

        if (rating < 1 || rating > 5) {
            throw new IllegalArgumentException("Рейтинг должен быть от 1 до 5");
        }

        CoachReview review = new CoachReview(coach, currentUser, rating);
        review.setComment(comment != null && !comment.trim().isEmpty() ? comment.trim() : null);

        CoachReview saved = coachReviewRepository.save(review);
        updateCoachRating(coachId);
        return saved;
    }

    @Transactional
    public void updateCoachRating(Long coachId) {
        Double avg = coachReviewRepository.getAverageRatingByCoachId(coachId);
        User coach = userRepository.findById(coachId).orElseThrow();
        if (avg == null) {
            coach.setRating(0.0);
        } else {
            double rounded = Math.round(avg * 100.0) / 100.0;
            coach.setRating(rounded);
        }
        userRepository.save(coach);
    }

    @Transactional
    public void deleteReview(Long reviewId, User currentUser, boolean isAdmin) {
        CoachReview review = coachReviewRepository.findById(reviewId)
                .orElseThrow(() -> new IllegalArgumentException("Отзыв не найден"));
        if (!isAdmin && !review.getUser().getId().equals(currentUser.getId())) {
            throw new IllegalArgumentException("Вы не можете удалить этот отзыв");
        }
        Long coachId = review.getCoach().getId();
        coachReviewRepository.delete(review);
        updateCoachRating(coachId);
    }
}