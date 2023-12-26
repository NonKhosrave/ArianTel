dotnet build
dotnet ef migrations --startup-project ../Mahak.API/ remove --context ApplicationDbContext --force
pause
