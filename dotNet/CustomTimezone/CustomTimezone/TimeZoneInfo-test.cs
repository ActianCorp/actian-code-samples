using CustomTimezone;
using Ingres.Client;
using System.Text;

/*
 *  This application was created to test the use of a custom TimeZoneInfo to get around the problem
 *  with the (UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna Windows time zone missing
 *  historical information regarding DST adjustment rules.
 *  
 *  In order to test this specific problem please set II_TIMEZONE_NAME=EUROPE-CENTRAL for the DBMS
 *  and "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna" as the Time zone for your
 *  the machine where you are running this application from. 
 */

namespace DateTime_DSTtest
{
    class DSTtest
    {
        private DSTtest(string connectionString)
        {
            if (connectionString == string.Empty) 
            {
                Console.WriteLine("Please provide a valid connection string via the DOTNET_TEST_CONNECTIONSTRING environment variable!");
                Environment.Exit(0);
            }
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        TimeZoneInfo custom_timezone = CreateCustomTimeZoneInfoWithDstRules();
        static void Main(string[] args)
        {
            var DSTtest = new DSTtest(Config.ConnectionString);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            DateTime date = new DateTime(1924, 08, 01, 0, 0, 0);
            Console.WriteLine("DateTime date = new DateTime(1924, 08, 01, 0, 0, 0);");
            Console.WriteLine("date: " + date);
            DateTime utc_date = new DateTime(1924, 08, 01, 0, 0, 0, DateTimeKind.Utc);
            Console.WriteLine("date in UTC using DateTimeKind.Utc: " + utc_date);
            var dt = UTCtoCustom(utc_date);
            Console.WriteLine("date after conversion to Custom Timezone: " + dt);
            var selectedTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            Console.WriteLine("\nW. Europe Standard Time Timezone Display Name - TimeZoneInfo.DisplayName: " + selectedTimeZone.DisplayName);
            if (selectedTimeZone.IsDaylightSavingTime(dt))
            {
                Console.WriteLine("DateTime: " + dt + " is DST!");
            }
            else
            {
                Console.WriteLine("DateTime: " + dt + " is not DST!");
            }
 
            var isDstCustom = DSTtest.custom_timezone.IsDaylightSavingTime(dt);
            Console.WriteLine("\nCustom W. Europe Standard Time Timezone Display Name - TimeZoneInfo.DisplayName: " + DSTtest.custom_timezone.DisplayName);
            if (isDstCustom)
            {
                Console.WriteLine("DateTime: " + dt + " is DST in custom TimeZone!\n");
            }
            else
            {
                Console.WriteLine("DateTime: " + dt + " is not DST in custom TimeZone!\n");
            }

            Console.WriteLine("Connection String: '" + DSTtest.ConnectionString + "'\n");
            Ingres.Client.IngresConnection conn = new Ingres.Client.IngresConnection(DSTtest.ConnectionString);

            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            string sql = "drop if exists ingres_date_test";
            IngresCommand cmd = new IngresCommand(sql, conn);
            cmd.ExecuteNonQuery();
            sql = $"create table ingres_date_test(ingres_date ingresdate, ansi_date ansidate, time_stamp_wotz timestamp(6) without time zone, time_stamp_wtz timestamp(6) with time zone, time_stamp_wltz timestamp(6) with local time zone, comments varchar(25))";
            Console.WriteLine("Create table sql command: '" + sql + "'\n");
            Console.WriteLine("*******************************************************************************************************************************");

            cmd.Parameters.Clear();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            var format = "yyyy-MMM-dd HH:mm:ssK";
            DateTime dt1 = new DateTime(1924, 08, 01);
            dt1 = DateTime.SpecifyKind(dt1, DateTimeKind.Local);
            Console.WriteLine("Inserting DateTime value with System time zone: '" + dt1.ToString(format) + "'");
            cmd.Parameters.Clear();
            cmd.CommandText = "insert into ingres_date_test values(?, ?, ?, ?, ?, ?)";
            var parm1 = new IngresParameter("dtm1", dt1);
            var parm2 = new IngresParameter("dtm2", dt1);
            var parm3 = new IngresParameter("dtm3", dt1);
            var parm4 = new IngresParameter("dtm4", dt1);
            var parm5 = new IngresParameter("dtm5", dt1);
            var parm6 = new IngresParameter("dtm6", "system timezone");
            parm1.IngresType = IngresType.DateTime;
            parm2.IngresType = IngresType.Date;
            parm3.IngresType = IngresType.DateTime;
            parm4.IngresType = IngresType.DateTime;
            parm5.IngresType = IngresType.DateTime;
            parm6.IngresType = IngresType.VarChar;

            cmd.Parameters.Add(parm1);
            cmd.Parameters.Add(parm2);
            cmd.Parameters.Add(parm3);
            cmd.Parameters.Add(parm4);
            cmd.Parameters.Add(parm5);
            cmd.Parameters.Add(parm6);
            Console.WriteLine("insert into ingres_date_test values\n(" + cmd.Parameters[0].Value + ", " + cmd.Parameters[1].Value + ", " +
                                cmd.Parameters[2].Value + ", " + cmd.Parameters[3].Value + ", " + cmd.Parameters[4].Value + ", " + cmd.Parameters[5].Value + ")");
            Console.WriteLine("*******************************************************************************************************************************");

            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            //Console.WriteLine("*******************************************************************************************************************************");
            Console.WriteLine("Inserting DateTime value with Custom time zone '" + dt.ToString(format) + "' into an ingresdate column.");
            cmd.CommandText = "insert into ingres_date_test values(?, ?, ?, ?, ?, ?)";
            parm1 = new IngresParameter("dtm1", dt);
            parm2 = new IngresParameter("dtm2", dt);
            parm3 = new IngresParameter("dtm3", dt);
            parm4 = new IngresParameter("dtm4", dt);
            parm5 = new IngresParameter("dtm5", dt);
            parm6 = new IngresParameter("dtm6", "custom timezone");
            parm1.IngresType = IngresType.DateTime;
            parm2.IngresType = IngresType.Date;
            parm3.IngresType = IngresType.DateTime;
            parm4.IngresType = IngresType.DateTime;
            parm5.IngresType = IngresType.DateTime;
            parm6.IngresType = IngresType.VarChar;

            cmd.Parameters.Add(parm1);
            cmd.Parameters.Add(parm2);
            cmd.Parameters.Add(parm3);
            cmd.Parameters.Add(parm4);
            cmd.Parameters.Add(parm5);
            cmd.Parameters.Add(parm6);
            Console.WriteLine("insert into ingres_date_test values\n(" + cmd.Parameters[0].Value + ", " + cmd.Parameters[1].Value + ", " +
                                cmd.Parameters[2].Value + ", " + cmd.Parameters[3].Value + ", " + cmd.Parameters[4].Value + ", " + cmd.Parameters[5].Value + ")");
            Console.WriteLine("*******************************************************************************************************************************\n");

            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            cmd.CommandText = "select * from ingres_date_test";
            IngresDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("select * from ingres_date_test\n");

            string comments;
            DateTime idt_date, adt_date, ts_date, ts_w_tzone_date, ts_w_ltzone_date;

            while (reader.Read())
            {
                idt_date = (DateTime)reader.GetValue(0);
                adt_date = (DateTime)reader.GetValue(1);
                ts_date = (DateTime)reader.GetValue(2);
                ts_w_tzone_date = (DateTime)reader.GetValue(3);
                ts_w_ltzone_date = (DateTime)reader.GetValue(4);
                comments = (string)reader.GetValue(5);

                Console.WriteLine("Returned values that were inserted with " + comments + ":");
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine("ingres_date     DateTime value: " + idt_date.ToString(format));
                if (comments.Contains("custom"))
                {
                    idt_date = CustomToUTC(idt_date);
                    Console.WriteLine("ingres_date UTC DateTime value: " + idt_date.ToString(format));
                    idt_date = UTCtoCustom(idt_date);
                    Console.WriteLine("ingres_date w. Custom Timezone: " + idt_date.ToString(format));
                }

                Console.WriteLine("ansi_date       DateTime value: " + adt_date.ToString(format));
                Console.WriteLine("time_stamp_wotz DateTime value: " + ts_date.ToString(format));
                Console.WriteLine("time_stamp_wtz  DateTime value: " + ts_w_tzone_date.ToString(format));
                Console.WriteLine("time_stamp_wltz DateTime value: " + ts_w_ltzone_date.ToString(format));
                Console.WriteLine("\n");

            }
            reader.Close();

            cmd.Parameters.Clear();
            //cmd.CommandText = "delete from ingres_date_test";
            //cmd.ExecuteNonQuery();

            conn.Close();
        }

        public static TimeZoneInfo CreateCustomTimeZoneInfoWithDstRules()
        {

            // Clock is adjusted one hour forward or backward
            var delta = new TimeSpan(1, 0, 0);
            //This will hold all the DST adjustment rules
            var listOfAdjustments = new List<TimeZoneInfo.AdjustmentRule>();

            /*
                DST Adjustment rule from 4/30/1916 to 10/1/1916:
                Delta: 01:00:00
                Begins at 11:00 PM on April 30
                Ends at 12:59 AM on October 1
            */
            var ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 23, 0, 0), 04, 30);
            var ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 1, 0, 0), 10, 01);
            var adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1916, 1, 1), new DateTime(1916, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 4/16/1917 to 9/17/1917:
                Delta: 01:00:00
                Begins at 2:00 AM on April 16
                Ends at 2:59 AM on September 17
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 04, 16);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 09, 17);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1917, 1, 1), new DateTime(1917, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 4/15/1918 to 9/16/1918:
                Delta: 01:00:00
                Begins at 2:00 AM on April 15
                Ends at 2:59 AM on September 16
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 04, 15);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 09, 16);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1918, 1, 1), new DateTime(1918, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 4/1/1940 to 12/31/1940:
                Delta: 01:00:00
                Begins at 2:00 AM on April 1
                Ends at 11:59 PM on December 31
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 04, 01);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 23, 59, 59), 12, 31);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1940, 1, 1), new DateTime(1940, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 1/1/1941 to 12/31/1941:
                Delta: 01:00:00
                Begins at 12:00 AM on January 1
                Ends at 11:59 PM on December 31
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 0, 0, 0), 01, 01);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 23, 59, 59), 12, 31);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1941, 1, 1), new DateTime(1941, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 1/1/1942 to 11/2/1942:
                Delta: 01:00:00
                Begins at 12:00 AM on January 1
                Ends at 2:59 AM on November 2
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 0, 0, 0), 01, 01);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 11, 02);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1942, 1, 1), new DateTime(1942, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 3/29/1943 to 10/4/1943:
                Delta: 01:00:00
                Begins at 2:00 AM on March 29
                Ends at 2:59 AM on October 4
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 03, 29);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 10, 04);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1943, 1, 1), new DateTime(1943, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 4/3/1944 to 10/2/1944:
                Delta: 01:00:00
                Begins at 2:00 AM on April 3
                Ends at 2:59 AM on October 2
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 04, 03);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 10, 02);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1944, 1, 1), new DateTime(1944, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 4/2/1945 to 5/24/1945:
                Delta: 01:00:00
                Begins at 2:00 AM on April 2
                Ends at 1:59 AM on May 24
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 04, 02);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 05, 24);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1945, 1, 1), new DateTime(1945, 5, 23), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 5/24/1945 to 9/24/1945:
                Delta: 02:00:00
                Begins at 1:00 AM on May 24
                Ends at 2:59 AM on September 24
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 1, 0, 0), 05, 24);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 09, 24);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1945, 5, 24), new DateTime(1945, 9, 23), new TimeSpan(2, 0, 0), ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 9/24/1945 to 11/18/1945:
                Delta: 01:00:00
                Begins at 1:00 AM on September 24
                Ends at 2:59 AM on November 18
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 1, 0, 0), 09, 24);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 11, 18);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1945, 9, 24), new DateTime(1945, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 4/14/1946 to 10/7/1946:
                Delta: 01:00:00
                Begins at 2:00 AM on April 14
                Ends at 2:59 AM on October 7
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 04, 14);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 10, 07);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1946, 1, 1), new DateTime(1946, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 4/6/1947 to 5/11/1947:
                Delta: 01:00:00
                Begins at 3:00 AM on April 6
                Ends at 2:59 AM on May 11
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 04, 06);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 05, 11);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1947, 1, 1), new DateTime(1947, 5, 10), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 5/11/1947 to 6/29/1947:
                Delta: 02:00:00
                Begins at 2:00 AM on May 11
                Ends at 2:59 AM on June 29
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 05, 11);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 06, 29);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1947, 5, 11), new DateTime(1947, 6, 28), new TimeSpan(2, 0, 0), ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 6/29/1947 to 10/5/1947:
                Delta: 01:00:00
                Begins at 1:00 AM on June 29
                Ends at 2:59 AM on October 5
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 1, 0, 0), 06, 29);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 10, 05);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1947, 6, 29), new DateTime(1947, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 4/18/1948 to 10/3/1948:
                Delta: 01:00:00
                Begins at 2:00 AM on April 18
                Ends at 2:59 AM on October 3
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 04, 18);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 10, 03);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1948, 1, 1), new DateTime(1948, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 4/10/1949 to 10/2/1949:
                Delta: 01:00:00
                Begins at 2:00 AM on April 10
                Ends at 2:59 AM on October 2
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 04, 10);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 10, 02);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1949, 1, 1), new DateTime(1949, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 4/6/1980 to 9/28/1980:
                Delta: 01:00:00
                Begins at 2:00 AM on April 6
                Ends at 2:59 AM on September 28
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 04, 06);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 3, 0, 0), 09, 28);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1980, 1, 1), new DateTime(1980, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            /*
                DST Adjustment rule from 3/29/1981 to 9/24/1995:
                Delta: 01:00:00
                Begins at 2:00 AM on March 29
                Ends at 2:59 AM on September 27
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 03, 05, DayOfWeek.Sunday);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 3, 0, 0), 09, 05, DayOfWeek.Sunday);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1995, 1, 1), new DateTime(1995, 12, 31), delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);
            /*
                DST Adjustment rule from 3/29/1996 to 12/31/9999:
                Delta: 01:00:00
                Begins at 2:00 AM on the Last Sunday of March
                Ends at 2:59 AM on the Last Sunday of October
            */
            ruleStart = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 03, 05, DayOfWeek.Sunday);
            ruleEnd = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 3, 0, 0), 10, 05, DayOfWeek.Sunday);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1996, 1, 1), DateTime.MaxValue.Date, delta, ruleStart, ruleEnd);
            listOfAdjustments.Add(adjustment);

            var adjustments = new TimeZoneInfo.AdjustmentRule[listOfAdjustments.Count];
            listOfAdjustments.CopyTo(adjustments);
            return TimeZoneInfo.CreateCustomTimeZone("Custom Europe Central Time", new TimeSpan(+1, 0, 0),
                  "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna", "Europe Central Time", "Europe Central Time", adjustments);
        }

        public static DateTime UTCtoCustom(DateTime dateTime)
        {
            var test = new DSTtest(Config.ConnectionString);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, test.custom_timezone);
        }

        public static DateTime CustomToUTC(DateTime dateTime)
        {
            var test = new DSTtest(Config.ConnectionString);
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, test.custom_timezone);
        }
    }
}
