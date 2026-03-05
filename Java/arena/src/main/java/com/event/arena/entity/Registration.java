package com.event.arena.entity;

import jakarta.persistence.*;

import java.time.LocalDateTime;

@Entity
@Table(name = "registrations")
public class Registration {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id", nullable = false)
    private User user;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "event_id", nullable = false)
    private Event event;

    @Column(name = "registration_date")
    private LocalDateTime registrationDate;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private RegistrationStatus status = RegistrationStatus.registered;

    public Registration() {}

    public Registration(User user, Event event, LocalDateTime registrationDate) {
        this.user = user;
        this.event = event;
        this.registrationDate = registrationDate;
    }

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public User getUser() { return user; }
    public void setUser(User user) { this.user = user; }
    public Event getEvent() { return event; }
    public void setEvent(Event event) { this.event = event; }
    public LocalDateTime getRegistrationDate() { return registrationDate; }
    public void setRegistrationDate(LocalDateTime registrationDate) { this.registrationDate = registrationDate; }
    public RegistrationStatus getStatus() { return status; }
    public void setStatus(RegistrationStatus status) { this.status = status; }
}
