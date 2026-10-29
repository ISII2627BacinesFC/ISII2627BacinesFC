namespace AppForSEII.API.Models
{
    public class PiezaMaterial
    {
        public PiezaMaterial() {}

        public PiezaMaterial(int piezaId, int materialId)
        {
            PiezaId = piezaId;
            MaterialId = materialId;
        }

        public PiezaMaterial(Pieza3D pieza, Material material)
        {
            Pieza = pieza;
            PiezaId = pieza.Id;
            Material = material;
            MaterialId = material.ID;
        }

        public int PiezaId { get;set;}
        public Pieza3D Pieza { get; set;} = null!;
        public int MaterialId { get;set;}
        public Material Material { get; set;} = null!;

        public override bool Equals(object? obj)
        {
            return obj is PiezaMaterial material && PiezaId == material.PiezaId && MaterialId == material.Material.ID;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(PiezaId, MaterialId);
        }

    }
}