package com.example.kavalenok.controller;

import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.UserRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.ArrayList;
import java.util.List;

@Controller
public class FriendController {

    @Autowired
    private UserRepository userRepository;

    @GetMapping("/friends")
    public String searchFriends(HttpSession session, Model model,
                                @RequestParam(required = false) String q) {
        User user = (User) session.getAttribute("user");
        if (user == null) {
            return "redirect:/login";
        }

        List<User> searchResults = new ArrayList<>();
        if (q != null && !q.trim().isEmpty()) {
            searchResults = userRepository.searchUsers(q.trim(), user.getId());
        }

        model.addAttribute("user", user);
        model.addAttribute("searchResults", searchResults);
        model.addAttribute("searchQuery", q != null ? q : "");
        return "friends";
    }
}
