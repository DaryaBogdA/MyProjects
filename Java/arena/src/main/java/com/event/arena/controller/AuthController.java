package com.event.arena.controller;


import com.event.arena.dto.LoginRequestDto;
import com.event.arena.dto.RegistrationRequestDto;
import com.event.arena.entity.User;
import com.event.arena.service.JwtService;
import com.event.arena.service.UserService;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

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
        try {
            User user = userService.registerUser(dto);
            return ResponseEntity.ok(Map.of(
                    "message", "Регистрация прошла успешно! Теперь вы можете войти",
                    "email", user.getEmail()
            ));
        } catch (RuntimeException e) {
            return ResponseEntity.badRequest().body(Map.of("error", e.getMessage()));
        }
    }

    @PostMapping("/login")
    public ResponseEntity<?> login(@RequestBody LoginRequestDto dto) {
        try {
            User user = userService.loadUser(dto.getEmail());
            if (!passwordEncoder.matches(dto.getPassword(), user.getPasswordHash())) {
                return ResponseEntity.status(401).body(Map.of("error", "Неверный пароль или email"));
            }
            String token = jwtService.generateToken(user);
            return ResponseEntity.ok(Map.of("token", token));
        } catch (UsernameNotFoundException e) {
            return ResponseEntity.status(401).body(Map.of("error", "Неверный пароль или email"));
        }
    }
}

