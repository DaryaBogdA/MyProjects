# Docker Compose для разработки

## Быстрый старт

### 1. Запуск MySQL

```bash
docker-compose -f docker-compose.dev.yml up -d
```

### 2. Проверка статуса

```bash
docker-compose -f docker-compose.dev.yml ps
```

### 3. Запуск приложения

```bash
mvn spring-boot:run -Dspring-boot.run.profiles=dev
```

Или через IDE: установите `SPRING_PROFILES_ACTIVE=dev`

## Настройки MySQL

### Параметры в docker-compose.dev.yml:

- **`MYSQL_ROOT_PASSWORD`** - пароль для пользователя **root** (администратор)
- **`MYSQL_DATABASE`** - имя базы данных, которая будет создана автоматически
- **`MYSQL_USER`** и **`MYSQL_PASSWORD`** (опционально) - дополнительный пользователь (не админ)

### Важно:

1. **В Docker MySQL нельзя запустить без пароля root** - это требование безопасности
2. Если нужен только root, оставьте только `MYSQL_ROOT_PASSWORD` и `MYSQL_DATABASE`
3. `MYSQL_USER` и `MYSQL_PASSWORD` создают дополнительного пользователя (не админ), его можно удалить

### Пример конфигурации (только root):

```yaml
environment:
  MYSQL_ROOT_PASSWORD: rootpassword  # Пароль для root (админ)
  MYSQL_DATABASE: bank
  # MYSQL_USER и MYSQL_PASSWORD - опционально, можно удалить
```

### Пример конфигурации (root + дополнительный пользователь):

```yaml
environment:
  MYSQL_ROOT_PASSWORD: rootpassword  # Пароль для root (админ)
  MYSQL_DATABASE: bank
  MYSQL_USER: bankuser                # Дополнительный пользователь
  MYSQL_PASSWORD: bankpassword        # Пароль для дополнительного пользователя
```

## Полезные команды

### Управление контейнерами

```bash
# Запуск
docker-compose -f docker-compose.dev.yml up -d

# Остановка
docker-compose -f docker-compose.dev.yml down

# Остановка с удалением данных
docker-compose -f docker-compose.dev.yml down -v

# Перезапуск
docker-compose -f docker-compose.dev.yml restart
```

### Просмотр логов

```bash
# Логи MySQL
docker-compose -f docker-compose.dev.yml logs -f mysql

# Все логи
docker-compose -f docker-compose.dev.yml logs -f
```

### Подключение к MySQL

```bash
# Через Docker exec (как root)
docker exec -it bank-mysql-dev mysql -uroot -prootpassword

# Или через внешний клиент
# Host: localhost
# Port: 3306
# Username: root
# Password: rootpassword (из MYSQL_ROOT_PASSWORD)
# Database: bank
```

## Конфигурация приложения

### Параметры подключения (application-dev.yaml)

- **URL:** `jdbc:mysql://localhost:3306/bank`
- **Username:** `root` (администратор)
- **Password:** `rootpassword` (должен совпадать с MYSQL_ROOT_PASSWORD)
- **Database:** `bank`

### Изменение пароля root

Если изменили `MYSQL_ROOT_PASSWORD` в docker-compose.dev.yml, обновите также `application-dev.yaml`:

```yaml
spring:
  datasource:
    username: root
    password: ваш_новый_пароль  # Должен совпадать с MYSQL_ROOT_PASSWORD
```

## Полный запуск (MySQL + приложение)

Если нужно запустить все через Docker:

1. Раскомментируйте секцию `app` в `docker-compose.yml`
2. Запустите:
```bash
docker-compose up -d
```

Приложение будет доступно на `http://localhost:8080/api`

## Устранение проблем

### Порт 3306 уже занят

Измените порт в `docker-compose.dev.yml`:

```yaml
ports:
  - "3307:3306"  # Внешний порт:внутренний порт
```

И обновите `application-dev.yaml`:

```yaml
url: jdbc:mysql://localhost:3307/bank...
```

### Очистка данных

```bash
docker-compose -f docker-compose.dev.yml down -v
```

Это удалит все данные из базы данных.

### Проверка здоровья MySQL

```bash
docker exec bank-mysql-dev mysqladmin ping -h localhost -uroot -prootpassword
```

### Ошибка подключения

Убедитесь, что:
1. Контейнер запущен: `docker-compose -f docker-compose.dev.yml ps`
2. Пароль в `application-dev.yaml` совпадает с `MYSQL_ROOT_PASSWORD`
3. База данных создана (проверьте логи: `docker-compose -f docker-compose.dev.yml logs mysql`)

## Структура файлов

- `docker-compose.dev.yml` - конфигурация для разработки (только MySQL)
- `docker-compose.yml` - полная конфигурация (MySQL + приложение)
- `Dockerfile` - образ для приложения
- `.dockerignore` - исключения для Docker build
- `src/main/resources/application-dev.yaml` - конфигурация приложения для dev-среды
