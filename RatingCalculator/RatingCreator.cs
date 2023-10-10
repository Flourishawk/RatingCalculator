namespace RatingCalculator
{
    public static class RatingCreator
    {
        //додаємо отриманого абітурієнта у відповідний рейтинговий список
        public static void AddToRating(List<List<Entrant>> rating, Entrant ent, List<string> uniqueSpec)
        {
            for (int counterSpec = 0; counterSpec < uniqueSpec.Count; counterSpec++)
            {
                if (ent.Specialty == uniqueSpec[counterSpec])
                {
                    rating[counterSpec].Add(ent);
                    return;
                }
            }
        }

        //Бульбашкове сортування по значенню рейтингу
        public static void SortListByRating(List<Entrant> rating)
        {

            for (int i = 0; i < rating.Count - 1; i++)
            {
                for (int j = 0; j < rating.Count - i - 1; j++)
                {
                    double score1 = Convert.ToDouble(rating[j].Score);
                    double score2 = Convert.ToDouble(rating[j + 1].Score);

                    if (score1 < score2)
                    {
                        (rating[j], rating[j + 1]) = (rating[j + 1], rating[j]);
                    }
                }
            }
        }

        //Видалення абітурієнтів, які не пройшли по поточному пріорітету та додавання їх у списки по наступному
        public static void ModifyingTheRating(List<List<Entrant>> rating, List<List<Entrant>> edbo, List<string> uniqueSpecialties)
        {

            for (int d = 0; d < rating.Count; d++)
            {
                for (int e = rating[d].Count - 1; e >= 0; e--)
                {
                    if (e >= rating[d][0].SizeSpecialty)
                    {
                        SortListByRating(rating[d]);
                        Entrant? newEntrant = FindTheFollowingPriority(edbo, rating[d][e], rating[d][e].Priority);
                        if (newEntrant != null)
                        {
                            AddToRating(rating, newEntrant, uniqueSpecialties);
                            rating[d].RemoveAt(e);
                        }
                    }
                }
            }
        }

        public static List<Entrant> MergeLists(List<List<Entrant>> rating)
        {
            List<Entrant> mergeList = new();

            foreach (var spec in rating)
            {
                foreach (var ent in spec)
                {
                    mergeList.Add(ent);
                }
            }
            SortListByRating(mergeList);
            return mergeList;
        }

        public static List<Entrant> TrimRating(List<Entrant> rating)
        {

            for (int f = rating.Count - 1; f >= 0; f--)
            {
                rating[f].Percent = ((double)(f + 1
                    ) / rating.Count) * 100;
            }
            return rating;
        }

        //Пошук наступного пріорітету для абітурієнта, який не пройшов по поточному
        public static Entrant? FindTheFollowingPriority(List<List<Entrant>> edbo, Entrant ent, int priority)
        {
            foreach (var spec in edbo)
            {
                foreach (var enttryFromEdbo in spec)
                {
                    if ((ent.FullName == enttryFromEdbo.FullName) && (enttryFromEdbo.Priority == (priority + 1)))
                    {
                        return enttryFromEdbo;
                    }
                }
            }
            return null;
        }

        public static List<Entrant> AllocationLowestPriority(List<Entrant> edbo)
        {
            List<Entrant> LowestPriority = new();

            for (int i = 0; i < edbo.Count - 1; i++)
            {

                string fullName1 = edbo[i].FullName;
                string fullName2 = edbo[i + 1].FullName;


                if ((fullName1 != fullName2))
                {
                    LowestPriority.Add(edbo[i]);
                }
                if (i == edbo.Count - 2)
                {
                    LowestPriority.Add(edbo[i + 1]);
                }
            }
            return LowestPriority;
        }
    }
}
