using Librarian.Sephirah.Server;

var builder = WebApplication.CreateBuilder(args);

StartUp.ConfigureServices(builder);

var app = builder.Build();

StartUp.Configure(app);

app.Run();
