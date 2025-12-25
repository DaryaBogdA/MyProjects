package com.example.demo.check;

import com.example.demo.model.CardTaro;
import com.example.demo.model.DayCard;
import com.example.demo.model.User;
import com.example.demo.repository.DayCardRepository;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.util.Optional;

@Service
public class DayCardCheckService {
    private final DayCardRepository dayCardRepository;

    public DayCardCheckService(DayCardRepository dayCardRepository) {
        this.dayCardRepository = dayCardRepository;
    }


    @Transactional
    public DayCard getOrCreateCard(User user, CardTaro card) {
        LocalDate today = LocalDate.now();

        Optional<DayCard> existing = dayCardRepository.findByIdUserIdAndIdDate(user.getId(), today);
        if (existing.isPresent()) {
            return existing.get();
        }

        DayCard newCard = new DayCard();
        newCard.setUser(user);
        newCard.setCard(card);
        newCard.setDate(today);

        try {
            return dayCardRepository.save(newCard);
        } catch (DataIntegrityViolationException ex) {
            return dayCardRepository.findByIdUserIdAndIdDate(user.getId(), today)
                    .orElseThrow(() -> ex);
        }
    }

    public Optional<DayCard> findByUserIdAndDate(Integer userId, LocalDate date) {
        return dayCardRepository.findByIdUserIdAndIdDate(userId, date);
    }

    @Transactional
    public DayCard createForUserAndDate(User user, CardTaro card, LocalDate date) {
        DayCard newCard = new DayCard();
        newCard.setUser(user);
        newCard.setCard(card);
        newCard.setDate(date);
        return dayCardRepository.save(newCard);
    }
}
