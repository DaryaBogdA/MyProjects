package com.example.kavalenok.service;

import com.example.kavalenok.model.User;
import com.example.kavalenok.repository.UserRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.nio.file.*;
import java.time.LocalDate;
import java.time.Period;
import java.util.UUID;

@Service
public class MoreInfoService {
    private final UserRepository userRepository;

    private static final String AVATARS_UPLOAD_DIR = "src/main/resources/static/img/saveAvatars/";
    private static final String AVATARS_UPLOAD_URL = "/img/saveAvatars/";
    private static final String AVATARS_DEFAULT_URL = "/img/avatars/";

    public MoreInfoService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    public void validateMoreInfo(String name, String last_name, String phone, LocalDate date) {
        if (name == null || name.trim().isEmpty()) {
            throw new IllegalArgumentException("Имя обязательно");
        }

        if (last_name == null || last_name.trim().isEmpty()) {
            throw new IllegalArgumentException("Фамилия обязательна");
        }
        phone = phone.replaceAll("[^\\d+]", "");

        if (!phone.matches("^((\\+375|80|375)?[\\s\\-]?\\(?[234]\\d{1,2}\\)?[\\s\\-]?\\d{3}[\\s\\-]?\\d{2}[\\s\\-]?\\d{2})$")) {
            throw new IllegalArgumentException("Неверно записан номер телефона");

        }

        if (date == null) {
            throw new IllegalArgumentException("Дата рождения обязательна");
        }

        LocalDate today = LocalDate.now();

        if (date.isAfter(today)) {
            throw new IllegalArgumentException("Дата рождения не может быть в будущем");
        }

        if (Period.between(date, today).getYears() > 120) {
            throw new IllegalArgumentException("Проверьте корректность даты рождения");
        }
    }

    public User updateUserMoreInfo(User user,
                                   String firstName,
                                   String lastName,
                                   String surName,
                                   String phone,
                                   LocalDate birthDate,
                                   String avatarPath) {

        user.setFirstName(firstName);
        user.setLastName(lastName);
        if (surName != null && !surName.trim().isEmpty()) {
            user.setSurName(surName.trim());
        }
        user.setPhone(phone);
        user.setDate(birthDate);
        if (user.getAvatarUrl() != null &&
                user.getAvatarUrl().startsWith(AVATARS_UPLOAD_URL)) {
            deleteOldAvatar(user.getAvatarUrl());
        }

        if (avatarPath != null && !avatarPath.isEmpty()) {
            user.setAvatarUrl(avatarPath);
        } else {
            user.setAvatarUrl(AVATARS_DEFAULT_URL + "1.png");
        }
        return userRepository.save(user);
    }

    public String saveAvatar(MultipartFile avatarFile, String defaultAvatar, Long userId) throws IOException {
        if (avatarFile != null && !avatarFile.isEmpty()) {
            return saveUploadedAvatar(avatarFile, userId);
        } else if (defaultAvatar != null && !defaultAvatar.isEmpty()) {
            return AVATARS_DEFAULT_URL + defaultAvatar;
        }
        return AVATARS_DEFAULT_URL + "1.png";
    }

    private String saveUploadedAvatar(MultipartFile avatarFile, Long userId) throws IOException {
        String originalFileName = avatarFile.getOriginalFilename();
        String fileExtension = getFileExtension(originalFileName);
        String uniqueFileName = "avatar_" + userId + "_" + UUID.randomUUID() + fileExtension;

        // Создаём директорию если не существует
        Path uploadDir = Paths.get(AVATARS_UPLOAD_DIR);
        if (!Files.exists(uploadDir)) {
            Files.createDirectories(uploadDir);
        }

        Path filePath = uploadDir.resolve(uniqueFileName);

        if (avatarFile.getSize() > 5 * 1024 * 1024) {
            throw new IllegalArgumentException("Размер файла не должен превышать 5MB");
        }

        String contentType = avatarFile.getContentType();
        if (contentType == null ||
                (!contentType.equals("image/jpeg") &&
                        !contentType.equals("image/png") &&
                        !contentType.equals("image/jpg"))) {
            throw new IllegalArgumentException("Разрешены только файлы JPG, JPEG и PNG");
        }

        Files.copy(avatarFile.getInputStream(), filePath, StandardCopyOption.REPLACE_EXISTING);

        return AVATARS_UPLOAD_URL + uniqueFileName;
    }

    private String getFileExtension(String fileName) {
        if (fileName == null || fileName.isEmpty()) {
            return ".png";
        }
        int lastDot = fileName.lastIndexOf('.');
        return (lastDot > 0) ? fileName.substring(lastDot) : ".png";
    }

    public void deleteOldAvatar(String oldAvatarUrl) {
        if (oldAvatarUrl != null &&
                oldAvatarUrl.startsWith(AVATARS_UPLOAD_URL) &&
                !oldAvatarUrl.isEmpty()) {
            try {
                // Преобразуем URL в путь к файлу
                String fileName = oldAvatarUrl.substring(AVATARS_UPLOAD_URL.length());
                Path oldPath = Paths.get(AVATARS_UPLOAD_DIR + fileName);
                if (Files.exists(oldPath)) {
                    Files.delete(oldPath);
                }
            } catch (IOException e) {
                System.err.println("Не удалось удалить старый аватар: " + e.getMessage());
            }
        }
    }
}
