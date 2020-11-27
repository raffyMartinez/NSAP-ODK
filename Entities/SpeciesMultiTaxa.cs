namespace NSAP_ODK.Entities
{
    public class SpeciesMultiTaxa
    {
        public int SpeciesID { get; set; }

        public string SpeciesName { get; set; }
        public Taxa Taxa { get; set; }
        public SizeType SizeType { get; set; }
        public double? Size { get; set; }
    }
}