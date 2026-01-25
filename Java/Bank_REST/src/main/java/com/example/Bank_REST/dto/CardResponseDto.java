package com.example.Bank_REST.dto;

import java.math.BigDecimal;
import java.time.LocalDate;

/**
 * DTO для ответа с данными банковской карты.
 * 
 * @author darya
 */
public class CardResponseDto {
    private Long id;
    private String maskedNumber;
    private Long ownerId;
    private LocalDate period;
    private String status;
    private BigDecimal balance;


    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getMaskedNumber() {
        return maskedNumber;
    }

    public void setMaskedNumber(String maskedNumber) {
        this.maskedNumber = maskedNumber;
    }

    public Long getOwnerId() {
        return ownerId;
    }

    public void setOwnerId(Long ownerId) {
        this.ownerId = ownerId;
    }

    public LocalDate getPeriod() {
        return period;
    }

    public void setPeriod(LocalDate period) {
        this.period = period;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public BigDecimal getBalance() {
        return balance;
    }

    public void setBalance(BigDecimal balance) {
        this.balance = balance;
    }
}
