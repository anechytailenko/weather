using System.Text.Json;
using DNET.Backend.Api.DTOs;

namespace DNET.Backend.Api.Services.Interfaces
{
    public interface IAlertService
    
    {
        public Task<List<AlertDTO>> GetAllAlert();
        
        public Task<AlertDTO?> GetAlertById(int id);
        
        public Task<AlertDTO?> CreateAlert(CreateAlertDTO alertDto);

        public Task<AlertDTO?> UpdateEntirelyAlert(int id, CreateAlertDTO updatedAlertDto);
        
        public Task<bool> DeleteAlertById(int id);
        
    }
}