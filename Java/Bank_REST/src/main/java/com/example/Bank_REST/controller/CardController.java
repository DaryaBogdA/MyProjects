package com.example.Bank_REST.controller;

import com.example.Bank_REST.dto.CardCreateDto;
import com.example.Bank_REST.dto.CardResponseDto;
import com.example.Bank_REST.dto.CardUpdateDto;
import com.example.Bank_REST.entity.Cards;
import com.example.Bank_REST.security.CustomUserDetails;
import com.example.Bank_REST.service.CardService;
import jakarta.validation.Valid;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;
import io.swagger.v3.oas.annotations.Parameter;

import java.math.BigDecimal;
import java.util.HashMap;
import java.util.Map;

/**
 * REST контроллер для управления банковскими картами.
 * 
 * <p>Предоставляет эндпоинты для создания, просмотра, обновления и удаления карт.
 * Пользователи могут просматривать только свои карты, администраторы имеют полный доступ.</p>
 * 
 * @author darya
 */
@RestController
@RequestMapping("/cards")
public class CardController {
    private final CardService cardService;

    /**
     * Конструктор контроллера карт.
     * 
     * @param cardService сервис для работы с картами
     */
    public CardController(CardService cardService) {
        this.cardService = cardService;
    }

    /**
     * Создает новую банковскую карту (только для администраторов).
     * 
     * @param dto данные для создания карты
     * @return созданная карта
     */
    @PostMapping
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<CardResponseDto> create(@Valid @RequestBody CardCreateDto dto) {
        Cards card = cardService.createCard(dto);
        return ResponseEntity.ok(toDto(card));
    }

    /**
     * Получает все карты в системе (только для администраторов).
     * 
     * @param pageable параметры пагинации
     * @return страница с картами
     */
    @GetMapping
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<Page<CardResponseDto>> getAllCards(@Parameter(hidden = true) Pageable pageable) {
        Page<Cards> cards = cardService.getAllCards(pageable);
        return ResponseEntity.ok(cards.map(this::toDto));
    }

    /**
     * Получает карты текущего пользователя с возможностью поиска.
     * 
     * @param userDetails данные аутентифицированного пользователя
     * @param search поисковый запрос (по маскированному номеру карты)
     * @param pageable параметры пагинации
     * @return страница с картами пользователя
     */
    @GetMapping("/my")
    public ResponseEntity<Page<CardResponseDto>> myCards(
            @Parameter(hidden = true) @AuthenticationPrincipal CustomUserDetails userDetails,
            @RequestParam(required = false) String search,
            @Parameter(hidden = true) Pageable pageable) {
        Long userId = userDetails.getId();
        Page<Cards> cards = cardService.getUserCardsWithSearch(userId, search, pageable);
        return ResponseEntity.ok(cards.map(this::toDto));
    }

    /**
     * Получает карту по идентификатору.
     * Пользователи могут получить только свои карты, администраторы - любые.
     * 
     * @param id идентификатор карты
     * @param userDetails данные аутентифицированного пользователя
     * @return данные карты
     */
    @GetMapping("/{id}")
    public ResponseEntity<CardResponseDto> getCardById(
            @PathVariable Long id,
            @Parameter(hidden = true) @AuthenticationPrincipal CustomUserDetails userDetails) {
        if (userDetails == null) {
            throw new RuntimeException("User not authenticated. Please provide a valid JWT token in Authorization header");
        }
        Long userId = userDetails.getId();
        boolean isAdmin = userDetails.getAuthorities().stream()
                .anyMatch(a -> a.getAuthority().equals("ROLE_ADMIN"));
        
        Cards card = cardService.getCardById(id, userId, isAdmin);
        return ResponseEntity.ok(toDto(card));
    }

    /**
     * Получает баланс карты по идентификатору.
     * Пользователи могут получить баланс только своих карт, администраторы - любых.
     * 
     * @param id идентификатор карты
     * @param userDetails данные аутентифицированного пользователя
     * @return баланс карты
     */
    @GetMapping("/{id}/balance")
    public ResponseEntity<Map<String, BigDecimal>> getBalance(
            @PathVariable Long id,
            @Parameter(hidden = true) @AuthenticationPrincipal CustomUserDetails userDetails) {
        if (userDetails == null) {
            throw new RuntimeException("User not authenticated. Please provide a valid JWT token in Authorization header");
        }
        Long userId = userDetails.getId();
        boolean isAdmin = userDetails.getAuthorities().stream()
                .anyMatch(a -> a.getAuthority().equals("ROLE_ADMIN"));
        
        try {
            BigDecimal balance = cardService.getCardBalance(id, userId, isAdmin);
            Map<String, BigDecimal> response = new HashMap<>();
            response.put("balance", balance);
            return ResponseEntity.ok(response);
        } catch (RuntimeException e) {
            throw e;
        }
    }

