## Todo REST API with ASP.NET Core Minimal APIs
Todo REST API samples using ASP.NET Core minimal APIs
# also check https://github.com/davidfowl/CommunityStandUpMinimalAPI
# got from https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0&tabs=visual-studio-code#update-the-generated-code
# to add the test project: >dotnet new xunit - o TodoApi.Tests

## How to create dbcontex config files
1. Create new project in visual studio
2. Go to menu Tools->Nuget Package Manager->Package Manager Console
3. run one command at a time
> PM>Install-Package Microsoft.EntityFrameworkCore.Tools
> Scaffold-DbContext 'Data Source=myserver;User Id=ss;Password=111111;Initial Catalog=mydb;encrypt=false' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Database\Models -ContextDir Database -Schemas dbo -Force
This will generate **Database** folder.
4. in Database folder we have the context file and inside there is OnModelCreating method and from here we can copy from here what we need for the config file
PS: this command also generated the databse models
5. links
> https://learn.microsoft.com/en-us/ef/core/modeling

> https://www.entityframeworktutorial.net/efcore/create-model-for-existing-database-in-ef-core.aspx


## see sql behind ef core (api)
1. in startup:
               > var loggerFactory = LoggerFactory.Create(builder =>
                > {
                  >  builder
                   >     .AddConsole((options) => { })
                    >    .AddFilter((category, level) =>
                     >       category == DbLoggerCategory.Database.Command.Name&& level == LogLevel.Information);
                > }); 
> builder.UseSqlServer(..)
> .EnableSensitiveDataLogging()                      
> .UseLoggerFactory(loggerFactory); 
2. it will be shown in OUTPUT: api
