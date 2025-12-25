package com.example.demo.model;

import jakarta.persistence.Embeddable;
import java.io.Serializable;
import java.time.LocalDate;
import java.util.Objects;

@Embeddable
public class DailyCardId implements Serializable {

    private Integer userId;
    private LocalDate date;

    public DailyCardId() {
    }

    public DailyCardId(Integer userId, LocalDate date) {
        this.userId = userId;
        this.date = date;
    }

    public Integer getUserId() {
        return userId;
    }

    public void setUserId(Integer userId) {
        this.userId = userId;
    }

    public LocalDate getDate() {
        return date;
    }

    public void setDate(LocalDate date) {
        this.date = date;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof DailyCardId)) return false;
        DailyCardId that = (DailyCardId) o;
        return Objects.equals(userId, that.userId) &&
                Objects.equals(date, that.date);
    }

    @Override
    public int hashCode() {
        return Objects.hash(userId, date);
    }
}
