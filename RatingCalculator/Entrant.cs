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

        
        internal readonly string FullName;
        internal int Priority { get;}
        internal double Score { get;}
        internal string Specialty { get;}
        internal int NumSpecialty { get;}
        internal int SizeSpecialty { get;}
        internal double Percent { get; set; }
    }
}

