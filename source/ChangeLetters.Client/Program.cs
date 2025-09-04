using Blazored.Modal;
using ChangeLetters.Client.Connectors;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddBlazoredModal();
builder.Services.AddFluentUIComponents();
builder.Services.AddTransient<IFtpConnectorClient, FtpConnectorClient>();
builder.Services.AddTransient<IVocabularyConnector, VocabularyConnector>();
builder.Services.AddTransient<IConfigurationConnector, ConfigurationConnector>();
builder.Services.AddTransient<ISignalRRenameConnector, SignalRRenameConnector>();
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
await builder.Build().RunAsync();
