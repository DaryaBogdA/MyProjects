package com.example.Bank_REST.config;

import com.example.Bank_REST.security.JwtAuthFilter;
import com.example.Bank_REST.security.UserDetailsServiceImpl;
import com.example.Bank_REST.service.JwtService;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.config.annotation.authentication.configuration.AuthenticationConfiguration;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;
import org.springframework.web.cors.CorsConfigurationSource;
import jakarta.servlet.http.HttpServletResponse;

/**
 * Конфигурация безопасности Spring Security.
 * 
 * <p>Настраивает:
 * <ul>
 *   <li>JWT аутентификацию через фильтр</li>
 *   <li>Правила авторизации для различных эндпоинтов</li>
 *   <li>CORS настройки</li>
 *   <li>Обработку исключений безопасности</li>
 * </ul>
 * </p>
 * 
 * @author darya
 */
@Configuration
@EnableWebSecurity
@EnableMethodSecurity
public class SecurityConfig {
    private final JwtService jwtService;
    private final UserDetailsServiceImpl userDetailsService;
    private final CorsConfigurationSource corsConfigurationSource;

    /**
     * Конструктор конфигурации безопасности.
     * 
     * @param jwtService сервис для работы с JWT токенами
     * @param userDetailsService сервис для загрузки данных пользователя
     * @param corsConfigurationSource конфигурация CORS
     */
    public SecurityConfig(JwtService jwtService, UserDetailsServiceImpl userDetailsService, CorsConfigurationSource corsConfigurationSource) {
        this.jwtService = jwtService;
        this.userDetailsService = userDetailsService;
        this.corsConfigurationSource = corsConfigurationSource;
    }

    /**
     * Создает фильтр для JWT аутентификации.
     * 
     * @return фильтр JWT аутентификации
     */
    @Bean
    public JwtAuthFilter jwtAuthFilter() {
        return new JwtAuthFilter(jwtService, userDetailsService);
    }

    /**
     * Создает менеджер аутентификации.
     * 
     * @param configuration конфигурация аутентификации
     * @return менеджер аутентификации
     * @throws Exception если не удалось создать менеджер
     */
    @Bean
    public AuthenticationManager authenticationManager(AuthenticationConfiguration configuration) throws Exception {
        return configuration.getAuthenticationManager();
    }

    /**
     * Настраивает цепочку фильтров безопасности.
     * 
     * <p>Настройки включают:
     * <ul>
     *   <li>Отключение CSRF (так как используется JWT)</li>
     *   <li>Включение CORS</li>
     *   <li>Stateless сессии</li>
     *   <li>Правила доступа к эндпоинтам</li>
     *   <li>Обработку ошибок аутентификации и авторизации</li>
     * </ul>
     * </p>
     * 
     * @param http объект для настройки безопасности
     * @return настроенная цепочка фильтров
     * @throws Exception если не удалось настроить цепочку
     */
    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        return http 
                .csrf(csrf -> csrf.disable())
                .cors(cors -> cors.configurationSource(corsConfigurationSource))
                .sessionManagement(sm -> sm.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
                .authorizeHttpRequests(auth -> auth
                        .requestMatchers("/auth/**").permitAll()
                        .requestMatchers("/swagger-ui/**", "/v3/api-docs/**", "/swagger-ui.html", "/swagger-ui/index.html", "/swagger-ui.html/**").permitAll()
                        .requestMatchers("/webjars/**").permitAll()
                        .requestMatchers("/admin/**").hasRole("ADMIN")
                        .requestMatchers("/cards/**").authenticated()
                        .anyRequest().authenticated()
                )
                .exceptionHandling(ex -> ex
                        .authenticationEntryPoint((request, response, authException) -> {
                            response.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
                            response.setContentType("application/json");
                            response.getWriter().write("{\"error\": \"Authentication required. Please provide a valid JWT token in Authorization header\"}");
                        })
                        .accessDeniedHandler((request, response, accessDeniedException) -> {
                            response.setStatus(HttpServletResponse.SC_FORBIDDEN);
                            response.setContentType("application/json");
                            response.getWriter().write("{\"error\": \"Access denied: " + accessDeniedException.getMessage() + "\"}");
                        })
                )
                .addFilterBefore(jwtAuthFilter(), UsernamePasswordAuthenticationFilter.class)
                .build();
    }

    /**
     * Создает кодировщик паролей BCrypt.
     * 
     * @return кодировщик паролей
     */
    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }

}
