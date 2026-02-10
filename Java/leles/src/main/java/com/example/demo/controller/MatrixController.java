package com.example.demo.controller;

import com.example.demo.model.DigitWithMatrix;
import com.example.demo.model.Matrix;
import com.example.demo.model.MatrixHistory;
import com.example.demo.model.User;
import com.example.demo.repository.MatrixHistoryRepository;
import com.example.demo.repository.MatrixRepository;
import com.example.demo.repository.UserRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.ResponseBody;

import java.time.LocalDate;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@Controller
public class MatrixController {
    @Autowired
    private MatrixRepository matrixRepository;
    @Autowired
    private UserRepository userRepository;
    @Autowired
    private MatrixHistoryRepository matrixHistoryRepository;

    private static final int MAX_FREE_MATRIX = 3;

    @GetMapping("/matrix")
    public String matrix(HttpSession session, Model model) {
        String login = (String) session.getAttribute("login");
        if (login == null) {
            return "redirect:/login";
        }

        User user = userRepository.findByLogin(login).orElse(null);
        if (user == null) {
            session.removeAttribute("login");
            return "redirect:/login";
        }

        model.addAttribute("login", login);
        model.addAttribute("isVip", user.getIsVip());
        model.addAttribute("madeCount", user.getRequestCount());
        model.addAttribute("maxFree", MAX_FREE_MATRIX);
        model.addAttribute("remaining", Math.max(0, MAX_FREE_MATRIX - user.getRequestCount()));
        return "matrix";
    }

    @PostMapping("/matrix/descriptions")
    @ResponseBody
    public ResponseEntity<Map<String, Object>> getDescriptions(@RequestBody Map<String, Object> body,
                                                               HttpSession session) {
        Map<String, Object> response = new HashMap<>();

        String login = (String) session.getAttribute("login");
        if (login == null) {
            response.put("success", false);
            response.put("error", "login");
            return ResponseEntity.ok(response);
        }

        User user = userRepository.findByLogin(login).orElse(null);
        if (user == null) {
            response.put("success", false);
            response.put("error", "login");
            return ResponseEntity.ok(response);
        }

        if (!Boolean.TRUE.equals(user.getIsVip()) && user.getRequestCount() >= MAX_FREE_MATRIX) {
            response.put("success", false);
            response.put("error", "limit");
            response.put("message", "Вы исчерпали лимит бесплатных расчётов матрицы (3). Приобретите VIP для неограниченных расчётов.");
            return ResponseEntity.ok(response);
        }

        @SuppressWarnings("unchecked")
        List<Integer> digits = (List<Integer>) body.get("digits");
        String birthDateStr = (String) body.get("birthDate");

        if (digits == null || digits.isEmpty()) {
            response.put("success", false);
            response.put("error", "digits");
            return ResponseEntity.ok(response);
        }

        try {
            List<Matrix> matrices = matrixRepository.findByIdIn(digits);

            Map<Integer, Matrix> descriptions = matrices.stream()
                    .collect(Collectors.toMap(Matrix::getId, m -> m));

            if (!Boolean.TRUE.equals(user.getIsVip())) {
                user.setRequestCount((user.getRequestCount() == null ? 0 : user.getRequestCount()) + 1);
                userRepository.save(user);
            }

            MatrixHistory history = new MatrixHistory();
            history.setUser(user);
            history.setBirthDate(birthDateStr != null ? LocalDate.parse(birthDateStr) : LocalDate.now());
            history.setDigits(digits.stream().map(String::valueOf).collect(Collectors.joining(",")));
            matrixHistoryRepository.save(history);

            response.put("success", true);
            response.put("descriptions", descriptions);
            response.put("historyId", history.getId());
            return ResponseEntity.ok(response);

        } catch (Exception e) {
            response.put("success", false);
            response.put("error", e.getMessage());
            return ResponseEntity.badRequest().body(response);
        }
    }

    @GetMapping("/matrix/history")
    public String history(HttpSession session, Model model) {
        String login = (String) session.getAttribute("login");
        if (login == null) return "redirect:/login";

        User user = userRepository.findByLogin(login).orElse(null);
        if (user == null) {
            session.removeAttribute("login");
            return "redirect:/login";
        }

        List<MatrixHistory> historyList = matrixHistoryRepository.findByUserIdOrderByCreatedAtDesc(user.getId());
        model.addAttribute("historyList", historyList);
        model.addAttribute("login", login);
        return "matrixHistory";
    }

    @GetMapping("/matrix/history/{id}")
    public String historyView(@PathVariable Integer id, HttpSession session, Model model) {
        String login = (String) session.getAttribute("login");
        if (login == null) return "redirect:/login";

        User user = userRepository.findByLogin(login).orElse(null);
        if (user == null) {
            session.removeAttribute("login");
            return "redirect:/login";
        }

        MatrixHistory history = matrixHistoryRepository.findById(id).orElse(null);
        if (history == null || !history.getUser().getId().equals(user.getId())) {
            return "redirect:/matrix/history";
        }

        int[] digits = history.getDigitsArray();
        List<Integer> mainDigitsList = digits.length >= 5
                ? List.of(digits[0], digits[1], digits[2], digits[3], digits[4])
                : List.of();

        List<DigitWithMatrix> descriptionItems = new java.util.ArrayList<>();
        for (Integer digitId : mainDigitsList) {
            matrixRepository.findById(digitId).ifPresent(m -> descriptionItems.add(new DigitWithMatrix(digitId, m)));
        }

        model.addAttribute("history", history);
        model.addAttribute("digits", digits);
        model.addAttribute("descriptionItems", descriptionItems);
        model.addAttribute("login", login);
        return "matrixHistoryView";
    }
}
