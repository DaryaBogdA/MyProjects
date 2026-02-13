package com.example.kavalenok.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.core.io.Resource;
import org.springframework.core.io.support.ResourcePatternResolver;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

@Controller
public class GalleryController {

    @Autowired
    private ResourcePatternResolver resourcePatternResolver;

    @GetMapping("/gallery")
    public String gallery(Model model) {
        List<String> imageUrls = new ArrayList<>();
        try {
            Resource[] resources = resourcePatternResolver.getResources(
                    "classpath:/static/img/volley/*.{jpg,jpeg,png,gif,webp,avif,JPG,JPEG,PNG,GIF,WEBP,AVIF}"
            );            imageUrls = Arrays.stream(resources)
                    .map(resource -> "/img/volley/" + resource.getFilename())
                    .toList();
        } catch (IOException e) {
            e.printStackTrace();
        }
        model.addAttribute("images", imageUrls);
        return "gallery";
    }
}