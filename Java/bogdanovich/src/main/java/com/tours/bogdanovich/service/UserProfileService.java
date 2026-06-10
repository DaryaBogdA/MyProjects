package com.tours.bogdanovich.service;

import com.tours.bogdanovich.dto.UserProfileDto;
import com.tours.bogdanovich.entity.UserProfile;
import com.tours.bogdanovich.entity.Users;
import com.tours.bogdanovich.repository.UserProfileRepository;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

import java.util.regex.Pattern;

@Service
public class UserProfileService {

    private static final Pattern PHONE_PATTERN = Pattern.compile("^\\+375(29|33|44|25)\\d{7}$");

    private final UserProfileRepository userProfileRepository;
    private final UserService userService;

    public UserProfileService(UserProfileRepository userProfileRepository, UserService userService) {
        this.userProfileRepository = userProfileRepository;
        this.userService = userService;
    }

    @Transactional(readOnly = true)
    public UserProfileDto getMe(String email) {
        Users user = userService.loadUser(email);
        UserProfile profile = userProfileRepository.findByUserId(user.getId()).orElse(null);
        return toDto(user, profile);
    }

    @Transactional
    public UserProfileDto upsertMe(String email, UserProfileDto dto) {
        Users user = userService.loadUser(email);

        if (dto.getPhone() != null && !dto.getPhone().isBlank()) {
            if (!PHONE_PATTERN.matcher(dto.getPhone()).matches()) {
                throw new ResponseStatusException(HttpStatus.BAD_REQUEST,
                        "Неверный формат телефона. Ожидается +375XXXXXXXXX (9 цифр после кода)");
            }
        }

        UserProfile profile = userProfileRepository.findByUserId(user.getId()).orElseGet(() -> {
            UserProfile p = new UserProfile();
            p.setUser(user);
            return p;
        });

        profile.setFirstName(dto.getFirstName());
        profile.setLastName(dto.getLastName());
        profile.setSurName(dto.getSurName());
        profile.setBirthDate(dto.getBirthDate());
        profile.setPassportNumber(dto.getPassportNumber());
        profile.setPassportDate(dto.getPassportDate());
        profile.setPhone(dto.getPhone());
        profile.setPreferences(dto.getPreferences());
        profile.setNotificationEmail(dto.getNotificationEmail() == null ? true : dto.getNotificationEmail());

        UserProfile saved = userProfileRepository.save(profile);
        return toDto(user, saved);
    }

    private UserProfileDto toDto(Users user, UserProfile profile) {
        UserProfileDto dto = new UserProfileDto();
        dto.setEmail(user.getEmail());
        if (profile == null) {
            dto.setNotificationEmail(true);
            return dto;
        }
        dto.setFirstName(profile.getFirstName());
        dto.setLastName(profile.getLastName());
        dto.setSurName(profile.getSurName());
        dto.setBirthDate(profile.getBirthDate());
        dto.setPassportNumber(profile.getPassportNumber());
        dto.setPassportDate(profile.getPassportDate());
        dto.setPhone(profile.getPhone());
        dto.setPreferences(profile.getPreferences());
        dto.setNotificationEmail(profile.getNotificationEmail());
        return dto;
    }
}