
using DNET.Backend.Api.DB;
using System.Text.Json;
using DNET.Backend.Api.Models;

namespace DNET.Backend.Api.Services.Interfaces
{
    public interface ILocationService
    {

        public IEnumerable<Location> GetAllLocations() ;


        public Location? GetLocationById(int id);


        public Location Create(Location location);

        public Location? UpdateEntirelyLocation(int id, Location updatedLocation);


        public bool DeleteLLocationById(int id);


    }
}