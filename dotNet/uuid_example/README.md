### Example .NET c# program inserting and reading a uuid value using the ADO.NET provider for Actian.
#### Preparation
Environment variables:
- DOTNET_CONNECTION_STRING - Connection String used to connect to the database _(required)_
e.g: DOTNET_CONNECTION_STRING=Server=myserver;Port=II7;Database=mydb;User ID=ingres;Password=ca-ingres

### In Visual Studio

To run the test application in Visual Studio 2022 start from a Command Prompt: 

    set DOTNET_CONNECTION_STRING=...
    start uuid_example.sln
