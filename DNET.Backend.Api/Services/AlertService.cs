using Models;
using DNET.Backend.Api.DB;
using System.Text.Json;
namespace Services
{
    public class AlertService()
    {

        public IEnumerable<Alert> GetAllAlert() => DB.alertModel;
       
        
        public Alert? GetAlertById(int id)
        {
            return DB.alertModel.Find(alert => alert.Id == id);
        } 


        public Alert Create(Alert alert)
        {
            alert.Id = DB.alertModel.Max(a => a.Id) + 1;
            DB.alertModel.Add(alert);
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
               DB.alertModel.Remove(existedAlert);
               return true; 
           }
           
           return false;
       }
        
        
    }
}
