using Flurl;
using System.Reflection;
using System.Diagnostics;
using ChangeLetters.DTOs;
using ChangeLetters.SignalR;
using ChangeLetters.Swagger;
using SignalRSwaggerGen.Enums;
using Microsoft.OpenApi.Models;
using System.Runtime.InteropServices;

namespace ChangeLetters.StartUp;

/// <summary> 
/// Class SwaggerRegistration. 
/// </summary>
public static class SwaggerRegistration
{
    /// <summary>Adds the swagger.</summary>
    /// <param name="services">The services.</param>
    /// <returns>See description.</returns>
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            var runningEnvironment = GetRunningEnvironment();
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            var xmlModel = $"{typeof(FileItem).Assembly.GetName().Name}.xml";
            var xmlModelPath = Path.Combine(AppContext.BaseDirectory, xmlModel);
            var xmlSignalR = $"{typeof(SignalRHubRename).Assembly.GetName().Name}.xml";
            var xmlSignalRPath = Path.Combine(AppContext.BaseDirectory, xmlSignalR);
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

            o.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                        ? $"ChangeLetters version {version} running in docker environment"
                        : $"ChangeLetters service version {version} running {runningEnvironment} on {Environment.MachineName}",
                    Version = "v1",
                    Description = "(C) by Robert Härle",
                });


            o.IncludeXmlComments(xmlModelPath);
            o.IncludeXmlComments(xmlSignalRPath);
            o.IncludeXmlComments(xmlPath, true);
            o.DocumentFilter<AddEnumDescriptions>();
            o.AddSignalRSwaggerGen(options =>
            {
                options.ScanAssembly(typeof(SignalRHubRename).Assembly);
                options.AutoDiscover = AutoDiscover.MethodsAndParams;
                options.UseXmlComments(xmlSignalRPath);
                options.UseXmlComments(xmlModelPath);
                options.UseHubXmlCommentsSummaryAsTag = true;
            });
        });

        return services;
    }

    /// <summary>Starts the swagger.</summary>
    /// <param name="app">The application.</param>
    /// <returns>See description.</returns>
    public static IApplicationBuilder StartSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            var pathPrefix = Environment.GetEnvironmentVariable("PATH_PREFIX") ?? "/";
            var urlV1 = new Url(pathPrefix).AppendPathSegment("swagger/v1/swagger.json");
            c.SwaggerEndpoint(urlV1, "Punch Persistence API V1");
            //var urlV2 = new Url(pathPrefix).AppendPathSegment("swagger/v2/swagger.json");
            //c.SwaggerEndpoint(urlV2, "Punch Persistence API V2");
        });

        return app;
    }

    private static string GetRunningEnvironment()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "as a container";
        if (Environment.UserInteractive)
            return "locally";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "as a windows service";
        return "in an unknown state";
    }
}

