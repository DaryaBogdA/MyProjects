package com.example.kavalenok.model;

public class TeamMemberId implements java.io.Serializable {
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
