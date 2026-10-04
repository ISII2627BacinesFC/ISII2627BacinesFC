namespace AppForSEII.API.Models
{
    public class Pieza3D
    {
        public int Id {get;set;}
        public string Nombre {get;set;} = string.Empty;
        public string Descripcion {get;set;} = string.Empty;
        public decimal PesoGramos {get;set;}
        public int TiempoEstimadoMinutos {get;set;}

        // Enum de categoría
        public CategoriaPieza Categoria {get;set;}

        //Relacion con material
        public int MaterialId {get;set;}
        public Material Material {get;set;} = null!;

    }      
}
