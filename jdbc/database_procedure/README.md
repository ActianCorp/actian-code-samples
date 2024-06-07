## Example JDBC program that calls a database procedure and receives results

### Use existing Actian demonstration database or an alternate database

#### Actian demonstration database

Actian Ingres provides a [demonstration database](https://docs.actian.com/actianx/12.0/index.html#page/QuickStart_Linux/Creating_a_Database.htm#ww306790) called `demodb` that is used with these instructions.

If your Ingres DBMS does not contain the Actian demodb database, it can be created with scripts available with your Ingres installation.

For [Linux](https://docs.actian.com/actianx/12.0/index.html#page/QuickStart_Linux/Requirements_for_the_Demonstration_Application.htm
), the scripts are located in `$II_SYSTEM/ingres/demo/data`  

For [Windows](https://docs.actian.com/actianx/12.0/index.html#page/QuickStart_Win/Requirements_for_the_Demonstration_Application.htm
), the scripts are located in `%II_SYSTEM%\ingres\demo\data`  

Use the script `recreatedemodb.bat` to create the `demodb` database and populate the tables.

#### Optional: Use a different database

If you prefer to use a new or existing database other than `demodb`, then do the following:

- Open an Ingres command prompt
- If creating a new database:  `createdb airdb` _(the database name can be of your choosing)_
- Create and populate the database table used for this example. The Ingres Terminal Monitor script `create_airline_insert_data.sql` is provided alongside these instructions. Note that this script creates only the one table required for the JDBC/procedure demo and does not create the entire `demodb` database as described in the previous steps.  

        sql airdb < create_airline_insert_data.sql


_The remaining instructions assumes that your database name is `demodb`. If you are using a different database, use that name accordingly._

### Remaining steps that apply to any database situation from the above actions

#### Create database procedure using Ingres Terminal Monitor

    sql demodb < create_db_procedure.sql

#### Ensure the Java environment is configured and compile the Java program

    javac getAirlines.java

#### Set environment variables used by the Java program to connect to the database

e.g.

    JDBC_UID=testuid
    JDBC_PWD=testpwd
    JDBC_URL=jdbc:ingres://localhost:II7/demodb

#### Run the Java program with no parameters to see country codes

    java getAirlines

#### Run the Java program, passing a valid country code

    java getAirlines US

### Permissions error attempting to run procedure

If an error occurs in regard to lack of permissions accessing the procedure, you will need to grant appropriate permissions.  

e.g. (using Terminal Monitor)  

    GRANT ALL ON PROCEDURE get_airlines_by_country TO PUBLIC \g

### Configurations that were used for testing the above steps

**Client**

    Microsoft Windows [Version 10.0.19045.4291]
    OpenJDK version 17.0.2

**Database servers**

    Ingres 11.2.0 (a64.win/100) 15807
    Microsoft Windows [Version 10.0.19045.4291]

    Vector 6.3.0 (a64.lnx/146) 14608
    Ubuntu 20.04.6 LTS
