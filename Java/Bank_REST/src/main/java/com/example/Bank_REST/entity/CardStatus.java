package com.example.Bank_REST.entity;

/**
 * Статусы банковской карты.
 * 
 * @author darya
 */
public enum CardStatus {
    /** Активная карта, доступна для операций */
    ACTIVE,
    /** Заблокированная карта, операции недоступны */
    BLOCKED,
    /** Карта с истекшим сроком действия */
    EXPIRED
}
