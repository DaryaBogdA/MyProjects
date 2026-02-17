package com.example.kavalenok.repository;

import com.example.kavalenok.model.Friend;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface FriendRepository extends JpaRepository<Friend, Long> {

    @Query("SELECT f FROM Friend f WHERE " +
            "(f.user1.id = :userId1 AND f.user2.id = :userId2) OR " +
            "(f.user1.id = :userId2 AND f.user2.id = :userId1)")
    Optional<Friend> findFriendship(@Param("userId1") Long userId1, @Param("userId2") Long userId2);

    @Query("SELECT f FROM Friend f WHERE " +
            "((f.user1.id = :userId OR f.user2.id = :userId) AND f.status = 'ACCEPTED')")
    List<Friend> findAllFriends(@Param("userId") Long userId);

    @Query("SELECT f FROM Friend f WHERE f.user2.id = :userId AND f.status = 'PENDING'")
    List<Friend> findPendingRequests(@Param("userId") Long userId);

    @Query("SELECT f FROM Friend f WHERE f.user1.id = :userId AND f.status = 'PENDING'")
    List<Friend> findSentRequests(@Param("userId") Long userId);

}