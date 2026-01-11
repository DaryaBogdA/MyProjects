# Bank REST API

REST API для управления банковскими картами и пользователями с использованием Spring Boot, Spring Security и JWT аутентификации.

## Технологии

- **Java 17**
- **Spring Boot 4.0.1**
- **Spring Security** - аутентификация и авторизация
- **JWT** - токены для аутентификации
- **Spring Data JPA** - работа с базой данных
- **MySQL** - база данных
- **Liquibase** - миграции базы данных
- **SpringDoc OpenAPI** - документация API (Swagger UI)
- **Maven** - управление зависимостями

## Требования

- Java 17 или выше
- Maven 3.6+
- MySQL 8.0+
- Docker (опционально, для MySQL)

## Быстрый старт

```bash
# 1. Клонирование репозитория
git clone <repository-url>
cd Bank_REST

# 2. Запуск MySQL через Docker Compose (dev-среда)
docker-compose -f docker-compose.dev.yml up -d

# 3. Сборка проекта
mvn clean install

# 4. Запуск приложения
mvn spring-boot:run
# или
java -jar target/Bank_REST-0.0.1-SNAPSHOT.jar

# 5. Проверка работы
# API: http://localhost:8080/api
# Swagger UI: http://localhost:8080/api/swagger-ui.html

# 6. Запуск тестов
mvn test
# или запуск конкретного класса тестов:
mvn test -Dtest=CardServiceTest