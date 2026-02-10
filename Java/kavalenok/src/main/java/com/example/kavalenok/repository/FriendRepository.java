package com.example.kavalenok.repository;

import com.example.kavalenok.model.Friend;
import com.example.kavalenok.model.FriendId;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import java.util.List;
import java.util.Optional;

public interface FriendRepository extends JpaRepository<Friend, FriendId> {

    @Query("SELECT f FROM Friend f WHERE (f.user1.id = :userId OR f.user2.id = :userId) AND f.status = 'ACCEPTED'")
    List<Friend> findFriendsByUserId(Long userId);

    @Query("SELECT f FROM Friend f WHERE ((f.user1.id = :user1Id AND f.user2.id = :user2Id) OR (f.user1.id = :user2Id AND f.user2.id = :user1Id))")
    Optional<Friend> findByIds(Long user1Id, Long user2Id);
}
