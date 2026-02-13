package com.example.kavalenok.controller;

import com.example.kavalenok.model.*;
import com.example.kavalenok.repository.*;
import jakarta.servlet.http.HttpSession;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import java.util.*;

@Controller
public class FriendController {

    @Autowired
    private UserRepository userRepository;

    @Autowired
    private FriendRepository friendRepository;

    @GetMapping("/friends")
    public String searchFriends(HttpSession session, Model model,
                                @RequestParam(required = false) String q) {
        User currentUser = (User) session.getAttribute("user");
        if (currentUser == null) {
            return "redirect:/login";
        }

        List<User> searchResults = new ArrayList<>();
        Map<Long, String> friendshipStatusMap = new HashMap<>();
        List<Friend> pendingRequests = friendRepository.findPendingRequests(currentUser.getId());

        if (q != null && !q.trim().isEmpty()) {
            searchResults = userRepository.searchUsers(q.trim(), currentUser.getId());

            for (User foundUser : searchResults) {
                Optional<Friend> friendship = friendRepository.findFriendship(
                        currentUser.getId(),
                        foundUser.getId()
                );

                if (friendship.isPresent()) {
                    friendshipStatusMap.put(foundUser.getId(), friendship.get().getStatus().toString());
                } else {
                    friendshipStatusMap.put(foundUser.getId(), null);
                }
            }
        }

        model.addAttribute("user", currentUser);
        model.addAttribute("searchResults", searchResults);
        model.addAttribute("searchQuery", q != null ? q : "");
        model.addAttribute("friendshipStatusMap", friendshipStatusMap);
        model.addAttribute("pendingRequests", pendingRequests);
        return "friends";
    }

    private List<User> getFriendsForUser(User user) {
        List<Friend> friendships = friendRepository.findAllFriends(user.getId());
        List<User> friends = new ArrayList<>();

        for (Friend friendship : friendships) {
            if (friendship.getUser1().getId().equals(user.getId())) {
                friends.add(friendship.getUser2());
            } else {
                friends.add(friendship.getUser1());
            }
        }

        return friends;
    }

    @PostMapping("/friends/send-request")
    public String sendFriendRequest(@RequestParam Long friendId,
                                    HttpSession session,
                                    RedirectAttributes redirectAttributes) {
        User currentUser = (User) session.getAttribute("user");
        if (currentUser == null) {
            return "redirect:/login";
        }

        if (currentUser.getId().equals(friendId)) {
            redirectAttributes.addFlashAttribute("error", "Нельзя отправить запрос самому себе");
            return "redirect:/friends";
        }

        Optional<User> friendOpt = userRepository.findById(friendId);
        if (friendOpt.isEmpty()) {
            redirectAttributes.addFlashAttribute("error", "Пользователь не найден");
            return "redirect:/friends";
        }

        Optional<Friend> existingFriendship = friendRepository.findFriendship(
                currentUser.getId(),
                friendId
        );

        if (existingFriendship.isPresent()) {
            Friend.Status status = existingFriendship.get().getStatus();
            if (status == Friend.Status.PENDING) {
                redirectAttributes.addFlashAttribute("error", "Запрос уже отправлен");
            } else if (status == Friend.Status.ACCEPTED) {
                redirectAttributes.addFlashAttribute("error", "Вы уже друзья");
            } else if (status == Friend.Status.BLOCKED) {
                redirectAttributes.addFlashAttribute("error", "Пользователь заблокирован");
            }
            return "redirect:/friends";
        }

        Friend friendRequest = new Friend(currentUser, friendOpt.get());
        friendRequest.setStatus(Friend.Status.PENDING);
        friendRepository.save(friendRequest);

        redirectAttributes.addFlashAttribute("success", "Запрос в друзья отправлен");
        return "redirect:/friends";
    }

    @PostMapping("/friends/accept")
    public String acceptFriendRequest(@RequestParam Long user1Id,
                                      @RequestParam Long user2Id,
                                      HttpSession session,
                                      RedirectAttributes redirectAttributes) {
        User currentUser = (User) session.getAttribute("user");
        if (currentUser == null) {
            return "redirect:/login";
        }

        Optional<Friend> requestOpt = friendRepository.findFriendship(user1Id, user2Id);
        if (requestOpt.isEmpty() ||
                !requestOpt.get().getUser2().getId().equals(currentUser.getId()) ||
                requestOpt.get().getStatus() != Friend.Status.PENDING) {
            redirectAttributes.addFlashAttribute("error", "Запрос не найден");
            return "redirect:/friends";
        }

        Friend request = requestOpt.get();
        request.setStatus(Friend.Status.ACCEPTED);
        friendRepository.save(request);

        redirectAttributes.addFlashAttribute("success", "Запрос принят");
        return "redirect:/friends";
    }

