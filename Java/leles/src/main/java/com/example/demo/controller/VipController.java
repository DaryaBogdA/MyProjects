package com.example.demo.controller;

import com.example.demo.model.User;
import com.example.demo.model.VipRequest;
import com.example.demo.model.YesNo;
import com.example.demo.repository.UserRepository;
import com.example.demo.repository.VipRequestRepository;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;

import java.util.Optional;

@Controller
public class VipController {

    private final UserRepository userRepository;
    private final VipRequestRepository vipRequestRepository;

    public VipController(UserRepository userRepository,
                         VipRequestRepository vipRequestRepository) {
        this.userRepository = userRepository;
        this.vipRequestRepository = vipRequestRepository;
    }

    @PostMapping("/vip/request")
    public String requestVip(HttpSession session, RedirectAttributes redirectAttributes) {
        String login = (String) session.getAttribute("login");
        if (login == null) return "redirect:/login";

        User user = userRepository.findByLogin(login).orElse(null);
        if (user == null) {
            session.removeAttribute("login");
            return "redirect:/login";
        }

        Optional<VipRequest> existing = vipRequestRepository
                .findFirstByUserIdAndStatusAndClose(user.getId(), YesNo.NO, YesNo.NO);

        if (existing.isPresent()) {
            redirectAttributes.addFlashAttribute("error", 1);
            redirectAttributes.addFlashAttribute("message", "Вы уже отправляли запрос. Ждите одобрения администратора.");
            return "redirect:/user";
        }

        VipRequest req = new VipRequest();
        req.setUserId(user.getId());
        req.setEmail(user.getEmail());
        vipRequestRepository.save(req);

        redirectAttributes.addFlashAttribute("error", 0);
        redirectAttributes.addFlashAttribute("message", "Запрос отправлен. Ждите одобрения администратора.");
        return "redirect:/user";
    }
}
