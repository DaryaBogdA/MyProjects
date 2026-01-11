package com.example.Bank_REST.service;

import com.example.Bank_REST.entity.Role;
import com.example.Bank_REST.entity.Users;
import com.example.Bank_REST.repository.UsersRepository;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
public class UserService {
    private final UsersRepository usersRepository;
    private final PasswordEncoder passwordEncoder;

    public UserService(UsersRepository usersRepository, PasswordEncoder passwordEncoder) {
        this.usersRepository = usersRepository;
        this.passwordEncoder = passwordEncoder;
    }

    public Page<Users> getAllUsers(Pageable pageable) {
        return usersRepository.findAll(pageable);
    }

    public Users getUserById(Long id) {
        return usersRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("User not found"));
    }

    @Transactional
    public Users updateUserRole(Long id, Role role) {
        Users user = getUserById(id);
        user.setRole(role);
        return usersRepository.save(user);
    }

    @Transactional
    public void deleteUser(Long id) {
        Users user = getUserById(id);
        usersRepository.delete(user);
    }
}
