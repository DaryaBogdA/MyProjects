package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.LoginRequestDto;
import com.tours.bogdanovich.dto.RegistrationRequestDto;
import com.tours.bogdanovich.entity.Users;
import com.tours.bogdanovich.service.JwtService;
import com.tours.bogdanovich.service.UserService;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.Map;

@RestController
@RequestMapping("/auth")
public class AuthController {

    private final UserService userService;
    private final JwtService jwtService;
    private final PasswordEncoder passwordEncoder;

    public AuthController(UserService userService, JwtService jwtService, PasswordEncoder passwordEncoder) {
        this.userService = userService;
        this.jwtService = jwtService;
        this.passwordEncoder = passwordEncoder;
    }

    @PostMapping("/register")
    public ResponseEntity<?> register(@RequestBody RegistrationRequestDto dto) {
        if (dto.getPassword() == null || dto.getConfirmPassword() == null
                || !dto.getPassword().equals(dto.getConfirmPassword())) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Пароли не совпадают");
        }
        if (dto.getPassword().length() < 8) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Пароль должен быть не короче 8 символов");
        }
        Users user = userService.registerUser(dto);
        return ResponseEntity.ok(Map.of(
                "message", "Регистрация прошла успешно! Теперь вы можете войти",
                "email", user.getEmail()
        ));
    }

    @PostMapping("/login")
    public ResponseEntity<?> login(@RequestBody LoginRequestDto dto) {
        try {
            Users user = userService.loadUser(dto.getEmail());
            if (!passwordEncoder.matches(dto.getPassword(), user.getPasswordHash())) {
                return ResponseEntity.status(401).body(Map.of("error", "Неверный пароль или email"));
            }
            String token = jwtService.generateToken(user);
            return ResponseEntity.ok(Map.of("token", token, "role", user.getRole().name()));
        } catch (UsernameNotFoundException e) {
            return ResponseEntity.status(401).body(Map.of("error", "Неверный пароль или email"));
        }
    }

    @GetMapping("/me")
    public ResponseEntity<?> me(Authentication authentication) {
        if (authentication == null) {
            return ResponseEntity.status(401).build();
        }
        Users user = userService.loadUser(authentication.getName());
        return ResponseEntity.ok(Map.of(
                "email", user.getEmail(),
                "role", user.getRole().name()
        ));
    }
}
