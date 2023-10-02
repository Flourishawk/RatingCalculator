namespace RatingCalculator
{
    internal static class Program
    {
        static void Main()
        {

            //загальні списки та рейтинги до спеціальностей
            List<List<Entrant>> edbo = new();//список усіх студентів усіх спеціальностей
            List<List<Entrant>> rating = new();
            List<List<Entrant>> firstRunList = new();//Список студентів на спеціальності з найменшим пріорітетом

            //базова частина для шляху файлів
            string baseExportFilePath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "Export\\");
            string baseImportFilePath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "Import");

            //шляхи до даних
            List<string> filePaths = new(Directory.GetFiles(baseImportFilePath, "*.csv"));

            int countSpeciality = filePaths.Count;

            if (countSpeciality == 0)
            {
                return;
            }

            //ініціалізація списків усіх студентів, які подалися на спеціальності та тих, хто проходить на спеціальності
            for (int a = 0; a < countSpeciality; a++)
            {
                edbo.Add(new List<Entrant>());
                rating.Add(new List<Entrant>());
                firstRunList.Add(new List<Entrant>());
            }

            //зменшую значення кількостей спеціальностей для індексації
            countSpeciality--;

            //зчитування даних з файлів у загальний список абітурієнтів
            foreach (string filePath in filePaths)
            {
                using (StreamReader reader = new(filePath, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        string? line = reader.ReadLine() ?? "";
                        string[] fields = line.Split(';');//Розділити рядок на поля

                        //прибираю значення контрактників та квотників
                        if (fields[1] == "1" || fields[1] == "2" || fields[1] == "3" || fields[1] == "4" || fields[1] == "5")
                        {
                            Entrant newEntrant = new(fields[0], int.Parse(fields[1]), double.Parse(fields[2]), fields[3], int.Parse(fields[4]), int.Parse(fields[5]));
                            edbo[countSpeciality].Add(newEntrant);
                        }
                    }
                }
                countSpeciality--;
            }

            List<Entrant> edboRating = MergeLists(edbo);//зібрав усіх студентів в єдиний List
            SortListByRating(edboRating);//відосртував по рейтингу
            List<Entrant> lowestPriority = AllocationLowestPriority(edboRating);//виділяємо найменші пріорітети студентів
            List<string> uniqueSpecialties = edboRating.Select(e => e.Specialty).Distinct().ToList();//список спеціальностей серед вступників

            foreach (var ent in lowestPriority)//розбиваємо List<Entrant> з найменшими пріорітетами на List<List<Entrant>> по спеціальностям
            {
                AddToRating(firstRunList, ent, uniqueSpecialties);//firstRunList - ліст, який містить найменші пріорітети по спеціальностям List<LisT<Entrant>>
            }

            //Занесення найменших пріорітетів в рейтинги
            for (int b = 0; b < firstRunList.Count; b++)
            {
                for (int c = 0; c < firstRunList[b].Count; c++)
                {
                    rating[b].Add(firstRunList[b][c]);
                }
            }

            //сортуємо абітурієнтів з найменшими пріорітетами по рейтингу
            foreach (var list in rating)
            {
                SortListByRating(list);
            }



            //цикл обробки всіх пріорітетів
            for (int f = 0; f <= 4; f++)
            {
                //Видаляємо абітурієнтів, які не пройшли та додаємо їх по наступному пріорітету
                ModifyingTheRating(rating, edbo, uniqueSpecialties);
                //сортування лістів
                foreach (var list in rating)
                {
                    SortListByRating(list);
                }
            }

            
            //Створення файлу з результатами для вступу
            foreach (var spec in rating)
            {
                //називаємо файл по спеціальності
                string entryFilePath = baseExportFilePath + spec[0].Specialty + ".csv";
                CreateCSVFile(spec, entryFilePath);
            }


            List<Entrant> studentsRating2 = TrimRating(edboRating);

            string ratingFilePath = baseExportFilePath + "Rating.csv";

            //Створення файлу з результатами для стипендій
            CreateCSVFile(studentsRating2, ratingFilePath);


            ConsoleTest();
        }

        //Пошук наступного пріорітету для абітурієнта, який не пройшов по поточному
        private static Entrant? FindTheFollowingPriority(List<List<Entrant>> edbo, Entrant ent, int priority)
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

        private static List<Entrant> AllocationLowestPriority(List<Entrant> edbo)
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

        //додаємо отриманого абітурієнта у відповідний рейтинговий список
        private static void AddToRating(List<List<Entrant>> rating, Entrant ent, List<string> uniqueSpec)
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
        private static void SortListByRating(List<Entrant> rating)
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
        private static void ModifyingTheRating(List<List<Entrant>> rating, List<List<Entrant>> edbo, List<string> uniqueSpecialties)
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
                        }
                        rating[d].RemoveAt(e);
                    }
                }
            }
        }

        private static void CreateCSVFile(List<Entrant> rating, string filePath)
        {

                using StreamWriter writer = new(filePath, false, Encoding.UTF8);
                // Записуємо заголовок CSV файлу
                writer.WriteLine("ПІБ;Пріорітет;Заг_бал;Спеціальність");


                // Записуємо дані абітурієнтів у CSV файл
                foreach (var ent in rating)
                {
                    writer.WriteLine($"{ent.FullName};{ent.Score};{ent.Percent}");
                }
        }

        private static List<Entrant> MergeLists(List<List<Entrant>> rating)
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

        private static List<Entrant> TrimRating(List<Entrant> rating)
        {

            for (int f = rating.Count - 1; f >= 0; f--)
            {
                rating[f].Percent = ((double)(f + 1
                    ) / rating.Count) * 100;
            }
            return rating;
        }

        private static void ConsoleTest()
        {
            Console.WriteLine("");
        }

    }
}