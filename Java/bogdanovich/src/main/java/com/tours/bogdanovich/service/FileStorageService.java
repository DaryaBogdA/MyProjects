package com.tours.bogdanovich.service;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.web.server.ResponseStatusException;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Set;
import java.util.UUID;

@Service
public class FileStorageService {

    private static final Set<String> ALLOWED = Set.of(
            "image/jpeg", "image/jpg", "image/pjpeg", "image/png", "image/webp", "image/gif", "image/avif");

    private final Path uploadRoot;

    public FileStorageService(@Value("${app.upload-dir:uploads}") String uploadDir) {
        this.uploadRoot = Paths.get(uploadDir).toAbsolutePath().normalize();
        try {
            Files.createDirectories(uploadRoot.resolve("tours"));
        } catch (IOException e) {
            throw new IllegalStateException("Cannot create upload directory", e);
        }
    }

    public String storeTourImage(MultipartFile file) {
        if (file == null || file.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Файл не выбран");
        }
        String contentType = file.getContentType();
        if (contentType == null || !ALLOWED.contains(contentType)) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Допустимы JPG, PNG, WEBP, GIF");
        }
        String ext = switch (contentType) {
            case "image/png" -> ".png";
            case "image/webp" -> ".webp";
            case "image/gif" -> ".gif";
            case "image/avif" -> ".avif";
            default -> {
                String original = file.getOriginalFilename();
                if (original != null && original.contains(".")) {
                    yield original.substring(original.lastIndexOf('.')).toLowerCase();
                }
                yield ".jpg";
            }
        };
        String filename = UUID.randomUUID() + ext;
        Path target = uploadRoot.resolve("tours").resolve(filename);
        try {
            file.transferTo(target);
        } catch (IOException e) {
            throw new ResponseStatusException(HttpStatus.INTERNAL_SERVER_ERROR, "Не удалось сохранить файл");
        }
        return "/api/uploads/tours/" + filename;
    }
}
