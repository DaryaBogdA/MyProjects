package com.example.kavalenok.model;

import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "friends")
@IdClass(FriendId.class)
public class Friend {

    @Id
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id1", nullable = false)
    private User user1;

    @Id
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id2", nullable = false)
    private User user2;

    @Enumerated(EnumType.STRING)
    @Column(columnDefinition = "ENUM('pending', 'accepted', 'blocked') default 'pending'")
    private Status status = Status.PENDING;

    @Column(name = "created_at")
    private LocalDateTime createdAt;

    public Friend() {}

    public Friend(User user1, User user2) {
        this.user1 = user1;
        this.user2 = user2;
    }

    public User getUser1() {
        return user1;
    }

    public void setUser1(User user1) {
        this.user1 = user1;
    }

    public User getUser2() {
        return user2;
    }

    public void setUser2(User user2) {
        this.user2 = user2;
    }

    public Status getStatus() {
        return status;
    }

    public void setStatus(Status status) {
        this.status = status;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }

    @PrePersist
    protected void onCreate() {
        this.createdAt = LocalDateTime.now();
    }

    public enum Status {
        PENDING, ACCEPTED, BLOCKED
    }
}