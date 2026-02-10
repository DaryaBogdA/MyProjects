package com.example.demo.model;

import jakarta.persistence.*;

@Entity
@Table(name = "compatibility")
public class Compatibility {

    @Id
    private Integer id;

    @Column(columnDefinition = "TEXT")
    private String text;

    public Compatibility() {}

    public Compatibility(Integer id, String text) {
        this.id = id;
        this.text = text;
    }

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }
}