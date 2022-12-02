import java.sql.*;

public class iidbdb
{
    static final String DRIVER = "com.ingres.jdbc.IngresDriver";
    static final String CONNST = "jdbc:ingres://SERVER:PORT/iidbdb;UID=USERNAME;PWD=PASSWORD";

    public static void main(String argvs[])
    {
        Connection connection = null;
        String qry = null;
        Statement stmt = null;
        ResultSet rSet = null;
        
        try
        {
            Class.forName(DRIVER);
            connection = DriverManager.getConnection(CONNST);

            qry = "select database_name from iidatabase_info";

            stmt = connection.createStatement();
            rSet = stmt.executeQuery(qry);
            System.out.println("\nDatabases:\n");
            while(rSet.next() )
            {
                String dbname = rSet.getString("database_name");
                System.out.print("   " + dbname + "\n");
            }
        }
        catch(SQLException sqlExpn)   // JDBC errors
        {
            sqlExpn.printStackTrace();
        }
        catch(Exception expn)   // Class.forName errors
        {
            expn.printStackTrace();
        }
        finally
        {
            try
            {
                rSet.close();
                stmt.close();
                connection.close();
            }
            catch(SQLException sqlExpn)
            {
                sqlExpn.printStackTrace();
            }
        }
    }
}
