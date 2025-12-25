package com.example.demo.model;

import jakarta.persistence.AttributeConverter;
import jakarta.persistence.Converter;

@Converter(autoApply = true)
public class YesNoConverter implements AttributeConverter<YesNo, String> {

    @Override
    public String convertToDatabaseColumn(YesNo attribute) {
        return attribute == null ? YesNo.NO.getDbValue() : attribute.getDbValue();
    }

    @Override
    public YesNo convertToEntityAttribute(String dbData) {
        return YesNo.fromDb(dbData);
    }
}
