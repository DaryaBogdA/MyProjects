package com.example.kavalenok.service;

import com.example.kavalenok.model.Training;
import com.example.kavalenok.model.TrainingParticipant;
import com.example.kavalenok.model.User;
import com.example.kavalenok.model.UserAchievement;
import com.example.kavalenok.repository.TrainingParticipantRepository;
import com.example.kavalenok.repository.TrainingRepository;
import com.example.kavalenok.repository.UserAchievementRepository;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@Service
public class ProfileInsightsService {

    private final TrainingRepository trainingRepository;
    private final TrainingParticipantRepository trainingParticipantRepository;
    private final UserAchievementRepository userAchievementRepository;

    public ProfileInsightsService(TrainingRepository trainingRepository,
                                  TrainingParticipantRepository trainingParticipantRepository,
                                  UserAchievementRepository userAchievementRepository) {
        this.trainingRepository = trainingRepository;
        this.trainingParticipantRepository = trainingParticipantRepository;
        this.userAchievementRepository = userAchievementRepository;
    }

    public List<Training> getRecommendationsFor(User user) {
        List<TrainingParticipant> participations =
                trainingParticipantRepository.findByUserIdOrderByAppliedAtDesc(user.getId());

        String preferredLocation = participations.stream()
                .map(TrainingParticipant::getTraining)
                .map(Training::getLocation)
                .filter(v -> v != null && !v.isBlank())
                .findFirst()
                .orElse(null);

        Long preferredCoachId = participations.stream()
                .map(TrainingParticipant::getTraining)
                .filter(t -> t.getCoach() != null)
                .map(t -> t.getCoach().getId())
                .findFirst()
                .orElse(null);

        List<Training> allUpcoming = trainingRepository.findTrainingsWithFilters(null, null, null, null, null);
        List<Training> filtered = allUpcoming.stream()
                .filter(t -> t.getDateTime() != null && t.getDateTime().isAfter(LocalDateTime.now()))
                .filter(t -> preferredLocation == null || preferredLocation.equalsIgnoreCase(t.getLocation()))
                .sorted(Comparator.comparing(Training::getDateTime))
                .toList();

        if (filtered.isEmpty() && preferredCoachId != null) {
            filtered = allUpcoming.stream()
                    .filter(t -> t.getCoach() != null && preferredCoachId.equals(t.getCoach().getId()))
                    .sorted(Comparator.comparing(Training::getDateTime))
                    .toList();
        }

        return filtered.stream().limit(5).toList();
    }

    public List<UserAchievement> syncAndGetAchievements(User user) {
        return syncAchievements(user).unlocked();
    }

    public List<AchievementProgress> getAllAchievementsProgress(User user) {
        AchievementSyncResult syncResult = syncAchievements(user);
        Map<String, UserAchievement> unlockedMap = new HashMap<>();
        for (UserAchievement unlocked : syncResult.unlocked()) {
            unlockedMap.put(unlocked.getBadgeName(), unlocked);
        }

        List<AchievementProgress> progress = new ArrayList<>();
        for (AchievementDefinition definition : ACHIEVEMENT_DEFINITIONS) {
            UserAchievement unlocked = unlockedMap.get(definition.badgeName());
            progress.add(new AchievementProgress(
                    definition.badgeName(),
                    definition.badgeIconUrl(),
                    definition.description(),
                    unlocked != null,
                    unlocked != null ? unlocked.getAwardedAt() : null
            ));
        }
        return progress;
    }

    private AchievementSyncResult syncAchievements(User user) {
        List<TrainingParticipant> participations =
                trainingParticipantRepository.findByUserIdOrderByAppliedAtDesc(user.getId());

        long approvedCount = participations.stream()
                .filter(p -> p.getStatus() == TrainingParticipant.Status.APPROVED
                        || p.getStatus() == TrainingParticipant.Status.ATTENDED)
                .count();

        long attendedCount = participations.stream()
                .filter(p -> p.getStatus() == TrainingParticipant.Status.ATTENDED)
                .count();

        for (AchievementDefinition definition : ACHIEVEMENT_DEFINITIONS) {
            if (definition.unlockRule().matches(approvedCount, attendedCount)) {
                ensureBadge(user, definition.badgeName(), definition.badgeIconUrl());
            }
        }

        List<UserAchievement> unlocked = userAchievementRepository.findByUserIdOrderByAwardedAtDesc(user.getId());
        return new AchievementSyncResult(unlocked, approvedCount, attendedCount);
    }

