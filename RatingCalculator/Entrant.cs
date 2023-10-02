namespace RatingCalculator
{
    public class Entrant
    {
        public Entrant(string fullname, int priority, double score, string specialty, int numSpecialty, int sizeSpecialty)
        {

            FullName = fullname;
            Priority = priority;
            Score = score;
            Specialty = specialty;
            NumSpecialty = numSpecialty;
            SizeSpecialty = sizeSpecialty;
        }
        public Entrant()
        {
            FullName = "default";
            Specialty= "default";
        }

        // Клас для представлення абітурієнтів
        internal string FullName { get; set; }
        internal int Priority { get; set; }
        internal double Score { get; set; }
        internal string Specialty { get; set; }
        internal int NumSpecialty { get; set; }
        internal int SizeSpecialty { get; set; }
        internal double Percent { get; set; }
    }


}

