namespace DefinitelyNotFF7.api.Models
{
    public class Character
    {
        public required string Name { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Attack { get; set; }
        public int CurrentAtb { get; set; }
        public int MaxAtb { get; set; }
        public int Level { get; set; }
        public int CurrentXp { get; set; }
        public int XpToNextLevel { get; set; }
        public int MaxMp { get; set; }
        public int CurrentMp { get; set; }
        public int MateriaSlots { get; set; } = 2;
        public List<Materia> EquippedMateria { get; set; } = new();
    }
}
