package com.example.kavalenok.model;

import jakarta.persistence.AttributeConverter;
import jakarta.persistence.Converter;

@Converter(autoApply = false)
public class ReportStatusConverter implements AttributeConverter<Report.Status, String> {

    @Override
    public String convertToDatabaseColumn(Report.Status attribute) {
        if (attribute == null) {
            return null;
        }
        return attribute.name().toLowerCase();
    }

    @Override
    public Report.Status convertToEntityAttribute(String dbData) {
        if (dbData == null || dbData.isBlank()) {
            return Report.Status.PENDING;
        }
        return Report.Status.valueOf(dbData.trim().toUpperCase());
    }
}
