namespace DefinitelyNotFF7.api.Models
{
    public class Materia
    {
        public required string Name { get; set; }
        public int MpCost { get; set; }
        public int BaseDamage { get; set; }
        public required string Element { get; set; }
    }
}
