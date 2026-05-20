package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.UserProfileDto;
import com.tours.bogdanovich.service.UserProfileService;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/profile")
public class UserProfileController {

    private final UserProfileService userProfileService;

    public UserProfileController(UserProfileService userProfileService) {
        this.userProfileService = userProfileService;
    }

    @GetMapping("/me")
    public ResponseEntity<UserProfileDto> getMe(Authentication authentication) {
        return ResponseEntity.ok(userProfileService.getMe(authentication.getName()));
    }

    @PutMapping("/me")
    public ResponseEntity<UserProfileDto> updateMe(@RequestBody UserProfileDto dto, Authentication authentication) {
        return ResponseEntity.ok(userProfileService.upsertMe(authentication.getName(), dto));
    }
}
