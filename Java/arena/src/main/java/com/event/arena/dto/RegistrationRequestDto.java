package com.event.arena.dto;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

public class RegistrationRequestDto {

    @NotBlank(message = "Имя обязательно")
    @Size(max = 100, message = "Имя не должно превышать 100 символов")
    private String firstName;

    @NotBlank(message = "Фамилия обязательна")
    @Size(max = 100, message = "Фамилия не должна превышать 100 символов")
    private String lastName;

    @NotBlank(message = "Email обязателен")
    @Email(message = "Неверный формат email")
    @Size(max = 255, message = "Email не должен превышать 255 символов")
    private String email;

    @NotBlank(message = "Пароль обязателен")
    @Size(min = 8, message = "Пароль должен содержать минимум 8 символов")
    private String password;

    public RegistrationRequestDto() {
    }

    public RegistrationRequestDto(String firstName, String lastName, String email, String password) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.password = password;
    }

    public String getFirstName() {
        return firstName;
    }

    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }

    public String getLastName() {
        return lastName;
    }

    public void setLastName(String lastName) {
        this.lastName = lastName;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }
}