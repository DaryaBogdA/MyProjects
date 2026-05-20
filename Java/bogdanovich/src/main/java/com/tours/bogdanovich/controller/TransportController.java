package com.tours.bogdanovich.controller;

import com.tours.bogdanovich.dto.TransportDto;
import com.tours.bogdanovich.entity.Transport;
import com.tours.bogdanovich.repository.TransportRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/transport")
public class TransportController {

    private final TransportRepository transportRepository;

    public TransportController(TransportRepository transportRepository) {
        this.transportRepository = transportRepository;
    }

    @GetMapping("/between")
    public ResponseEntity<List<TransportDto>> getTransportBetween(
            @RequestParam Integer fromCityId,
            @RequestParam Integer toCityId) {
        List<Transport> transports = transportRepository.findByFromCityIdAndToCityId(fromCityId, toCityId);
        List<TransportDto> dtos = transports.stream().map(this::toDto).collect(Collectors.toList());
        return ResponseEntity.ok(dtos);
    }

    private TransportDto toDto(Transport t) {
        TransportDto dto = new TransportDto();
        dto.setId(t.getId());
        dto.setType(t.getType().toString());
        dto.setCompany(t.getCompany());
        dto.setDepartureTime(t.getDepartureTime());
        dto.setArrivalTime(t.getArrivalTime());
        dto.setPrice(t.getPrice());
        dto.setInfo(t.getInfo());
        return dto;
    }
}