package com.tours.bogdanovich.service;

import com.tours.bogdanovich.entity.*;
import com.tours.bogdanovich.repository.BookingRepository;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigDecimal;
import java.nio.charset.StandardCharsets;
import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.util.EnumSet;

@Service
@Transactional(readOnly = true)
public class BookingDocumentService {

    private static final EnumSet<BookingStatus> DOWNLOAD_ALLOWED = EnumSet.of(
            BookingStatus.CONFIRMED, BookingStatus.PAID, BookingStatus.COMPLETED);

    private static final DateTimeFormatter DATE_FMT = DateTimeFormatter.ofPattern("dd.MM.yyyy");

    private final BookingRepository bookingRepository;
    private final UserService userService;
    private final PricingService pricingService;

    public BookingDocumentService(BookingRepository bookingRepository,
                                  UserService userService,
                                  PricingService pricingService) {
        this.bookingRepository = bookingRepository;
        this.userService = userService;
        this.pricingService = pricingService;
    }

    public byte[] buildDocument(Integer bookingId, String userEmail) {
        Users user = userService.loadUser(userEmail);
        Booking booking = bookingRepository.findByIdAndUser_Id(bookingId, user.getId())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Booking not found"));
        if (booking.getStatus() == null || !DOWNLOAD_ALLOWED.contains(booking.getStatus())) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN,
                    "Скачивание доступно после одобрения заявки администратором");
        }
        return formatDocument(booking, user).getBytes(StandardCharsets.UTF_8);
    }

    public static boolean canDownload(String status) {
        if (status == null) {
            return false;
        }
        try {
            return DOWNLOAD_ALLOWED.contains(BookingStatus.valueOf(status));
        } catch (IllegalArgumentException e) {
            return false;
        }
    }

    private String formatDocument(Booking booking, Users user) {
        StringBuilder sb = new StringBuilder();
        sb.append("══════════════════════════════════════════════════\n");
        sb.append("           TravelCanvas — информация о туре\n");
        sb.append("══════════════════════════════════════════════════\n\n");
        sb.append("Номер бронирования: ").append(booking.getId()).append("\n");
        sb.append("Статус: ").append(statusRu(booking.getStatus())).append("\n");
        sb.append("Email: ").append(user.getEmail()).append("\n");
        if (booking.getTotalPrice() != null) {
            sb.append("Сумма: ").append(booking.getTotalPrice()).append(" €\n");
        }
        sb.append("\n");

        if (booking.getTour() != null) {
            appendReadyTour(sb, booking);
        } else if (booking.getTrip() != null) {
            appendCustomTrip(sb, booking);
        } else {
            sb.append("Данные о туре отсутствуют.\n");
        }

        sb.append("\n──────────────────────────────────────────────────\n");
        sb.append("Документ сформирован автоматически. Приятного путешествия!\n");
        return sb.toString();
    }

    private void appendReadyTour(StringBuilder sb, Booking booking) {
        Tour tour = booking.getTour();
        sb.append("Тип: готовый тур\n");
        sb.append("Название: ").append(nullToDash(tour.getName())).append("\n");
        if (tour.getDurationDays() != null) {
            sb.append("Длительность: ").append(tour.getDurationDays()).append(" дн.\n");
        }
        if (booking.getTravelDate() != null) {
            sb.append("Дата начала: ").append(formatDate(booking.getTravelDate())).append("\n");
            if (tour.getDurationDays() != null && tour.getDurationDays() > 0) {
                LocalDate end = booking.getTravelDate().plusDays(tour.getDurationDays() - 1L);
                sb.append("Дата окончания: ").append(formatDate(end)).append("\n");
            }
        }
        if (tour.getCity() != null) {
            sb.append("Город: ").append(tour.getCity().getName()).append("\n");
            if (tour.getCity().getCountry() != null) {
                sb.append("Страна: ").append(tour.getCity().getCountry().getName()).append("\n");
            }
        }
        sb.append("\n--- Описание ---\n");
        sb.append(nullToEmpty(tour.getDescription())).append("\n");
        if (tour.getProgramInfo() != null && !tour.getProgramInfo().isBlank()) {
            sb.append("\n--- Программа по дням ---\n");
            sb.append(tour.getProgramInfo().trim()).append("\n");
        }
    }

    private void appendCustomTrip(StringBuilder sb, Booking booking) {
        Trip trip = booking.getTrip();
        sb.append("Тип: собственный тур\n");
        sb.append("Название: ").append(nullToDash(trip.getTitle())).append("\n");
        if (trip.getDateFrom() != null || trip.getDateTo() != null) {
            sb.append("Период: ")
                    .append(formatDate(trip.getDateFrom()))
                    .append(" — ")
                    .append(formatDate(trip.getDateTo()))
                    .append("\n");
        }
        if (trip.getPeopleCount() != null) {
            sb.append("Количество человек: ").append(trip.getPeopleCount()).append("\n");
        }
        if (trip.getCountry() != null) {
            sb.append("Страна: ").append(trip.getCountry().getName()).append("\n");
        }
        if (trip.getCity() != null) {
            sb.append("Город: ").append(trip.getCity().getName()).append("\n");
        }
        trip.getItems().size();
        if (!trip.getItems().isEmpty()) {
            sb.append("\n--- Состав тура ---\n");
            for (TripItem it : trip.getItems()) {
                int q = it.getQuantity() != null ? it.getQuantity() : 1;
                BigDecimal unit = pricingService.getPricePerUnit(it.getItemType(), it.getItemId());
                BigDecimal line = unit.multiply(BigDecimal.valueOf(q));
                sb.append("• ")
                        .append(pricingService.describeItem(it.getItemType(), it.getItemId()))
                        .append(" × ").append(q)
                        .append(" — ").append(line).append(" €\n");
            }
        }
    }

    private static String statusRu(BookingStatus status) {
        if (status == null) {
            return "—";
        }
        return switch (status) {
            case PENDING -> "На рассмотрении";
            case CONFIRMED -> "Подтверждено";
            case PAID -> "Оплачено";
            case CANCELLED -> "Отменено";
            case COMPLETED -> "Завершено";
        };
    }

    private static String formatDate(LocalDate date) {
        return date != null ? date.format(DATE_FMT) : "—";
    }

    private static String nullToDash(String s) {
        return s != null && !s.isBlank() ? s : "—";
    }

    private static String nullToEmpty(String s) {
        return s != null ? s : "";
    }
}
