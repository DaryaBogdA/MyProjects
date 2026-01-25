package com.example.Bank_REST.exception;

import io.swagger.v3.oas.annotations.Hidden;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.validation.FieldError;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;

import java.util.HashMap;
import java.util.Map;

/**
 * Глобальный обработчик исключений для REST API.
 * 
 * <p>Обрабатывает различные типы исключений и возвращает структурированные
 * ответы с соответствующими HTTP статусами.</p>
 * 
 * @author darya
 */
@RestControllerAdvice
@Hidden
public class GlobalExceptionHandler {

    /**
     * Обрабатывает ошибки валидации входных данных.
     * 
     * @param ex исключение валидации
     * @return ответ с ошибками валидации и статусом 400
     */
    @ExceptionHandler(MethodArgumentNotValidException.class)
    public ResponseEntity<Map<String, String>> handleValidationExceptions(MethodArgumentNotValidException ex) {
        Map<String, String> errors = new HashMap<>();
        ex.getBindingResult().getAllErrors().forEach((error) -> {
            String fieldName = ((FieldError) error).getField();
            String errorMessage = error.getDefaultMessage();
            errors.put(fieldName, errorMessage);
        });
        return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(errors);
    }

    /**
     * Обрабатывает исключения доступа (403 Forbidden).
     * 
     * @param ex исключение доступа
     * @return ответ с ошибкой и статусом 403
     */
    @ExceptionHandler(AccessDeniedException.class)
    public ResponseEntity<Map<String, String>> handleAccessDeniedException(AccessDeniedException ex) {
        Map<String, String> error = new HashMap<>();
        error.put("error", "Access denied: " + ex.getMessage());
        return ResponseEntity.status(HttpStatus.FORBIDDEN).body(error);
    }

    /**
     * Обрабатывает общие RuntimeException.
     * 
     * <p>Определяет тип ошибки по сообщению и возвращает соответствующий HTTP статус:
     * 403 для ошибок доступа, 400 для остальных.</p>
     * 
     * @param ex исключение времени выполнения
     * @return ответ с ошибкой и соответствующим статусом
     */
    @ExceptionHandler(RuntimeException.class)
    public ResponseEntity<Map<String, String>> handleRuntimeException(RuntimeException ex) {
        Map<String, String> error = new HashMap<>();
        String message = ex.getMessage();
        
        if (message != null && (message.contains("Access denied") || message.contains("does not belong"))) {
            error.put("error", message);
            return ResponseEntity.status(HttpStatus.FORBIDDEN).body(error);
        }
        
        error.put("error", message);
        return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(error);
    }

    /**
     * Обрабатывает все остальные необработанные исключения.
     * 
     * @param ex исключение
     * @return ответ с ошибкой и статусом 500
     */
    @ExceptionHandler(Exception.class)
    public ResponseEntity<Map<String, String>> handleGenericException(Exception ex) {
        Map<String, String> error = new HashMap<>();
        error.put("error", "An unexpected error occurred: " + ex.getMessage());
        return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(error);
    }
}
