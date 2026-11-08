
namespace AppForSEII.API.DTOs.EncargoImpresionDTOs
{
    public class LineaEncargoImpresionDTO
    {
        public LineaEncargoImpresionDTO(){}

        public LineaEncargoImpresionDTO(int id, string piezanombre, string materialnombre, int cantidad, decimal precioUnitario, decimal subtotal)
        {
            Id = id;
            PiezaNombre = piezanombre;
            MaterialNombre = materialnombre;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            Subtotal = subtotal;
        }

        public int Id { get; set; }
        public string PiezaNombre { get; set; }
        public string MaterialNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is LineaEncargoImpresionDTO dto &&
                Id == dto.Id &&
                PiezaNombre == dto.PiezaNombre &&
                MaterialNombre == dto.MaterialNombre &&
                Cantidad == dto.Cantidad &&
                PrecioUnitario == dto.PrecioUnitario &&
                Subtotal == dto.Subtotal;
        }
    }
}