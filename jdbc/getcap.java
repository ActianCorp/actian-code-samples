// Requires the following environment variables to be set before execution
//   JDBC_URL - Example: jdbc:ingres://mymachine.mydomain.com:12345/mydb
//   JDBC_UID - Userid for connecting to database
//   JDBC_PWD - Password for connecting to database with given userid

import java.sql.*;

public class getcap
{
    public static void main(String argvs[])
    {
        Connection connection = null;
        String qry = null;
        Statement stmt = null;
        ResultSet rSet = null;
        
        try
        {
		String url = System.getenv("JDBC_URL");
	        String uid = System.getenv("JDBC_UID");
		String pwd = System.getenv("JDBC_PWD");

		if (url != null) {
			connection = DriverManager.getConnection(url, uid, pwd);
			System.out.println("Connected successfully\n");
			qry = "select cap_capability, cap_value from iidbcapabilities";
			stmt = connection.createStatement();
			rSet = stmt.executeQuery(qry);
			while(rSet.next() )
			{
				String capability = rSet.getString("cap_capability");
				String capabvalue = rSet.getString("cap_value");
				System.out.print(String.format("%30s", capability) + "   " + String.format("%30s", capabvalue) + "\n");
			}
		} else {
			System.out.println("Connection URL is empty. Please set environment variable JDBC_URL. Exiting...");
		}
        }
        catch(SQLException sqlex)
        {
            System.out.println("Error Code: " + sqlex.getErrorCode());
            System.out.println("SQL State: " + sqlex.getSQLState());
            System.out.println("Error Messsage: " + sqlex.getMessage());
        }
        catch(Exception ex)
        {
            System.out.println("Error Messsage: " + ex.getMessage());
        }
        finally
        {
            try
            {
                rSet.close();
                stmt.close();
                connection.close();
            }
            catch(SQLException sqlex)
            {
                System.out.println("Error Code: " + sqlex.getErrorCode());
                System.out.println("SQL State: " + sqlex.getSQLState());
                System.out.println("Error Messsage: " + sqlex.getMessage());
            }
        }
    }
}
