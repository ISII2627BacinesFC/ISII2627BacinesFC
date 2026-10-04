namespace AppForSEII.API.Models
{
    public class Material
    {
        public int ID {get;set;}
        public string Nombre {get;set;} = string.Empty;
        public decimal PrecioPorGramo {get;set;}
        public decimal StockGramos{get;set;}
    }
}