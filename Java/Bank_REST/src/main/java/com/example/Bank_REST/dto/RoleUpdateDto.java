package com.example.Bank_REST.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;

public class RoleUpdateDto {
    @NotBlank(message = "Role is required")
    @Pattern(regexp = "^(ADMIN|USER)$", flags = Pattern.Flag.CASE_INSENSITIVE, message = "Role must be either ADMIN or USER")
    private String role;

    public String getRole() {
        return role;
    }

    public void setRole(String role) {
        this.role = role;
    }
}
