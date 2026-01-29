using Microsoft.EntityFrameworkCore;
using People.Application.Abstractions;
using People.Application.Commands.People.CreatePerson;
using People.Application.Commands.People.DeletePerson;
using People.Application.Commands.People.UpdatePerson;
using People.Application.Queries.People.GetPersonById;
using People.Infrastructure.ReadDb;
using People.Infrastructure.ReadDb.Repositories;
using People.Infrastructure.WriteDb;
using People.Infrastructure.WriteDb.Outbox;
using People.Infrastructure.WriteDb.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---- Connection strings (fail-fast) ----
var writeCs = builder.Configuration.GetConnectionString("WriteSqlServer")
    ?? throw new InvalidOperationException("Connection string 'WriteSqlServer' not found.");

var readCs = builder.Configuration.GetConnectionString("ReadMySql")
    ?? throw new InvalidOperationException("Connection string 'ReadMySql' not found.");

// ---- DbContexts ----
builder.Services.AddDbContext<WriteDbContext>(opt =>
    opt.UseSqlServer(writeCs));

builder.Services.AddDbContext<ReadDbContext>(opt =>
    opt.UseMySql(readCs, ServerVersion.AutoDetect(readCs)));

// ---- Repositories ----
builder.Services.AddScoped<IPersonWriteRepository, PersonWriteRepository>();
builder.Services.AddScoped<IPersonReadRepository, PersonReadRepository>();

// ---- Outbox ----
builder.Services.AddScoped<IOutboxWriter, EfOutboxWriter>();

// ---- Handlers ----
builder.Services.AddScoped<CreatePersonHandler>();
builder.Services.AddScoped<GetPersonByIdHandler>();
builder.Services.AddScoped<DeletePersonHandler>();
builder.Services.AddScoped<UpdatePersonHandler>();

var app = builder.Build();

// ---- Middleware pipeline ----
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
