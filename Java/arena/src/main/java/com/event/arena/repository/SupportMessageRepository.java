package com.event.arena.repository;

import com.event.arena.entity.SupportMessage;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface SupportMessageRepository extends JpaRepository<SupportMessage, Long> {
    @Query("""
            SELECT m FROM SupportMessage m
            WHERE (m.sender.id = :firstUserId AND m.recipient.id = :secondUserId)
               OR (m.sender.id = :secondUserId AND m.recipient.id = :firstUserId)
            ORDER BY m.createdAt ASC
            """)
    List<SupportMessage> findConversation(@Param("firstUserId") Long firstUserId, @Param("secondUserId") Long secondUserId);
    List<SupportMessage> findBySenderIdOrRecipientIdOrderByCreatedAtAsc(Long senderId, Long recipientId);
}
