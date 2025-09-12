using Blazored.Modal;
using ChangeLetters.Client.Connectors;
using ChangeLetters.Client.HubConnection;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddBlazoredModal();
builder.Services.AddFluentUIComponents();
builder.Services.AddTransient<IFtpConnectorClient, FtpConnectorClient>();
builder.Services.AddTransient<IVocabularyConnector, VocabularyConnector>();
builder.Services.AddTransient<IHubConnectionFactory, HubConnectionFactory>();
builder.Services.AddTransient<IConfigurationConnector, ConfigurationConnector>();
builder.Services.AddTransient<ISignalRRenameConnector, SignalRRenameConnector>();
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
await builder.Build().RunAsync();
