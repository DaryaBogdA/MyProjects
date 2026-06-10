package com.tours.bogdanovich.listener;

import com.tours.bogdanovich.entity.Booking;
import com.tours.bogdanovich.event.BookingRejectedEvent;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.event.EventListener;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.scheduling.annotation.Async;
import org.springframework.stereotype.Component;

@Component
public class RejectEmailNotificationListener {

    private static final Logger log = LoggerFactory.getLogger(RejectEmailNotificationListener.class);
    private final JavaMailSender mailSender;

    public RejectEmailNotificationListener(JavaMailSender mailSender) {
        this.mailSender = mailSender;
    }

    @Async
    @EventListener
    public void onBookingRejected(BookingRejectedEvent event) {
        Booking booking = event.getBooking();
        String userEmail = booking.getUser().getEmail();
        Integer bookingId = booking.getId();

        try {
            SimpleMailMessage message = new SimpleMailMessage();
            message.setTo(userEmail);
            message.setSubject("Ваша заявка отклонена");
            message.setText("Здравствуйте!\n\nВаша заявка №" + bookingId + " на сайте TravelCanvas была отклонена администратором.\n\nС уважением,\nКоманда TravelCanvas");
            message.setFrom("darya.bog.danovich1@gmail.com");
            mailSender.send(message);
            log.info(" Письмо об отказе отправлено пользователю {} по заявке №{}", userEmail, bookingId);
        } catch (Exception e) {
            log.error("Ошибка отправки письма об отказе: {}", e.getMessage());
        }
    }
}