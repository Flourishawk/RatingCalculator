using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace RatingCalculator
{
    internal class Program
    {

        // Клас для представлення абітурієнтів
        public class Entrant
        {
            public string fullName { get; set; }
            public int priority { get; set; }
            public double score { get; set; }
            public string specialty { get; set; }
            public int numSpecialty { get; set; }
            public int sizeSpecialty { get; set; }
            public double percent { get; set; }
        }

        static void Main(string[] args)
        {
            //додаю українську і
            Console.OutputEncoding = Encoding.GetEncoding(1251);

            //загальні списки та рейтинги до спеціальностей
            List<List<Entrant>> edbo = new List<List<Entrant>>();//список усіх студентів усіх спеціальностей
            List<List<Entrant>> rating = new List<List<Entrant>>();
            List<List<Entrant>> firstRunList = new List<List<Entrant>>();//Список студентів на спеціальності з найменшим пріорітетом

            //базова частина для шляху файлів
            //string baseExportFilePath = Path.Combine(Environment.CurrentDirectory+ "..", "Export");
            string baseExportFilePath = Path.GetFullPath(Directory.GetCurrentDirectory() + "\\..\\..\\Export\\");
            string baseImportFilePath = Path.Combine(Environment.CurrentDirectory, "..", "..", "Import");

            //шляхи до даних
            List<string> filePaths = new List<string>(Directory.GetFiles(baseImportFilePath, "*.csv"));

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
                using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] fields = line.Split(';');//Розділити рядок на поля

                        //прибираю значення контрактників та квотників
                        if (fields[1] == "1" || fields[1] == "2" || fields[1] == "3" || fields[1] == "4" || fields[1] == "5")
                        {
                            Entrant newEntrant = new Entrant
                            {
                                fullName = fields[0],
                                priority = int.Parse(fields[1]),
                                score = double.Parse(fields[2]),
                                specialty = fields[3],
                                numSpecialty = int.Parse(fields[4]),
                                sizeSpecialty = int.Parse(fields[5])
                            };
                            edbo[countSpeciality].Add(newEntrant);
                        }
                    }
                }
                countSpeciality--;
            }

            List<Entrant> edboRating = mergeLists(edbo);//зібрав усіх студентів в єдиний List
            sortListByRating(edboRating);//відосртував по рейтингу
            List<Entrant> lowestPriority = allocationLowestPriority(edboRating);//виділяємо найменші пріорітети студентів
            List<string> uniqueSpecialties = edboRating.Select(e => e.specialty).Distinct().ToList();//список спеціальностей серед вступників
            List<int> uniqueNumSpecialties = edboRating.Select(e => e.numSpecialty).Distinct().ToList();//список спеціальностей серед вступників

            foreach (var ent in lowestPriority)//розбиваємо List<Entrant> з найменшими пріорітетами на List<List<Entrant>> по спеціальностям
            {
                addToRating(firstRunList, ent, uniqueSpecialties);//firstRunList - ліст, який містить найменші пріорітети по спеціальностям List<LisT<Entrant>>
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
                sortListByRating(list);
            }

            //цикл обробки всіх пріорітетів
            for (int f = 0; f <= 4; f++)
            {
                //Видаляємо абітурієнтів, які не пройшли та додаємо їх по наступному пріорітету
                modifyingTheRating(rating, edbo, uniqueSpecialties);
                //сортування лістів
                foreach (var list in rating)
                {
                    sortListByRating(list);
                }
            }

            //Створення файлу з результатами для вступу
            Directory.CreateDirectory(baseExportFilePath);
            foreach (var spec in rating)
            {
                //називаємо файл по спеціальності
                string entryFilePath = baseExportFilePath + spec[0].specialty + ".csv";
                createCSVFile(spec, entryFilePath, false);
            }

            List<List<Entrant>> studentsRating = distributionBySpecialtyNumber(rating, uniqueNumSpecialties);//розділяємо студентів по номерам спеціальностей
            foreach (var listRating in studentsRating)
            {
                List<Entrant> specialtyRating = calculationOfThePercentagePosition(listRating);
                string ratingFilePath = baseExportFilePath + $"{specialtyRating[0].numSpecialty}_Рейтинг.csv";
                createCSVFile(specialtyRating, ratingFilePath, true);
            }
        }

        //Пошук наступного пріорітету для абітурієнта, який не пройшов по поточному
        private static Entrant findTheFollowingPriority(List<List<Entrant>> edbo, Entrant ent, int priority)
        {
            foreach (var spec in edbo)
            {
                foreach (var enttryFromEdbo in spec)
                {
                    if ((ent.fullName == enttryFromEdbo.fullName) && (enttryFromEdbo.priority == (priority + 1)))
                    {
                        return enttryFromEdbo;
                    }
                    else if (ent.fullName == enttryFromEdbo.fullName && enttryFromEdbo.priority == ent.priority + 1 && ent.priority + 2 <= 5)
                    {
                        return enttryFromEdbo;
                    }
                    else if (ent.fullName == enttryFromEdbo.fullName && enttryFromEdbo.priority == ent.priority + 1 && ent.priority + 3 <= 5)
                    {
                        return enttryFromEdbo;
                    }
                    else if (ent.fullName == enttryFromEdbo.fullName && enttryFromEdbo.priority == ent.priority + 1 && ent.priority + 4 <= 5)
                    {
                        return enttryFromEdbo;
                    }
                }
            }
            return null;
        }

        private static List<Entrant> allocationLowestPriority(List<Entrant> edbo)
        {
            List<Entrant> LowestPriority = new List<Entrant>();

            for (int i = 0; i < edbo.Count - 1; i++)
            {
                double score1 = Convert.ToDouble(edbo[i].score);
                double score2 = Convert.ToDouble(edbo[i + 1].score);

                string fullName1 = edbo[i].fullName;
                string fullName2 = edbo[i + 1].fullName;


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
        private static void addToRating(List<List<Entrant>> rating, Entrant ent, List<string> uniqueSpec)
        {
            for (int counterSpec = 0; counterSpec < uniqueSpec.Count; counterSpec++)
            {
                if (ent.specialty == uniqueSpec[counterSpec])
                {
                    rating[counterSpec].Add(ent);
                    return;
                }
            }
        }

        //Бульбашкове сортування по значенню рейтингу
        private static void sortListByRating(List<Entrant> rating)
        {

            for (int i = 0; i < rating.Count - 1; i++)
            {
                for (int j = 0; j < rating.Count - i - 1; j++)
                {
                    double score1 = Convert.ToDouble(rating[j].score);
                    double score2 = Convert.ToDouble(rating[j + 1].score);

                    if (score1 < score2)
                    {
                        Entrant temp = rating[j];
                        rating[j] = rating[j + 1];
                        rating[j + 1] = temp;
                    }
                }
            }
        }

        //Видалення абітурієнтів, які не пройшли по поточному пріорітету та додавання їх у списки по наступному
        private static void modifyingTheRating(List<List<Entrant>> rating, List<List<Entrant>> edbo, List<string> uniqueSpecialties)
        {

            for (int d = 0; d < rating.Count; d++)
            {
                for (int e = rating[d].Count - 1; e >= 0; e--)
                {
                    if (e >= rating[d][0].sizeSpecialty)
                    {
                        sortListByRating(rating[d]);
                        Entrant newEntrant = findTheFollowingPriority(edbo, rating[d][e], rating[d][e].priority);
                        if (newEntrant != null)
                        {
                            addToRating(rating, newEntrant, uniqueSpecialties);
                        }
                        rating[d].RemoveAt(e);
                    }
                }
            }
        }

        private static void createCSVFile(List<Entrant> rating, string filePath, bool isNeedPercentages)
        {

            if (!isNeedPercentages)
            {
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // Записуємо заголовок CSV файлу
                    writer.WriteLine("ПІБ;Пріорітет;Номер спеціальності;Спеціальність;Заг. бал");


                    // Записуємо дані абітурієнтів у CSV файл
                    foreach (var ent in rating)
                    {
                        writer.WriteLine($"{ent.fullName};{ent.priority};{ent.numSpecialty};{ent.specialty};{ent.score}");
                    }
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // Записуємо заголовок CSV файлу
                    writer.WriteLine("ПІБ;Пріорітет;Номер спеціальності;Спеціальність;Заг. бал;Відсоткове положення");

                    // Записуємо дані абітурієнтів у CSV файл
                    foreach (var ent in rating)
                    {
                        writer.WriteLine($"{ent.fullName};{ent.priority};{ent.numSpecialty};{ent.specialty};{ent.score};{Math.Round(ent.percent,3)}");
                    }
                }
            }
        }

        private static List<Entrant> mergeLists(List<List<Entrant>> rating)
        {
            List<Entrant> mergeList = new List<Entrant>();

            foreach (var spec in rating)
            {
                foreach (var ent in spec)
                {
                    mergeList.Add(ent);
                }
            }
            sortListByRating(mergeList);
            return mergeList;
        }

        private static List<List<Entrant>> distributionBySpecialtyNumber(List<List<Entrant>> rating, List<int> uniqueNumSpec)
        {
            List<List<Entrant>> distributedList = new List<List<Entrant>>();

            for (int a = 0; a < uniqueNumSpec.Count; a++)
            {
                distributedList.Add(new List<Entrant>());
            }

            for (int bb = 0; bb < uniqueNumSpec.Count; bb++)
            {
                foreach (var spec in rating)
                {
                    foreach (var ent in spec)
                    {
                        if (ent.numSpecialty == uniqueNumSpec[bb])
                        {
                            distributedList[bb].Add(ent);
                        }
                    }
                }
            }
            foreach(var numSpec in distributedList)
            {
                sortListByRating(numSpec);
            }
                       return distributedList;
        }

        private static List<Entrant> calculationOfThePercentagePosition(List<Entrant> rating)
        {
            for (int f = rating.Count - 1; f >= 0; f--)
            {
                rating[f].percent = ((double)(f + 1) / rating.Count) * 100;
            }
            return rating;
        }
    }
}