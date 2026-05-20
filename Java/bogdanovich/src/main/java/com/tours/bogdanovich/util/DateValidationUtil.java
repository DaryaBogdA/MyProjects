package com.tours.bogdanovich.util;

import org.springframework.http.HttpStatus;
import org.springframework.web.server.ResponseStatusException;

import java.time.LocalDate;

public final class DateValidationUtil {

    private DateValidationUtil() {
    }

    public static void requireTravelDate(LocalDate travelDate) {
        if (travelDate == null) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Укажите дату начала поездки");
        }
        if (travelDate.isBefore(LocalDate.now())) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Нельзя бронировать тур в прошлом");
        }
    }

    public static void requireTripDates(LocalDate dateFrom, LocalDate dateTo) {
        if (dateFrom == null || dateTo == null) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Укажите даты поездки");
        }
        if (dateFrom.isBefore(LocalDate.now())) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Дата начала не может быть в прошлом");
        }
        if (dateTo.isBefore(dateFrom)) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Дата окончания раньше даты начала");
        }
    }
}
