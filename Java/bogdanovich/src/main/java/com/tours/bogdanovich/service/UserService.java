package com.tours.bogdanovich.service;

import com.tours.bogdanovich.dto.RegistrationRequestDto;
import com.tours.bogdanovich.entity.Role;
import com.tours.bogdanovich.entity.Users;
import com.tours.bogdanovich.repository.UsersRepository;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

@Service
public class UserService {

    private final UsersRepository usersRepository;
    private final PasswordEncoder passwordEncoder;

    public UserService(UsersRepository usersRepository, PasswordEncoder passwordEncoder) {
        this.usersRepository = usersRepository;
        this.passwordEncoder = passwordEncoder;
    }

    public Users loadUser(String email) throws UsernameNotFoundException {
        return usersRepository.findByEmail(email)
                .orElseThrow(() -> new UsernameNotFoundException("Нет такого пользователя"));
    }

    public Users registerUser(RegistrationRequestDto dto){
        if(usersRepository.findByEmail(dto.getEmail()).isPresent()){
            throw new RuntimeException("Email уже используется");
        }
        String encodedPassword = passwordEncoder.encode(dto.getPassword());

        Users user = new Users();
        user.setEmail(dto.getEmail());
        user.setPasswordHash(encodedPassword);
        user.setRole(Role.USER);

        return usersRepository.save(user);
    }
}
