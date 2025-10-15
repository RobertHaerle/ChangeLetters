using DryIoc;
using FluentFTP;
using OpenAI.Chat;
using FluentFTP.Logging;
using ChangeLetters.StartUp;
using ChangeLetters.Database;
using ChangeLetters.Domain.Configurations;
using ChangeLetters.Application.Http.Controllers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;

namespace ChangeLetters.Tests.Server.BootUp;

public class IocTests
{
    private  IContainer _container;

    [SetUp]
    public void Initialize()
    {
        var configuration = Substitute.For<IConfiguration>();
        _container = DryIocRegistration.GetContainer();

        _container.RegisterLogging();
        _container.Use(configuration);
        _container.Register<DatabaseContext>();
        _container.Use(Substitute.For<ChatClient>());
        _container.Use(Substitute.For<OpenAiSettings>());
        _container.Use(Substitute.For<AsyncFtpClient>());
        _container.Use(Substitute.For<DatabaseConfiguration>());
        _container.Use(Substitute.For<IDataProtectionProvider>());
        _container.Use(new FtpLogAdapter(Substitute.For<ILogger<FtpLogAdapter>>()));
        _container.Use(Substitute.For<IHubContext<SignalR.SignalRHubRename, SignalR.ISignalRHubRename>>());

        DryIocRegistration.Initialize(null!, _container);
    }

    [TearDown]
    public void TearDown()
        => _container.Dispose();

    [Test]
    public void TestAttributedIoc()
    {
        // act
        var errors = _container.Validate();

        // assert
        Assert.That(errors, Is.Empty);
    }

    [Test]
    public void TestConfigurationController()
    {
        _container.Register<ConfigurationController>();

        var errors = _container.Validate();

        Assert.That(errors, Is.Empty);
    }

    [Test]
    public void TestFtpController()
    {
        _container.Register<FtpController>();

        var errors = _container.Validate();

        Assert.That(errors, Is.Empty);
    }

    [Test]
    public void TestVocabularyController()
    {
        _container.Register<VocabularyController>();

        var errors = _container.Validate();

        Assert.That(errors, Is.Empty);
    }
}

public static class LoggerFactoryExtensions
{
    public static ILogger<T> CreateLogger<T>(this LoggerFactory f) { return Substitute.For<ILogger<T>>(); }

    public static IContainer RegisterLogging(this IContainer container)
    {
        container.Use(new LoggerFactory());
        container.Register(typeof(ILogger<>), made: Made.Of(
            req => typeof(LoggerFactoryExtensions).GetMethod("CreateLogger")!.MakeGenericMethod(req.Parent.ImplementationType)));
        return container;
    }
}
