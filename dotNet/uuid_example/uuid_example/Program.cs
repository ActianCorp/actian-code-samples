using Ingres.Client;

var ConnectionString = Environment.GetEnvironmentVariable("DOTNET_CONNECTION_STRING");
if (ConnectionString == null || ConnectionString == string.Empty)
{
    // Sample connection string:
    // ConnectionString = "Server=myserver;Port=II7;Database=mydb;User ID=ingres;Password=ca-ingres";

    Console.WriteLine("Please provide a valid connection string via the DOTNET_CONNECTION_STRING environment variable!");
    Console.WriteLine("DOTNET_CONNECTION_STRING=Server=myserver;Port=II7;Database=mydb;User ID=ingres;Password=ca-ingres");
    Environment.Exit(0);
}
var conn = new IngresConnection(ConnectionString);
try
{
    conn.Open();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    return;
}

IngresCommand cmd = new IngresCommand($"drop if exists uuid_test", conn);
cmd.ExecuteNonQuery();

cmd.Parameters.Clear();
cmd.CommandText = $"create table uuid_test(uuid_string varchar(36), uuid_value uuid)";
Console.WriteLine(cmd.CommandText);
cmd.ExecuteNonQuery();

Guid uuid = new Guid("E0918AE3-9847-46C4-9204-720AB2FA0758");
using (IngresCommand command = new IngresCommand($"insert into uuid_test values (?, ?)", conn))
{
    var uuid_string = new IngresParameter("uuid_string", uuid.ToString());
    var uuid_value = new IngresParameter("uuid", uuid);
    uuid_string.IngresType = IngresType.VarChar;
    uuid_value.IngresType = IngresType.UUID;
    command.Parameters.Add(uuid_string);
    command.Parameters.Add(uuid_value);
    Console.WriteLine(command.CommandText + " (" + uuid_string.Value + ", " + uuid_value.Value + ")");
    command.ExecuteNonQuery();

    command.Parameters.Clear();
    command.CommandText = $"select * from uuid_test";
    Console.WriteLine();
    Console.WriteLine(command.CommandText);
    IngresDataReader reader = command.ExecuteReader();

    Guid guid;
    String str;
    while (reader.Read())
    {
        str = reader.IsDBNull(0) ? "<none>" : reader.GetString(0);
        guid = reader.IsDBNull(0) ? Guid.Empty : reader.GetGuid(1);

        Console.WriteLine("string value: " + str);
        Console.WriteLine("guid value: " + guid);
    }
    reader.Close();
    conn.Close();
}