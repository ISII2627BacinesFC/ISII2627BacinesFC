using AppForSEII.API.Models;
namespace AppForSEII.API.DTOs.Pieza3DDTOs
{
    public class Pieza3DForEncargoDTO
    {
        public Pieza3DForEncargoDTO(int id, string nombre, CategoriaPieza categoria, decimal peso, string MaterialPorDefecto, decimal precioAproximado)
        {
            Id = id;
            Nombre = nombre;
            Categoria = categoria;
            Peso = peso;
            this.MaterialPorDefecto = MaterialPorDefecto;
            PrecioAproximado = precioAproximado;
        }
        public int Id { get; set;}
        public string Nombre { get; set;}
        public CategoriaPieza Categoria { get; set;}
        public decimal Peso { get; set;}
        public string MaterialPorDefecto { get; set;}
        public decimal PrecioAproximado { get; set;}

        public override bool Equals (object? obj)
        {
            return obj is Pieza3DForEncargoDTO dTO &&
                Id == dTO.Id &&
                Nombre == dTO.Nombre &&
                Categoria == dTO.Categoria &&
                Peso == dTO.Peso &&
                MaterialPorDefecto == dTO.MaterialPorDefecto &&
                PrecioAproximado == dTO.PrecioAproximado;
            
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Nombre, Categoria, Peso, MaterialPorDefecto, PrecioAproximado);
        }
    }
}