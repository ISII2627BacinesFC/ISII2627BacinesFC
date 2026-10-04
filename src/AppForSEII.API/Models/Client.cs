using AppForSEII.API.Models.ComprarAccesorios;

namespace AppForSEII.API.Models
{
    public class Client:ApplicationUser
    {
        public Client()
        {
        }

        public Client(string id, string name, string surname, string userName, string direccionFacturacion):base(id, name, surname, userName)
        {
            DireccionFacturacion = direccionFacturacion;
        }

        public string? DireccionFacturacion {get;set;}

        public IList<CompraAccesorios> ComprasAccesorios { get; set; } = new List<CompraAccesorios>();
    }
}