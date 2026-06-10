package com.tours.bogdanovich.listener;

import com.tours.bogdanovich.entity.Booking;
import com.tours.bogdanovich.event.BookingApprovedEvent;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.event.EventListener;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.scheduling.annotation.Async;
import org.springframework.stereotype.Component;

@Component
public class EmailNotificationListener {

    private static final Logger log = LoggerFactory.getLogger(EmailNotificationListener.class);
    private final JavaMailSender mailSender;

    public EmailNotificationListener(JavaMailSender mailSender) {
        this.mailSender = mailSender;
    }

    @Async
    @EventListener
    public void onBookingApproved(BookingApprovedEvent event) {
        Booking booking = event.getBooking();
        String userEmail = booking.getUser().getEmail();
        Integer bookingId = booking.getId();

        try {
            SimpleMailMessage message = new SimpleMailMessage();
            message.setTo(userEmail);
            message.setSubject("Ваша заявка подтверждена");
            message.setText("Здравствуйте!\n\nВаша заявка №" + bookingId + " на сайте TravelCanvas подтверждена.\n\nС уважением,\nКоманда TravelCanvas");
            message.setFrom("darya.bog.danovich1@gmail.com");
            mailSender.send(message);
            log.info(" Письмо отправлено пользователю {} по заявке №{}", userEmail, bookingId);
        } catch (Exception e) {
            log.error("Ошибка отправки письма: {}", e.getMessage());
        }
    }
}