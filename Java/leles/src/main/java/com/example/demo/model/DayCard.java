package com.example.demo.model;

import jakarta.persistence.*;
import java.time.LocalDate;

@Entity
@Table(name = "daily_card")
public class DayCard {

    @EmbeddedId
    private DailyCardId id;

    @ManyToOne(fetch = FetchType.LAZY)
    @MapsId("userId")
    @JoinColumn(name = "user_id", nullable = false)
    private User user;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "card_id", nullable = false)
    private CardTaro card;

    public DailyCardId getId() {
        return id;
    }

    public void setId(DailyCardId id) {
        this.id = id;
    }

    public User getUser() {
        return user;
    }

    public void setUser(User user) {
        this.user = user;
    }

    public CardTaro getCard() {
        return card;
    }

    public void setCard(CardTaro card) {
        this.card = card;
    }

    public LocalDate getDate() {
        return id != null ? id.getDate() : null;
    }

    public void setDate(LocalDate date) {
        if (id == null) {
            id = new DailyCardId();
        }
        id.setDate(date);
    }
}
