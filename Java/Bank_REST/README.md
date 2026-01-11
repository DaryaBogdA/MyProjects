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

## Установка и запуск

### 1. Клонирование репозитория

```bash
git clone <repository-url>
cd Bank_REST
```

### 2. Настройка базы данных

#### Вариант A: Использование существующего MySQL сервера

Отредактируйте файл `src/main/resources/application.yaml`:

```yaml
spring:
  datasource:
    url: jdbc:mysql://localhost:3306/bank?useSSL=false&serverTimezone=UTC&createDatabaseIfNotExist=true&allowPublicKeyRetrieval=true
    username: root
    password: your_password
```

#### Вариант B: Использование Docker Compose для dev-среды (рекомендуется)

**Быстрый старт с Docker Compose:**

1. Запустите MySQL через Docker Compose:
```bash
docker-compose -f docker-compose.dev.yml up -d
```

2. Убедитесь, что MySQL запущен и готов:
```bash
docker-compose -f docker-compose.dev.yml ps
```

3. Запустите приложение с профилем `dev`:
```bash
mvn spring-boot:run -Dspring-boot.run.profiles=dev
```

Или через IDE: установите переменную окружения `SPRING_PROFILES_ACTIVE=dev`

**Остановка MySQL:**
```bash
docker-compose -f docker-compose.dev.yml down
```

**Остановка с удалением данных:**
```bash
docker-compose -f docker-compose.dev.yml down -v
```

#### Вариант C: Использование Docker для MySQL (одиночный контейнер)

```bash
docker run --name mysql-bank -e MYSQL_ROOT_PASSWORD=root -e MYSQL_DATABASE=bank -p 3306:3306 -d mysql:8.0
```

### 3. Сборка проекта

```bash
mvn clean install
```

### 4. Запуск приложения

```bash
mvn spring-boot:run
```

Или используйте готовый JAR файл:

```bash
java -jar target/Bank_REST-0.0.1-SNAPSHOT.jar
```

### 5. Проверка работы

После запуска приложение будет доступно по адресу:
- **API**: http://localhost:8080/api
- **Swagger UI**: http://localhost:8080/api/swagger-ui.html

## Конфигурация

Основные настройки находятся в файле `src/main/resources/application.yaml`:

```yaml
server:
  port: 8080
  servlet:
    context-path: /api

spring:
  datasource:
    url: jdbc:mysql://localhost:3306/bank
    username: root
    password: your_password

jwt:
  secret: your-secret-key
  expirationMinutes: 60
```

## API Документация

### Swagger UI

После запуска приложения документация доступна по адресу:
- **Swagger UI**: http://localhost:8080/api/swagger-ui.html
- **OpenAPI JSON**: http://localhost:8080/api/v3/api-docs

### OpenAPI спецификация

Полная спецификация API находится в файле `docs/openapi.yaml`

## Использование API

### 1. Регистрация пользователя

```bash
POST /api/auth/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "password123"
}
```

### 2. Вход в систему

```bash
POST /api/auth/login
Content-Type: application/json

{
  "username": "john_doe",
  "password": "password123"
}
```

Ответ содержит JWT токен:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 3. Использование токена

Все защищенные эндпоинты требуют JWT токен в заголовке:

```
Authorization: Bearer <your_jwt_token>
```

## Основные эндпоинты

### Аутентификация
- `POST /api/auth/register` - Регистрация нового пользователя
- `POST /api/auth/login` - Вход в систему

### Карты (Cards)

#### Для всех пользователей:
- `GET /api/cards/my` - Получить свои карты (с поиском и пагинацией)
- `GET /api/cards/{id}` - Получить карту по ID
- `GET /api/cards/{id}/balance` - Получить баланс карты
- `POST /api/cards/{id}/request-block` - Запросить блокировку карты
- `POST /api/cards/transfer` - Перевод между своими картами

#### Только для администраторов:
- `GET /api/cards` - Получить все карты
- `POST /api/cards` - Создать новую карту
- `PUT /api/cards/{id}` - Обновить карту
- `PUT /api/cards/{id}/block` - Заблокировать карту
- `PUT /api/cards/{id}/activate` - Активировать карту
- `DELETE /api/cards/{id}` - Удалить карту

### Управление пользователями (только ADMIN)
- `GET /api/admin/users` - Получить всех пользователей
- `GET /api/admin/users/{id}` - Получить пользователя по ID
- `PUT /api/admin/users/{id}/role` - Изменить роль пользователя
- `DELETE /api/admin/users/{id}` - Удалить пользователя

## Роли пользователей

### ADMIN
- Создание, блокировка, активация, удаление карт
- Управление пользователями
- Просмотр всех карт
- Изменение баланса и периода действия карт

### USER
- Просмотр своих карт (с поиском и пагинацией)
- Запрос блокировки своей карты
- Переводы между своими картами
- Просмотр баланса своих карт

## Примеры запросов

### Создание карты (ADMIN)

```bash
POST /api/cards
Authorization: Bearer <admin_token>
Content-Type: application/json

{
  "number": "1234567890123456",
  "ownerId": 1,
  "period": "2025-12-31",
  "balance": 1000.00
}
```

### Получение своих карт с поиском

```bash
GET /api/cards/my?search=3456&page=0&size=10
Authorization: Bearer <user_token>
```

### Перевод между картами

