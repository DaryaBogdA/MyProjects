package com.example.Bank_REST.service;

import com.example.Bank_REST.dto.RegisterRequestDto;
import com.example.Bank_REST.entity.Role;
import com.example.Bank_REST.entity.Users;
import com.example.Bank_REST.repository.UsersRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.security.crypto.password.PasswordEncoder;

import java.time.Instant;
import java.util.Optional;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
@DisplayName("AuthService Unit Tests")
class AuthServiceTest {

    @Mock
    private UsersRepository usersRepository;

    @Mock
    private PasswordEncoder passwordEncoder;

    @InjectMocks
    private AuthService authService;

    private RegisterRequestDto registerRequestDto;

    @BeforeEach
    void setUp() {
        registerRequestDto = new RegisterRequestDto();
        registerRequestDto.setUsername("testuser");
        registerRequestDto.setEmail("test@example.com");
        registerRequestDto.setPassword("password123");
    }

    @Test
    @DisplayName("Should register user successfully")
    void testRegister_Success() {
        when(usersRepository.findByUsername("testuser")).thenReturn(Optional.empty());
        when(usersRepository.findByEmail("test@example.com")).thenReturn(Optional.empty());
        when(passwordEncoder.encode("password123")).thenReturn("encodedPassword");
        when(usersRepository.save(any(Users.class))).thenAnswer(invocation -> {
            Users user = invocation.getArgument(0);
            user.setId(1L);
            return user;
        });

        Users result = authService.register(registerRequestDto);

        assertNotNull(result);
        assertEquals("testuser", result.getUsername());
        assertEquals("test@example.com", result.getEmail());
        assertEquals("encodedPassword", result.getPasswordHash());
        assertEquals(Role.USER, result.getRole());
        assertNotNull(result.getCreatedAt());

        verify(usersRepository).findByUsername("testuser");
        verify(usersRepository).findByEmail("test@example.com");
        verify(passwordEncoder).encode("password123");
        verify(usersRepository).save(any(Users.class));
    }

    @Test
    @DisplayName("Should throw exception when username already exists")
    void testRegister_UsernameExists() {
        Users existingUser = new Users();
        existingUser.setUsername("testuser");
        when(usersRepository.findByUsername("testuser")).thenReturn(Optional.of(existingUser));

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            authService.register(registerRequestDto);
        });

        assertEquals("Username already exists", exception.getMessage());
        verify(usersRepository).findByUsername("testuser");
        verify(usersRepository, never()).findByEmail(any());
        verify(passwordEncoder, never()).encode(any());
        verify(usersRepository, never()).save(any());
    }

    @Test
    @DisplayName("Should throw exception when email already exists")
    void testRegister_EmailExists() {
        Users existingUser = new Users();
        existingUser.setEmail("test@example.com");
        when(usersRepository.findByUsername("testuser")).thenReturn(Optional.empty());
        when(usersRepository.findByEmail("test@example.com")).thenReturn(Optional.of(existingUser));

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            authService.register(registerRequestDto);
        });

        assertEquals("Email already exists", exception.getMessage());
        verify(usersRepository).findByUsername("testuser");
        verify(usersRepository).findByEmail("test@example.com");
        verify(passwordEncoder, never()).encode(any());
        verify(usersRepository, never()).save(any());
    }

    @Test
    @DisplayName("Should hash password during registration")
    void testRegister_PasswordHashing() {
        when(usersRepository.findByUsername("testuser")).thenReturn(Optional.empty());
        when(usersRepository.findByEmail("test@example.com")).thenReturn(Optional.empty());
        when(passwordEncoder.encode("password123")).thenReturn("$2a$10$encodedHash");
        when(usersRepository.save(any(Users.class))).thenAnswer(invocation -> invocation.getArgument(0));

        Users result = authService.register(registerRequestDto);

        assertNotNull(result);
        assertEquals("$2a$10$encodedHash", result.getPasswordHash());
        assertNotEquals("password123", result.getPasswordHash());
        verify(passwordEncoder).encode("password123");
    }

    @Test
    @DisplayName("Should set default role as USER")
    void testRegister_DefaultRole() {
        when(usersRepository.findByUsername("testuser")).thenReturn(Optional.empty());
        when(usersRepository.findByEmail("test@example.com")).thenReturn(Optional.empty());
        when(passwordEncoder.encode("password123")).thenReturn("encodedPassword");
        when(usersRepository.save(any(Users.class))).thenAnswer(invocation -> invocation.getArgument(0));

        Users result = authService.register(registerRequestDto);

        assertNotNull(result);
        assertEquals(Role.USER, result.getRole());
        assertNotEquals(Role.ADMIN, result.getRole());
    }

    @Test
    @DisplayName("Should set createdAt timestamp")
    void testRegister_SetCreatedAt() {
        when(usersRepository.findByUsername("testuser")).thenReturn(Optional.empty());
        when(usersRepository.findByEmail("test@example.com")).thenReturn(Optional.empty());
        when(passwordEncoder.encode("password123")).thenReturn("encodedPassword");
        when(usersRepository.save(any(Users.class))).thenAnswer(invocation -> invocation.getArgument(0));

        Instant before = Instant.now();
        Users result = authService.register(registerRequestDto);
        Instant after = Instant.now();

        assertNotNull(result.getCreatedAt());
        assertTrue(result.getCreatedAt().isAfter(before.minusSeconds(1)));
        assertTrue(result.getCreatedAt().isBefore(after.plusSeconds(1)));
    }
}
