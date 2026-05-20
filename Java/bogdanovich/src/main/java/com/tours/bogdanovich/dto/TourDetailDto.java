package com.tours.bogdanovich.dto;

import java.math.BigDecimal;
import java.util.List;

public class TourDetailDto {
    private Integer id;
    private String name;
    private String description;
    private String type;
    private Integer durationDays;
    private BigDecimal priceTotal;
    private String photoUrl;
    private Integer popularity;
    private Integer cityId;
    private String cityName;
    private String countryName;
    private String programInfo;
    private List<ExcursionDto> excursions;
    private List<AttractionResponceDto> attractions;

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public Integer getDurationDays() {
        return durationDays;
    }

    public void setDurationDays(Integer durationDays) {
        this.durationDays = durationDays;
    }

    public BigDecimal getPriceTotal() {
        return priceTotal;
    }

    public void setPriceTotal(BigDecimal priceTotal) {
        this.priceTotal = priceTotal;
    }

    public String getPhotoUrl() {
        return photoUrl;
    }

    public void setPhotoUrl(String photoUrl) {
        this.photoUrl = photoUrl;
    }

    public Integer getPopularity() {
        return popularity;
    }

    public void setPopularity(Integer popularity) {
        this.popularity = popularity;
    }

    public Integer getCityId() {
        return cityId;
    }

    public void setCityId(Integer cityId) {
        this.cityId = cityId;
    }

    public String getCityName() {
        return cityName;
    }

    public void setCityName(String cityName) {
        this.cityName = cityName;
    }

    public String getCountryName() {
        return countryName;
    }

    public void setCountryName(String countryName) {
        this.countryName = countryName;
    }

    public String getProgramInfo() {
        return programInfo;
    }

    public void setProgramInfo(String programInfo) {
        this.programInfo = programInfo;
    }

    public List<ExcursionDto> getExcursions() {
        return excursions;
    }

    public void setExcursions(List<ExcursionDto> excursions) {
        this.excursions = excursions;
    }

    public List<AttractionResponceDto> getAttractions() {
        return attractions;
    }

    public void setAttractions(List<AttractionResponceDto> attractions) {
        this.attractions = attractions;
    }
}
