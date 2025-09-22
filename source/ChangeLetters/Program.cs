using ChangeLetters.Components;
using ChangeLetters.Connectors;
using ChangeLetters.Database;
using ChangeLetters.DTOs;
using ChangeLetters.SignalR;
using ChangeLetters.StartUp;
using ChangeLetters.SystemMonitoring;
using DryIoc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.FluentUI.AspNetCore.Components;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .UseServiceProviderFactory(DryIocRegistration.GetDryIocFactory())
    .ConfigureContainer<Container>(DryIocRegistration.Initialize);

builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.IncludeScopes = false;
    options.UseUtcTimestamp = true;
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
    options.ColorBehavior = LoggerColorBehavior.Enabled;
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddFluentFtpComponents()
    .AddOpenAI(builder.Configuration)
    .AddDatabase(builder.Configuration)
    .AddDataProtectionComponents(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        options.MinimumSameSitePolicy = SameSiteMode.Lax;
        options.Secure = CookieSecurePolicy.None;
    });
}

builder.Services.AddSignalR();
builder.Services.AddSwagger();
builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck<HealthCheck>("alive");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<AliveService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.StartSwagger();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseExceptionHandler("/Error", createScopeForErrors: true);
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<SignalRHubRename>(SignalRPath.Rename.Path);

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ChangeLetters.Client._Imports).Assembly);

app.InitializeDatabase();
app.Services.AddLifetimeLogging();
app.Run();
