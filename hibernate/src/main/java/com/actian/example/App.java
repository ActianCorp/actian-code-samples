package com.actian.example;

import java.util.List;
import java.util.Scanner;

import org.hibernate.Session;
import org.hibernate.Transaction;

import com.actian.example.Util;
import com.actian.example.entity.Airline;

public class App {

	public static Session session = null;

	public static void openSession() {
		try {
			session = Util.getSessionFactory(false).openSession();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public static void retrieveRecords() {
		try {			
			List<Airline> airlines = session.createQuery("from Airline", Airline.class).list();
			System.out.println("==========  Airline Data  ==========");
			System.out.printf("%-4s  %-5s  %-11s  %-11s  %-62s  %-4s\n", "id","al_id","al_iatacode","al_icaocode","al_name","al_ccode");
			for (Airline al : airlines) {
				System.out.printf("%-4s  %-5s  %-11s  %-11s  %-62s  %-4s\n", al.getId(),al.getAl_id(),al.getAl_iatacode(),al.getAl_icaocode(),al.getAl_name(),al.getAl_ccode());
            }
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public static void addRecord() {
		Scanner sc = new Scanner(System.in);
		System.out.printf("\nEnter values for new record");
		System.out.print("\n  al_id: ");
		String al_id = sc.nextLine();
		System.out.print("  al_iatacode: ");
		String al_iatacode = sc.nextLine();
		System.out.print("  al_icaocode: ");
		String al_icaocode = sc.nextLine();
		System.out.print("  al_name: ");
		String al_name = sc.nextLine();
		System.out.print("  al_ccode: ");
		String al_ccode = sc.nextLine();

		Airline al = new Airline(Integer.parseInt(al_id),al_iatacode,al_icaocode,al_name,al_ccode);

		Transaction transaction = null;
		try {
			transaction = session.beginTransaction();
			session.merge(al);
			transaction.commit();
		} finally {
			System.out.println("= Record saved =");
		}
	}

	public static void modifyRecord() {
		Scanner sc = new Scanner(System.in);
		System.out.print("\n Enter id of record to modify: ");
		Integer id = Integer.parseInt(sc.nextLine());
		System.out.printf("\nEnter new values for record id %d",id);
		System.out.print("\n  al_id: ");
		String al_id = sc.nextLine();
		System.out.print("  al_iatacode: ");
		String al_iatacode = sc.nextLine();
		System.out.print("  al_icaocode: ");
		String al_icaocode = sc.nextLine();
		System.out.print("  al_name: ");
		String al_name = sc.nextLine();
		System.out.print("  al_ccode: ");
		String al_ccode = sc.nextLine();

		Airline al = new Airline(Integer.parseInt(al_id),al_iatacode,al_icaocode,al_name,al_ccode);
		al.setId(id);

		Transaction transaction = null;
		try {
			transaction = session.beginTransaction();
			session.merge(al);
			transaction.commit();
		} finally {
			System.out.println("= Record saved =");
		}
	}

	public static void deleteRecord() {
		Scanner sc = new Scanner(System.in);
		System.out.print("\n Enter id of record to delete: ");
		Integer id = Integer.parseInt(sc.nextLine());

		Airline al = session.get(Airline.class, id);

		Transaction transaction = null;
		try {
			transaction = session.beginTransaction();
			session.remove(al);
			transaction.commit();
		} finally {
			System.out.println("= Record deleted =");
		}
	}

    public static void commandLoop() {
		try{
			boolean done = false;
			Scanner sc = new Scanner(System.in);
            System.out.println("Enter command (help, list, add, mod, del, quit)");
			while(!done)
			{
            	System.out.print("\n=> ");
                String cmd = sc.nextLine();
				if (cmd.matches("help"))
			    	System.out.println("Enter command (help, list, add, mod, del, quit)");
                else if (cmd.matches("list"))
                    retrieveRecords();
                else if (cmd.contains("add"))
                    addRecord();
                else if (cmd.contains("mod"))
                    modifyRecord();
                else if (cmd.contains("del"))
                    deleteRecord();
				else if (cmd.matches("quit"))
                    done = true;
                else System.out.println("Command not recognized. Please try again or use 'help' for command list.");
                        }
            }catch(Exception e){
            	e.printStackTrace();
			}
        }

	public static void main(String[] args) {
		openSession();
		commandLoop();
        }

}
