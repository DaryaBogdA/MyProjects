package com.example.demo.controller;

import com.example.demo.model.Role;
import com.example.demo.model.User;
import com.example.demo.model.VipRequest;
import com.example.demo.model.YesNo;
import com.example.demo.repository.UserRepository;
import com.example.demo.repository.VipRequestRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import java.util.*;

@Controller
@RequestMapping("/admin")
public class AdminController {

    private final UserRepository userRepository;
    private final VipRequestRepository vipRequestRepository;

    public AdminController(UserRepository userRepository, VipRequestRepository vipRequestRepository) {
        this.userRepository = userRepository;
        this.vipRequestRepository = vipRequestRepository;
    }

    @GetMapping
    public String adminIndex(HttpSession session, RedirectAttributes ra) {
        User currentUser = getCurrentUser(session);
        if (!isAdmin(currentUser)) return denyAccess(currentUser, ra);
        return "redirect:/admin/vip-requests";
    }

    @GetMapping("/vip-requests")
    public String vipRequests(HttpSession session, Model model, RedirectAttributes ra) {
        User currentUser = getCurrentUser(session);
        if (!isAdmin(currentUser)) return denyAccess(currentUser, ra);

        List<VipRequest> requests = vipRequestRepository.findByStatusAndCloseOrderByIdDesc(YesNo.NO, YesNo.NO);

        Set<Integer> userIds = new HashSet<>();
        for (VipRequest r : requests) {
            if (r.getUserId() != null) userIds.add(r.getUserId());
        }

        Map<Integer, User> usersById = new HashMap<>();
        if (!userIds.isEmpty()) {
            for (User u : userRepository.findAllById(userIds)) {
                usersById.put(u.getId(), u);
            }
        }

        List<VipRequestRow> rows = new ArrayList<>(requests.size());
        for (VipRequest r : requests) {
            User u = r.getUserId() == null ? null : usersById.get(r.getUserId());
            rows.add(new VipRequestRow(r, u));
        }

        userRepository.flush();
        List<User> allUsers = userRepository.findAllByOrderByIdDesc();

        model.addAttribute("vipRequests", rows);
        model.addAttribute("users", allUsers);
        return "adminBoard";
    }

    @PostMapping("/vip-requests/{id}/approve")
    public String approveVip(@PathVariable("id") Integer requestId,
                             HttpSession session,
                             RedirectAttributes ra) {
        User currentUser = getCurrentUser(session);
        if (!isAdmin(currentUser)) return denyAccess(currentUser, ra);

        VipRequest req = vipRequestRepository.findById(requestId).orElse(null);
        if (req == null) {
            ra.addFlashAttribute("error", "Заявка не найдена");
            return "redirect:/admin/vip-requests";
        }

        if (req.getUserId() == null) {
            ra.addFlashAttribute("error", "У заявки нет пользователя");
            return "redirect:/admin/vip-requests";
        }

        User user = userRepository.findById(req.getUserId()).orElse(null);
        if (user == null) {
            ra.addFlashAttribute("error", "Пользователь по заявке не найден");
            return "redirect:/admin/vip-requests";
        }

        user.setIsVip(true);
        userRepository.save(user);

        req.setStatus(YesNo.YES);
        req.setClose(YesNo.YES);
        vipRequestRepository.save(req);

        ra.addFlashAttribute("message", "VIP одобрен для пользователя: " + user.getLogin());
        return "redirect:/admin/vip-requests";
    }

    @PostMapping("/vip-requests/{id}/delete")
    public String deleteVipRequest(@PathVariable("id") Integer requestId,
                                   HttpSession session,
                                   RedirectAttributes ra) {
        User currentUser = getCurrentUser(session);
        if (!isAdmin(currentUser)) return denyAccess(currentUser, ra);

        if (!vipRequestRepository.existsById(requestId)) {
            ra.addFlashAttribute("error", "Заявка не найдена");
            return "redirect:/admin/vip-requests";
        }

        vipRequestRepository.deleteById(requestId);
        ra.addFlashAttribute("message", "Заявка удалена");
        return "redirect:/admin/vip-requests";
    }

    @PostMapping("/users/{id}/toggle-role")
    public String toggleUserRole(@PathVariable("id") Integer userId,
                                 HttpSession session,
                                 RedirectAttributes ra) {
        User currentUser = getCurrentUser(session);
        if (!isAdmin(currentUser)) return denyAccess(currentUser, ra);

        User user = userRepository.findById(userId).orElse(null);
        if (user == null) {
            ra.addFlashAttribute("error", "Пользователь не найден");
            return "redirect:/admin/vip-requests";
        }

        if (user.getId().equals(currentUser.getId())) {
            ra.addFlashAttribute("error", "Нельзя изменить свою собственную роль");
            return "redirect:/admin/vip-requests";
        }

        if (user.getRole() == Role.ADMIN) {
            user.setRole(Role.USER);
            ra.addFlashAttribute("message", "Пользователю " + user.getLogin() + " убрана роль ADMIN");
        } else {
            user.setRole(Role.ADMIN);
            ra.addFlashAttribute("message", "Пользователю " + user.getLogin() + " назначена роль ADMIN");
        }

        userRepository.save(user);
        userRepository.flush();

        return "redirect:/admin/vip-requests";
    }

    @PostMapping("/users/{id}/toggle-vip")
    public String toggleUserVip(@PathVariable("id") Integer userId,
                                HttpSession session,
                                RedirectAttributes ra) {
        User currentUser = getCurrentUser(session);
        if (!isAdmin(currentUser)) return denyAccess(currentUser, ra);

        User user = userRepository.findById(userId).orElse(null);
        if (user == null) {
            ra.addFlashAttribute("error", "Пользователь не найден");
            return "redirect:/admin/vip-requests";
        }

        user.setIsVip(!user.getIsVip());
        userRepository.save(user);
        userRepository.flush();

        if (user.getIsVip()) {
            ra.addFlashAttribute("message", "Пользователю " + user.getLogin() + " выдан VIP статус");
        } else {
            ra.addFlashAttribute("message", "У пользователя " + user.getLogin() + " убран VIP статус");
        }

        return "redirect:/admin/vip-requests";
    }

    private User getCurrentUser(HttpSession session) {
        if (session == null) return null;
        Object loginObj = session.getAttribute("login");
        if (!(loginObj instanceof String login) || login.isBlank()) return null;
        return userRepository.findByLogin(login).orElse(null);
    }

    private boolean isAdmin(User user) {
        return user != null && user.getRole() == Role.ADMIN;
    }

    private String denyAccess(User user, RedirectAttributes ra) {
        if (user == null) {
            ra.addFlashAttribute("error", "Войдите в систему");
            return "redirect:/login";
        }
        ra.addFlashAttribute("error", "Доступ запрещён");
        return "redirect:/user";
    }

    public static class VipRequestRow {
        private final VipRequest request;
        private final User user;

        public VipRequestRow(VipRequest request, User user) {
            this.request = request;
            this.user = user;
        }

        public VipRequest getRequest() {
            return request;
        }

        public User getUser() {
            return user;
        }
    }
}