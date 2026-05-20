package com.tours.bogdanovich.dto;

import java.time.LocalDate;

public class BookTourRequestDto {
    private LocalDate travelDate;

    public LocalDate getTravelDate() {
        return travelDate;
    }

    public void setTravelDate(LocalDate travelDate) {
        this.travelDate = travelDate;
    }
}
