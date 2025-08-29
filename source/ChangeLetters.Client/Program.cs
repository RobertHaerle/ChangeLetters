using ChangeLetters.Client.Connectors;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddFluentUIComponents();
builder.Services.AddTransient<IFtpConnectorClient, FtpConnectorClient>();
builder.Services.AddTransient<IVocabularyConnector, VocabularyConnector>();
builder.Services.AddTransient<IConfigurationConnector, ConfigurationConnector>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
await builder.Build().RunAsync();
