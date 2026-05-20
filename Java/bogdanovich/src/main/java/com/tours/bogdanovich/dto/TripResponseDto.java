package com.tours.bogdanovich.dto;

import java.math.BigDecimal;
import java.util.List;

public class TripResponseDto {
    private Integer tripId;
    private BigDecimal totalPrice;
    private List<TripItemDetail> items;

    public Integer getTripId() {
        return tripId;
    }

    public void setTripId(Integer tripId) {
        this.tripId = tripId;
    }

    public BigDecimal getTotalPrice() {
        return totalPrice;
    }

    public void setTotalPrice(BigDecimal totalPrice) {
        this.totalPrice = totalPrice;
    }

    public List<TripItemDetail> getItems() {
        return items;
    }

    public void setItems(List<TripItemDetail> items) {
        this.items = items;
    }
}

class TripItemDetail {
    private String type;
    private String name;
    private BigDecimal price;
    private Integer quantity;
    private BigDecimal subtotal;

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public BigDecimal getPrice() {
        return price;
    }

    public void setPrice(BigDecimal price) {
        this.price = price;
    }

    public Integer getQuantity() {
        return quantity;
    }

    public void setQuantity(Integer quantity) {
        this.quantity = quantity;
    }

    public BigDecimal getSubtotal() {
        return subtotal;
    }

    public void setSubtotal(BigDecimal subtotal) {
        this.subtotal = subtotal;
    }
}