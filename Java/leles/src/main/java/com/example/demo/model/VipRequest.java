package com.example.demo.model;

import jakarta.persistence.*;
import java.util.Objects;

@Entity
@Table(name = "vip_requests")
public class VipRequest {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    @Column(name = "user_id", nullable = false)
    private Integer userId;

    @Column(nullable = false, length = 255)
    private String email;

    @Convert(converter = YesNoConverter.class)
    @Column(nullable = false, columnDefinition = "ENUM('no','yes') DEFAULT 'no'")
    private YesNo status = YesNo.NO;

    @Convert(converter = YesNoConverter.class)
    @Column(name = "close", nullable = false, columnDefinition = "ENUM('no','yes') DEFAULT 'no'")
    private YesNo close = YesNo.NO;

    public VipRequest() {}

    public VipRequest(Integer userId, String email) {
        this.userId = userId;
        this.email = email;
    }

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public Integer getUserId() {
        return userId;
    }

    public void setUserId(Integer userId) {
        this.userId = userId;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public YesNo getStatus() {
        return status;
    }

    public void setStatus(YesNo status) {
        this.status = status;
    }

    public YesNo getClose() {
        return close;
    }

    public void setClose(YesNo close) {
        this.close = close;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof VipRequest)) return false;
        VipRequest that = (VipRequest) o;
        return Objects.equals(id, that.id);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}