using System.Text.Json;
using DNET.Backend.Api.DTOs;

namespace DNET.Backend.Api.Services.Interfaces
{
    public interface ILocationService
    {

        public Task<List<LocationDTO>> GetAllLocations() ;


        public Task<LocationDTO?> GetLocationById(int id);


        public  Task<LocationDTO?> CreateLocation(CreateLocationDTO locationDto);

        public  Task<LocationDTO?> UpdateEntirelyLocation(int id, CreateLocationDTO updatedLocationDto);


        public Task<bool> DeleteLocationById(int id);


    }
}