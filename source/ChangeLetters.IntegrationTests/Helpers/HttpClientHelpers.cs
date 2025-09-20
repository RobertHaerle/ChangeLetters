namespace ChangeLetters.IntegrationTests.Helpers;
public static class HttpClientHelpers
{
    public static HttpClient GetHttpClient()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://0.0.0.0:18081");
        return httpClient;
    }
}