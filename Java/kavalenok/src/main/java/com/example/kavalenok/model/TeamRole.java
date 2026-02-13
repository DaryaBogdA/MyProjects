package com.example.kavalenok.model;

public enum TeamRole {
    SETTER("Связующий"),
    LIBERO("Либеро"),
    MIDDLE_BLOCKER("Центральный блокирующий"),
    OUTSIDE_HITTER("Доигровщик"),
    OPPOSITE("Диагональный");

    private final String displayName;

    TeamRole(String displayName) {
        this.displayName = displayName;
    }

    public String getDisplayName() {
        return displayName;
    }
}