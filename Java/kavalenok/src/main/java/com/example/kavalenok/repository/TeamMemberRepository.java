package com.example.kavalenok.repository;

import com.example.kavalenok.model.*;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import java.util.List;
import java.util.Optional;

public interface TeamMemberRepository extends JpaRepository<TeamMember, TeamMemberId> {

    @Query("SELECT tm FROM TeamMember tm WHERE tm.team.id = :teamId AND tm.user.id = :userId")
    Optional<TeamMember> findByTeamIdAndUserId(@Param("teamId") Long teamId, @Param("userId") Long userId);

    @Query("SELECT tm FROM TeamMember tm WHERE tm.team.id = :teamId AND tm.status = :status")
    List<TeamMember> findByTeamIdAndStatus(@Param("teamId") Long teamId, @Param("status") TeamMemberStatus status);

    @Query("SELECT tm FROM TeamMember tm WHERE tm.team.id = :teamId AND tm.status = :status ORDER BY tm.role ASC")
    List<TeamMember> findByTeamIdAndStatusOrderByRoleAsc(@Param("teamId") Long teamId, @Param("status") TeamMemberStatus status);

    @Query("SELECT COUNT(tm) FROM TeamMember tm WHERE tm.team.id = :teamId AND tm.status = 'APPROVED' AND tm.role = :role")
    long countApprovedByTeamAndRole(@Param("teamId") Long teamId, @Param("role") TeamRole role);

    @Query("SELECT CASE WHEN COUNT(tm) > 0 THEN true ELSE false END FROM TeamMember tm WHERE tm.team.id = :teamId AND tm.user.id = :userId AND tm.status = :status")
    boolean existsByTeamIdAndUserIdAndStatus(@Param("teamId") Long teamId, @Param("userId") Long userId, @Param("status") TeamMemberStatus status);
}