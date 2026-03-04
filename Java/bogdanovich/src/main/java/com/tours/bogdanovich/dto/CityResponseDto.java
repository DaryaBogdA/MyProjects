package com.tours.bogdanovich.dto;

import java.util.List;

public class CityResponseDto {
    private Integer id;
    private Integer countryId;
    private String name;
    private Integer popularity;
    private String description;
    private List<AttractionResponceDto> attractions;

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public Integer getCountryId() {
        return countryId;
    }

    public void setCountryId(Integer countryId) {
        this.countryId = countryId;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public Integer getPopularity() {
        return popularity;
    }

    public void setPopularity(Integer popularity) {
        this.popularity = popularity;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public List<AttractionResponceDto> getAttractions() {
        return attractions;
    }

    public void setAttractions(List<AttractionResponceDto> attractions) {
        this.attractions = attractions;
    }
}
