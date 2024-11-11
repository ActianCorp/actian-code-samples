using Ingres.Client;

string ConnectionString;

switch (args.Length)
{
    case 0:
        Console.WriteLine("Please provide a valid connection string!");
        Console.WriteLine("Sample Connection String: 'Server=myserver;Port=II7;Database=iidbdb;UserID=ingres;Password=ingres-password'");
        return;
    case 1:
        if (args[0].Equals(@"/?") || args[0].Equals("-help"))
        {
            Console.WriteLine("This application takes one input argument that will be used as a connection string to test the connection to a local or remote database.");
            Console.WriteLine("Sample Connection String: 'Server=myserver;Port=II7;Database=iidbdb;UserID=ingres;Password=ingres-password'");
            return;
        }
        ConnectionString = args[0];
        Console.WriteLine("\nConnection String: '" + ConnectionString + "'");
        string[] attributes = ConnectionString.Split(';');
        string server = "", port = "", database = "", userid = "", password = "";

        foreach (var word in attributes)
        {
            System.Console.WriteLine($"<{word}>");
            if (word.ToLower().Contains("server="))
                server = word;
            if (word.ToLower().Contains("port="))
                port = word;
            if (word.ToLower().Contains("database="))
                database = word;
            if (word.ToLower().Contains("user id=") || word.ToLower().Contains("userid="))
                userid = word;
            if (word.ToLower().Contains("password=") || word.ToLower().Contains("pwd="))
                password = word;
        }
        Console.WriteLine();

        if (server.Length == 0)
        {
            Console.WriteLine("Invalid connection string! Please provide a value for the 'Server' ");
            return;
        }
        if (port.Length == 0)
        {
            Console.WriteLine("Invalid connection string! Please provide a value for the 'Port' ");
            return;
        }
        if (database.Length == 0)
        {
            Console.WriteLine("Invalid connection string! Please provide a value for the 'Database' ");
            return;
        }
        if (userid.Length == 0)
        {
            Console.WriteLine("Invalid connection string! Please provide a value for the 'User ID' ");
            return;
        }
        if (password.Length == 0)
        {
            Console.WriteLine("Invalid connection string! Please provide a value for the 'Password' ");
            return;
        }
        Ingres.Client.IngresConnection conn = new Ingres.Client.IngresConnection(ConnectionString);
        try
        {
            conn.Open();

            string sql = "select USER;";
            IngresCommand cmd = new IngresCommand(sql, conn);
            IngresDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("Query: " + sql);
            GetData(reader, "string");

            cmd.CommandText = "SELECT dbmsinfo('_version')";
            reader = cmd.ExecuteReader();
            Console.WriteLine("Query: " + cmd.CommandText);
            GetData(reader, "string");

            cmd.CommandText = "SELECT IngresDate('now')";
            reader = cmd.ExecuteReader();
            Console.WriteLine("Query: " + cmd.CommandText);
            GetData(reader, "date");

            cmd.CommandText = "SELECT current_timestamp;";
            reader = cmd.ExecuteReader();
            Console.WriteLine("Query: " + cmd.CommandText);
            GetData(reader, "date");

            conn.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        break;
    default:
        Console.WriteLine("Invalid connection string!");
        break;
}

void GetData(IngresDataReader reader, string type)
{
    string value;
    DateTime dateTime = DateTime.MinValue;

    if (type == "date")
    {
        while (reader.Read())
        {
            dateTime = (DateTime)reader.GetValue(0);
            Console.WriteLine("Returned value: " + dateTime.ToString() + "\n");
        }
    }
    else
    {
        while (reader.Read())
        {
            value = (string)reader.GetValue(0);
            Console.WriteLine("Returned value: " + value + "\n");
        }
    }
    reader.Close();
}
