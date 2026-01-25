package com.example.Bank_REST.service;

import com.example.Bank_REST.dto.CardCreateDto;
import com.example.Bank_REST.entity.CardStatus;
import com.example.Bank_REST.entity.Cards;
import com.example.Bank_REST.entity.Users;
import com.example.Bank_REST.repository.CardsRepository;
import com.example.Bank_REST.repository.UsersRepository;
import jakarta.transaction.Transactional;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.math.BigDecimal;
import java.time.Instant;
import java.time.LocalDate;
import java.util.Base64;

/**
 * Сервис для управления банковскими картами.
 * 
 * <p>Предоставляет методы для создания, получения, обновления и удаления карт,
 * а также для выполнения переводов между картами.</p>
 * 
 * @author darya
 */
@Service
public class CardService {
    private final CardsRepository cardsRepository;
    private final UsersRepository usersRepository;

    /**
     * Конструктор сервиса карт.
     * 
     * @param cardsRepository репозиторий для работы с картами
     * @param usersRepository репозиторий для работы с пользователями
     */
    public CardService(CardsRepository cardsRepository, UsersRepository usersRepository) {
        this.cardsRepository = cardsRepository;
        this.usersRepository = usersRepository;
    }

    /**
     * Создает новую банковскую карту.
     * 
     * <p>Шифрует номер карты, создает маскированную версию и сохраняет карту
     * со статусом ACTIVE.</p>
     * 
     * @param dto данные для создания карты
     * @return созданная карта
     * @throws RuntimeException если владелец не найден
     */
    public Cards createCard(CardCreateDto dto) {
        Users owner = usersRepository.findById(dto.getOwnerId())
                .orElseThrow(() -> new RuntimeException("Owner not found"));

        Cards card = new Cards();
        card.setEncryptedNumber(encrypt(dto.getNumber()));
        card.setMaskedNumber(mask(dto.getNumber()));
        card.setOwner(owner);
        card.setExpiryDate(dto.getPeriod());
        card.setBalance(dto.getBalance());
        card.setStatus(CardStatus.ACTIVE);
        card.setCreatedAt(Instant.now());
        card.setUpdatedAt(Instant.now());

        return cardsRepository.save(card);
    }

    /**
     * Получает все карты пользователя с пагинацией.
     * 
     * @param ownerId идентификатор владельца карт
     * @param pageable параметры пагинации
     * @return страница с картами пользователя
     */
    public Page<Cards> getUserCards(Long ownerId, Pageable pageable) {
        return cardsRepository.findByOwnerId(ownerId, pageable);
    }

    /**
     * Получает карту по идентификатору с проверкой прав доступа.
     * 
     * @param cardId идентификатор карты
     * @param userId идентификатор пользователя
     * @param isAdmin является ли пользователь администратором
     * @return карта
     * @throws RuntimeException если карта не найдена или пользователь не имеет доступа
     */
    public Cards getCardById(Long cardId, Long userId, boolean isAdmin) {
        Cards card = cardsRepository.findById(cardId)
                .orElseThrow(() -> new RuntimeException("Card not found"));
        
        if (!isAdmin && !card.getOwner().getId().equals(userId)) {
            throw new RuntimeException("Access denied: Card does not belong to user");
        }
        
        return card;
    }

    /**
     * Получает баланс карты с проверкой прав доступа.
     * 
     * @param cardId идентификатор карты
     * @param userId идентификатор пользователя
     * @param isAdmin является ли пользователь администратором
     * @return баланс карты
     */
    public BigDecimal getCardBalance(Long cardId, Long userId, boolean isAdmin) {
        Cards card = getCardById(cardId, userId, isAdmin);
        return card.getBalance();
    }

    /**
     * Получает все карты в системе с пагинацией.
     * 
     * @param pageable параметры пагинации
     * @return страница со всеми картами
     */
    public Page<Cards> getAllCards(Pageable pageable) {
        return cardsRepository.findAll(pageable);
    }

    /**
     * Получает карты пользователя с поиском по маскированному номеру.
     * 
     * @param ownerId идентификатор владельца карт
     * @param search поисковый запрос
     * @param pageable параметры пагинации
     * @return страница с найденными картами
     */
    public Page<Cards> getUserCardsWithSearch(Long ownerId, String search, Pageable pageable) {
        if (search != null && !search.trim().isEmpty()) {
            return cardsRepository.findByOwnerIdAndMaskedNumberContainingIgnoreCase(ownerId, search.trim(), pageable);
        }
        return cardsRepository.findByOwnerId(ownerId, pageable);
    }

