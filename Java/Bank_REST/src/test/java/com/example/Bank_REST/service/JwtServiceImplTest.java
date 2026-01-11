package com.example.Bank_REST.service;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.userdetails.User;
import org.springframework.security.core.userdetails.UserDetails;

import java.util.Collections;

import static org.junit.jupiter.api.Assertions.*;

@DisplayName("JwtServiceImpl Unit Tests")
class JwtServiceImplTest {

    private JwtServiceImpl jwtService;
    private UserDetails userDetails;
    private static final String SECRET = "f7bf5efa77d9ed25ffc9c3bff6f6d532123456789012345678901234567890123";
    private static final long EXPIRATION_MINUTES = 60;

    @BeforeEach
    void setUp() {
        jwtService = new JwtServiceImpl(SECRET, EXPIRATION_MINUTES);
        userDetails = User.builder()
                .username("testuser")
                .password("password")
                .authorities(Collections.singletonList(new SimpleGrantedAuthority("ROLE_USER")))
                .build();
    }

    @Test
    @DisplayName("Should generate valid token")
    void testGenerateToken_Success() {
        String token = jwtService.generateToken(userDetails);

        assertNotNull(token);
        assertFalse(token.isEmpty());
        assertTrue(token.split("\\.").length == 3);
    }

    @Test
    @DisplayName("Should extract username from token")
    void testExtractUsername_Success() {
        String token = jwtService.generateToken(userDetails);
        String username = jwtService.extractUsername(token);
        assertEquals("testuser", username);
    }

    @Test
    @DisplayName("Should validate valid token")
    void testValidateToken_ValidToken() {
        String token = jwtService.generateToken(userDetails);
        boolean isValid = jwtService.validateToken(token, userDetails);
        assertTrue(isValid);
    }

    @Test
    @DisplayName("Should invalidate token with wrong username")
    void testValidateToken_WrongUsername() {
        String token = jwtService.generateToken(userDetails);
        UserDetails otherUser = User.builder()
                .username("otheruser")
                .password("password")
                .authorities(Collections.singletonList(new SimpleGrantedAuthority("ROLE_USER")))
                .build();

        boolean isValid = jwtService.validateToken(token, otherUser);

        assertFalse(isValid);
    }

    @Test
    @DisplayName("Should invalidate malformed token")
    void testValidateToken_MalformedToken() {
        String malformedToken = "invalid.token.here";
        boolean isValid = jwtService.validateToken(malformedToken, userDetails);
        assertFalse(isValid);
    }

    @Test
    @DisplayName("Should invalidate empty token")
    void testValidateToken_EmptyToken() {
        String emptyToken = "";
        boolean isValid = jwtService.validateToken(emptyToken, userDetails);
        assertFalse(isValid);
    }

    @Test
    @DisplayName("Should invalidate null token")
    void testValidateToken_NullToken() {
        String nullToken = null;
        boolean isValid = jwtService.validateToken(nullToken, userDetails);
        assertFalse(isValid);
    }

    @Test
    @DisplayName("Should generate different tokens for same user")
    void testGenerateToken_DifferentTokens() {
        String token1 = jwtService.generateToken(userDetails);
        try {
            Thread.sleep(1000);
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        }
        String token2 = jwtService.generateToken(userDetails);

        assertNotEquals(token1, token2);
        assertEquals("testuser", jwtService.extractUsername(token1));
        assertEquals("testuser", jwtService.extractUsername(token2));
    }

    @Test
    @DisplayName("Should extract username correctly from generated token")
    void testExtractUsername_Consistency() {
        String token = jwtService.generateToken(userDetails);

        String extractedUsername1 = jwtService.extractUsername(token);
        String extractedUsername2 = jwtService.extractUsername(token);

        assertEquals("testuser", extractedUsername1);
        assertEquals("testuser", extractedUsername2);
        assertEquals(extractedUsername1, extractedUsername2);
    }
}
