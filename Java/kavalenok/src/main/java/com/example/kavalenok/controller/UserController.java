package com.example.kavalenok.controller;

import com.example.kavalenok.model.Friend;
import com.example.kavalenok.model.Training;
import com.example.kavalenok.model.TrainingParticipant;
import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.FriendRepository;
import com.example.kavalenok.repository.TrainingParticipantRepository;
import com.example.kavalenok.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;

import jakarta.servlet.http.HttpSession;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.stream.Collectors;

@Controller
public class UserController {

    @Autowired
    private TrainingParticipantRepository trainingParticipantRepository;

    @Autowired
    private UserRepository userRepository;

    @Autowired
    private FriendRepository friendRepository;

    @GetMapping("/user")
    public String user(HttpSession session, Model model) {
        User user = (User) session.getAttribute("user");
        if (user == null) {
            return "redirect:/login";
        }

        List<Training> myTrainings = new ArrayList<>();
        try {
            List<TrainingParticipant> participations = trainingParticipantRepository.findByUserIdOrderByAppliedAtDesc(user.getId());
            myTrainings = participations.stream()
                    .map(TrainingParticipant::getTraining)
                    .filter(t -> t.getDateTime() != null && t.getDateTime().isAfter(java.time.LocalDateTime.now()))
                    .collect(Collectors.toList());
        } catch (Exception e) {
            myTrainings = Collections.emptyList();
        }

        List<User> friends = getFriendsForUser(user);

        model.addAttribute("friends", friends);
        model.addAttribute("user", user);
        model.addAttribute("myTrainings", myTrainings);
        return "user";
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

    @GetMapping("/profile/{id}")
    public String profile(@PathVariable Long id, HttpSession session, Model model) {
        User currentUser = (User) session.getAttribute("user");
        if (currentUser == null) {
            return "redirect:/login";
        }
        if (currentUser.getId().equals(id)) {
            return "redirect:/user";
        }
        var profileUserOpt = userRepository.findById(id);
        if (profileUserOpt.isEmpty()) {
            return "redirect:/friends";
        }
        model.addAttribute("profileUser", profileUserOpt.get());
        model.addAttribute("currentUser", currentUser);
        return "profile";
    }
}