    /**
     * Блокирует карту.
     * 
     * <p>Пользователи могут блокировать только свои активные карты.
     * Администраторы могут блокировать любые карты.</p>
     * 
     * @param cardId идентификатор карты
     * @param userId идентификатор пользователя
     * @param isAdmin является ли пользователь администратором
     * @return заблокированная карта
     * @throws RuntimeException если карта не найдена, не принадлежит пользователю
     *                          или не является активной (для обычных пользователей)
     */
    @Transactional
    public Cards blockCard(Long cardId, Long userId, boolean isAdmin) {
        Cards card = cardsRepository.findById(cardId)
                .orElseThrow(() -> new RuntimeException("Card not found"));
        
        if (!isAdmin) {
            if (!card.getOwner().getId().equals(userId)) {
                throw new RuntimeException("Access denied: Card does not belong to user");
            }
            if (card.getStatus() != CardStatus.ACTIVE) {
                throw new RuntimeException("Only active cards can be blocked");
            }
        }
        
        card.setStatus(CardStatus.BLOCKED);
        card.setUpdatedAt(Instant.now());
        return cardsRepository.save(card);
    }

    /**
     * Активирует карту.
     * 
     * @param cardId идентификатор карты
     * @return активированная карта
     * @throws RuntimeException если карта не найдена или истек срок действия
     */
    @Transactional
    public Cards activateCard(Long cardId) {
        Cards card = cardsRepository.findById(cardId)
                .orElseThrow(() -> new RuntimeException("Card not found"));
        
        if (card.getStatus() == CardStatus.EXPIRED) {
            throw new RuntimeException("Cannot activate expired card");
        }
        
        card.setStatus(CardStatus.ACTIVE);
        card.setUpdatedAt(Instant.now());
        return cardsRepository.save(card);
    }

    /**
     * Обновляет данные карты (период действия и/или баланс).
     * 
     * @param cardId идентификатор карты
     * @param userId идентификатор пользователя
     * @param isAdmin является ли пользователь администратором
     * @param period новый период действия (может быть null)
     * @param balance новый баланс (может быть null)
     * @return обновленная карта
     * @throws RuntimeException если карта не найдена, нет доступа или баланс отрицательный
     */
    @Transactional
    public Cards updateCard(Long cardId, Long userId, boolean isAdmin, LocalDate period, BigDecimal balance) {
        Cards card = getCardById(cardId, userId, isAdmin);
        
        if (period != null) {
            card.setExpiryDate(period);
        }
        
        if (balance != null) {
            if (balance.compareTo(BigDecimal.ZERO) < 0) {
                throw new RuntimeException("Balance cannot be negative");
            }
            card.setBalance(balance);
        }
        
        card.setUpdatedAt(Instant.now());
        return cardsRepository.save(card);
    }

    /**
     * Удаляет карту из системы.
     * 
     * @param cardId идентификатор карты
     * @throws RuntimeException если карта не найдена
     */
    @Transactional
    public void deleteCard(Long cardId) {
        Cards card = cardsRepository.findById(cardId)
                .orElseThrow(() -> new RuntimeException("Card not found"));
        cardsRepository.delete(card);
    }

    /**
     * Переводит средства между двумя картами пользователя.
     * 
     * <p>Обе карты должны принадлежать пользователю и быть активными.
     * На карте-отправителе должно быть достаточно средств.</p>
     * 
     * @param fromCardId идентификатор карты-отправителя
     * @param toCardId идентификатор карты-получателя
     * @param amount сумма перевода
     * @param userId идентификатор пользователя
     * @throws RuntimeException если карты не найдены, не принадлежат пользователю,
     *                          неактивны или недостаточно средств
     */
    @Transactional
    public void transfer(Long fromCardId, Long toCardId, BigDecimal amount, Long userId) {
        Cards from = cardsRepository.findById(fromCardId)
                .orElseThrow(() -> new RuntimeException("Card not found"));
        Cards to = cardsRepository.findById(toCardId)
                .orElseThrow(() -> new RuntimeException("Card not found"));

        if (!from.getOwner().getId().equals(userId) || !to.getOwner().getId().equals(userId)) {
            throw new RuntimeException("Cards do not belong to user");
        }

        if (from.getStatus() != CardStatus.ACTIVE || to.getStatus() != CardStatus.ACTIVE) {
            throw new RuntimeException("Cannot transfer from or to inactive/blocked cards");
        }

        if (from.getBalance().compareTo(amount) < 0) {
            throw new RuntimeException("Insufficient funds");
        }

        from.setBalance(from.getBalance().subtract(amount));
        to.setBalance(to.getBalance().add(amount));
        from.setUpdatedAt(Instant.now());
        to.setUpdatedAt(Instant.now());

        cardsRepository.save(from);
        cardsRepository.save(to);
    }

    /**
     * Шифрует номер карты с помощью Base64.
     * 
     * @param number номер карты
     * @return зашифрованный номер
     */
    private String encrypt(String number) {
        return Base64.getEncoder().encodeToString(number.getBytes());
    }

    /**
     * Создает маскированную версию номера карты (показывает только последние 4 цифры).
     * 
     * @param number номер карты
     * @return маскированный номер в формате "**** **** **** XXXX"
     */
    private String mask(String number) {
        return "**** **** **** " + number.substring(number.length() - 4);
    }
}
