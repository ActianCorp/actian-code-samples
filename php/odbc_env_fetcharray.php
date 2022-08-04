<?php
## NOTE uses extension "php_odbc", this is *often* included by default but does need enabling.
## For instance, the Windows binary downloads include it. php.ini needs editing to enable
## "php_odbc" and set "extension_dir"

ini_set('display_errors', 'On');
error_reporting(E_ALL | E_STRICT);

# Samples
$default_connection_string = 'Driver={Ingres};Server=vnodename;Database=iidbdb;ServerType=ingres';  # NOTE default Ingres driver with remote server using vnodename and custom server class
$default_connection_string = 'Driver={Ingres};Server=@HOSTNAME,II;Database=alias;ServerType=mssql';  # NOTE default Ingres driver with remote server using vnodename and custom server class (e.g. to access Enterprise Access gateways or custom server class)
$default_connection_string = 'Driver={Ingres X2};Server=(local);Database=iidbdb';  # NOTE explict installation ID in name
$default_connection_string = 'Driver={Ingres X2};Server=(local);Database=iidbdb;selectloops=N';  # NOTE SELECT LOOPS enabled
$default_connection_string = 'Driver={Actian};Server=(local);Database=iidbdb';  # NOTE default Actian driver with local database

# NOTE second parameter to getenv() requires `php --version` 7.0.9+
$connection_string = getenv('PHPODBC_DSN',  true) ?: getenv('PHPODBC_DSN') ?: $default_connection_string;
#print $connection_string;
#print "<br />\n";
$user = getenv('PHPODBC_USER',  true) ?: getenv('PHPODBC_USER') ?: '';  # For local connection with Operating System authentication leave username and password blank/empty string
$password = getenv('PHPODBC_PASSWORD',  true) ?: getenv('PHPODBC_PASSWORD') ?: '';


$conn = odbc_connect($connection_string,$user,$password);

if ($conn) {
        #echo "Connection succeeded.\n";
        $rc = odbc_exec($conn, "select cap_capability, cap_value from iidbcapabilities");
        if (is_resource($rc)) {
                #echo "Query succeeded.";
                #print "Query succeeded.\n";
                while ($row = odbc_fetch_array($rc)) {
                        echo $row["cap_capability"];
                        #echo "row";
                        print "<br />\n";
                }
        } else {
                echo "Query failed.";
        }
    odbc_close($conn);
} else {
    echo "Connection failed.";
}


?>
