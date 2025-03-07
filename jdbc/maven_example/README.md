## JDBC Example using Maven

Example of using Apache Maven to build a Java/JDBC program.
Configration details in the pom.xml file along with a Maven
build option allow the build process to locate and retrieve
the Actian JDBC driver from the Maven repository.

Note: These instructions are written for the Windows platform.
For Linux, use appropriate settings for the environment variables.

### Requirements

 - JDK 1.8 (or later)
 - Apache Maven
 - Access to an existing Actian database (Ingres, Vector, Data Platform)

See these pages to download and install the latest version of Apache Maven

 - https://maven.apache.org/download.cgi?.
 - https://maven.apache.org/install.html

Verify that your machine is able to access the Actian JDBC driver(s) in the maven repository.

    curl -I https://repo1.maven.org/maven2/com/ingres/jdbc/iijdbc/

### Build the Maven project

Run the following command in the directory containing the `pom.xml` file.

    `mvn compile dependency:copy-dependencies`

After the maven build is finished, the class file and JDBC driver can be found under the `target` directory.

### Set Environment Variables

 - <b>CLASSPATH</b> - Replace TESTDIR in the following example with the directory where the pom.xml file resides

    `set CLASSPATH=.;TESTDIR\target\dependency;TESTDIR\target\deppendency\iijdbc-12.0-4.4.4.jar`

 - <b>DB_URL</b> - Use a valid JDBC connection string

   `set DB_URL=jdbc:ingres://localhost:II7/iidbdb`

### Run the JDBC Program

Change to the directory `target\classes` and run the Java program:

     java dbmsVersion

