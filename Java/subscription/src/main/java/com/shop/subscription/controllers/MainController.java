package com.shop.subscription.controllers;

import com.shop.subscription.entity.Product;
import com.shop.subscription.repository.ProductRepositury;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;

@Controller
public class MainController {
    @Autowired
    private ProductRepositury productRepositury;

    @GetMapping("/")
    public String main(Model model) {
        model.addAttribute("title", "Главная страница");
        Iterable<Product> products = productRepositury.findAll();
        model.addAttribute("products", products);
        return "main";
    }
}