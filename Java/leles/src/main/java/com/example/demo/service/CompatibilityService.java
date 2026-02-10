package com.example.demo.service;

import com.example.demo.model.Compatibility;
import com.example.demo.repository.CompatibilityRepository;
import org.springframework.stereotype.Service;

import java.time.LocalDate;

@Service
public class CompatibilityService {

    private final CompatibilityRepository repository;

    public CompatibilityService(CompatibilityRepository repository) {
        this.repository = repository;
    }

    public String calculateCompatibility(LocalDate date1, LocalDate date2) {
        int number = calculateNumber(date1, date2);

        return repository.findById(number)
                .map(Compatibility::getText)
                .orElse("Описание для числа " + number + " не найдено.");
    }

    public int calculateNumber(LocalDate date1, LocalDate date2) {
        int n1 = sumDigits(dateToString(date1));
        int n2 = sumDigits(dateToString(date2));
        int sum = n1 + n2;

        while (sum > 22) {
            sum = sumDigits(Integer.toString(sum));
        }

        return sum;
    }

    private String dateToString(LocalDate date) {
        return String.format("%02d%02d%d",
                date.getDayOfMonth(),
                date.getMonthValue(),
                date.getYear());
    }

    private int sumDigits(String str) {
        int sum = 0;
        for (char c : str.toCharArray()) {
            if (Character.isDigit(c)) {
                sum += Character.getNumericValue(c);
            }
        }
        return sum;
    }
}