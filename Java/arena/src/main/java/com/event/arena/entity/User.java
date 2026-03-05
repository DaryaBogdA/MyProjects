package com.event.arena.entity;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;

import java.util.ArrayList;
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

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private Role role = Role.USER;

    @OneToMany(mappedBy = "user", cascade = CascadeType.ALL, orphanRemoval = true)
    @JsonIgnore
    private List<Favorite> favorites = new ArrayList<>();

    @OneToMany(mappedBy = "user")
    @JsonIgnore
    private List<Registration> registrations = new ArrayList<>();

    @OneToMany(mappedBy = "user")
    @JsonIgnore
    private List<Review> reviews = new ArrayList<>();

    @OneToMany(mappedBy = "createdBy")
    @JsonIgnore
    private List<Event> createdEvents = new ArrayList<>();

    public User() {
    }

    public User(String email, String passwordHash, String firstName, String lastName) {
        this.email = email;
        this.passwordHash = passwordHash;
        this.firstName = firstName;
        this.lastName = lastName;
    }

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }
    public String getPasswordHash() { return passwordHash; }
    public void setPasswordHash(String passwordHash) { this.passwordHash = passwordHash; }
    public String getFirstName() { return firstName; }
    public void setFirstName(String firstName) { this.firstName = firstName; }
    public String getLastName() { return lastName; }
    public void setLastName(String lastName) { this.lastName = lastName; }
    public Role getRole() { return role; }
    public void setRole(Role role) { this.role = role; }
    public List<Favorite> getFavorites() { return favorites; }
    public void setFavorites(List<Favorite> favorites) { this.favorites = favorites; }
    public List<Registration> getRegistrations() { return registrations; }
    public void setRegistrations(List<Registration> registrations) { this.registrations = registrations; }
    public List<Review> getReviews() { return reviews; }
    public void setReviews(List<Review> reviews) { this.reviews = reviews; }
    public List<Event> getCreatedEvents() { return createdEvents; }
    public void setCreatedEvents(List<Event> createdEvents) { this.createdEvents = createdEvents; }

}