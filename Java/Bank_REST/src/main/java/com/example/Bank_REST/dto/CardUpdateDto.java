package com.example.Bank_REST.dto;

import java.math.BigDecimal;
import java.time.LocalDate;

public class CardUpdateDto {
    private LocalDate period;
    private BigDecimal balance;

    public LocalDate getPeriod() {
        return period;
    }

    public void setPeriod(LocalDate period) {
        this.period = period;
    }

    public BigDecimal getBalance() {
        return balance;
    }

    public void setBalance(BigDecimal balance) {
        this.balance = balance;
    }
}
