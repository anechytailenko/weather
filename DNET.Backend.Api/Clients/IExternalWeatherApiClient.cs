using DNET.Backend.Api.Requests;

namespace DNET.Backend.Api.Clients
{
    public interface IExternalWeatherApiClient
    {
        Task<ExternalWeatherResponse> GetWeatherDataAsync(string location);
    }

   
}