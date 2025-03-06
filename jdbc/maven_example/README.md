## JDBC Example using Maven

Example of using Maven to build a Java/JDBC program.
Configration details in the pom.xml file allow the build process
to automatically retrieve the Actian JDBC driver from the 
Maven repository.

Note: These instructions are written for the Windows platform.
For Linux, use appropriate settings for the environment variables.

### Requirements

 - JDK 1.8 (or later) installed
 - Apache Maven installed
 - Access to an existing Actian database (Ingres, Vector, Data Platform)

If needed, see these pages to download and install the latest version of Apache Maven

 - https://maven.apache.org/download.cgi?.
 - https://maven.apache.org/install.html

### Set and Check Configuration

Ensure that Maven is accessible by running: `mvn -version`

Edit the `pom.xml` file to specify the JDBC driver version to be retrieved from the Maven repository.

    <dependencies>
      <dependency>
        <groupId>com.ingres.jdbc</groupId>
        <artifactId>iijdbc</artifactId>
        <version>12.0-4.4.4</version>

To see which Actian JDBC driver versions are available on Maven, go to:
https://repo1.maven.org/maven2/com/ingres/jdbc/iijdbc/

If needed, run the following command to verify that your machine is able to access the maven site.

    curl -I https://repo1.maven.org/maven2/com/ingres/jdbc/iijdbc/

### Build the Maven project

    `mvn compile dependency:copy-dependencies`

After the maven build is finished, the class file and JDBC driver can be found under the `target` directory.

### Run the JDBC program:

 - Set the CLASSPATH environment variable
   (In the show example, replace TESTDIR with the directory where the pom.xml file exists)

    `set CLASSPATH=.;TESTDIR\target\dependency;TESTDIR\target\deppendency\iijdbc-12.0-4.4.4.jar`

 - Set the environment variable DB_URL with an appropriate JDBC connection string for connecting to the database.

   `set DB_URL=jdbc:ingres://localhost:II7/iidbdb`

Change to the directory target\classes and run the Java program:

     java dbmsVersion

