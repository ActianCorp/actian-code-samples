package com.actian.example;

import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import org.hibernate.Session;
import org.hibernate.Transaction;

import com.actian.example.Util;
import com.actian.example.entity.Airline;

public class InitData {

	private static List<String[]> content = null;

	public static List<String[]> readData() throws IOException { 
		String dataFile = System.getenv("HIBDATA");
		content = new ArrayList<>();
    	try(BufferedReader br = new BufferedReader(new FileReader(dataFile))) {
        	String line = "";
	        while ((line = br.readLine()) != null) {
	            content.add(line.split(","));
    	    }
	    } catch (FileNotFoundException e) {
      		System.out.println(e.getMessage());
    	}
    return content;
	}

	public static void main(String[] args) {

		try {
			readData();
		} catch (IOException e) {
			System.out.println(e.getMessage());
		}

		Airline airline = null;
		Transaction transaction = null;
		Session session = null;

		try {
			session = Util.getSessionFactory(true).openSession();
			// start a transaction
			transaction = session.beginTransaction();

			for (String[] a : content) {
				airline = new Airline(Integer.parseInt(a[0]),a[1],a[2],a[3],a[4]);
				// save the objects
				session.persist(airline);
			}

			// commit transaction
			transaction.commit();
		} catch (Exception e) {
			if (transaction != null) {
				transaction.rollback();
			}
			e.printStackTrace();
		}

		try {
			List<Airline> airlines = session.createQuery("from Airline", Airline.class).list();
			System.out.println("==========  Airlines  ==========");
			for (Airline al : airlines) {
                                System.out.println(" "+al.getAl_name());
                        }
		} catch (Exception e) {
			if (transaction != null) {
				transaction.rollback();
			}
			e.printStackTrace();
		}
	}
}
