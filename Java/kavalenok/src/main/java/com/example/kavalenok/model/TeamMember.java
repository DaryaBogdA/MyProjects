package com.example.kavalenok.model;

import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "team_members")
@IdClass(TeamMemberId.class)
public class TeamMember {

    @Id
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "team_id", nullable = false)
    private Team team;

    @Id
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id", nullable = false)
    private User user;

    @Column(name = "joined_at")
    private LocalDateTime joinedAt;

    public TeamMember() {}

    public TeamMember(Team team, User user) {
        this.team = team;
        this.user = user;
    }

    public Team getTeam() {
        return team;
    }

    public void setTeam(Team team) {
        this.team = team;
    }

    public User getUser() {
        return user;
    }

    public void setUser(User user) {
        this.user = user;
    }

    public LocalDateTime getJoinedAt() {
        return joinedAt;
    }

    public void setJoinedAt(LocalDateTime joinedAt) {
        this.joinedAt = joinedAt;
    }

    @PrePersist
    protected void onCreate() {
        this.joinedAt = LocalDateTime.now();
    }
}

class TeamMemberId implements java.io.Serializable {
    private Long team;
    private Long user;

    public TeamMemberId() {}

    public TeamMemberId(Long team, Long user) {
        this.team = team;
        this.user = user;
    }

    public Long getTeam() {
        return team;
    }

    public void setTeam(Long team) {
        this.team = team;
    }

    public Long getUser() {
        return user;
    }

    public void setUser(Long user) {
        this.user = user;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;

        TeamMemberId that = (TeamMemberId) o;
        return team.equals(that.team) && user.equals(that.user);
    }

    @Override
    public int hashCode() {
        int result = team.hashCode();
        result = 31 * result + user.hashCode();
        return result;
    }
}