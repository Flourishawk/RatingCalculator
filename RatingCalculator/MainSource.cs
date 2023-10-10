using static RatingCalculator.RatingCreator;

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

            //шлях до даних
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

            CSVEditor.ReadCSVFile(countSpeciality, filePaths, edbo);

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
                if (spec.Count is 0)
                {
                    break;
                }
                //називаємо файл по спеціальності
                string entryFilePath = baseExportFilePath + spec[0].Specialty + ".csv";
                CSVEditor.CreateCSVFile(spec, entryFilePath);
            }

            List<Entrant> studentsRating2 = TrimRating(edboRating);

            string ratingFilePath = baseExportFilePath + "Rating.csv";

            //Створення файлу з результатами для стипендій
            CSVEditor.CreateCSVFile(studentsRating2, ratingFilePath);
        }
    }
}