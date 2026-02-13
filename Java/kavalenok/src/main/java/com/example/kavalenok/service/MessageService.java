package com.example.kavalenok.service;

import com.example.kavalenok.model.Friend;
import com.example.kavalenok.model.Message;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.FriendRepository;
import com.example.kavalenok.repository.MessageRepository;
import com.example.kavalenok.repository.UserRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import java.time.LocalDateTime;
import java.util.*;

@Service
public class MessageService {

    private final MessageRepository messageRepository;
    private final FriendRepository friendRepository;
    private final UserRepository userRepository;

    public MessageService(MessageRepository messageRepository,
                          FriendRepository friendRepository,
                          UserRepository userRepository) {
        this.messageRepository = messageRepository;
        this.friendRepository = friendRepository;
        this.userRepository = userRepository;
    }

    @Transactional
    public Message sendMessage(User sender, Long receiverId, String content) {
        User receiver = userRepository.findById(receiverId)
                .orElseThrow(() -> new IllegalArgumentException("Получатель не найден"));

        Optional<Friend> friendship = friendRepository.findFriendship(sender.getId(), receiverId);
        if (friendship.isEmpty() || friendship.get().getStatus() != Friend.Status.ACCEPTED) {
            throw new IllegalArgumentException("Вы можете отправлять сообщения только друзьям");
        }

        Message message = new Message(sender, receiver, content.trim());
        message.setIsRead(false);
        message.setSentAt(LocalDateTime.now());
        return messageRepository.save(message);
    }

    @Transactional(readOnly = true)
    public List<Message> getConversation(User currentUser, Long otherUserId) {
        return messageRepository.findConversation(currentUser.getId(), otherUserId);
    }

    @Transactional
    public void markConversationAsRead(User currentUser, Long senderId) {
        List<Message> unread = messageRepository.findByReceiverIdAndIsReadFalseOrderBySentAtAsc(currentUser.getId());
        unread.stream()
                .filter(m -> m.getSender().getId().equals(senderId))
                .forEach(m -> m.setIsRead(true));
        messageRepository.saveAll(unread);
    }

    @Transactional(readOnly = true)
    public int getUnreadCount(Long userId) {
        return messageRepository.findByReceiverIdAndIsReadFalseOrderBySentAtAsc(userId).size();
    }

    @Transactional(readOnly = true)
    public List<ConversationPreview> getConversations(User currentUser) {
        List<Friend> friendships = friendRepository.findAllFriends(currentUser.getId());
        List<User> friends = new ArrayList<>();
        for (Friend f : friendships) {
            if (f.getUser1().getId().equals(currentUser.getId())) {
                friends.add(f.getUser2());
            } else {
                friends.add(f.getUser1());
            }
        }

        List<ConversationPreview> result = new ArrayList<>();
        for (User friend : friends) {
            List<Message> lastMsgList = messageRepository.findLastMessageBetween(currentUser.getId(), friend.getId());
            Optional<Message> lastMsgOpt = lastMsgList.stream().findFirst();

            int unread = messageRepository.countByReceiverIdAndSenderIdAndIsReadFalse(currentUser.getId(), friend.getId());

            result.add(new ConversationPreview(friend, lastMsgOpt.orElse(null), unread));
        }

        result.sort((a, b) -> {
            if (a.getLastMessage() == null && b.getLastMessage() == null) return 0;
            if (a.getLastMessage() == null) return 1;
            if (b.getLastMessage() == null) return -1;
            return b.getLastMessage().getSentAt().compareTo(a.getLastMessage().getSentAt());
        });
        return result;
    }

    public static class ConversationPreview {
        private final User friend;
        private final Message lastMessage;
        private final int unreadCount;

        public ConversationPreview(User friend, Message lastMessage, int unreadCount) {
            this.friend = friend;
            this.lastMessage = lastMessage;
            this.unreadCount = unreadCount;
        }

        public User getFriend() { return friend; }
        public Message getLastMessage() { return lastMessage; }
        public int getUnreadCount() { return unreadCount; }
    }
}