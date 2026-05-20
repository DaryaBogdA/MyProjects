package com.tours.bogdanovich.dto;

import java.math.BigDecimal;

public class RoomTypeDto {
    private Integer id;
    private String name;
    private BigDecimal priceNight;
    private Integer maxOccupancy;

    public RoomTypeDto(Integer id, String name, BigDecimal priceNight, Integer maxOccupancy) {
        this.id = id;
        this.name = name;
        this.priceNight = priceNight;
        this.maxOccupancy = maxOccupancy;
    }

    public Integer getId() {
        return id;
    }

    public String getName() {
        return name;
    }

    public BigDecimal getPriceNight() {
        return priceNight;
    }

    public Integer getMaxOccupancy() {
        return maxOccupancy;
    }
}