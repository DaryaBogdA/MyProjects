package com.example.kavalenok.model;

import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "trainings")
public class Training {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "coach_id", nullable = false)
    private User coach;

    @Column(nullable = false)
    private String title;

    private String location;

    @Column(name = "max_participants")
    private Integer maxParticipants;

    @Column(name = "current_participants", columnDefinition = "INT default 0")
    private Integer currentParticipants = 0;

    @Column(name = "date_time", nullable = false)
    private LocalDateTime dateTime;

    @Column(name = "duration_minutes")
    private Integer durationMinutes;

    public Training() {}

    public Training(User coach, String title, LocalDateTime dateTime) {
        this.coach = coach;
        this.title = title;
        this.dateTime = dateTime;
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public User getCoach() {
        return coach;
    }

    public void setCoach(User coach) {
        this.coach = coach;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getLocation() {
        return location;
    }

    public void setLocation(String location) {
        this.location = location;
    }

    public Integer getMaxParticipants() {
        return maxParticipants;
    }

    public void setMaxParticipants(Integer maxParticipants) {
        this.maxParticipants = maxParticipants;
    }

    public Integer getCurrentParticipants() {
        return currentParticipants;
    }

    public void setCurrentParticipants(Integer currentParticipants) {
        this.currentParticipants = currentParticipants;
    }

    public LocalDateTime getDateTime() {
        return dateTime;
    }

    public void setDateTime(LocalDateTime dateTime) {
        this.dateTime = dateTime;
    }

    public Integer getDurationMinutes() {
        return durationMinutes;
    }

    public void setDurationMinutes(Integer durationMinutes) {
        this.durationMinutes = durationMinutes;
    }

    @Transient
    public boolean isFull() {
        return maxParticipants != null && currentParticipants != null &&
                currentParticipants >= maxParticipants;
    }

    @Transient
    public String getFormattedDate() {
        return dateTime != null ? dateTime.format(java.time.format.DateTimeFormatter.ofPattern("dd.MM.yyyy")) : "";
    }

    @Transient
    public String getFormattedTime() {
        return dateTime != null ? dateTime.format(java.time.format.DateTimeFormatter.ofPattern("HH:mm")) : "";
    }

    @Transient
    public String getDurationHours() {
        if (durationMinutes == null) return "1 час";
        int hours = durationMinutes / 60;
        int minutes = durationMinutes % 60;
        if (hours > 0 && minutes > 0) {
            return hours + "ч " + minutes + "мин";
        } else if (hours > 0) {
            return hours + "ч";
        } else {
            return minutes + "мин";
        }
    }


}