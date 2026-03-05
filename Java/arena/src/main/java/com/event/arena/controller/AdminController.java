package com.event.arena.controller;

import com.event.arena.entity.*;
import com.event.arena.repository.CityRepository;
import com.event.arena.repository.EventRepository;
import com.event.arena.repository.SportRepository;
import com.event.arena.repository.UserRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.time.LocalTime;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/admin")
@PreAuthorize("hasRole('ADMIN')")
public class AdminController {

    private final EventRepository eventRepository;
    private final UserRepository userRepository;
    private final SportRepository sportRepository;
    private final CityRepository cityRepository;

    public AdminController(EventRepository eventRepository, UserRepository userRepository, SportRepository sportRepository, CityRepository cityRepository) {
        this.eventRepository = eventRepository;
        this.userRepository = userRepository;
        this.sportRepository = sportRepository;
        this.cityRepository = cityRepository;
    }

    @GetMapping("/events/pending")
    public List<Event> getPendingEvents() {
        return eventRepository.findByStatus(EventStatus.pending);
    }

    @GetMapping("/events/{id}")
    public ResponseEntity<Event> getEvent(@PathVariable Long id) {
        return eventRepository.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    @PutMapping("/events/{id}/approve")
    public ResponseEntity<?> approveEvent(@PathVariable Long id) {
        Event event = eventRepository.findById(id).orElse(null);
        if (event == null) return ResponseEntity.notFound().build();
        event.setStatus(EventStatus.approved);
        eventRepository.save(event);
        return ResponseEntity.ok(Map.of("message", "Событие одобрено"));
    }

    @PutMapping("/events/{id}/reject")
    public ResponseEntity<?> rejectEvent(@PathVariable Long id) {
        Event event = eventRepository.findById(id).orElse(null);
        if (event == null) return ResponseEntity.notFound().build();
        event.setStatus(EventStatus.rejected);
        eventRepository.save(event);
        return ResponseEntity.ok(Map.of("message", "Событие отклонено"));
    }

    @PutMapping("/events/{id}")
    public ResponseEntity<?> updateEvent(@PathVariable Long id, @RequestBody Map<String, Object> payload) {
        Event event = eventRepository.findById(id).orElse(null);
        if (event == null) return ResponseEntity.notFound().build();

        event.setTitle((String) payload.get("title"));
        event.setDescription((String) payload.get("description"));
        event.setDate(LocalDate.parse((String) payload.get("date")));
        event.setTime(LocalTime.parse((String) payload.get("time")));
        event.setLocation((String) payload.get("location"));

        Object priceObj = payload.get("price");
        if (priceObj instanceof Number) {
            event.setPrice(BigDecimal.valueOf(((Number) priceObj).doubleValue()));
        } else {
            event.setPrice(BigDecimal.ZERO);
        }

        event.setMaxParticipants((Integer) payload.get("maxParticipants"));
        event.setImageUrl((String) payload.get("imageUrl"));

        if (payload.containsKey("sportId") && payload.get("sportId") != null) {
            Long sportId = Long.valueOf(payload.get("sportId").toString());
            Sport sport = sportRepository.findById(sportId).orElse(null);
            event.setSport(sport);
        }
        if (payload.containsKey("cityId") && payload.get("cityId") != null) {
            Long cityId = Long.valueOf(payload.get("cityId").toString());
            City city = cityRepository.findById(cityId).orElse(null);
            event.setCity(city);
        }

        eventRepository.save(event);
        return ResponseEntity.ok(event);
    }

    @DeleteMapping("/events/{id}")
    public ResponseEntity<?> deleteEvent(@PathVariable Long id) {
        if (!eventRepository.existsById(id)) return ResponseEntity.notFound().build();
        eventRepository.deleteById(id);
        return ResponseEntity.ok(Map.of("message", "Событие удалено"));
    }

    @GetMapping("/users")
    public List<User> getAllUsers() {
        return userRepository.findAll();
    }

    @DeleteMapping("/users/{id}")
    public ResponseEntity<?> deleteUser(@PathVariable Long id) {
        if (!userRepository.existsById(id)) return ResponseEntity.notFound().build();
        userRepository.deleteById(id);
        return ResponseEntity.ok(Map.of("message", "Пользователь удалён"));
    }

    @GetMapping("/stats")
    public Map<String, Object> getStats() {
        long totalEvents = eventRepository.count();
        long approvedEvents = eventRepository.countByStatus(EventStatus.approved);
        long pendingEvents = eventRepository.countByStatus(EventStatus.pending);
        long totalUsers = userRepository.count();
        Map<String, Object> stats = new HashMap<>();
        stats.put("totalEvents", totalEvents);
        stats.put("approvedEvents", approvedEvents);
        stats.put("pendingEvents", pendingEvents);
        stats.put("totalUsers", totalUsers);
        return stats;
    }

    @GetMapping("/reports/participants")
    public List<Map<String, Object>> getParticipantsReport() {
        List<Event> events = eventRepository.findAll();
        return events.stream()
                .map(e -> {
                    Map<String, Object> map = new HashMap<>();
                    map.put("eventId", e.getId());
                    map.put("title", e.getTitle());
                    map.put("participants", e.getCurrentParticipants());
                    map.put("maxParticipants", e.getMaxParticipants());
                    return map;
                })
                .collect(Collectors.toList());
    }

    @GetMapping("/reports/sports")
    public List<Map<String, Object>> getSportsReport() {
        List<Event> events = eventRepository.findAll();
        Map<String, Long> sportCount = events.stream()
                .filter(e -> e.getSport() != null)
                .collect(Collectors.groupingBy(
                        e -> e.getSport().getName(),
                        Collectors.summingLong(e -> e.getCurrentParticipants())
                ));
        return sportCount.entrySet().stream()
                .map(e -> {
                    Map<String, Object> map = new HashMap<>();
                    map.put("sport", e.getKey());
                    map.put("totalParticipants", e.getValue());
                    return map;
                })
                .collect(Collectors.toList());
    }

    @GetMapping("/reports/popular")
    public List<Map<String, Object>> getPopularEvents() {
        return eventRepository.findAll().stream()
                .filter(e -> e.getStatus() == EventStatus.approved)
                .sorted((a, b) -> b.getCurrentParticipants().compareTo(a.getCurrentParticipants()))
                .limit(5)
                .map(e -> {
                    Map<String, Object> map = new HashMap<>();
                    map.put("id", e.getId());
                    map.put("title", e.getTitle());
                    map.put("participants", e.getCurrentParticipants());
                    map.put("date", e.getDate());
                    return map;
                })
                .collect(Collectors.toList());
    }

    @GetMapping("/reports/revenue")
    public Map<String, Object> getRevenueReport() {
        List<Event> events = eventRepository.findAll();
        BigDecimal totalRevenue = events.stream()
                .filter(e -> e.getStatus() == EventStatus.approved)
                .map(e -> e.getPrice().multiply(BigDecimal.valueOf(e.getCurrentParticipants())))
                .reduce(BigDecimal.ZERO, BigDecimal::add);

        Map<String, BigDecimal> revenueByEvent = events.stream()
                .filter(e -> e.getStatus() == EventStatus.approved)
                .collect(Collectors.toMap(
                        e -> e.getTitle(),
                        e -> e.getPrice().multiply(BigDecimal.valueOf(e.getCurrentParticipants()))
                ));

        Map<String, Object> result = new HashMap<>();
        result.put("totalRevenue", totalRevenue);
        result.put("byEvent", revenueByEvent);
        return result;
    }
}