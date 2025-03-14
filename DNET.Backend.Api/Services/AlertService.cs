
using DNET.Backend.Api.DB;
using System.Text.Json;
using DNET.Backend.Api.Models;
using DNET.Backend.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DNET.Backend.Api.Services
{
    public class AlertService: IAlertService
    {
        private readonly AlertServiceSettings _alertServiceSettings;
        
        public AlertService(IOptionsSnapshot<AlertServiceSettings> alertServiceSettings)
        {
            _alertServiceSettings = alertServiceSettings.Value;
        }
        
        public IEnumerable<Alert> GetAllAlert() => Db.AlertModel;
       
        
        public Alert? GetAlertById(int id)
        {
            return Db.AlertModel.Find(alert => alert.Id == id);
        } 


        public Alert Create(Alert alert)
        {
            if (Db.AlertModel.Count() >= _alertServiceSettings.MaxAlerts)
            {
                return null; 
            }
            
            alert.Id = Db.AlertModel.Max(a => a.Id) + 1;
            Db.AlertModel.Add(alert);
            return alert;
        }
        
        public Alert? UpdateEntirelyAlert (int id ,Alert updatedAlert)
        {
            var existedAlert = GetAlertById(id);
            if (existedAlert != null)
            {
                existedAlert.Message = updatedAlert.Message;
                existedAlert.LocationId = updatedAlert.LocationId;
                existedAlert.IssuedAt = updatedAlert.IssuedAt;
                return  existedAlert;
            }
            return  null;
        }
        

       public bool DeleteLAlertById(int id)
       {
           var existedAlert = GetAlertById(id);
           
           if (existedAlert != null)
           {
               Db.AlertModel.Remove(existedAlert);
               return true; 
           }
           
           return false;
       }
        
        
    }
}
