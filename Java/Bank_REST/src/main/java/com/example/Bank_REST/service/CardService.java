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

@Service
public class CardService {
    private final CardsRepository cardsRepository;
    private final UsersRepository usersRepository;

    public CardService(CardsRepository cardsRepository, UsersRepository usersRepository) {
        this.cardsRepository = cardsRepository;
        this.usersRepository = usersRepository;
    }

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

    public Page<Cards> getUserCards(Long ownerId, Pageable pageable) {
        return cardsRepository.findByOwnerId(ownerId, pageable);
    }

    public Cards getCardById(Long cardId, Long userId, boolean isAdmin) {
        Cards card = cardsRepository.findById(cardId)
                .orElseThrow(() -> new RuntimeException("Card not found"));
        
        if (!isAdmin && !card.getOwner().getId().equals(userId)) {
            throw new RuntimeException("Access denied: Card does not belong to user");
        }
        
        return card;
    }

    public BigDecimal getCardBalance(Long cardId, Long userId, boolean isAdmin) {
        Cards card = getCardById(cardId, userId, isAdmin);
        return card.getBalance();
    }

    public Page<Cards> getAllCards(Pageable pageable) {
        return cardsRepository.findAll(pageable);
    }

    public Page<Cards> getUserCardsWithSearch(Long ownerId, String search, Pageable pageable) {
        if (search != null && !search.trim().isEmpty()) {
            return cardsRepository.findByOwnerIdAndMaskedNumberContainingIgnoreCase(ownerId, search.trim(), pageable);
        }
        return cardsRepository.findByOwnerId(ownerId, pageable);
    }

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

    @Transactional
    public void deleteCard(Long cardId) {
        Cards card = cardsRepository.findById(cardId)
                .orElseThrow(() -> new RuntimeException("Card not found"));
        cardsRepository.delete(card);
    }

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

    private String encrypt(String number) {
        return Base64.getEncoder().encodeToString(number.getBytes());
    }

    private String mask(String number) {
        return "**** **** **** " + number.substring(number.length() - 4);
    }
}
