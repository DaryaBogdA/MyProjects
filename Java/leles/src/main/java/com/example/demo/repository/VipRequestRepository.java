package com.example.demo.repository;

import com.example.demo.model.VipRequest;
import com.example.demo.model.YesNo;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface VipRequestRepository extends JpaRepository<VipRequest, Integer> {
    Optional<VipRequest> findFirstByUserIdAndStatusAndClose(Integer userId, YesNo status, YesNo close);

    List<VipRequest> findByStatusAndCloseOrderByIdDesc(YesNo status, YesNo close);
}
