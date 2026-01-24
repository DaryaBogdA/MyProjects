package com.shop.subscription.controllers;


import com.shop.subscription.entity.Post;
import com.shop.subscription.repository.PostReposittory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.ArrayList;
import java.util.Optional;

@Controller
public class ShopController {
    @Autowired
    private PostReposittory postReposittory;

    @GetMapping("/shop")
    public String main(Model model) {
        Iterable<Post> posts = postReposittory.findAll();
        model.addAttribute("posts", posts);
        return "shop";
    }

    @GetMapping("/shop/add")
    public String shopAdd(Model model) {
        return "shopAdd";
    }

    @PostMapping("/shop/add")
    public String shopPostAdd(@RequestParam String title, @RequestParam String anons, @RequestParam String full_text, Model model) {
        Post post = new Post(title, anons, full_text);
        postReposittory.save(post);
        return "redirect:/shop";
    }

    @GetMapping("/shop/{id}")
    public String shopInfo(@PathVariable(value = "id") long id, Model model) {
        if(!postReposittory.existsById(id)){
            return "redirect:/shop";
        }
        Optional<Post> post = postReposittory.findById(id);
        ArrayList<Post> res = new ArrayList<>();
        post.ifPresent(res::add);
        model.addAttribute("post", res);
        return "shopInfo";
    }

    @GetMapping("/shop/{id}/edit")
    public String shopEdit(@PathVariable(value = "id") long id, Model model) {
        if(!postReposittory.existsById(id)){
            return "redirect:/shop";
        }

        Optional<Post> post = postReposittory.findById(id);
        ArrayList<Post> res = new ArrayList<>();
        post.ifPresent(res::add);
        model.addAttribute("post", res);
        return "shopEdit";
    }

    @PostMapping("/shop/{id}/edit")
    public String shopPostEdit(@PathVariable(value = "id") long id, @RequestParam String title, @RequestParam String anons, @RequestParam String full_text, Model model) {
        Post post = postReposittory.findById(id).orElseThrow();
        post.setTitle(title);
        post.setAnons(anons);
        post.setFull_text(full_text);
        postReposittory.save(post);
        return "redirect:/shop";
    }

    @PostMapping("/shop/{id}/delete")
    public String shopPostDelete(@PathVariable(value = "id") long id, Model model) {
        Post post = postReposittory.findById(id).orElseThrow();
        postReposittory.delete(post);

        return "redirect:/shop";
    }
}
