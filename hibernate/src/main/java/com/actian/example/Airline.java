package com.actian.example.entity;

import jakarta.persistence.*;

@Entity
@Table(name = "airline")
public class Airline {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int id;
    private int al_id;
    private String al_iatacode;
    private String al_icaocode;
    private String al_name;
    private String al_ccode;

    public Airline(){

    }

    public Airline(int al_id, String al_iatacode, String al_icaocode, String al_name, String al_ccode) {
        this.al_id = al_id;
        this.al_iatacode = al_iatacode;
        this.al_icaocode = al_icaocode;
        this.al_name = al_name;
        this.al_ccode = al_ccode;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getAl_id() {
        return al_id;
    }

    public void setAl_Id(int al_id) {
        this.id = al_id;
    }

    public String getAl_iatacode() {
        return al_iatacode;
    }

    public void setAl_iatacode(String al_iatacode) {
        this.al_iatacode = al_iatacode;
    }

    public String getAl_icaocode() {
        return al_icaocode;
    }

    public void setAl_icaocode(String al_icaocode) {
        this.al_icaocode = al_icaocode;
    }

    public String getAl_name() {
        return al_name;
    }

    public void setAl_name(String al_name) {
        this.al_name = al_name;
    }

    public String getAl_ccode() {
        return al_ccode;
    }

    public void setAl_ccode(String al_ccode) {
        this.al_ccode = al_ccode;
    }
}
