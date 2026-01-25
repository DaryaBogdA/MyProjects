package com.example.Bank_REST.service;

import io.jsonwebtoken.*;
import io.jsonwebtoken.security.Keys;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.stereotype.Service;

import javax.crypto.SecretKey;
import java.nio.charset.StandardCharsets;
import java.util.Date;

/**
 * Реализация сервиса для работы с JWT токенами.
 * 
 * <p>Использует библиотеку jjwt для генерации и валидации JWT токенов
 * с алгоритмом подписи HS256.</p>
 * 
 * @author darya
 */
@Service
public class JwtServiceImpl implements JwtService {

    private final SecretKey secretKey;
    private final long expirationMillis;

    /**
     * Конструктор сервиса JWT.
     * 
     * @param secret секретный ключ для подписи токенов (из конфигурации)
     * @param expirationMinutes время жизни токена в минутах (из конфигурации)
     */
    public JwtServiceImpl(@Value("${jwt.secret}") String secret, @Value("${jwt.expirationMinutes}") long expirationMinutes) {
        this.secretKey = Keys.hmacShaKeyFor(secret.getBytes(StandardCharsets.UTF_8));
        this.expirationMillis = expirationMinutes * 60_000;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public String generateToken(UserDetails userDetails) {
        Date now = new Date();
        Date exp = new Date(now.getTime() + expirationMillis);

        return Jwts.builder()
                .setSubject(userDetails.getUsername())
                .setIssuedAt(now)
                .setExpiration(exp)
                .signWith(secretKey, SignatureAlgorithm.HS256)
                .compact();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public String extractUsername(String token) {
        return parseClaims(token).getSubject();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean validateToken(String token, UserDetails userDetails) {
        try {
            Claims claims = parseClaims(token);
            String username = claims.getSubject();
            Date exp = claims.getExpiration();
            return username.equals(userDetails.getUsername()) && exp.after(new Date());
        } catch (JwtException | IllegalArgumentException e) {
            return false;
        }
    }

    /**
     * Парсит JWT токен и извлекает claims (данные токена).
     * 
     * @param token JWT токен
     * @return claims токена
     * @throws JwtException если токен невалиден
     */
    private Claims parseClaims(String token) {
        return Jwts.parserBuilder()
                .setSigningKey(secretKey)
                .build()
                .parseClaimsJws(token)
                .getBody();
    }
}
