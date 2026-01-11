package com.example.Bank_REST.service;

import com.example.Bank_REST.dto.CardCreateDto;
import com.example.Bank_REST.entity.CardStatus;
import com.example.Bank_REST.entity.Cards;
import com.example.Bank_REST.entity.Users;
import com.example.Bank_REST.repository.CardsRepository;
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

import java.math.BigDecimal;
import java.time.Instant;
import java.time.LocalDate;
import java.util.Arrays;
import java.util.List;
import java.util.Optional;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
@DisplayName("CardService Unit Tests")
class CardServiceTest {

    @Mock
    private CardsRepository cardsRepository;

    @Mock
    private UsersRepository usersRepository;

    @InjectMocks
    private CardService cardService;

    private Users owner;
    private Cards card;
    private CardCreateDto cardCreateDto;

    @BeforeEach
    void setUp() {
        owner = new Users();
        owner.setId(1L);
        owner.setUsername("testuser");
        owner.setEmail("test@example.com");

        card = new Cards();
        card.setId(1L);
        card.setEncryptedNumber("MTIzNDU2Nzg5MDEyMzQ1Ng==");
        card.setMaskedNumber("**** **** **** 3456");
        card.setOwner(owner);
        card.setExpiryDate(LocalDate.of(2025, 12, 31));
        card.setBalance(new BigDecimal("1000.00"));
        card.setStatus(CardStatus.ACTIVE);
        card.setCreatedAt(Instant.now());
        card.setUpdatedAt(Instant.now());

        cardCreateDto = new CardCreateDto();
        cardCreateDto.setNumber("1234567890123456");
        cardCreateDto.setOwnerId(1L);
        cardCreateDto.setPeriod(LocalDate.of(2025, 12, 31));
        cardCreateDto.setBalance(new BigDecimal("1000.00"));
    }

    @Test
    @DisplayName("Should create card successfully")
    void testCreateCard_Success() {
        when(usersRepository.findById(1L)).thenReturn(Optional.of(owner));
        when(cardsRepository.save(any(Cards.class))).thenAnswer(invocation -> {
            Cards savedCard = invocation.getArgument(0);
            savedCard.setId(1L);
            return savedCard;
        });

        Cards result = cardService.createCard(cardCreateDto);

        assertNotNull(result);
        assertEquals(1L, result.getId());
        assertEquals(new BigDecimal("1000.00"), result.getBalance());
        assertEquals(CardStatus.ACTIVE, result.getStatus());
        assertEquals("**** **** **** 3456", result.getMaskedNumber());
        assertNotNull(result.getEncryptedNumber());
        verify(usersRepository).findById(1L);
        verify(cardsRepository).save(any(Cards.class));
    }