    @PostMapping("/friends/reject")
    public String rejectFriendRequest(@RequestParam Long user1Id,
                                      @RequestParam Long user2Id,
                                      HttpSession session,
                                      RedirectAttributes redirectAttributes) {
        User currentUser = (User) session.getAttribute("user");
        if (currentUser == null) {
            return "redirect:/login";
        }

        Optional<Friend> requestOpt = friendRepository.findFriendship(user1Id, user2Id);
        if (requestOpt.isEmpty() ||
                !requestOpt.get().getUser2().getId().equals(currentUser.getId()) ||
                requestOpt.get().getStatus() != Friend.Status.PENDING) {
            redirectAttributes.addFlashAttribute("error", "Запрос не найден");
            return "redirect:/friends";
        }

        friendRepository.delete(requestOpt.get());
        redirectAttributes.addFlashAttribute("success", "Запрос отклонен");
        return "redirect:/friends";
    }

    @PostMapping("/friends/cancel")
    public String cancelFriendRequest(@RequestParam Long user1Id,
                                      @RequestParam Long user2Id,
                                      HttpSession session,
                                      RedirectAttributes redirectAttributes) {
        User currentUser = (User) session.getAttribute("user");
        if (currentUser == null) {
            return "redirect:/login";
        }

        Optional<Friend> requestOpt = friendRepository.findFriendship(user1Id, user2Id);
        if (requestOpt.isEmpty() ||
                !requestOpt.get().getUser1().getId().equals(currentUser.getId()) ||
                requestOpt.get().getStatus() != Friend.Status.PENDING) {
            redirectAttributes.addFlashAttribute("error", "Запрос не найден");
            return "redirect:/friends";
        }

        friendRepository.delete(requestOpt.get());
        redirectAttributes.addFlashAttribute("success", "Запрос отменен");
        return "redirect:/friends";
    }

    @PostMapping("/friends/remove")
    public String removeFriend(@RequestParam Long friendId,
                               HttpSession session,
                               RedirectAttributes redirectAttributes) {
        User currentUser = (User) session.getAttribute("user");
        if (currentUser == null) {
            return "redirect:/login";
        }

        Optional<Friend> friendship = friendRepository.findFriendship(
                currentUser.getId(),
                friendId
        );

        if (friendship.isEmpty() || friendship.get().getStatus() != Friend.Status.ACCEPTED) {
            redirectAttributes.addFlashAttribute("error", "Пользователь не найден в друзьях");
            return "redirect:/user";
        }

        friendRepository.delete(friendship.get());
        redirectAttributes.addFlashAttribute("success", "Пользователь удален из друзей");
        return "redirect:/user";
    }

    @GetMapping("/user/friends")
    public String showFriends(HttpSession session, Model model) {
        User currentUser = (User) session.getAttribute("user");
        if (currentUser == null) {
            return "redirect:/login";
        }

        List<User> friends = getFriendsForUser(currentUser);

        List<Friend> incomingRequests = friendRepository.findPendingRequests(currentUser.getId());

        List<Friend> outgoingRequests = friendRepository.findSentRequests(currentUser.getId());

        model.addAttribute("user", currentUser);
        model.addAttribute("friends", friends);
        model.addAttribute("incomingRequests", incomingRequests);
        model.addAttribute("outgoingRequests", outgoingRequests);
        return "friends-list";
    }

    @PostMapping("/friends/cancel-request")
    public String cancelFriendRequestByUser(@RequestParam Long friendId,
                                            HttpSession session,
                                            RedirectAttributes redirectAttributes) {
        User currentUser = (User) session.getAttribute("user");
        if (currentUser == null) {
            return "redirect:/login";
        }

        Optional<Friend> requestOpt = friendRepository.findFriendship(currentUser.getId(), friendId);

        if (requestOpt.isEmpty() ||
                !requestOpt.get().getUser1().getId().equals(currentUser.getId()) ||
                requestOpt.get().getStatus() != Friend.Status.PENDING) {
            redirectAttributes.addFlashAttribute("error", "Запрос не найден");
            return "redirect:/friends";
        }

        friendRepository.delete(requestOpt.get());
        redirectAttributes.addFlashAttribute("success", "Запрос отменен");
        return "redirect:/friends";
    }
}