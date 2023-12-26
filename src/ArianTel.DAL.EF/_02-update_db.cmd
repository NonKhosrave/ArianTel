dotnet tool restore
dotnet build
dotnet ef --startup-project ../Mahak.API/ database update --context ApplicationDbContext
pause