    @Test
    @DisplayName("Should throw exception when owner not found")
    void testCreateCard_OwnerNotFound() {
        when(usersRepository.findById(1L)).thenReturn(Optional.empty());

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            cardService.createCard(cardCreateDto);
        });

        assertEquals("Owner not found", exception.getMessage());
        verify(usersRepository).findById(1L);
        verify(cardsRepository, never()).save(any());
    }

    @Test
    @DisplayName("Should get card by id for admin")
    void testGetCardById_AdminAccess() {
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));

        Cards result = cardService.getCardById(1L, 2L, true);

        assertNotNull(result);
        assertEquals(1L, result.getId());
        verify(cardsRepository).findById(1L);
    }

    @Test
    @DisplayName("Should get card by id for owner")
    void testGetCardById_OwnerAccess() {
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));

        Cards result = cardService.getCardById(1L, 1L, false);

        assertNotNull(result);
        assertEquals(1L, result.getId());
        verify(cardsRepository).findById(1L);
    }

    @Test
    @DisplayName("Should throw exception when accessing card that doesn't belong to user")
    void testGetCardById_AccessDenied() {
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            cardService.getCardById(1L, 2L, false);
        });

        assertEquals("Access denied: Card does not belong to user", exception.getMessage());
        verify(cardsRepository).findById(1L);
    }

    @Test
    @DisplayName("Should throw exception when card not found")
    void testGetCardById_CardNotFound() {
        when(cardsRepository.findById(1L)).thenReturn(Optional.empty());

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            cardService.getCardById(1L, 1L, false);
        });

        assertEquals("Card not found", exception.getMessage());
        verify(cardsRepository).findById(1L);
    }

    @Test
    @DisplayName("Should get card balance successfully")
    void testGetCardBalance_Success() {
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));

        BigDecimal balance = cardService.getCardBalance(1L, 1L, false);

        assertNotNull(balance);
        assertEquals(new BigDecimal("1000.00"), balance);
        verify(cardsRepository).findById(1L);
    }

    @Test
    @DisplayName("Should block card successfully by admin")
    void testBlockCard_ByAdmin() {
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));
        when(cardsRepository.save(any(Cards.class))).thenReturn(card);

        Cards result = cardService.blockCard(1L, 2L, true);

        assertNotNull(result);
        assertEquals(CardStatus.BLOCKED, result.getStatus());
        verify(cardsRepository).findById(1L);
        verify(cardsRepository).save(card);
    }

    @Test
    @DisplayName("Should block card successfully by owner")
    void testBlockCard_ByOwner() {
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));
        when(cardsRepository.save(any(Cards.class))).thenReturn(card);

        Cards result = cardService.blockCard(1L, 1L, false);

        assertNotNull(result);
        assertEquals(CardStatus.BLOCKED, result.getStatus());
        verify(cardsRepository).findById(1L);
        verify(cardsRepository).save(card);
    }

    @Test
    @DisplayName("Should throw exception when trying to block non-active card by user")
    void testBlockCard_NonActiveCardByUser() {
        card.setStatus(CardStatus.BLOCKED);
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            cardService.blockCard(1L, 1L, false);
        });

        assertEquals("Only active cards can be blocked", exception.getMessage());
        verify(cardsRepository).findById(1L);
        verify(cardsRepository, never()).save(any());
    }

    @Test
    @DisplayName("Should activate card successfully")
    void testActivateCard_Success() {
        card.setStatus(CardStatus.BLOCKED);
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));
        when(cardsRepository.save(any(Cards.class))).thenReturn(card);

        Cards result = cardService.activateCard(1L);

        assertNotNull(result);
        assertEquals(CardStatus.ACTIVE, result.getStatus());
        verify(cardsRepository).findById(1L);
        verify(cardsRepository).save(card);
    }

    @Test
    @DisplayName("Should throw exception when trying to activate expired card")
    void testActivateCard_ExpiredCard() {
        card.setStatus(CardStatus.EXPIRED);
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            cardService.activateCard(1L);
        });

        assertEquals("Cannot activate expired card", exception.getMessage());
        verify(cardsRepository).findById(1L);
        verify(cardsRepository, never()).save(any());
    }

    @Test
    @DisplayName("Should update card balance successfully")
    void testUpdateCard_UpdateBalance() {
        BigDecimal newBalance = new BigDecimal("2000.00");
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));
        when(cardsRepository.save(any(Cards.class))).thenReturn(card);

        Cards result = cardService.updateCard(1L, 1L, true, null, newBalance);

        assertNotNull(result);
        assertEquals(newBalance, result.getBalance());
        verify(cardsRepository).findById(1L);
        verify(cardsRepository).save(card);
    }

    @Test
    @DisplayName("Should throw exception when setting negative balance")
    void testUpdateCard_NegativeBalance() {
        BigDecimal negativeBalance = new BigDecimal("-100.00");
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            cardService.updateCard(1L, 1L, true, null, negativeBalance);
        });

        assertEquals("Balance cannot be negative", exception.getMessage());
        verify(cardsRepository).findById(1L);
        verify(cardsRepository, never()).save(any());
    }

    @Test
    @DisplayName("Should delete card successfully")
    void testDeleteCard_Success() {
        when(cardsRepository.findById(1L)).thenReturn(Optional.of(card));
        doNothing().when(cardsRepository).delete(card);

        cardService.deleteCard(1L);

        verify(cardsRepository).findById(1L);
        verify(cardsRepository).delete(card);
    }

    @Test
    @DisplayName("Should transfer funds successfully")
    void testTransfer_Success() {
        Cards fromCard = new Cards();
        fromCard.setId(1L);
        fromCard.setOwner(owner);
        fromCard.setBalance(new BigDecimal("1000.00"));
        fromCard.setStatus(CardStatus.ACTIVE);

        Cards toCard = new Cards();
        toCard.setId(2L);
        toCard.setOwner(owner);
        toCard.setBalance(new BigDecimal("500.00"));
        toCard.setStatus(CardStatus.ACTIVE);

        BigDecimal amount = new BigDecimal("200.00");

        when(cardsRepository.findById(1L)).thenReturn(Optional.of(fromCard));
        when(cardsRepository.findById(2L)).thenReturn(Optional.of(toCard));
        when(cardsRepository.save(any(Cards.class))).thenAnswer(invocation -> invocation.getArgument(0));

        cardService.transfer(1L, 2L, amount, 1L);

        assertEquals(new BigDecimal("800.00"), fromCard.getBalance());
        assertEquals(new BigDecimal("700.00"), toCard.getBalance());
        verify(cardsRepository).save(fromCard);
        verify(cardsRepository).save(toCard);
    }

    @Test
    @DisplayName("Should throw exception when insufficient funds")
    void testTransfer_InsufficientFunds() {
        Cards fromCard = new Cards();
        fromCard.setId(1L);
        fromCard.setOwner(owner);
        fromCard.setBalance(new BigDecimal("100.00"));
        fromCard.setStatus(CardStatus.ACTIVE);

        Cards toCard = new Cards();
        toCard.setId(2L);
        toCard.setOwner(owner);
        toCard.setBalance(new BigDecimal("500.00"));
        toCard.setStatus(CardStatus.ACTIVE);

        BigDecimal amount = new BigDecimal("200.00");

        when(cardsRepository.findById(1L)).thenReturn(Optional.of(fromCard));
        when(cardsRepository.findById(2L)).thenReturn(Optional.of(toCard));

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            cardService.transfer(1L, 2L, amount, 1L);
        });

        assertEquals("Insufficient funds", exception.getMessage());
        verify(cardsRepository, never()).save(any());
    }

    @Test
    @DisplayName("Should throw exception when cards don't belong to user")
    void testTransfer_CardsNotBelongToUser() {
        Users otherUser = new Users();
        otherUser.setId(2L);

        Cards fromCard = new Cards();
        fromCard.setId(1L);
        fromCard.setOwner(otherUser);
        fromCard.setStatus(CardStatus.ACTIVE);

        Cards toCard = new Cards();
        toCard.setId(2L);
        toCard.setOwner(owner);
        toCard.setStatus(CardStatus.ACTIVE);

        BigDecimal amount = new BigDecimal("200.00");

        when(cardsRepository.findById(1L)).thenReturn(Optional.of(fromCard));
        when(cardsRepository.findById(2L)).thenReturn(Optional.of(toCard));

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            cardService.transfer(1L, 2L, amount, 1L);
        });

        assertEquals("Cards do not belong to user", exception.getMessage());
        verify(cardsRepository, never()).save(any());
    }

    @Test
    @DisplayName("Should throw exception when transferring from inactive card")
    void testTransfer_FromInactiveCard() {
        Cards fromCard = new Cards();
        fromCard.setId(1L);
        fromCard.setOwner(owner);
        fromCard.setBalance(new BigDecimal("1000.00"));
        fromCard.setStatus(CardStatus.BLOCKED);

        Cards toCard = new Cards();
        toCard.setId(2L);
        toCard.setOwner(owner);
        toCard.setStatus(CardStatus.ACTIVE);

        BigDecimal amount = new BigDecimal("200.00");

        when(cardsRepository.findById(1L)).thenReturn(Optional.of(fromCard));
        when(cardsRepository.findById(2L)).thenReturn(Optional.of(toCard));

        RuntimeException exception = assertThrows(RuntimeException.class, () -> {
            cardService.transfer(1L, 2L, amount, 1L);
        });

        assertEquals("Cannot transfer from or to inactive/blocked cards", exception.getMessage());
        verify(cardsRepository, never()).save(any());
    }

    @Test
    @DisplayName("Should get user cards with search")
    void testGetUserCardsWithSearch() {
        Pageable pageable = PageRequest.of(0, 10);
        List<Cards> cards = Arrays.asList(card);
        Page<Cards> cardPage = new PageImpl<>(cards, pageable, 1);

        when(cardsRepository.findByOwnerIdAndMaskedNumberContainingIgnoreCase(eq(1L), eq("3456"), eq(pageable)))
                .thenReturn(cardPage);

        Page<Cards> result = cardService.getUserCardsWithSearch(1L, "3456", pageable);

        assertNotNull(result);
        assertEquals(1, result.getTotalElements());
        verify(cardsRepository).findByOwnerIdAndMaskedNumberContainingIgnoreCase(eq(1L), eq("3456"), eq(pageable));
    }

    @Test
    @DisplayName("Should get user cards without search")
    void testGetUserCardsWithoutSearch() {
        Pageable pageable = PageRequest.of(0, 10);
        List<Cards> cards = Arrays.asList(card);
        Page<Cards> cardPage = new PageImpl<>(cards, pageable, 1);

        when(cardsRepository.findByOwnerId(eq(1L), eq(pageable))).thenReturn(cardPage);

        Page<Cards> result = cardService.getUserCardsWithSearch(1L, null, pageable);

        assertNotNull(result);
        assertEquals(1, result.getTotalElements());
        verify(cardsRepository).findByOwnerId(eq(1L), eq(pageable));
        verify(cardsRepository, never()).findByOwnerIdAndMaskedNumberContainingIgnoreCase(any(), any(), any());
    }
}
