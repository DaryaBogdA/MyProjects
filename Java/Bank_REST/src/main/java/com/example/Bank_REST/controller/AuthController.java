package com.example.Bank_REST.controller;

import com.example.Bank_REST.dto.RegisterRequestDto;
import com.example.Bank_REST.dto.AuthRequestDto;
import com.example.Bank_REST.entity.Users;
import com.example.Bank_REST.service.AuthService;
import com.example.Bank_REST.service.JwtService;
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.bind.annotation.*;

/**
 * REST контроллер для аутентификации и регистрации пользователей.
 * 
 * <p>Предоставляет эндпоинты для регистрации новых пользователей и входа в систему.
 * После успешной аутентификации возвращает JWT токен для доступа к защищенным ресурсам.</p>
 * 
 * @author darya
 */
@RestController
@RequestMapping("/auth")
public class AuthController {

    private final AuthService authService;
    private final JwtService jwtService;
    private final AuthenticationManager authenticationManager;

    /**
     * Конструктор контроллера аутентификации.
     * 
     * @param authService сервис для регистрации пользователей
     * @param jwtService сервис для генерации JWT токенов
     * @param authenticationManager менеджер аутентификации Spring Security
     */
    public AuthController(AuthService authService, JwtService jwtService, AuthenticationManager authenticationManager) {
        this.authService = authService;
        this.jwtService = jwtService;
        this.authenticationManager = authenticationManager;
    }

    /**
     * Регистрирует нового пользователя в системе.
     * 
     * @param dto данные для регистрации (username, email, password)
     * @return сообщение об успешной регистрации с именем пользователя
     */
    @PostMapping("/register")
    public ResponseEntity<String> register(@Valid @RequestBody RegisterRequestDto dto) {
        Users user = authService.register(dto);
        return ResponseEntity.ok("User registered: " + user.getUsername());
    }

    /**
     * Аутентифицирует пользователя и возвращает JWT токен.
     * 
     * @param dto данные для входа (username, password)
     * @return JWT токен для доступа к защищенным ресурсам
     */
    @PostMapping("/login")
    public ResponseEntity<String> login(@Valid @RequestBody AuthRequestDto dto) {
        Authentication authentication = authenticationManager.authenticate(
                new UsernamePasswordAuthenticationToken(dto.getUsername(), dto.getPassword())
        );

        UserDetails userDetails = (UserDetails) authentication.getPrincipal();
        String token = jwtService.generateToken(userDetails);
        return ResponseEntity.ok(token);
    }
}
