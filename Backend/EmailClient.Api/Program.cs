using EmailClient.Api.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Use PascalCase
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // Handle string enums
        options.JsonSerializerOptions.WriteIndented = true; // Better readability in dev
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register email services with proper lifetime management and interfaces
// Using Singleton to maintain IMAP connection state across requests
builder.Services.AddSingleton<IImapConnectionService, ImapConnectionService>();
builder.Services.AddSingleton<IEmailParsingService, EmailParsingService>();
builder.Services.AddSingleton<IEmailRetrievalService, EmailRetrievalService>();
builder.Services.AddSingleton<IEmailDeletionService, EmailDeletionService>();
builder.Services.AddSingleton<IImapService, ImapService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
