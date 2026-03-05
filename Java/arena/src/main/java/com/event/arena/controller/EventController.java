package com.event.arena.controller;

import com.event.arena.entity.*;
import com.event.arena.repository.CityRepository;
import com.event.arena.repository.EventRepository;
import com.event.arena.repository.GalleryImageRepository;
import com.event.arena.repository.SportRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.time.LocalTime;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@RestController
@RequestMapping
public class EventController {
    private final SportRepository sportRepository;
    private final CityRepository cityRepository;
    private final EventRepository eventRepository;
    private final GalleryImageRepository galleryImageRepository;

    public EventController(EventRepository eventRepository, GalleryImageRepository galleryImageRepository, SportRepository sportRepository, CityRepository cityRepository) {
        this.eventRepository = eventRepository;
        this.galleryImageRepository = galleryImageRepository;
        this.sportRepository = sportRepository;
        this.cityRepository = cityRepository;
    }

    @GetMapping("/sports")
    public List<Sport> getAllSports() {
        return sportRepository.findAll();
    }

    @GetMapping("/cities")
    public List<City> getAllCities() {
        return cityRepository.findAll();
    }

    @GetMapping("/events")
    public ResponseEntity<List<Map<String, Object>>> getApprovedEvents() {
        List<Event> events = eventRepository.findByStatusWithSportAndCity(EventStatus.approved);
        List<Map<String, Object>> result = events.stream()
                .map(this::eventToMap)
                .collect(Collectors.toList());
        return ResponseEntity.ok(result);
    }

    @PostMapping("/events")
    public ResponseEntity<?> createEvent(@AuthenticationPrincipal User user,
                                         @RequestBody Map<String, Object> payload) {
        if (user == null) {
            return ResponseEntity.status(401).body(Map.of("error", "Необходимо войти в систему"));
        }
        try {
            String title = (String) payload.get("title");
            String description = (String) payload.get("description");
            Long sportId = Long.valueOf(payload.get("sportId").toString());
            Long cityId = Long.valueOf(payload.get("cityId").toString());
            LocalDate date = LocalDate.parse((String) payload.get("date"));
            LocalTime time = LocalTime.parse((String) payload.get("time"));
            String location = (String) payload.get("location");
            Object priceObj = payload.get("price");
            BigDecimal price;
            if (priceObj instanceof Number) {
                price = BigDecimal.valueOf(((Number) priceObj).doubleValue());
            } else {
                price = BigDecimal.ZERO;
            }
            Integer maxParticipants = (Integer) payload.get("maxParticipants");
            String imageUrl = (String) payload.get("imageUrl");
            Double latitude = (Double) payload.get("latitude");
            Double longitude = (Double) payload.get("longitude");

            Sport sport = sportRepository.findById(sportId).orElse(null);
            City city = cityRepository.findById(cityId).orElse(null);
            if (sport == null || city == null) {
                return ResponseEntity.badRequest().body(Map.of("error", "Неверный вид спорта или город"));
           }

            Event event = new Event();
            event.setTitle(title);
            event.setDescription(description);
            event.setSport(sport);
            event.setCity(city);
            event.setDate(date);
            event.setTime(time);
            event.setLocation(location);
            event.setPrice(price);
            event.setMaxParticipants(maxParticipants);
            event.setCurrentParticipants(0);
            event.setImageUrl(imageUrl);
            event.setLatitude(latitude);
            event.setLongitude(longitude);
            event.setStatus(EventStatus.pending);
            event.setCreatedBy(user);

            eventRepository.save(event);
            return ResponseEntity.ok(Map.of("message", "Событие отправлено на модерацию"));
        } catch (Exception e) {
            return ResponseEntity.badRequest().body(Map.of("error", "Ошибка при создании события: " + e.getMessage()));
        }
    }

    @GetMapping("/gallery")
    public ResponseEntity<List<Map<String, Object>>> getGallery() {
        List<GalleryImage> images = galleryImageRepository.findAllByOrderByDisplayOrderAsc();
        List<Map<String, Object>> result = images.stream()
                .map(img -> {
                    Map<String, Object> m = new HashMap<>();
                    m.put("id", img.getId());
                    m.put("imageUrl", img.getImageUrl());
                    m.put("caption", img.getCaption() != null ? img.getCaption() : "");
                    return m;
                })
                .collect(Collectors.toList());
        return ResponseEntity.ok(result);
    }

    private Map<String, Object> eventToMap(Event e) {
        Map<String, Object> m = new HashMap<>();
        m.put("id", e.getId());
        m.put("title", e.getTitle());
        m.put("description", e.getDescription());
        m.put("date", e.getDate() != null ? e.getDate().toString() : null);
        m.put("time", e.getTime() != null ? e.getTime().toString() : null);
        m.put("location", e.getLocation());
        m.put("price", e.getPrice() != null ? e.getPrice().doubleValue() : 0);
        m.put("maxParticipants", e.getMaxParticipants());
        m.put("currentParticipants", e.getCurrentParticipants() != null ? e.getCurrentParticipants() : 0);
        m.put("imageUrl", e.getImageUrl() != null ? e.getImageUrl() : "");
        m.put("status", e.getStatus() != null ? e.getStatus().name() : "approved");
        m.put("latitude", 0);
        m.put("longitude", 0);
        if (e.getSport() != null) {
            Map<String, Object> sport = new HashMap<>();
            sport.put("id", e.getSport().getId());
            sport.put("name", e.getSport().getName());
            m.put("sport", sport);
        } else {
            m.put("sport", null);
        }
        if (e.getCity() != null) {
            Map<String, Object> city = new HashMap<>();
            city.put("id", e.getCity().getId());
            city.put("name", e.getCity().getName());
            m.put("city", city);
        } else {
            m.put("city", null);
        }
        return m;
    }
}
