using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.EncargoImpresionDTOs
{
    public class LineaEncargoImpresionCreateDTO
    {
        public LineaEncargoImpresionCreateDTO() { }

        public LineaEncargoImpresionCreateDTO(int piezaId, int materialId, int cantidad)
        {
            PiezaId = piezaId;
            MaterialId = materialId;
            Cantidad = cantidad;
        }

        [Required]
        public int PiezaId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }
    }
}