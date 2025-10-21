# Add Migrations since ConsultTechApp.Core
dotnet ef migrations add InitDatabase --context ApplicationStoreContext --startup-project ../ConsultTechApp.Api

# Update database since ConsultTechApp.Core
dotnet ef database update --startup-project ../ConsultTechApp.Api --context ApplicationStoreContext