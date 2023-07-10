# Requires installation of pyodbc package prior to execution.
# If package pyodbc is not found, code will attempt to use package pypyodbc.

import sys
try:
    import pyodbc as odbcpkg
except ModuleNotFoundError:
    import pypyodbc as odbcpkg

if len(sys.argv) < 2:
    print("\nPlease provide connection URL surrounded by double-quotes. e.g.\n")
    print("  python getcap.py \"Driver={Actian II};Server=(local);Database=anydb\"")
    quit()

url = sys.argv[1]
cnxn = odbcpkg.connect(url)
cursor = cnxn.cursor()
cursor.execute("select cap_capability, cap_value from iidbcapabilities")
rs = cursor.fetchall()
for i in range(0,len(rs)):
    print(f"{rs[i][0].rstrip() :<25}","   ", rs[i][1].rstrip())
cursor.close()
cnxn.close()
