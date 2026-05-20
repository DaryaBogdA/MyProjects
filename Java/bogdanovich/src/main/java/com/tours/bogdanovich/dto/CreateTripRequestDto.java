package com.tours.bogdanovich.dto;

import java.time.LocalDate;
import java.util.List;

public class CreateTripRequestDto {
    private Integer countryId;
    private Integer cityId;
    private LocalDate dateFrom;
    private LocalDate dateTo;
    private Integer peopleCount;
    private String title;
    private List<TripItemRequestDto> items;

    public Integer getCountryId() {
        return countryId;
    }

    public void setCountryId(Integer countryId) {
        this.countryId = countryId;
    }

    public Integer getCityId() {
        return cityId;
    }

    public void setCityId(Integer cityId) {
        this.cityId = cityId;
    }

    public LocalDate getDateFrom() {
        return dateFrom;
    }

    public void setDateFrom(LocalDate dateFrom) {
        this.dateFrom = dateFrom;
    }

    public LocalDate getDateTo() {
        return dateTo;
    }

    public void setDateTo(LocalDate dateTo) {
        this.dateTo = dateTo;
    }

    public Integer getPeopleCount() {
        return peopleCount;
    }

    public void setPeopleCount(Integer peopleCount) {
        this.peopleCount = peopleCount;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public List<TripItemRequestDto> getItems() {
        return items;
    }

    public void setItems(List<TripItemRequestDto> items) {
        this.items = items;
    }
}
