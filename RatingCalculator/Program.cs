using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

            public double percent { get; set; }
        }

        static void Main(string[] args)
        {
            //додаю українську і
            Console.OutputEncoding = Encoding.GetEncoding(1251);

            //загальні списки та рейтинги до спеціальностей
            List<List<Entrant>> edbo = new List<List<Entrant>>();
            List<List<Entrant>> rating = new List<List<Entrant>>();
            List<List<Entrant>> firstRunList = new List<List<Entrant>>();

            //кількість спеціальностей
            int countSpeciality = 2;

            //ініціалізація списків усіх студентів, яку подалися на спеціальності та тих, хто проходить на спеціальності
            for (int a = 0; a < countSpeciality; a++)
            {
                edbo.Add(new List<Entrant>());
                rating.Add(new List<Entrant>());
                firstRunList.Add(new List<Entrant>());
            }

            //базова частина для шляху файлів
            string baseFilePath = "C:\\Users\\poijn\\Desktop\\";
            
            //шляхи до даних
            List<string> filePaths = new List<string>
            {
            baseFilePath + "Процеси.csv",
            baseFilePath + "Системи.csv",
            //baseFilePath + "122_Інформаційні управляючі системи та технології.csv",
            //baseFilePath + "122_Інформаційні технології проектування.csv",
            //baseFilePath + "122_Системи штучного інтелекту.csv",
            //baseFilePath + "122_Системне проектування.csv",
            //baseFilePath + "122_Управління проектами в галузі інформаційних технологій.csv"
            };
            
            //зменшую значення кількостей спеціальностей для використання у індексах
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
                                specialty = fields[3]
                            };
                            edbo[countSpeciality].Add(newEntrant);
                        }
                    }
                }
                countSpeciality--;
            }

            List<Entrant> edboRating = mergeLists(edbo);
            sortListByRating(edboRating);


            /*List<Entrant> lowestPriority = allocationLowestPriority(edboRating);

            foreach (var ent in lowestPriority)
            {
                addToRating(firstRunList, ent);
            }

            //Занесення 1 пріорітетів в рейтинги
            for (int b = 0; b < firstRunList.Count; b++)
            {
                for (int c = 0; c < firstRunList[b].Count; c++)
                {
                        rating[b].Add(firstRunList[b][c]);
                }
            }

            //сортуємо абітурієнтів 1 пріорітетів по рейтингу
            foreach (var list in rating)
            {
                sortListByRating(list);
            }

            //цикл обробки всіх пріорітетів
            for (int f = 0; f <= 4; f++)
            {

                //Видаляємо абітурієнтів, які не пройшли та додаємо їх по наступному пріорітету
                modifyingTheRating(rating, edbo);
                //сортування лістів
                foreach (var list in rating)
                {
                    sortListByRating(list);
                }


            }

            //Створення файлу з результатами для вступу
            foreach (var spec in rating)
            {
                //називаємо файл по спеціальності
                string entryFilePath = baseFilePath + spec[0].specialty + ".csv";
                createCSVFile(spec, entryFilePath);
            }
            
            //створюємо рейтинговий список для стипендій
            List<Entrant> studentsRating = mergeLists(rating);*/


            List<Entrant> studentsRating2 = trimRating(edboRating, 40);

            string ratingFilePath = baseFilePath + "Rating.csv";

            //Створення файлу з результатами для стипендій
            createCSVFile(studentsRating2, ratingFilePath);
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
                    /*
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
                    */
                }
            }
            return null;
        }

        private static List<Entrant> allocationLowestPriority(List<Entrant> edbo)
        {
            List<Entrant> LowestPriority = new List<Entrant>();
            //LowestPriority.Add(edbo[0]);
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
                    LowestPriority.Add(edbo[i+1]);
                }
            }
            return LowestPriority;
        }



        //додаємо отриманого абітурієнта у відповідний рейтинговий список
        private static void addToRating(List<List<Entrant>> rating, Entrant ent)
        {
            if (ent.specialty == "Науки про дані")
            {
                rating[6].Add(ent);
            }
            else if (ent.specialty == "Інформатика")
            {
                rating[5].Add(ent);
            }
            else if (ent.specialty == "Інформаційні управляючі системи та технології")
            {
                rating[4].Add(ent);
            }
            else if (ent.specialty == "Інформаційні технології проектування")
            {
                rating[3].Add(ent);
            }
            else if (ent.specialty == "Системи штучного інтелекту")
            {
                rating[2].Add(ent);
            }
            else if (ent.specialty == "Системне проектування")
            {
                rating[1].Add(ent);
            }
            else if (ent.specialty == "Управління проектами в галузі інформаційних технологій")
            {
                rating[0].Add(ent);
            }
        }

        //Бульбашкове сортування по значенню рейтингу
        private static void sortListByRating(List <Entrant> rating)
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

        //Бульбашкове сортування по значенню ПІБ та пріорітету
        private static void sortListByFullname(List<Entrant> rating)
        {
            //по ПІБ
            for (int i = 0; i < rating.Count - 1; i++)
            {
                for (int j = 0; j < rating.Count - i - 1; j++)
                {
                    string fullName1 = rating[j].fullName;
                    string fullName2 = rating[j + 1].fullName;

                    double score1 = Convert.ToDouble(rating[j].score);
                    double score2 = Convert.ToDouble(rating[j + 1].score);

                    if (string.Compare(fullName1, fullName2) < 0)
                    {
                        Entrant temp = rating[j];
                        rating[j] = rating[j + 1];
                        rating[j + 1] = temp;
                    }
                }
            }
            //По пріорітету
            for (int i = 0; i < rating.Count - 1; i++)
            {
                for (int j = 0; j < rating.Count - i - 1; j++)
                {
                    string fullName1 = rating[j].fullName;
                    string fullName2 = rating[j + 1].fullName;

                    double score1 = Convert.ToDouble(rating[j].score);
                    double score2 = Convert.ToDouble(rating[j + 1].score);

                    int prioirity1 = rating[j].priority;
                    int prioirity2 = rating[j + 1].priority;

                    if (string.Compare(fullName1, fullName2) == 0 && score1==score2 && prioirity1<prioirity2)
                    {
                        Entrant temp = rating[j];
                        rating[j] = rating[j + 1];
                        rating[j + 1] = temp;
                    }
                }
            }
        }

        private static void sortListBySpeciality(List<Entrant> rating)
        {

            for (int i = 0; i < rating.Count - 1; i++)
            {
                for (int j = 0; j < rating.Count - i - 1; j++)
                {
                    string fullName1 = rating[j].specialty;
                    string fullName2 = rating[j + 1].specialty;

                    if (string.Compare(fullName1, fullName2) < 0)
                    {
                        Entrant temp = rating[j];
                        rating[j] = rating[j + 1];
                        rating[j + 1] = temp;
                    }
                }
            }
        }

        //Видалення абітурієнтів, які не пройшли по поточному пріорітету та додавання їй у списки по наступному
        private static void modifyingTheRating(List<List<Entrant>> rating, List<List<Entrant>> edbo)
        {

            for (int d = 0; d < rating.Count; d++)
            {
                switch (d)
                {
                    case 6://Data Science

                        for (int e = rating[d].Count - 1; e >= 0; e--)
                        {
                            //19
                            if (e >= 19)
                            {
                                sortListByRating(rating[d]);
                                Entrant newEntrant = findTheFollowingPriority(edbo, rating[d][e], rating[d][e].priority);
                                if (newEntrant != null)
                                {
                                    addToRating(rating, newEntrant);
                                }
                                rating[d].RemoveAt(e);
                            }
                        }

                        break;

                    case 5://Інформатика

                        for (int e = rating[d].Count - 1; e >= 0; e--)
                        {
                            if (e >= 27)
                            {
                                sortListByRating(rating[d]);
                                Entrant newEntrant = findTheFollowingPriority(edbo, rating[d][e], rating[d][e].priority);
                                if (newEntrant != null)
                                {
                                    addToRating(rating, newEntrant);
                                }
                                rating[d].RemoveAt(e);
                            }
                        }

                        break;

                    case 4://Інформаційні управляючі системи та технології

                        for (int e = rating[d].Count - 1; e >= 0; e--)
                        {
                            
                            if (e >= 17)
                            {
                                sortListByRating(rating[d]);
                                Entrant newEntrant = findTheFollowingPriority(edbo, rating[d][e], rating[d][e].priority);
                                if (newEntrant != null)
                                {
                                    addToRating(rating, newEntrant);
                                }
                                rating[d].RemoveAt(e);
                            }
                        }

                        break;
                    case 3://Інформаційні технології проєктування

                        for (int e = rating[d].Count - 1; e >= 0; e--)
                        {
                            
                            if (e >= 22)
                            {
                                sortListByRating(rating[d]);
                                Entrant newEntrant = findTheFollowingPriority(edbo, rating[d][e], rating[d][e].priority);
                                if (newEntrant != null)
                                {
                                    addToRating(rating, newEntrant);
                                }
                                rating[d].RemoveAt(e);
                            }
                        }

                        break;
                    case 2://Системи штучного інтелекту

                        for (int e = rating[d].Count - 1; e >= 0; e--)
                        {
                            
                            if (e >= 15)
                            {
                                sortListByRating(rating[d]);
                                Entrant newEntrant = findTheFollowingPriority(edbo, rating[d][e], rating[d][e].priority);
                                if (newEntrant != null)
                                {
                                    addToRating(rating, newEntrant);
                                }
                                rating[d].RemoveAt(e);
                            }
                        }

                        break;
                    case 1://Системне проєктування

                        for (int e = rating[d].Count - 1; e >= 0; e--)
                        {
                            
                            if (e >= 15)
                            {
                                sortListByRating(rating[d]);
                                Entrant newEntrant = findTheFollowingPriority(edbo, rating[d][e], rating[d][e].priority);
                                if (newEntrant != null)
                                {
                                    addToRating(rating, newEntrant);
                                }
                                rating[d].RemoveAt(e);
                            }
                        }

                        break;
                    case 0://Управління проєктами в галузі інформаційних технологій

                        for (int e = rating[d].Count - 1; e >= 0; e--)
                        {
                            if (e >= 15)
                            {
                                sortListByRating(rating[d]);
                                Entrant newEntrant = findTheFollowingPriority(edbo, rating[d][e], rating[d][e].priority);
                                if (newEntrant != null)
                                {
                                    addToRating(rating, newEntrant);
                                }
                                rating[d].RemoveAt(e);
                            }
                        }

                        break;
                }
            }
        }

        private static void createCSVFile(List<Entrant> rating, string filePath)
        {
                
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // Записуємо заголовок CSV файлу
                    writer.WriteLine("ПІБ;Пріорітет;Заг_бал;Спеціальність");


                    // Записуємо дані абітурієнтів у CSV файл
                    foreach (var ent in rating)
                    {
                        writer.WriteLine($"{ent.fullName};{ent.score};{ent.percent}");
                    }
                }
        }

        private static List<Entrant> mergeLists(List<List<Entrant>> rating)
        {
            List<Entrant> mergeList = new List<Entrant>();

            foreach (var spec in rating) 
            {
                foreach(var ent in spec)
                {
                    mergeList.Add(ent);
                }
            }
            sortListByRating(mergeList);
            return mergeList;
        }

        private static List<Entrant> trimRating(List<Entrant> rating, double percent)
        {
            //int limit = (int)Math.Floor(rating.Count * (percent / 100));

            for (int f = rating.Count - 1; f >= 0; f--)
            {
                rating[f].percent = ((double)(f+1
                    ) / rating.Count) * 100;
                Console.WriteLine(rating[f].percent);
            }
            Console.ReadLine();
            return rating;
        }
    }
}