```bash
POST /api/cards/transfer?fromCardId=1&toCardId=2&amount=100.00
Authorization: Bearer <user_token>
```

### Изменение роли пользователя (ADMIN)

```bash
PUT /api/admin/users/3/role
Authorization: Bearer <admin_token>
Content-Type: application/json

{
  "role": "ADMIN"
}
```

## Структура проекта

```
Bank_REST/
├── src/
│   ├── main/
│   │   ├── java/com/example/Bank_REST/
│   │   │   ├── config/          # Конфигурация (Security, etc.)
│   │   │   ├── controller/      # REST контроллеры
│   │   │   ├── dto/             # Data Transfer Objects
│   │   │   ├── entity/          # JPA сущности
│   │   │   ├── exception/       # Обработка исключений
│   │   │   ├── repository/      # JPA репозитории
│   │   │   ├── security/        # Security компоненты
│   │   │   └── service/         # Бизнес-логика
│   │   └── resources/
│   │       ├── application.yaml # Конфигурация приложения
│   │       └── db/migration/    # Liquibase миграции
│   └── test/                    # Тесты
├── docs/
│   └── openapi.yaml            # OpenAPI спецификация
├── pom.xml                      # Maven конфигурация
└── README.md                    # Документация
```

## Безопасность

- Все эндпоинты (кроме `/auth/**`) требуют JWT аутентификации
- Пароли хранятся в зашифрованном виде (BCrypt)
- Номера карт шифруются в базе данных
- Пользователи могут работать только со своими картами
- Администраторы имеют полный доступ

## Обработка ошибок

API возвращает стандартные HTTP коды статуса:

- `200 OK` - Успешный запрос
- `400 Bad Request` - Ошибка валидации или некорректный запрос
- `401 Unauthorized` - Требуется аутентификация
- `403 Forbidden` - Доступ запрещен
- `404 Not Found` - Ресурс не найден
- `500 Internal Server Error` - Внутренняя ошибка сервера

## Разработка

### Docker Compose для dev-среды

Проект включает Docker Compose конфигурацию для быстрого развертывания dev-среды.

#### Файлы Docker:
- `docker-compose.dev.yml` - конфигурация для разработки (только MySQL)
- `docker-compose.yml` - полная конфигурация (MySQL + приложение)
- `Dockerfile` - образ для приложения
- `.dockerignore` - исключения для Docker build

#### Использование:

**1. Запуск только MySQL (рекомендуется для разработки):**
```bash
# Запуск MySQL
docker-compose -f docker-compose.dev.yml up -d

# Проверка статуса
docker-compose -f docker-compose.dev.yml ps

# Просмотр логов
docker-compose -f docker-compose.dev.yml logs -f mysql

# Остановка
docker-compose -f docker-compose.dev.yml down

# Остановка с удалением данных
docker-compose -f docker-compose.dev.yml down -v
```

**2. Запуск приложения с профилем dev:**
```bash
# Через Maven
mvn spring-boot:run -Dspring-boot.run.profiles=dev

# Или через JAR
java -jar -Dspring.profiles.active=dev target/Bank_REST-0.0.1-SNAPSHOT.jar
```

**3. Полный запуск через Docker Compose (MySQL + приложение):**

Раскомментируйте секцию `app` в `docker-compose.yml` и запустите:
```bash
docker-compose up -d
```

#### Параметры подключения к MySQL (dev-профиль):
- **Host:** localhost
- **Port:** 3306
- **Database:** bank
- **Username:** root
- **Password:** rootpassword

Эти параметры настроены в `src/main/resources/application-dev.yaml`

#### Полезные команды Docker:

```bash
# Подключение к MySQL контейнеру
docker exec -it bank-mysql-dev mysql -uroot -prootpassword

# Просмотр логов MySQL
docker logs bank-mysql-dev

# Очистка всех данных
docker-compose -f docker-compose.dev.yml down -v
```

### Запуск тестов

#### Вариант 1: Через Maven Wrapper (если Maven не установлен)

```bash
# Все тесты
.\mvnw.cmd test

# Конкретный класс тестов
.\mvnw.cmd test -Dtest=CardServiceTest
.\mvnw.cmd test -Dtest=AuthServiceTest
.\mvnw.cmd test -Dtest=UserServiceTest
.\mvnw.cmd test -Dtest=JwtServiceImplTest
```

#### Вариант 2: Через установленный Maven

```bash
mvn test
```

#### Вариант 3: Через IDE (рекомендуется)

- **IntelliJ IDEA**: Правый клик на папку `src/test/java` → "Run All Tests"
- **VS Code**: Откройте файл теста и нажмите "Run Test"
- **Eclipse**: Правый клик на класс теста → "Run As" → "JUnit Test"

**Покрытие тестами:**
- `CardServiceTest` - 23 теста (создание карт, переводы, блокировка, активация)
- `AuthServiceTest` - 6 тестов (регистрация пользователей)
- `UserServiceTest` - 7 тестов (управление пользователями)
- `JwtServiceImplTest` - 9 тестов (JWT токены)

### Сборка без тестов

```bash
# Через Maven Wrapper
.\mvnw.cmd clean package -DskipTests

# Или через Maven
mvn clean package -DskipTests
```

## Лицензия

Этот проект создан в образовательных целях.

## Контакты

Для вопросов и предложений создайте issue в репозитории проекта.
