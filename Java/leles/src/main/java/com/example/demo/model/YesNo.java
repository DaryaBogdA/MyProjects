package com.example.demo.model;

public enum YesNo {
    NO("no"),
    YES("yes");

    private final String dbValue;

    YesNo(String dbValue) {
        this.dbValue = dbValue;
    }

    public String getDbValue() {
        return dbValue;
    }

    public static YesNo fromDb(String dbValue) {
        if (dbValue == null) return NO;
        return "yes".equalsIgnoreCase(dbValue) ? YES : NO;
    }
}
