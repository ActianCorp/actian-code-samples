#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>     
#include <fcntl.h>     
#include <inttypes.h>
#include <stdint.h>

exec sql include sqlca;
exec sql include sqlda;

#define dbg(...) do { 													\
					fprintf(stderr, "[DBG ] : ");						\
					fprintf(stderr, "\n********************** \n");	\
					fprintf(stderr, __VA_ARGS__);						\
					fprintf(stderr,"\n********************** \n");		\
					fprintf(stderr, "\n");								\
				} while(0)


static void setup_demo_dbproc(void)
{
	EXEC SQL BEGIN DECLARE SECTION;
        char drop_procedure [] = {"drop procedure dbproc1"};
        char drop_table [] = {"drop table table1"};
        char remove_procedure[] = {"remove procedure dbproc1"};
            
        char create_proc_cmd[] = {
                                    "create or replace procedure dbproc1(a1 in char) "
                                    "as begin "
                                    "insert into table1 values(a1);"
                                    "end;"
                                 };
	EXEC SQL END DECLARE SECTION;

    EXEC SQL WHENEVER SQLERROR CONTINUE;
    EXEC SQL EXECUTE IMMEDIATE :remove_procedure;
    EXEC SQL EXECUTE IMMEDIATE :drop_table;
    EXEC SQL DIRECT EXECUTE IMMEDIATE :drop_procedure;
    EXEC SQL WHENEVER SQLERROR STOP;
    EXEC SQL CREATE TABLE table1 (col1 char(100));
    EXEC SQL DIRECT EXECUTE IMMEDIATE :create_proc_cmd;
    EXEC SQL COMMIT;
}

int main(int argc, char **argv)
{
	EXEC SQL BEGIN DECLARE SECTION;
		char *dbname = NULL;
	EXEC SQL END DECLARE SECTION;

    if (argc < 1 || argv[0] == NULL) {
        fprintf(stderr, "Connection String not provided!! Please provide connection string as argument");
        return -1;
    }
    dbname = argv[1];
	EXEC SQL WHENEVER SQLERROR STOP;
	dbg("connecting to %s...\n", dbname);
	EXEC SQL CONNECT :dbname;

    setup_demo_dbproc();

	EXEC SQL DISCONNECT;
}
