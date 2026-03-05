package com.event.arena.entity;

import jakarta.persistence.*;

@Entity
@Table(name = "favorites")
@IdClass(FavoriteId.class)
public class Favorite {
    @Id
    @ManyToOne
    @JoinColumn(name = "user_id")
    private User user;

    @Id
    @ManyToOne
    @JoinColumn(name = "event_id")
    private Event event;

    public Favorite() {}

    public Favorite(User user, Event event) {
        this.user = user;
        this.event = event;
    }

    public User getUser() { return user; }
    public void setUser(User user) { this.user = user; }
    public Event getEvent() { return event; }
    public void setEvent(Event event) { this.event = event; }
}

class FavoriteId implements java.io.Serializable {
    private Long user;
    private Long event;

    public FavoriteId() {}

    public FavoriteId(Long user, Long event) {
        this.user = user;
        this.event = event;
    }

    public Long getUser() { return user; }
    public void setUser(Long user) { this.user = user; }
    public Long getEvent() { return event; }
    public void setEvent(Long event) { this.event = event; }

}
