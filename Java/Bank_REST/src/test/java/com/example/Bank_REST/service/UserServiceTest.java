package com.example.Bank_REST.service;

import com.example.Bank_REST.entity.Role;
import com.example.Bank_REST.entity.Users;
import com.example.Bank_REST.repository.UsersRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageImpl;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.security.crypto.password.PasswordEncoder;

import java.time.Instant;
import java.util.Arrays;
import java.util.List;
import java.util.Optional;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
@DisplayName("UserService Unit Tests")
class UserServiceTest {

    @Mock
    private UsersRepository usersRepository;

    @Mock
    private PasswordEncoder passwordEncoder;

    @InjectMocks
    private UserService userService;

    private Users user;

    @BeforeEach
    void setUp() {
        user = new Users();
        user.setId(1L);
        user.setUsername("testuser");
        user.setEmail("test@example.com");
        user.setPasswordHash("encodedPassword");
        user.setRole(Role.USER);
        user.setCreatedAt(Instant.now());
    }

    @Test
    @DisplayName("Should get all users with pagination")
    void testGetAllUsers_Success() {
        Pageable pageable = PageRequest.of(0, 10);
        List<Users> users = Arrays.asList(user);
        Page<Users> userPage = new PageImpl<>(users, pageable, 1);

        when(usersRepository.findAll(pageable)).thenReturn(userPage);

        Page<Users> result = userService.getAllUsers(pageable);

        assertNotNull(result);
        assertEquals(1, result.getTotalElements());
        assertEquals(1, result.getContent().size());
        assertEquals("testuser", result.getContent().get(0).getUsername());
        verify(usersRepository).findAll(pageable);
    }

    @Test
    @DisplayName("Should get user by id successfully")
    void testGetUserById_Success() {
        when(usersRepository.findById(1L)).thenReturn(Optional.of(user));

        Users result = userService.getUserById(1L);

        assertNotNull(result);
        assertEquals(1L, result.getId());
        assertEquals("testuser", result.getUsername());
        verify(usersRepository).findById(1L);
    }

    @Test
    @DisplayName("Should throw exception when user not found")
    void testGetUserById_NotFound() {
        when(usersRepository.findById(1L)).thenReturn(Optional.empty());

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            userService.getUserById(1L);
        });

        assertEquals("User not found", exception.getMessage());
        verify(usersRepository).findById(1L);
    }

    @Test
    @DisplayName("Should update user role successfully")
    void testUpdateUserRole_Success() {
        when(usersRepository.findById(1L)).thenReturn(Optional.of(user));
        when(usersRepository.save(any(Users.class))).thenAnswer(invocation -> invocation.getArgument(0));

        Users result = userService.updateUserRole(1L, Role.ADMIN);

        assertNotNull(result);
        assertEquals(Role.ADMIN, result.getRole());
        assertNotEquals(Role.USER, result.getRole());
        verify(usersRepository).findById(1L);
        verify(usersRepository).save(user);
    }

    @Test
    @DisplayName("Should update user role from ADMIN to USER")
    void testUpdateUserRole_AdminToUser() {
        user.setRole(Role.ADMIN);
        when(usersRepository.findById(1L)).thenReturn(Optional.of(user));
        when(usersRepository.save(any(Users.class))).thenAnswer(invocation -> invocation.getArgument(0));

        Users result = userService.updateUserRole(1L, Role.USER);

        assertNotNull(result);
        assertEquals(Role.USER, result.getRole());
        verify(usersRepository).findById(1L);
        verify(usersRepository).save(user);
    }

    @Test
    @DisplayName("Should throw exception when updating role of non-existent user")
    void testUpdateUserRole_UserNotFound() {
        when(usersRepository.findById(1L)).thenReturn(Optional.empty());

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            userService.updateUserRole(1L, Role.ADMIN);
        });

        assertEquals("User not found", exception.getMessage());
        verify(usersRepository).findById(1L);
        verify(usersRepository, never()).save(any());
    }

    @Test
    @DisplayName("Should delete user successfully")
    void testDeleteUser_Success() {
        when(usersRepository.findById(1L)).thenReturn(Optional.of(user));
        doNothing().when(usersRepository).delete(user);

        userService.deleteUser(1L);

        verify(usersRepository).findById(1L);
        verify(usersRepository).delete(user);
    }

    @Test
    @DisplayName("Should throw exception when deleting non-existent user")
    void testDeleteUser_UserNotFound() {
        when(usersRepository.findById(1L)).thenReturn(Optional.empty());

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            userService.deleteUser(1L);
        });

        assertEquals("User not found", exception.getMessage());
        verify(usersRepository).findById(1L);
        verify(usersRepository, never()).delete(any());
    }
}
