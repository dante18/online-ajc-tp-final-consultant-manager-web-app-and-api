namespace ConsultTechApp.Tests;

/// <summary>
/// Faux HttpMessageHandler pour intercepter les appels HTTP
/// et retourner une réponse prédéfinie sans appeler un vrai serveur.
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;

    /// <summary>
    /// Permet de vérifier après coup la requête envoyée.
    /// </summary>
    public HttpRequestMessage? LastRequest { get; private set; }

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
        _response = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        return Task.FromResult(_response);
    }
}