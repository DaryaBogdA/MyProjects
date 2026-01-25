package com.example.Bank_REST.config;

import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.info.Info;
import io.swagger.v3.oas.models.security.SecurityRequirement;
import io.swagger.v3.oas.models.security.SecurityScheme;
import io.swagger.v3.oas.models.Components;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

/**
 * Конфигурация OpenAPI (Swagger) для документации REST API.
 * 
 * <p>Настраивает метаданные API и схему безопасности JWT Bearer токенов.</p>
 * 
 * @author darya
 */
@Configuration
public class OpenApiConfig {

    /**
     * Создает конфигурацию OpenAPI с информацией об API и схемой безопасности JWT.
     * 
     * @return настроенный объект OpenAPI
     */
    @Bean
    public OpenAPI customOpenAPI() {
        final String securitySchemeName = "bearerAuth";
        return new OpenAPI()
                .info(new Info()
                        .title("Bank REST API")
                        .version("1.0.0")
                        .description("REST API для управления банковскими картами и пользователями"))
                .addSecurityItem(new SecurityRequirement().addList(securitySchemeName))
                .components(new Components()
                        .addSecuritySchemes(securitySchemeName, new SecurityScheme()
                                .name(securitySchemeName)
                                .type(SecurityScheme.Type.HTTP)
                                .scheme("bearer")
                                .bearerFormat("JWT")));
    }
}
