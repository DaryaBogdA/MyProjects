package com.example.Bank_REST.service;

import com.example.Bank_REST.entity.Role;
import com.example.Bank_REST.entity.Users;
import com.example.Bank_REST.repository.UsersRepository;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

/**
 * Сервис для управления пользователями.
 * 
 * <p>Предоставляет методы для получения, обновления ролей и удаления пользователей.
 * Используется администраторами для управления пользователями системы.</p>
 * 
 * @author darya
 */
@Service
public class UserService {
    private final UsersRepository usersRepository;
    private final PasswordEncoder passwordEncoder;

    /**
     * Конструктор сервиса пользователей.
     * 
     * @param usersRepository репозиторий для работы с пользователями
     * @param passwordEncoder кодировщик паролей
     */
    public UserService(UsersRepository usersRepository, PasswordEncoder passwordEncoder) {
        this.usersRepository = usersRepository;
        this.passwordEncoder = passwordEncoder;
    }

    /**
     * Получает всех пользователей с пагинацией.
     * 
     * @param pageable параметры пагинации
     * @return страница с пользователями
     */
    public Page<Users> getAllUsers(Pageable pageable) {
        return usersRepository.findAll(pageable);
    }

    /**
     * Получает пользователя по идентификатору.
     * 
     * @param id идентификатор пользователя
     * @return пользователь
     * @throws RuntimeException если пользователь не найден
     */
    public Users getUserById(Long id) {
        return usersRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("User not found"));
    }

    /**
     * Обновляет роль пользователя.
     * 
     * @param id идентификатор пользователя
     * @param role новая роль
     * @return обновленный пользователь
     */
    @Transactional
    public Users updateUserRole(Long id, Role role) {
        Users user = getUserById(id);
        user.setRole(role);
        return usersRepository.save(user);
    }

    /**
     * Удаляет пользователя из системы.
     * 
     * @param id идентификатор пользователя
     * @throws RuntimeException если пользователь не найден
     */
    @Transactional
    public void deleteUser(Long id) {
        Users user = getUserById(id);
        usersRepository.delete(user);
    }
}
