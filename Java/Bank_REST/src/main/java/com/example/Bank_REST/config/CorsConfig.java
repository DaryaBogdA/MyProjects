package com.example.Bank_REST.config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.cors.CorsConfiguration;
import org.springframework.web.cors.CorsConfigurationSource;
import org.springframework.web.cors.UrlBasedCorsConfigurationSource;

import java.util.Arrays;

/**
 * Конфигурация CORS (Cross-Origin Resource Sharing) для REST API.
 * 
 * <p>Настраивает правила для обработки кросс-доменных запросов,
 * разрешая взаимодействие с фронтенд приложениями из различных источников.</p>
 * 
 * @author darya
 */
@Configuration
public class CorsConfig {

    /**
     * Создает конфигурацию CORS для всех эндпоинтов API.
     * 
     * <p>Настройки включают:
     * <ul>
     *   <li>Разрешение всех источников (origins)</li>
     *   <li>Поддержка всех основных HTTP методов</li>
     *   <li>Разрешение всех заголовков</li>
     *   <li>Поддержка credentials для JWT токенов</li>
     * </ul>
     * </p>
     * 
     * @return источник конфигурации CORS
     */
    @Bean
    public CorsConfigurationSource corsConfigurationSource() {
        CorsConfiguration configuration = new CorsConfiguration();
        
        configuration.setAllowedOriginPatterns(Arrays.asList("*"));
        
        configuration.setAllowedMethods(Arrays.asList("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS"));
        
        configuration.setAllowedHeaders(Arrays.asList("*"));
        
        configuration.setAllowCredentials(true);
        
        configuration.setExposedHeaders(Arrays.asList("Authorization", "Content-Type"));
        
        configuration.setMaxAge(3600L);
        
        UrlBasedCorsConfigurationSource source = new UrlBasedCorsConfigurationSource();
        source.registerCorsConfiguration("/**", configuration);
        
        return source;
    }
}
