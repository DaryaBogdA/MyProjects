package com.event.arena.service;

import com.event.arena.dto.RegistrationRequestDto;
import com.event.arena.entity.Role;
import com.event.arena.entity.User;
import com.event.arena.repository.UserRepository;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

@Service
public class UserService {
    private final UserRepository usersRepository;
    private final PasswordEncoder passwordEncoder;

    public UserService(UserRepository usersRepository, PasswordEncoder passwordEncoder) {
        this.usersRepository = usersRepository;
        this.passwordEncoder = passwordEncoder;
    }

    public User loadUser(String email) throws UsernameNotFoundException {
        return usersRepository.findByEmail(email)
                .orElseThrow(() -> new UsernameNotFoundException("Нет такого пользователя"));
    }

    public User registerUser(RegistrationRequestDto dto){
        if(usersRepository.findByEmail(dto.getEmail()).isPresent()){
            throw new RuntimeException("Email уже используется");
        }
        String encodedPassword = passwordEncoder.encode(dto.getPassword());

        User user = new User();
        user.setEmail(dto.getEmail());
        user.setPasswordHash(encodedPassword);
        user.setRole(Role.USER);
        user.setFirstName(dto.getFirstName());
        user.setLastName(dto.getLastName());

        return usersRepository.save(user);
    }
}
