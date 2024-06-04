// Environment variables required to be set prior to execution
//   JDBC_URL - Example: jdbc:ingres://mymachine.mydomain.com:12345/mydb
//   JDBC_UID - Userid for connecting to database
//   JDBC_PWD - Password for the given userid


import java.sql.*;

public class getAirlines
{
    static Connection connection = null;
    static String qry = null;
    static CallableStatement cstm = null;
    static ResultSet rSet = null;
    static String countryCode = null;

    public static void getConnection() {
        try {
	    String url = System.getenv("JDBC_URL");
	    String uid = System.getenv("JDBC_UID");
            String pwd = System.getenv("JDBC_PWD");

            if (url != null) {
	        connection = DriverManager.getConnection(url, uid, pwd);
		printMsg("Connected successfully\n");
	    } else {
		printMsg("Connection URL is empty. Please set environment variable JDBC_URL.");
		System.exit(1);
	    }
        }
        catch(SQLException sqlex) {
            printMsg("Error Code: " + sqlex.getErrorCode());
            printMsg("SQL State: " + sqlex.getSQLState());
            printMsg("Error Messsage: " + sqlex.getMessage());
        }
    }

    public static void printMsg(String msg) {
	System.out.println(msg);
    }

    public static void getCountryArgument(String[] args) {
        if (args.length > 0) {
            countryCode = args[0];
        } else {
            System.out.println("Missing country code argument (AU CA DE ES FI FR GB IE IN JP KR NL NO RU SE US ZA ZW)");
            System.exit(1);
        }
    }

    public static void callDatabaseProcedure() {
        try {
            qry = "{call get_airlines_by_country(?)}";
            cstm = connection.prepareCall(qry);
            cstm.setString(1, countryCode);
            boolean hasResults = cstm.execute();
            if (hasResults) {
                rSet = cstm.getResultSet();
		if (rSet.next()) {
                    do {
			String airlineName = rSet.getString(1);
			System.out.println(String.format("%-60s", airlineName));
                    } while (rSet.next());
                } else {
                    System.out.println("No match for country code " + countryCode);
                }
            }
        }
        catch(SQLException sqlex) {
            printMsg("Error Code: " + sqlex.getErrorCode());
            printMsg("SQL State: " + sqlex.getSQLState());
            printMsg("Error Messsage: " + sqlex.getMessage());
        }
        catch(Exception ex) {
            printMsg("Error Messsage: " + ex.getMessage());
        }
        finally {
            try {
                rSet.close();
                cstm.close();
                connection.close();
            }
            catch(SQLException sqlex) {
                printMsg("Error Code: " + sqlex.getErrorCode());
                printMsg("SQL State: " + sqlex.getSQLState());
                printMsg("Error Messsage: " + sqlex.getMessage());
            }
        }
    }

    public static void main(String[] args) {
        getCountryArgument(args);
        getConnection();
        callDatabaseProcedure();
    }
}

