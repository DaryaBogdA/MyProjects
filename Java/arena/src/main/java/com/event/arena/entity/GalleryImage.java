package com.event.arena.entity;

import jakarta.persistence.*;

@Entity
@Table(name = "gallery_images")
public class GalleryImage {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "event_id")
    private Event event;

    @Column(name = "image_url", nullable = false)
    private String imageUrl;

    private String caption;

    @Column(name = "display_order")
    private Integer displayOrder = 0;

    public GalleryImage() {}

    public GalleryImage(Event event, String imageUrl, String caption, Integer displayOrder) {
        this.event = event;
        this.imageUrl = imageUrl;
        this.caption = caption;
        this.displayOrder = displayOrder;
    }

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public Event getEvent() { return event; }
    public void setEvent(Event event) { this.event = event; }
    public String getImageUrl() { return imageUrl; }
    public void setImageUrl(String imageUrl) { this.imageUrl = imageUrl; }
    public String getCaption() { return caption; }
    public void setCaption(String caption) { this.caption = caption; }
    public Integer getDisplayOrder() { return displayOrder; }
    public void setDisplayOrder(Integer displayOrder) { this.displayOrder = displayOrder; }
}
