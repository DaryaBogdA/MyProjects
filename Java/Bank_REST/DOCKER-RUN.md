# Запуск приложения через Docker

## Быстрый старт

### 1. Сборка и запуск всех сервисов (MySQL + приложение)

```bash
docker-compose up -d --build
```

Эта команда:
- Соберет образ приложения из `Dockerfile`
- Запустит MySQL контейнер
- Запустит приложение в контейнере
- Подождет, пока MySQL будет готов (healthcheck)

### 2. Проверка статуса

```bash
docker-compose ps
```

Должны быть запущены оба контейнера:
- `bank-mysql` - база данных
- `bank-app` - приложение

### 3. Просмотр логов

```bash
# Логи приложения
docker-compose logs -f app

# Логи MySQL
docker-compose logs -f mysql

# Все логи
docker-compose logs -f
```

### 4. Проверка работы

После запуска приложение будет доступно:
- **API**: http://localhost:8080/api
- **Swagger UI**: http://localhost:8080/api/swagger-ui.html

## Остановка

```bash
# Остановка контейнеров
docker-compose down

# Остановка с удалением данных
docker-compose down -v
```

## Пересборка после изменений кода

Если вы изменили код приложения:

```bash
# Пересборка и перезапуск
docker-compose up -d --build

# Или только пересборка образа
docker-compose build app
docker-compose up -d
```

## Полезные команды

### Перезапуск приложения

```bash
docker-compose restart app
```

### Перезапуск всех сервисов

```bash
docker-compose restart
```

### Подключение к контейнеру приложения

```bash
docker exec -it bank-app sh
```

### Подключение к MySQL

```bash
docker exec -it bank-mysql mysql -uroot -prootpassword
```

### Просмотр использования ресурсов

```bash
docker stats
```

## Конфигурация

### Переменные окружения приложения

В `docker-compose.yml` настроены следующие переменные:

- `SPRING_PROFILES_ACTIVE=dev` - профиль Spring Boot
- `SPRING_DATASOURCE_URL` - URL базы данных (использует имя сервиса `mysql`, не `localhost`)
- `SPRING_DATASOURCE_USERNAME=root` - пользователь БД
- `SPRING_DATASOURCE_PASSWORD=rootpassword` - пароль БД

### Изменение портов

Если порт 8080 занят, измените в `docker-compose.yml`:

```yaml
ports:
  - "8081:8080"  # Внешний порт:внутренний порт
```

Тогда приложение будет доступно на `http://localhost:8081/api`

## Устранение проблем

### Приложение не запускается

1. Проверьте логи:
   ```bash
   docker-compose logs app
   ```

2. Убедитесь, что MySQL запущен:
   ```bash
   docker-compose ps
   ```

3. Проверьте подключение к БД:
   ```bash
   docker exec -it bank-mysql mysql -uroot -prootpassword -e "SHOW DATABASES;"
   ```

### Ошибка подключения к базе данных

Убедитесь, что:
- MySQL контейнер запущен и здоров
- Пароль в `SPRING_DATASOURCE_PASSWORD` совпадает с `MYSQL_ROOT_PASSWORD`
- URL использует имя сервиса `mysql`, а не `localhost`

### Очистка и пересборка

Если что-то пошло не так:

```bash
# Остановка и удаление контейнеров
docker-compose down -v

# Удаление образов
docker rmi bank_rest-app

# Полная пересборка
docker-compose up -d --build --force-recreate
```

## Разработка

### Запуск только MySQL (для локальной разработки)

Если хотите запускать приложение локально, а MySQL в Docker:

```bash
docker-compose -f docker-compose.dev.yml up -d
```

Затем запустите приложение локально:
```bash
mvn spring-boot:run -Dspring-boot.run.profiles=dev
```

### Hot reload в Docker

Для разработки с hot reload можно использовать volume для исходного кода, но это требует дополнительной настройки. Рекомендуется использовать локальный запуск приложения с MySQL в Docker.

## Структура

- `docker-compose.yml` - полная конфигурация (MySQL + приложение)
- `docker-compose.dev.yml` - только MySQL для локальной разработки
- `Dockerfile` - образ приложения (multi-stage build)
- `.dockerignore` - исключения для сборки
