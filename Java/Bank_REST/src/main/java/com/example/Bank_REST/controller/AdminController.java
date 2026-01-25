package com.example.Bank_REST.controller;

import com.example.Bank_REST.dto.RoleUpdateDto;
import com.example.Bank_REST.dto.UserResponseDto;
import com.example.Bank_REST.entity.Role;
import com.example.Bank_REST.entity.Users;
import com.example.Bank_REST.service.UserService;
import jakarta.validation.Valid;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;
import io.swagger.v3.oas.annotations.Parameter;

import java.time.format.DateTimeFormatter;

/**
 * REST контроллер для управления пользователями (только для администраторов).
 * 
 * <p>Предоставляет эндпоинты для просмотра, обновления ролей и удаления пользователей.
 * Доступен только пользователям с ролью ADMIN.</p>
 * 
 * @author darya
 */
@RestController
@RequestMapping("/admin/users")
@PreAuthorize("hasRole('ADMIN')")
public class AdminController {
    private final UserService userService;
    private static final DateTimeFormatter formatter = DateTimeFormatter.ISO_INSTANT;

    /**
     * Конструктор контроллера администратора.
     * 
     * @param userService сервис для работы с пользователями
     */
    public AdminController(UserService userService) {
        this.userService = userService;
    }

    /**
     * Получает всех пользователей в системе с пагинацией.
     * 
     * @param pageable параметры пагинации
     * @return страница с пользователями
     */
    @GetMapping
    public ResponseEntity<Page<UserResponseDto>> getAllUsers(@Parameter(hidden = true) Pageable pageable) {
        Page<Users> users = userService.getAllUsers(pageable);
        return ResponseEntity.ok(users.map(this::toDto));
    }

    /**
     * Получает пользователя по идентификатору.
     * 
     * @param id идентификатор пользователя
     * @return данные пользователя
     */
    @GetMapping("/{id}")
    public ResponseEntity<UserResponseDto> getUserById(@PathVariable Long id) {
        Users user = userService.getUserById(id);
        return ResponseEntity.ok(toDto(user));
    }

    /**
     * Обновляет роль пользователя.
     * 
     * @param id идентификатор пользователя
     * @param dto данные с новой ролью (ADMIN или USER)
     * @return обновленный пользователь
     */
    @PutMapping("/{id}/role")
    public ResponseEntity<UserResponseDto> updateUserRole(
            @PathVariable Long id,
            @Valid @RequestBody RoleUpdateDto dto) {
        Role role;
        try {
            role = Role.valueOf(dto.getRole().toUpperCase());
        } catch (IllegalArgumentException e) {
            throw new RuntimeException("Invalid role: " + dto.getRole() + ". Valid roles are: ADMIN, USER");
        }
        Users user = userService.updateUserRole(id, role);
        return ResponseEntity.ok(toDto(user));
    }

    /**
     * Удаляет пользователя из системы.
     * 
     * @param id идентификатор пользователя
     * @return сообщение об успешном удалении
     */
    @DeleteMapping("/{id}")
    public ResponseEntity<String> deleteUser(@PathVariable Long id) {
        userService.deleteUser(id);
        return ResponseEntity.ok("User deleted successfully");
    }

    /**
     * Преобразует сущность Users в DTO для ответа.
     * 
     * @param user сущность пользователя
     * @return DTO пользователя
     */
    private UserResponseDto toDto(Users user) {
        UserResponseDto dto = new UserResponseDto();
        dto.setId(user.getId());
        dto.setUsername(user.getUsername());
        dto.setEmail(user.getEmail());
        dto.setRole(user.getRole().name());
        dto.setCreatedAt(user.getCreatedAt().toString());
        return dto;
    }
}
