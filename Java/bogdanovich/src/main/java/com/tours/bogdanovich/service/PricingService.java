package com.tours.bogdanovich.service;

import com.tours.bogdanovich.entity.*;
import com.tours.bogdanovich.repository.ExcursionRepository;
import com.tours.bogdanovich.repository.RoomTypeRepository;
import com.tours.bogdanovich.repository.TransportRepository;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigDecimal;

@Service
@Transactional(readOnly = true)
public class PricingService {

    private final RoomTypeRepository roomTypeRepository;
    private final TransportRepository transportRepository;
    private final ExcursionRepository excursionRepository;

    public PricingService(RoomTypeRepository roomTypeRepository,
                          TransportRepository transportRepository,
                          ExcursionRepository excursionRepository) {
        this.roomTypeRepository = roomTypeRepository;
        this.transportRepository = transportRepository;
        this.excursionRepository = excursionRepository;
    }

    public BigDecimal getPricePerUnit(ItemType type, Integer itemId) {
        if (itemId == null) {
            return BigDecimal.ZERO;
        }
        return switch (type) {
            case HOTEL -> roomTypeRepository.findById(itemId)
                    .map(RoomType::getPriceNight)
                    .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "RoomType not found"));
            case TRANSPORT -> transportRepository.findById(itemId)
                    .map(Transport::getPrice)
                    .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Transport not found"));
            case EXCURSION -> excursionRepository.findById(itemId)
                    .map(Excursion::getPrice)
                    .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Excursion not found"));
            case ATTRACTION -> BigDecimal.ZERO;
        };
    }

    public String describeItem(ItemType type, Integer itemId) {
        if (itemId == null) {
            return type.name();
        }
        return switch (type) {
            case HOTEL -> roomTypeRepository.findById(itemId)
                    .map(rt -> "Отель: " + rt.getHotel().getName() + " — " + rt.getName())
                    .orElse("Номер №" + itemId);
            case TRANSPORT -> transportRepository.findById(itemId)
                    .map(t -> "Транспорт: " + t.getType() + " " + (t.getCompany() != null ? t.getCompany() : ""))
                    .orElse("Транспорт №" + itemId);
            case EXCURSION -> excursionRepository.findById(itemId)
                    .map(e -> "Экскурсия: " + e.getName())
                    .orElse("Экскурсия №" + itemId);
            case ATTRACTION -> "Достопримечательность №" + itemId;
        };
    }
}
