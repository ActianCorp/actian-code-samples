## Example JDBC program that calls a database procedure and receives results

### Steps to set up environment and run example

(1) Open an Ingres command prompt  

(2) Create a new database  

e.g.  

    createdb airdb

(3) Create and populate database table using Ingres Terminal Monitor  

    sql airdb < create_airline_insert_data.sql

(4) Create database procedure using Ingres Terminal Monitor  

    sql airdb < create_db_procedure.sql

(5) Ensure the Java environment is configured and compile the Java program  

    javac getAirlines.java

(6) Set environment variables used by the Java program to connect to the database  

e.g.  
 
    JDBC_UID=testuid
    JDBC_PWD=testpwd
    JDBC_URL=jdbc:ingres://localhost:II7/airdb

Alternate example URL:  

    JDBC_URL=jdbc:ingres://testmachine01.testdomain.com:VW7/airdb
   
(7) Run the Java program with no parameters to see country codes  

    java getAirlines

(8) Run the Java program, passing a valid country code  

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
