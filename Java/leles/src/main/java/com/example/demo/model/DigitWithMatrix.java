package com.example.demo.model;

/**
 * Пара (цифра матрицы, описание) для передачи в шаблон истории раскладов.
 */
public record DigitWithMatrix(int digit, Matrix matrix) {
}
