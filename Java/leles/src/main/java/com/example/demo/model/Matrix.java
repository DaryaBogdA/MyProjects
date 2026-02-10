package com.example.demo.model;

import jakarta.persistence.*;

@Entity
@Table(name = "matrix")
public class Matrix {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id")
    private Integer id;

    @Column(name = "essence_energy_plus", columnDefinition = "TEXT", nullable = false)
    private String essenceEnergyPlus;

    @Column(name = "strengths_plus", columnDefinition = "TEXT", nullable = false)
    private String strengthsPlus;

    @Column(name = "risks_plus", columnDefinition = "TEXT", nullable = false)
    private String risksPlus;

    @Column(name = "essence_energy_minus", columnDefinition = "TEXT", nullable = false)
    private String essenceEnergyMinus;

    @Column(name = "in_minus", columnDefinition = "TEXT", nullable = false)
    private String inMinus;

    @Column(name = "traps_minus", columnDefinition = "TEXT", nullable = false)
    private String trapsMinus;

    @Column(name = "essence_energy_practice", columnDefinition = "TEXT", nullable = false)
    private String essenceEnergyPractice;

    @Column(name = "in_practice", columnDefinition = "TEXT", nullable = false)
    private String inPractice;

    @Column(name = "money_practice", columnDefinition = "TEXT", nullable = false)
    private String moneyPractice;

    @Column(name = "relationship_practice", columnDefinition = "TEXT", nullable = false)
    private String relationshipPractice;

    public Matrix() {
    }

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getEssenceEnergyPlus() {
        return essenceEnergyPlus;
    }

    public void setEssenceEnergyPlus(String essenceEnergyPlus) {
        this.essenceEnergyPlus = essenceEnergyPlus;
    }

    public String getStrengthsPlus() {
        return strengthsPlus;
    }

    public void setStrengthsPlus(String strengthsPlus) {
        this.strengthsPlus = strengthsPlus;
    }

    public String getRisksPlus() {
        return risksPlus;
    }

    public void setRisksPlus(String risksPlus) {
        this.risksPlus = risksPlus;
    }

    public String getEssenceEnergyMinus() {
        return essenceEnergyMinus;
    }

    public void setEssenceEnergyMinus(String essenceEnergyMinus) {
        this.essenceEnergyMinus = essenceEnergyMinus;
    }

    public String getInMinus() {
        return inMinus;
    }

    public void setInMinus(String inMinus) {
        this.inMinus = inMinus;
    }

    public String getTrapsMinus() {
        return trapsMinus;
    }

    public void setTrapsMinus(String trapsMinus) {
        this.trapsMinus = trapsMinus;
    }

    public String getEssenceEnergyPractice() {
        return essenceEnergyPractice;
    }

    public void setEssenceEnergyPractice(String essenceEnergyPractice) {
        this.essenceEnergyPractice = essenceEnergyPractice;
    }

    public String getInPractice() {
        return inPractice;
    }

    public void setInPractice(String inPractice) {
        this.inPractice = inPractice;
    }

    public String getMoneyPractice() {
        return moneyPractice;
    }

    public void setMoneyPractice(String moneyPractice) {
        this.moneyPractice = moneyPractice;
    }

    public String getRelationshipPractice() {
        return relationshipPractice;
    }

    public void setRelationshipPractice(String relationshipPractice) {
        this.relationshipPractice = relationshipPractice;
    }
}