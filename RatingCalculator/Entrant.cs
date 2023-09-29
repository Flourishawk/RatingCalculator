namespace RatingCalculator
{
    internal class Entrant
    {
        public Entrant() { }
        public Entrant(string fullname, int priority, double score, string specialty, int numSpecialty, int sizeSpecialty)
        {
            FullName = fullname;
            Priority = priority;
            Score = score;
            Specialty = specialty;
            NumSpecialty = numSpecialty;
            SizeSpecialty = sizeSpecialty;
        }
        // Клас для представлення абітурієнтів
        public string FullName { get; set; }
        public int Priority { get; set; }
        public double Score { get; set; }
        public string Specialty { get; set; }
        public int NumSpecialty { get; set; }
        public int SizeSpecialty { get; set; }
        public double Percent { get; set; }
    }
}
