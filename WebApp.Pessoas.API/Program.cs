using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WebApp.Pessoas.API;
using WebApp.Pessoas.API.Database;
using WebApp.Pessoas.API.Model;

var builder = WebApplication.CreateBuilder(args);

// Disable logging as this is not required for stress test
builder.Logging.ClearProviders();

builder.WebHost.ConfigureKestrel(options =>
{
     options.AllowSynchronousIO = true;
});

var conexao = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

if (conexao is null)
{
    // Load custom configuration
    var appSettings = new AppSettings();
    builder.Configuration.Bind(appSettings);
    conexao = appSettings.ConnectionString;
}

var dataSource = NpgsqlDataSource.Create(conexao);

// Add services to the container.
builder.Services.AddSingleton(dataSource);
builder.Services.AddScoped<Db>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

app.MapGet("/pessoas", (Db db, string? t) => {
    if (t is null)
        return Results.BadRequest(new {error = "A consulta precisa possuir o paramentro 't'. Ex.: /pessoas?t=termo"});
    return Results.Json(db.BuscarPessoas(t));
});

app.MapPost("/pessoas", async (Db db, Pessoa pessoa) =>
{
    var valida = pessoa.Valida();
    if (!valida)
        return Results.UnprocessableEntity(new { Error = "Presta atenção no serviço!!!" });

    var existe = await db.Existe(pessoa.Apelido);
    if (existe)
        return Results.UnprocessableEntity(new { Error = "Presta atenção no serviço!!!" });

    var ok = await db.InserirPessoa(pessoa);
    return ok
        ? TypedResults.Created($"/pessoas/{pessoa.Id}")
        : Results.Problem("Server Error", app.Environment.EnvironmentName, StatusCodes.Status503ServiceUnavailable, "Service Unavailable");
});

app.MapGet("/pessoas/{id}", async (Db db, Guid id) => {
    return await db.BuscarPessoaId(id) is null 
        ? Results.NotFound() 
        : Results.Json(await db.BuscarPessoaId(id));
});

app.MapGet("/contagem-pessoas", async (Db db) => {
    return await db.ContarPessoas();
});

await app.RunAsync();

