package main

import (
	"database/sql"
	"flag"
	"fmt"
	"os"

	_ "github.com/alexbrainman/odbc"
)

func main() {
	connString := flag.String("connstr", "", "Please provide the connection string \"Driver={DriverName};server=@SERVERNAME,tcp_ip,INSTANCE ID;uid=USERNAME;pwd=PASSWORD;database=DATABASE;\"")
	flag.Parse()
	if (*connString == "") {
		flag.PrintDefaults()
		os.Exit(-1)
	}
	db, err := sql.Open("odbc", *connString)
	if err != nil {
		fmt.Println("Error sql.Open ->", err)
		return
	}
	defer db.Close()
	err = db.Ping()
	if err != nil {
		fmt.Println("db.Ping error -> ", err)
		return
	}
	rows, err := db.Query("select cap_capability, cap_value from iidbcapabilities")
	if err != nil {
		fmt.Println("db.Query error -> ", err)
		return
	}
	defer rows.Close()
	for rows.Next() {
		var cap string
		var capval string 
		if err := rows.Scan(&cap, &capval); err != nil {
			fmt.Println("rows.Scan error -> ", err)
		}
		fmt.Println("cap = ", cap, " capval = ", capval)
	}
}
