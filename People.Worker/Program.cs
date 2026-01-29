using Microsoft.EntityFrameworkCore;
using People.Infrastructure.ReadDb;
using People.Infrastructure.WriteDb;
using People.Worker.Options;
using People.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

// ---- Options ----
builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection("Outbox"));

// ---- Connection strings (fail-fast) ----
var writeCs = builder.Configuration.GetConnectionString("WriteSqlServer")
    ?? throw new InvalidOperationException("Connection string 'WriteSqlServer' not found.");

var readCs = builder.Configuration.GetConnectionString("ReadMySql")
    ?? throw new InvalidOperationException("Connection string 'ReadMySql' not found.");

// ---- DbContexts ----
builder.Services.AddDbContext<WriteDbContext>(opt => opt.UseSqlServer(writeCs));
builder.Services.AddDbContext<ReadDbContext>(opt => opt.UseMySql(readCs, ServerVersion.AutoDetect(readCs)));

// ---- Services ----
builder.Services.AddScoped<OutboxProcessor>();
builder.Services.AddHostedService<OutboxProcessorWorker>();

var host = builder.Build();
await host.RunAsync();
