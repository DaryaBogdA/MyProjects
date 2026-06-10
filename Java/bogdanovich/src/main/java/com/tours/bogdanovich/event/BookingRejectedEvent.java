package com.tours.bogdanovich.event;

import com.tours.bogdanovich.entity.Booking;
import org.springframework.context.ApplicationEvent;

public class BookingRejectedEvent extends ApplicationEvent {
    private final Booking booking;
    public BookingRejectedEvent(Object source, Booking booking) {
        super(source);
        this.booking = booking;
    }
    public Booking getBooking() { return booking; }
}