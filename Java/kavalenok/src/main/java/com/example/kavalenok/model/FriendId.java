package com.example.kavalenok.model;

import java.io.Serializable;
import java.util.Objects;

public class FriendId implements Serializable {

    private Long user1;
    private Long user2;

    public FriendId() {}

    public FriendId(Long user1, Long user2) {
        this.user1 = user1;
        this.user2 = user2;
    }

    public Long getUser1() {
        return user1;
    }

    public void setUser1(Long user1) {
        this.user1 = user1;
    }

    public Long getUser2() {
        return user2;
    }

    public void setUser2(Long user2) {
        this.user2 = user2;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        FriendId friendId = (FriendId) o;
        return Objects.equals(user1, friendId.user1) && Objects.equals(user2, friendId.user2);
    }

    @Override
    public int hashCode() {
        return Objects.hash(user1, user2);
    }
}
