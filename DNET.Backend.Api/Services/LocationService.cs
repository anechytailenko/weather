
using DNET.Backend.Api.DB;
using System.Text.Json;
using DNET.Backend.Api.Models;
using DNET.Backend.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DNET.Backend.Api.Services
{
    public class LocationService: ILocationService
    {
        private readonly LocationServiceSettings _locationServiceSettings;
        
        public LocationService(IOptionsSnapshot<LocationServiceSettings> locationServiceSettings)
        {
            _locationServiceSettings = locationServiceSettings.Value;
        }
        
        
        public IEnumerable<Location> GetAllLocations() => Db.LocationModel;
       
        
        public Location? GetLocationById(int id)
        {
            return Db.LocationModel.Find(location => location.Id == id);
        } 


        public Location Create(Location location)
        {
            if (Db.LocationModel.Count() >= _locationServiceSettings.MaxLocations)
            {
                return null; 
            }
            
            location.Id = Db.LocationModel.Max(a => a.Id) + 1;
            Db.LocationModel.Add(location);
            return location;
        }
        
        public Location? UpdateEntirelyLocation (int id ,Location updatedLocation)
        {
            var existedLocation = GetLocationById(id);
            if (existedLocation != null)
            {
                existedLocation.City = updatedLocation.City;
                existedLocation.Country = updatedLocation.Country;
            
                return  existedLocation;
            }
            return  null;
        }
        

        public bool DeleteLLocationById(int id)
        {
            var existedLocation = GetLocationById(id);
           
            if (existedLocation != null)
            {
                Db.LocationModel.Remove(existedLocation);
                return true; 
            }
           
            return false;
        }
        
        
    }
}