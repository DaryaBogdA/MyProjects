package com.example.Bank_REST.dto;

import jakarta.validation.constraints.DecimalMin;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import java.math.BigDecimal;
import java.time.LocalDate;

/**
 * DTO для создания новой банковской карты.
 * 
 * @author darya
 */
public class CardCreateDto {
    @NotNull
    @Size(min = 13, max = 19)
    private String number;

    @NotNull
    private Long ownerId;

    @NotNull
    private LocalDate period;

    @NotNull
    @DecimalMin("0.00")
    private BigDecimal balance;

    public @NotNull @Size(min = 13, max = 19) String getNumber() {
        return number;
    }

    public void setNumber(@NotNull @Size(min = 13, max = 19) String number) {
        this.number = number;
    }

    public @NotNull Long getOwnerId() {
        return ownerId;
    }

    public void setOwnerId(@NotNull Long ownerId) {
        this.ownerId = ownerId;
    }

    public @NotNull LocalDate getPeriod() {
        return period;
    }

    public void setPeriod(@NotNull LocalDate period) {
        this.period = period;
    }

    public @NotNull @DecimalMin("0.00") BigDecimal getBalance() {
        return balance;
    }

    public void setBalance(@NotNull @DecimalMin("0.00") BigDecimal balance) {
        this.balance = balance;
    }
}
