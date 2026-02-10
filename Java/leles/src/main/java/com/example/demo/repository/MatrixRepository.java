package com.example.demo.repository;

import com.example.demo.model.Matrix;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface MatrixRepository extends JpaRepository<Matrix, Integer> {
    List<Matrix> findByIdIn(List<Integer> ids);
}
