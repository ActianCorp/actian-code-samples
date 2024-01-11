
namespace CustomTimezone
{
    public static class Config
    {
        public static string ConnectionString => Coalesce(
            Environment.GetEnvironmentVariable("DOTNET_TEST_CONNECTIONSTRING") ?? string.Empty,
            "Server=MIANCULOVICI-P1;Port=II7;Database=mydb;User ID=ingres;Password=ca-ingres;TZ=EUROPE-CENTRAL"//SendIngresDates=true"
        );

        private static string Coalesce(params string[] strings)
        {
            foreach (var str in strings)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    return str;
                }
            }
            return string.Empty;
        }
    }
}