    /**
     * Обновляет данные карты (только для администраторов).
     * 
     * @param id идентификатор карты
     * @param dto данные для обновления (период действия, баланс)
     * @param userDetails данные аутентифицированного пользователя
     * @return обновленная карта
     */
    @PutMapping("/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<CardResponseDto> updateCard(
            @PathVariable Long id,
            @Valid @RequestBody CardUpdateDto dto,
            @Parameter(hidden = true) @AuthenticationPrincipal CustomUserDetails userDetails) {
        Long userId = userDetails.getId();
        boolean isAdmin = userDetails.getAuthorities().stream()
                .anyMatch(a -> a.getAuthority().equals("ROLE_ADMIN"));
        
        Cards card = cardService.updateCard(id, userId, isAdmin, dto.getPeriod(), dto.getBalance());
        return ResponseEntity.ok(toDto(card));
    }

    /**
     * Блокирует карту (только для администраторов).
     * 
     * @param id идентификатор карты
     * @param userDetails данные аутентифицированного пользователя
     * @return заблокированная карта
     */
    @PutMapping("/{id}/block")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<CardResponseDto> blockCard(
            @PathVariable Long id,
            @Parameter(hidden = true) @AuthenticationPrincipal CustomUserDetails userDetails) {
        Long userId = userDetails.getId();
        Cards card = cardService.blockCard(id, userId, true);
        return ResponseEntity.ok(toDto(card));
    }

    /**
     * Активирует карту (только для администраторов).
     * 
     * @param id идентификатор карты
     * @return активированная карта
     */
    @PutMapping("/{id}/activate")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<CardResponseDto> activateCard(@PathVariable Long id) {
        Cards card = cardService.activateCard(id);
        return ResponseEntity.ok(toDto(card));
    }

    /**
     * Удаляет карту (только для администраторов).
     * 
     * @param id идентификатор карты
     * @return сообщение об успешном удалении
     */
    @DeleteMapping("/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<String> deleteCard(@PathVariable Long id) {
        cardService.deleteCard(id);
        return ResponseEntity.ok("Card deleted successfully");
    }

    /**
     * Запрос на блокировку карты пользователем.
     * Пользователи могут блокировать только свои активные карты.
     * 
     * @param id идентификатор карты
     * @param userDetails данные аутентифицированного пользователя
     * @return сообщение об успешной блокировке
     */
    @PostMapping("/{id}/request-block")
    public ResponseEntity<String> requestBlockCard(
            @PathVariable Long id,
            @Parameter(hidden = true) @AuthenticationPrincipal CustomUserDetails userDetails) {
        Long userId = userDetails.getId();
        boolean isAdmin = userDetails.getAuthorities().stream()
                .anyMatch(a -> a.getAuthority().equals("ROLE_ADMIN"));
        
        cardService.blockCard(id, userId, isAdmin);
        return ResponseEntity.ok("Card blocked successfully");
    }

    /**
     * Переводит средства между картами пользователя.
     * Обе карты должны принадлежать текущему пользователю и быть активными.
     * 
     * @param userDetails данные аутентифицированного пользователя
     * @param fromCardId идентификатор карты-отправителя
     * @param toCardId идентификатор карты-получателя
     * @param amount сумма перевода
     * @return сообщение об успешном переводе
     */
    @PostMapping("/transfer")
    public ResponseEntity<String> transfer(
            @Parameter(hidden = true) @AuthenticationPrincipal CustomUserDetails userDetails,
            @RequestParam Long fromCardId,
            @RequestParam Long toCardId,
            @RequestParam BigDecimal amount) {
        Long userId = userDetails.getId();
        cardService.transfer(fromCardId, toCardId, amount, userId);
        return ResponseEntity.ok("Transfer successful");
    }

    /**
     * Преобразует сущность Cards в DTO для ответа.
     * 
     * @param card сущность карты
     * @return DTO карты
     */
    private CardResponseDto toDto(Cards card) {
        CardResponseDto dto = new CardResponseDto();
        dto.setId(card.getId());
        dto.setMaskedNumber(card.getMaskedNumber());
        dto.setOwnerId(card.getOwner().getId());
        dto.setPeriod(card.getExpiryDate());
        dto.setStatus(card.getStatus().name());
        dto.setBalance(card.getBalance());
        return dto;
    }
}
