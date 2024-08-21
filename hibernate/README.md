## Hibernate ORM 6.x Demo for Ingres / Actian X

### Required Tools
 - Apache Maven - https://maven.apache.org/download.cgi?.
 - JDK 11, 17, or 21 - [Oracle OpenJDK Builds](https://jdk.java.net/)
 - Actian Data Platform Client Runtime (with appropriately set CLASSPATH to use the iijdbc.jar)
 - See pom.xml for additional information on tools and versions.

### Example Configuration
     λ mvn -version
     Apache Maven 3.9.5 (57804ffe001d7215b5e7bcb531cf83df38f93546)
     Maven home: C:\Users\mhabiger\Tools\ApacheMaven\apache-maven-3.9.5
     Java version: 17.0.2, vendor: Oracle Corporation, runtime: C:\program
     files\microsoft\jdk-17.0.2
     Default locale: en_US, platform encoding: Cp1252
     OS name: "windows 10", version: "10.0", arch: "amd64", family: "windows"

### The demo consists of two Java programs and a shared utility class:
 - **InitData.java** - Creates airline table and populates the table with sample data.
 - **App.java** - Simple menu-driven program to list, add, modify, and delete records.
 - **Util.java** - Creates the session, connects to the database, performs shutdown.

### Helpful Commands used for Building and Running the Demo
 - **mvn clean** - Purge generated files.
 - **mvnCompile.bat** - Compile source code. Auto-download dependency jar files.
 - **InitData.bat** - (Re)Create table and populate with data.
 - **runApp.bat** - Execute the interactive program to manage data.

### Connection Properties
There are several connection properties in the file **hibernate.cfg.xml** that need to be updated with appropriate values.  
e.g.  

     <property name="connection.driver_class">com.ingres.jdbc.IngresDriver</property>
     <property name="connection.url">jdbc:ingres://localhost:II7/traveldb</property>
     <property name="connection.username">testuser</property>
     <property name="connection.password">testpass</property>

### Source Code
The base source code used for this project is from [JavaGuides.net](https://www.javaguides.net/2023/03/hibernate-6-example-tutorial.html).
Additional code used was found from posts on various sites, including:
 - [StackOverflow.com](https://stackoverflow.com/)
 - [JBoss.org](https://docs.jboss.org/hibernate/orm/6.4/userguide/html_single/Hibernate_User_Guide.html)
 - Examples from other online documentation sites

### Other References
Hibernate Compatibility [Matrix](https://hibernate.org/orm/releases/)

