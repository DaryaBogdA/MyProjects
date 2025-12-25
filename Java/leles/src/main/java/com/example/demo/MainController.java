package com.example.demo;
import org.springframework.ui.Model;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;

@Controller
public class MainController {

    @GetMapping("/")
    public String index(HttpSession session, Model model) {
        model.addAttribute("login", session.getAttribute("login"));
        return "index";
    }

    @GetMapping("/logout")
    public String logout(HttpSession session) {
        if (session != null) {
            session.invalidate();
        }
        return "redirect:/";
    }

    @GetMapping("/matrix")
    public String matrix(HttpSession session, Model model) {
        String login = (String) session.getAttribute("login");
        if (login == null) {
            return "redirect:/login";
        }

        model.addAttribute("login", login);
        return "matrix";
    }

}