    public List<String> buildDevelopmentPlan(User user) {
        List<TrainingParticipant> participations =
                trainingParticipantRepository.findByUserIdOrderByAppliedAtDesc(user.getId());
        long attendedCount = participations.stream()
                .filter(p -> p.getStatus() == TrainingParticipant.Status.ATTENDED)
                .count();
        List<String> plan = new ArrayList<>();

        if (attendedCount == 0) {
            plan.add("Начните с 2 базовых тренировок в ближайшие 10 дней (прием и перемещение).");
            plan.add("После каждой тренировки фиксируйте 1 сильную сторону и 1 точку роста.");
        } else if (attendedCount < 3) {
            plan.add("Посетите минимум 3 тренировки для адаптации к игровому ритму.");
            plan.add("Сделайте акцент на стабильной подаче: 30 повторений в конце каждой тренировки.");
        } else if (attendedCount < 8) {
            plan.add("Сфокусируйтесь на приеме и подаче: 2 профильные тренировки в неделю.");
            plan.add("Добавьте 1 игровую тренировку 6x6 в неделю для чтения эпизода.");
        } else if (attendedCount < 15) {
            plan.add("Усильте блок и взаимодействие на сетке: отрабатывайте связки по позициям.");
            plan.add("Раз в неделю пересматривайте видео своих розыгрышей и отмечайте 3 ошибки.");
        } else {
            plan.add("Добавьте игровую вариативность: участвуйте в разных форматах тренировок.");
            plan.add("Работайте над лидерством на площадке: подсказывайте партнерам по эпизоду.");
        }
        plan.add("Запланируйте не менее 2 тренировок на ближайшие 14 дней.");
        plan.add("Оставьте отзыв тренеру после следующей тренировки для фиксации прогресса.");
        return plan;
    }

    private void ensureBadge(User user, String badgeName, String badgeIconUrl) {
        if (userAchievementRepository.findByUserIdAndBadgeName(user.getId(), badgeName).isEmpty()) {
            UserAchievement achievement = new UserAchievement(user, badgeName);
            achievement.setBadgeIconUrl(badgeIconUrl);
            userAchievementRepository.save(achievement);
        }
    }

    public static class AchievementProgress {
        private final String badgeName;
        private final String badgeIconUrl;
        private final String description;
        private final boolean unlocked;
        private final LocalDateTime awardedAt;

        public AchievementProgress(String badgeName, String badgeIconUrl, String description, boolean unlocked, LocalDateTime awardedAt) {
            this.badgeName = badgeName;
            this.badgeIconUrl = badgeIconUrl;
            this.description = description;
            this.unlocked = unlocked;
            this.awardedAt = awardedAt;
        }

        public String getBadgeName() { return badgeName; }
        public String getBadgeIconUrl() { return badgeIconUrl; }
        public String getDescription() { return description; }
        public boolean isUnlocked() { return unlocked; }
        public LocalDateTime getAwardedAt() { return awardedAt; }
    }

    private record AchievementDefinition(
            String badgeName,
            String badgeIconUrl,
            String description,
            AchievementUnlockRule unlockRule
    ) {}

    @FunctionalInterface
    private interface AchievementUnlockRule {
        boolean matches(long approvedCount, long attendedCount);
    }

    private record AchievementSyncResult(
            List<UserAchievement> unlocked,
            long approvedCount,
            long attendedCount
    ) {}

