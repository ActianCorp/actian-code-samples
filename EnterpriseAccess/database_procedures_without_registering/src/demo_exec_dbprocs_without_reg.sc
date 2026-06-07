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
				

/*
 * No registration of the procedure with EA is performed here.
 */
static void demo_dbproc_with_out_reg(void)
{
	EXEC SQL BEGIN DECLARE SECTION;
        char enable_procedures_without_registration[] = {"set NOREGPROC"};
        char disable_procedures_without_registration[] = {"set REGPROC"};
        char c1[] = {"DBPROC_NO_REG"};
    EXEC SQL END DECLARE SECTION;
    EXEC SQL WHENEVER SQLERROR STOP;
    EXEC SQL EXECUTE IMMEDIATE :enable_procedures_without_registration;
    EXEC SQL EXECUTE PROCEDURE dbproc1(a1=:c1);
    EXEC SQL EXECUTE IMMEDIATE :disable_procedures_without_registration;

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

    demo_dbproc_with_out_reg();

	EXEC SQL DISCONNECT;
}
