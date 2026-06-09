package com.tours.bogdanovich.service;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.scheduling.annotation.Async;
import org.springframework.stereotype.Service;

@Service
public class NotificationService {

    private static final Logger log = LoggerFactory.getLogger(NotificationService.class);

    @Async("taskExecutor")
    public void sendBookingApprovedEmail(String userEmail, Integer bookingId) {
        try {
            log.info("Начинаем отправку уведомления для {} о бронировании #{} в потоке {}",
                    userEmail, bookingId, Thread.currentThread().getName());
            Thread.sleep(2000);
            log.info(" Уведомление успешно отправлено пользователю {} по заявке #{}", userEmail, bookingId);
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
            log.error("Отправка уведомления прервана для bookingId={}", bookingId);
        }
    }

    @Async("taskExecutor")
    public void sendBookingRejectedEmail(String userEmail, Integer bookingId) {
        try {
            log.info("Отправка отказа пользователю {} по заявке #{} в потоке {}",
                    userEmail, bookingId, Thread.currentThread().getName());
            Thread.sleep(1000);
            log.info("Уведомление об отказе отправлено");
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        }
    }
}