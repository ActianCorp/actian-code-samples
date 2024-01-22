### Example SQLAlchemy ORM program using the Ingres dialect
#### Operations
- Creates table "Employees" (drops/recreates if exists)
- Adds rows to the table
- Updates a row
- Deletes a row
- Queries and prints the rows after each operation.

Environment variables:
- SQLALCHEMY_INGRES_ODBC_DRIVER_NAME - Name of Ingres ODBC driver _(required)_
- DEMO_DB - Name of database to use _(required)_
- DEMO_ID - Userid for connecting _(optional)_
- DEMO_PW - Password for connecting _(optional)_
  _Note: DEMO_ID and DEMO_PW will only be used if both are set._

Code is partly modeled after the tutorial in this video by NeuralNine
  [SQLAlchemy Turns Python Objects Into Database Entries](https://www.youtube.com/watch?v=AKQ3XEDI9Mw)
