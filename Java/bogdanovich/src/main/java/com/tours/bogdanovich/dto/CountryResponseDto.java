package com.tours.bogdanovich.dto;

import java.util.List;

public class CountryResponseDto {
    private Integer id;
    private String name;
    private Integer popularity;
    private String path_url;
    private String description;
    private List<CityResponseDto> cities;

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

    public Integer getPopularity() {
        return popularity;
    }

    public void setPopularity(Integer popularity) {
        this.popularity = popularity;
    }

    public String getPath_url() {
        return path_url;
    }

    public void setPath_url(String path_url) {
        this.path_url = path_url;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public List<CityResponseDto> getCities() {
        return cities;
    }

    public void setCities(List<CityResponseDto> cities) {
        this.cities = cities;
    }
}
