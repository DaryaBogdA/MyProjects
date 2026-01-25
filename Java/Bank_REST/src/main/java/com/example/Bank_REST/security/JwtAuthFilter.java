package com.example.Bank_REST.security;

import com.example.Bank_REST.service.JwtService;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.web.filter.OncePerRequestFilter;
import org.springframework.util.StringUtils;

import java.io.IOException;

/**
 * Фильтр для JWT аутентификации.
 * 
 * <p>Перехватывает каждый HTTP запрос, извлекает JWT токен из заголовка Authorization,
 * валидирует его и устанавливает аутентификацию в SecurityContext.</p>
 * 
 * @author darya
 */
public class JwtAuthFilter extends OncePerRequestFilter {

    private final JwtService jwtService;
    private final UserDetailsServiceImpl userDetailsService;

    /**
     * Конструктор фильтра JWT аутентификации.
     * 
     * @param jwtService сервис для работы с JWT токенами
     * @param userDetailsService сервис для загрузки данных пользователя
     */
    public JwtAuthFilter(JwtService jwtService, UserDetailsServiceImpl userDetailsService) {
        this.jwtService = jwtService;
        this.userDetailsService = userDetailsService;
    }

    /**
     * Обрабатывает каждый HTTP запрос для извлечения и валидации JWT токена.
     * 
     * @param request HTTP запрос
     * @param response HTTP ответ
     * @param filterChain цепочка фильтров
     * @throws ServletException если произошла ошибка сервлета
     * @throws IOException если произошла ошибка ввода-вывода
     */
    @Override
    protected void doFilterInternal(
            HttpServletRequest request,
            HttpServletResponse response,
            FilterChain filterChain
    ) throws ServletException, IOException {

        String authHeader = request.getHeader("Authorization");
        String token = null;

        if (StringUtils.hasText(authHeader) && authHeader.startsWith("Bearer ")) {
            token = authHeader.substring(7);
        }

        if (token != null && SecurityContextHolder.getContext().getAuthentication() == null) {
            String username = jwtService.extractUsername(token);

            if (username != null) {
                UserDetails userDetails = userDetailsService.loadUserByUsername(username);

                if (jwtService.validateToken(token, userDetails)) {
                    UsernamePasswordAuthenticationToken authToken =
                            new UsernamePasswordAuthenticationToken(
                                    userDetails,
                                    null,
                                    userDetails.getAuthorities()
                            );
                    authToken.setDetails(new WebAuthenticationDetailsSource().buildDetails(request));

                    SecurityContextHolder.getContext().setAuthentication(authToken);
                }
            }
        }

        filterChain.doFilter(request, response);
    }
}
