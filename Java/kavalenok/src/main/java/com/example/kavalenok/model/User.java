package com.example.kavalenok.model;

import jakarta.persistence.*;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.Collections;
import java.util.List;

@Entity
@Table(name = "users")
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(unique = true, nullable = false)
    private String email;

    @Column(name = "password_hash", nullable = false)
    private String passwordHash;

    @Column(name = "first_name")
    private String firstName;

    @Column(name = "last_name")
    private String lastName;

    @Column(name = "avatar_url")
    private String avatarUrl;

    @Enumerated(EnumType.STRING)
    @Column(columnDefinition = "ENUM('USER', 'COACH', 'ADMIN') default 'USER'")
    private Role role = Role.USER;

    @Column(columnDefinition = "DECIMAL(3,2) default 0.0")
    private Double rating = 0.0;

    @Column(name = "created_at")
    private LocalDateTime createdAt;

    @Column(name = "last_login")
    private LocalDateTime lastLogin;

    @Column(name = "sur_name")
    private String surName;

    @Column(name = "phone")
    private String phone;

    @Column(name = "date")
    private LocalDate date;

    @OneToOne(mappedBy = "user", cascade = CascadeType.ALL, fetch = FetchType.LAZY)
    private CoachProfile coachProfile;

    @Column(nullable = false, columnDefinition = "BOOLEAN DEFAULT FALSE")
    private Boolean locked = false;



    public User() {}

    public User(String email, String passwordHash) {
        this.email = email;
        this.passwordHash = passwordHash;
    }

    public Boolean getLocked() { return locked; }

    public void setLocked(Boolean locked) { this.locked = locked; }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getPasswordHash() {
        return passwordHash;
    }

    public void setPasswordHash(String passwordHash) {
        this.passwordHash = passwordHash;
    }

    public String getFirstName() {
        return firstName;
    }

    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }

    public String getLastName() {
        return lastName;
    }

    public void setLastName(String lastName) {
        this.lastName = lastName;
    }

    public String getSurName() {
        return surName;
    }

    public void setSurName(String surName) {
        this.surName = surName;
    }

    public String getPhone() {
        return phone;
    }

    public void setPhone(String phone) {
        this.phone = phone;
    }

    public LocalDate getDate() {
        return date;
    }

    public void setDate(LocalDate date) {
        this.date = date;
    }

    public String getAvatarUrl() {
        return avatarUrl;
    }

    public void setAvatarUrl(String avatarUrl) {
        this.avatarUrl = avatarUrl;
    }

    public Role getRole() {
        return role;
    }

    public void setRole(Role role) {
        this.role = role;
    }

    public Double getRating() {
        return rating;
    }

    public void setRating(Double rating) {
        this.rating = rating;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }

    public LocalDateTime getLastLogin() {
        return lastLogin;
    }

    public void setLastLogin(LocalDateTime lastLogin) {
        this.lastLogin = lastLogin;
    }

    @PrePersist
    protected void onCreate() {
        this.createdAt = LocalDateTime.now();
    }

    @Transient
    public Integer getExperienceYears() {
        return coachProfile != null ? coachProfile.getExperienceYears() : 0;
    }

    @Transient
    public String getAchievements() {
        return coachProfile != null ? coachProfile.getAchievements() : null;
    }

    @Transient
    public String getBio() {
        return coachProfile != null ? coachProfile.getBio() : null;
    }

    @Transient
    public String getSpecialty() {
        return "Тренер по волейболу";
    }

    @Transient
    public String getLocation() {
        return "Основной зал";
    }

    @Transient
    public Integer getStudentsCount() {
        return 0;
    }

    @Transient
    public List<String> getSkills() {
        return Collections.emptyList();
    }

    public CoachProfile getCoachProfile() {
        return coachProfile;
    }

    public void setCoachProfile(CoachProfile coachProfile) {
        this.coachProfile = coachProfile;
    }

    public enum Role {
        USER, COACH, ADMIN
    }
}

