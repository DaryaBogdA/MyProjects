package com.example.demo.controller;

import com.example.demo.model.DigitWithMatrix;
import com.example.demo.model.Matrix;
import com.example.demo.model.MatrixHistory;
import com.example.demo.model.User;
import com.example.demo.repository.MatrixHistoryRepository;
import com.example.demo.repository.MatrixRepository;
import com.example.demo.repository.UserRepository;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpHeaders;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.ResponseBody;

import java.io.IOException;
import java.io.PrintWriter;
import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
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

        List<DigitWithMatrix> descriptionItems = new ArrayList<>();
        for (Integer digitId : mainDigitsList) {
            matrixRepository.findById(digitId).ifPresent(m -> descriptionItems.add(new DigitWithMatrix(digitId, m)));
        }

        model.addAttribute("history", history);
        model.addAttribute("digits", digits);
        model.addAttribute("descriptionItems", descriptionItems);
        model.addAttribute("login", login);
        return "matrixHistoryView";
    }

    @GetMapping("/matrix/history/{id}/export")
    public void exportSingleMatrix(@PathVariable Integer id,
                                   HttpSession session,
                                   HttpServletResponse response) throws IOException {
        String login = (String) session.getAttribute("login");
        if (login == null) {
            response.sendRedirect("/login");
            return;
        }

        User user = userRepository.findByLogin(login).orElse(null);
        if (user == null) {
            session.removeAttribute("login");
            response.sendRedirect("/login");
            return;
        }

        MatrixHistory history = matrixHistoryRepository.findById(id).orElse(null);
        if (history == null || !history.getUser().getId().equals(user.getId())) {
            response.sendRedirect("/matrix/history");
            return;
        }

        int[] digits = history.getDigitsArray();
        List<Integer> mainDigitsList = digits.length >= 5
                ? List.of(digits[0], digits[1], digits[2], digits[3], digits[4])
                : List.of();

        List<DigitWithMatrix> descriptionItems = new ArrayList<>();
        for (Integer digitId : mainDigitsList) {
            matrixRepository.findById(digitId).ifPresent(m -> descriptionItems.add(new DigitWithMatrix(digitId, m)));
        }

        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("dd-MM-yyyy");
        String fileName = "matrix_" + history.getBirthDate().format(formatter) + ".txt";

        response.setContentType("text/plain;charset=UTF-8");
        response.setHeader(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=\"" + fileName + "\"");

        try (PrintWriter writer = response.getWriter()) {
            writer.println("МАТРИЦА СУДЬБЫ");
            writer.println("=" .repeat(50));
            writer.println("Дата рождения: " + history.getBirthDate().format(DateTimeFormatter.ofPattern("dd.MM.yyyy")));
            writer.println("Сохранено: " + history.getCreatedAt().format(DateTimeFormatter.ofPattern("dd.MM.yyyy HH:mm")));
            writer.println();

            writer.println("ЦИФРЫ МАТРИЦЫ:");
            writer.println("-" .repeat(30));
            if (digits.length >= 9) {
                writer.println("Верхний угол: " + digits[0]);
                writer.println("Левый угол: " + digits[1]);
                writer.println("Правый угол: " + digits[2]);
                writer.println("Нижний угол: " + digits[3]);
                writer.println("Центр: " + digits[4]);
            }
            writer.println();

            writer.println("РАСШИФРОВКА:");
            writer.println("=" .repeat(50));

            for (DigitWithMatrix item : descriptionItems) {
                writer.println();
                writer.println("ЦИФРА " + item.digit());
                writer.println("-" .repeat(30));

                writer.println("--- В ПЛЮСЕ ---");
                writer.println("Основная энергия: " + (item.matrix().getEssenceEnergyPlus() != null ?
                        item.matrix().getEssenceEnergyPlus().replaceAll("<[^>]*>", "") : ""));
                writer.println("Сильные стороны: " + (item.matrix().getStrengthsPlus() != null ?
                        item.matrix().getStrengthsPlus().replaceAll("<[^>]*>", "") : ""));
                writer.println("Риски: " + (item.matrix().getRisksPlus() != null ?
                        item.matrix().getRisksPlus().replaceAll("<[^>]*>", "") : ""));

                writer.println("--- В МИНУСЕ ---");
                writer.println("Основная энергия: " + (item.matrix().getEssenceEnergyMinus() != null ?
                        item.matrix().getEssenceEnergyMinus().replaceAll("<[^>]*>", "") : ""));
                writer.println("В минусе: " + (item.matrix().getInMinus() != null ?
                        item.matrix().getInMinus().replaceAll("<[^>]*>", "") : ""));
                writer.println("Ловушки: " + (item.matrix().getTrapsMinus() != null ?
                        item.matrix().getTrapsMinus().replaceAll("<[^>]*>", "") : ""));

                writer.println("--- ПРАКТИКИ ---");
                writer.println("Практика: " + (item.matrix().getEssenceEnergyPractice() != null ?
                        item.matrix().getEssenceEnergyPractice().replaceAll("<[^>]*>", "") : ""));
                writer.println("Доп. практика: " + (item.matrix().getInPractice() != null ?
                        item.matrix().getInPractice().replaceAll("<[^>]*>", "") : ""));
                writer.println("Деньги: " + (item.matrix().getMoneyPractice() != null ?
                        item.matrix().getMoneyPractice().replaceAll("<[^>]*>", "") : ""));
                writer.println("Отношения: " + (item.matrix().getRelationshipPractice() != null ?
                        item.matrix().getRelationshipPractice().replaceAll("<[^>]*>", "") : ""));

                writer.println("-" .repeat(30));
            }
        }
    }

    @GetMapping("/matrix/history/export-all")
    public void exportAllMatrices(HttpSession session, HttpServletResponse response) throws IOException {
        String login = (String) session.getAttribute("login");
        if (login == null) {
            response.sendRedirect("/login");
            return;
        }

        User user = userRepository.findByLogin(login).orElse(null);
        if (user == null) {
            session.removeAttribute("login");
            response.sendRedirect("/login");
            return;
        }

        List<MatrixHistory> historyList = matrixHistoryRepository.findByUserIdOrderByCreatedAtDesc(user.getId());

        String fileName = "all_matrices_" + LocalDate.now().format(DateTimeFormatter.ofPattern("dd-MM-yyyy")) + ".txt";

        response.setContentType("text/plain;charset=UTF-8");
        response.setHeader(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=\"" + fileName + "\"");

        try (PrintWriter writer = response.getWriter()) {
            writer.println("ВСЕ МАТРИЦЫ ПОЛЬЗОВАТЕЛЯ: " + user.getLogin());
            writer.println("=" .repeat(50));
            writer.println("Всего раскладов: " + historyList.size());
            writer.println("=" .repeat(50));

            DateTimeFormatter dateFormatter = DateTimeFormatter.ofPattern("dd.MM.yyyy");

            for (int i = 0; i < historyList.size(); i++) {
                MatrixHistory history = historyList.get(i);
                int[] digits = history.getDigitsArray();

                writer.println();
                writer.println("РАСКЛАД #" + (i + 1));
                writer.println("-" .repeat(30));
                writer.println("Дата рождения: " + history.getBirthDate().format(dateFormatter));
                writer.println("Сохранено: " + history.getCreatedAt().format(DateTimeFormatter.ofPattern("dd.MM.yyyy HH:mm")));

                if (digits.length >= 9) {
                    writer.println("Цифры матрицы: " + digits[0] + ", " + digits[1] + ", " + digits[2] + ", " +
                            digits[3] + ", " + digits[4] + ", " + digits[5] + ", " +
                            digits[6] + ", " + digits[7] + ", " + digits[8]);
                }

                writer.println("Ссылка: " + "http://localhost:8080/matrix/history/" + history.getId());
            }
        }
    }
}