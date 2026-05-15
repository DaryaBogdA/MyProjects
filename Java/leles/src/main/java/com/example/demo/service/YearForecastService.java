package com.example.demo.service;

import org.springframework.stereotype.Service;

import java.time.LocalDate;
import java.time.Month;
import java.util.ArrayList;
import java.util.List;

@Service
public class YearForecastService {

    public record MonthForecast(int month, String monthName, int energy) {}

    public record YearForecastResult(int personalYear, List<MonthForecast> months) {}

    public YearForecastResult calculate(LocalDate birthDate, int forecastYear) {
        int dayEnergy = sumDigits(birthDate.getDayOfMonth());
        int monthEnergy = birthDate.getMonthValue();
        int yearEnergy = sumDigits(forecastYear);
        int personalYear = sumDigits(dayEnergy + monthEnergy + yearEnergy);

        List<MonthForecast> months = new ArrayList<>(12);
        for (int m = 1; m <= 12; m++) {
            int monthForecast = sumDigits(personalYear + m);
            months.add(new MonthForecast(m, monthName(m), monthForecast));
        }

        return new YearForecastResult(personalYear, months);
    }

    private String monthName(int month) {
        return switch (month) {
            case 1 -> "Январь";
            case 2 -> "Февраль";
            case 3 -> "Март";
            case 4 -> "Апрель";
            case 5 -> "Май";
            case 6 -> "Июнь";
            case 7 -> "Июль";
            case 8 -> "Август";
            case 9 -> "Сентябрь";
            case 10 -> "Октябрь";
            case 11 -> "Ноябрь";
            case 12 -> "Декабрь";
            default -> Month.of(month).getDisplayName(java.time.format.TextStyle.FULL_STANDALONE,
                    java.util.Locale.forLanguageTag("ru"));
        };
    }

    private int sumDigits(int num) {
        while (num > 22) {
            int sum = 0;
            while (num > 0) {
                sum += num % 10;
                num /= 10;
            }
            num = sum;
        }
        return num;
    }
}
