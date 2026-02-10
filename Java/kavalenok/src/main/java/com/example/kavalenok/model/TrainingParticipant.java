package com.example.kavalenok.model;

import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "training_participants",
        uniqueConstraints = @UniqueConstraint(columnNames = {"training_id", "user_id"}))
public class TrainingParticipant {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "training_id", nullable = false)
    private Training training;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id", nullable = false)
    private User user;

    @Enumerated(EnumType.STRING)
    @Column(columnDefinition = "ENUM('pending', 'approved', 'rejected', 'attended') default 'pending'")
    private Status status = Status.PENDING;

    @Column(name = "applied_at")
    private LocalDateTime appliedAt;

    public TrainingParticipant() {}

    public TrainingParticipant(Training training, User user) {
        this.training = training;
        this.user = user;
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public Training getTraining() {
        return training;
    }

    public void setTraining(Training training) {
        this.training = training;
    }

    public User getUser() {
        return user;
    }

    public void setUser(User user) {
        this.user = user;
    }

    public Status getStatus() {
        return status;
    }

    public void setStatus(Status status) {
        this.status = status;
    }

    public LocalDateTime getAppliedAt() {
        return appliedAt;
    }

    public void setAppliedAt(LocalDateTime appliedAt) {
        this.appliedAt = appliedAt;
    }

    @PrePersist
    protected void onCreate() {
        this.appliedAt = LocalDateTime.now();
    }

    public enum Status {
        PENDING, APPROVED, REJECTED, ATTENDED
    }
}