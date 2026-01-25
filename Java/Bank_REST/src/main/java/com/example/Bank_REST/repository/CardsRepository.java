package com.example.Bank_REST.repository;

import com.example.Bank_REST.entity.Cards;
import org.springframework.data.domain.Page;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.domain.Pageable;

import java.util.Optional;

/**
 * Репозиторий для работы с банковскими картами.
 * 
 * @author darya
 */
public interface CardsRepository extends JpaRepository<Cards, Long> {
    /**
     * Находит все карты владельца с пагинацией.
     * 
     * @param ownerId идентификатор владельца
     * @param pageable параметры пагинации
     * @return страница с картами
     */
    Page<Cards> findByOwnerId(Long ownerId, Pageable pageable);
    
    /**
     * Находит карты владельца по маскированному номеру с пагинацией.
     * 
     * @param ownerId идентификатор владельца
     * @param search поисковый запрос
     * @param pageable параметры пагинации
     * @return страница с найденными картами
     */
    Page<Cards> findByOwnerIdAndMaskedNumberContainingIgnoreCase(Long ownerId, String search, Pageable pageable);
    
    /**
     * Находит карту по зашифрованному номеру.
     * 
     * @param encryptedNumber зашифрованный номер карты
     * @return Optional с картой, если найдена
     */
    Optional<Cards> findByEncryptedNumber(String encryptedNumber);
}
