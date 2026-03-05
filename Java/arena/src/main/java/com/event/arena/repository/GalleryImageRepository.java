package com.event.arena.repository;

import com.event.arena.entity.GalleryImage;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface GalleryImageRepository extends JpaRepository<GalleryImage, Long> {

    List<GalleryImage> findAllByOrderByDisplayOrderAsc();
}
