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

@RestController
@RequestMapping("/admin/users")
@PreAuthorize("hasRole('ADMIN')")
public class AdminController {
    private final UserService userService;
    private static final DateTimeFormatter formatter = DateTimeFormatter.ISO_INSTANT;

    public AdminController(UserService userService) {
        this.userService = userService;
    }

    @GetMapping
    public ResponseEntity<Page<UserResponseDto>> getAllUsers(@Parameter(hidden = true) Pageable pageable) {
        Page<Users> users = userService.getAllUsers(pageable);
        return ResponseEntity.ok(users.map(this::toDto));
    }

    @GetMapping("/{id}")
    public ResponseEntity<UserResponseDto> getUserById(@PathVariable Long id) {
        Users user = userService.getUserById(id);
        return ResponseEntity.ok(toDto(user));
    }

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

    @DeleteMapping("/{id}")
    public ResponseEntity<String> deleteUser(@PathVariable Long id) {
        userService.deleteUser(id);
        return ResponseEntity.ok("User deleted successfully");
    }

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
