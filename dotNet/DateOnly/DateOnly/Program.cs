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

IngresCommand cmd = new IngresCommand($"drop if exists date_test", conn);
cmd.ExecuteNonQuery();

cmd.Parameters.Clear();
cmd.CommandText = $"create table date_test(ingres_date ingresdate, ansi_date ansidate, time_stamp timestamp(6) without time zone)";
Console.WriteLine(cmd.CommandText);
cmd.ExecuteNonQuery();

DateOnly dto = new DateOnly(1924, 08, 01);

using (IngresCommand command = new IngresCommand($"insert into date_test values(?, ?, ?)", conn))
{
    var parm1 = new IngresParameter("dtm1", dto);
    var parm2 = new IngresParameter("dtm2", dto);
    var parm3 = new IngresParameter("dtm3", dto);

    parm1.IngresType = IngresType.DateOnly;
    parm2.IngresType = IngresType.DateOnly;
    parm3.IngresType = IngresType.DateOnly;

    command.Parameters.Add(parm1);
    command.Parameters.Add(parm2);
    command.Parameters.Add(parm3);

    Console.WriteLine(command.CommandText + " (" + parm1.Value + ", " + parm2.Value + ", " + parm3.Value + ")");
    command.ExecuteNonQuery();

    command.Parameters.Clear();
    command.CommandText = $"select * from date_test";
    Console.WriteLine();
    Console.WriteLine(command.CommandText);
    IngresDataReader reader = command.ExecuteReader();

    DateOnly idt_date, adt_date, ts_date;

    while (reader.Read())
    {

        // All date/time database data types are mapped to the DateTime .Net Data Type:
        // https://docs.actian.com/ingres/12.0/index.html#page/Connectivity/Data_Types_Mapping.htm
        // Use FromDateTime() method to convert the DateTime values to DateOnly values:

        idt_date = reader.IsDBNull(0) ? DateOnly.MinValue : DateOnly.FromDateTime((DateTime)reader.GetValue(0));
        adt_date = reader.IsDBNull(0) ? DateOnly.MinValue : DateOnly.FromDateTime((DateTime)reader.GetValue(1));
        ts_date = reader.IsDBNull(0) ? DateOnly.MinValue : DateOnly.FromDateTime((DateTime)reader.GetValue(2));

        Console.WriteLine("idt_date: " + idt_date);
        Console.WriteLine("adt_date: " + adt_date);
        Console.WriteLine("ts_date: " + ts_date);
    }
    reader.Close();
    conn.Close();
}