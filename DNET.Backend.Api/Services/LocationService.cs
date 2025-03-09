using Models;
using DNET.Backend.Api.DB;
using System.Text.Json;
namespace Services
{
    public class LocationService()
    {

        public IEnumerable<Location> GetAllLocations() => DB.locationModel;
       
        
        public Location? GetLocationById(int id)
        {
            return DB.locationModel.Find(location => location.Id == id);
        } 


        public Location Create(Location location)
        {
            location.Id = DB.locationModel.Max(a => a.Id) + 1;
            DB.locationModel.Add(location);
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
                DB.locationModel.Remove(existedLocation);
                return true; 
            }
           
            return false;
        }
        
        
    }
}