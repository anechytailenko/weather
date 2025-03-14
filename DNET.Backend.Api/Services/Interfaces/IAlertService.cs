
using DNET.Backend.Api.DB;
using System.Text.Json;
using DNET.Backend.Api.Models;

namespace DNET.Backend.Api.Services.Interfaces
{
    public interface IAlertService
    
    {
        public IEnumerable<Alert> GetAllAlert();
        
        public Alert? GetAlertById(int id);
        
        public Alert Create(Alert alert);

        public Alert? UpdateEntirelyAlert(int id, Alert updatedAlert);
        
        public bool DeleteLAlertById(int id);
        
    }
}