    private static final List<AchievementDefinition> ACHIEVEMENT_DEFINITIONS = List.of(
            new AchievementDefinition(
                    "Первые шаги",
                    "/img/avatars/1.png",
                    "Записаться хотя бы на 1 тренировку.",
                    (approved, attended) -> approved >= 1
            ),
            new AchievementDefinition(
                    "В игре",
                    "/img/avatars/2.png",
                    "Посетить 1 тренировку.",
                    (approved, attended) -> attended >= 1
            ),
            new AchievementDefinition(
                    "Пять шагов вперед",
                    "/img/avatars/3.png",
                    "Подтвердить участие в 5 тренировках.",
                    (approved, attended) -> approved >= 5
            ),
            new AchievementDefinition(
                    "Ритм недели",
                    "/img/avatars/4.png",
                    "Посетить 5 тренировок.",
                    (approved, attended) -> attended >= 5
            ),
            new AchievementDefinition(
                    "Десятка",
                    "/img/avatars/5.png",
                    "Подтвердить участие в 10 тренировках.",
                    (approved, attended) -> approved >= 10
            ),
            new AchievementDefinition(
                    "Командный ритм",
                    "/img/avatars/6.png",
                    "Посетить 10 тренировок.",
                    (approved, attended) -> attended >= 10
            ),
            new AchievementDefinition(
                    "Надежный партнер",
                    "/img/avatars/7.png",
                    "Подтвердить участие в 15 тренировках.",
                    (approved, attended) -> approved >= 15
            ),
            new AchievementDefinition(
                    "Набор формы",
                    "/img/avatars/8.png",
                    "Посетить 15 тренировок.",
                    (approved, attended) -> attended >= 15
            ),
            new AchievementDefinition(
                    "Железная дисциплина",
                    "/img/avatars/9.png",
                    "Посетить 20 тренировок.",
                    (approved, attended) -> attended >= 20
            ),
            new AchievementDefinition(
                    "Постоянный прогресс",
                    "/img/avatars/10.png",
                    "Подтвердить участие в 25 тренировках.",
                    (approved, attended) -> approved >= 25
            ),
            new AchievementDefinition(
                    "Выносливость",
                    "/img/avatars/11.png",
                    "Посетить 25 тренировок.",
                    (approved, attended) -> attended >= 25
            ),
            new AchievementDefinition(
                    "Игровой интеллект",
                    "/img/avatars/12.png",
                    "Подтвердить участие в 30 тренировках.",
                    (approved, attended) -> approved >= 30
            ),
            new AchievementDefinition(
                    "Марафонец площадки",
                    "/img/avatars/13.png",
                    "Посетить 30 тренировок.",
                    (approved, attended) -> attended >= 30
            ),
            new AchievementDefinition(
                    "Серия без пауз",
                    "/img/avatars/14.png",
                    "Подтвердить участие в 40 тренировках.",
                    (approved, attended) -> approved >= 40
            ),
            new AchievementDefinition(
                    "Мощный сезон",
                    "/img/avatars/15.png",
                    "Посетить 40 тренировок.",
                    (approved, attended) -> attended >= 40
            ),
            new AchievementDefinition(
                    "Опора команды",
                    "/img/avatars/16.png",
                    "Подтвердить участие в 50 тренировках.",
                    (approved, attended) -> approved >= 50
            ),
            new AchievementDefinition(
                    "Лидер площадки",
                    "/img/avatars/17.png",
                    "Посетить 50 тренировок.",
                    (approved, attended) -> attended >= 50
            ),
            new AchievementDefinition(
                    "Профессиональный подход",
                    "/img/avatars/18.png",
                    "Подтвердить участие в 75 тренировках.",
                    (approved, attended) -> approved >= 75
            ),
            new AchievementDefinition(
                    "Элита тренировок",
                    "/img/avatars/19.png",
                    "Посетить 75 тренировок.",
                    (approved, attended) -> attended >= 75
            ),
            new AchievementDefinition(
                    "Легенда VolleyPro",
                    "/img/avatars/20.png",
                    "Посетить 100 тренировок.",
                    (approved, attended) -> attended >= 100
            )
    );
}
