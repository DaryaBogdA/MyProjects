package com.event.arena.controller;

import com.event.arena.entity.User;
import org.springframework.core.io.Resource;
import org.springframework.core.io.UrlResource;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.web.servlet.support.ServletUriComponentsBuilder;

import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardCopyOption;
import java.util.Map;
import java.util.UUID;

@RestController
@RequestMapping("/files")
public class FileController {
    private final Path uploadRoot = Paths.get("uploads", "event-images");

    @PostMapping("/upload")
    public ResponseEntity<?> upload(@AuthenticationPrincipal User user, @RequestParam("file") MultipartFile file) {
        if (user == null) {
            return ResponseEntity.status(401).body(Map.of("error", "Требуется авторизация"));
        }
        if (file == null || file.isEmpty()) {
            return ResponseEntity.badRequest().body(Map.of("error", "Файл не выбран"));
        }
        try {
            Files.createDirectories(uploadRoot);
            String original = file.getOriginalFilename() == null ? "image.jpg" : file.getOriginalFilename();
            String ext = "";
            int dotIdx = original.lastIndexOf('.');
            if (dotIdx > -1) {
                ext = original.substring(dotIdx);
            }
            String fileName = UUID.randomUUID() + ext;
            Path destination = uploadRoot.resolve(fileName).normalize();
            Files.copy(file.getInputStream(), destination, StandardCopyOption.REPLACE_EXISTING);
            String fileUrl = ServletUriComponentsBuilder.fromCurrentContextPath()
                    .path("/files/")
                    .path(fileName)
                    .toUriString();
            return ResponseEntity.ok(Map.of("url", fileUrl));
        } catch (Exception e) {
            return ResponseEntity.internalServerError().body(Map.of("error", "Ошибка сохранения файла"));
        }
    }

    @GetMapping("/{fileName:.+}")
    public ResponseEntity<Resource> getFile(@PathVariable String fileName) {
        try {
            Path filePath = uploadRoot.resolve(fileName).normalize();
            if (!filePath.startsWith(uploadRoot.normalize())) {
                return ResponseEntity.notFound().build();
            }
            if (!Files.exists(filePath)) {
                return ResponseEntity.notFound().build();
            }
            Resource resource = new UrlResource(filePath.toUri());
            String contentType = Files.probeContentType(filePath);
            if (contentType == null) {
                contentType = MediaType.APPLICATION_OCTET_STREAM_VALUE;
            }
            return ResponseEntity.ok()
                    .header(HttpHeaders.CONTENT_TYPE, contentType)
                    .body(resource);
        } catch (Exception e) {
            return ResponseEntity.notFound().build();
        }
    }
}
