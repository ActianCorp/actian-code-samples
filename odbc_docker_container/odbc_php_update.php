<?php
echo "ODBCINI : ";
echo getenv('ODBCINI');
echo "\n";
echo "\nODBCSYSINI : ";
echo getenv('ODBCSYSINI');
echo "\n";
echo "\nPATH= : ";
echo getenv('PATH');
echo "\n";
echo "\nLD_LIBRARY_PATH : ";
echo getenv('LD_LIBRARY_PATH');
echo "\n";
$conn = odbc_connect("dsn=demodb;database=demodb;", "", "");
if (!$conn) {
	die("Connection failed: " . odbc_errormsg());
}
if($conn) {
	odbc_autocommit($conn,false);
	echo "odbc autocommit status : ";
	echo odbc_autocommit($conn) ? "On" : "Off";
	echo "\n";
	echo ("Executing Update...\n");
	odbc_exec($conn,"update tab set col2=00 where col1=22");
	echo ("Update Submitted...\n");
	sleep(10);
	echo ("Sleep exit...8\n");
}
?>
