namespace AppForSEII.API.Models
{
    public class Client
    {
        public Client()
        {
        }

        public Client(string id, string name, string surname, string userName, string email, string direccionFacturacion)
        {
            DireccionFacturacion = direccionFacturacion;
        }

        public string? DireccionFacturacion {get;set;}
    }
}