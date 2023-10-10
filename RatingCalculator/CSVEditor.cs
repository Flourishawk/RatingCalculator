namespace RatingCalculator
{
    public static class CSVEditor
    {
        public static void CreateCSVFile(List<RatingCalculator.Entrant> rating, string filePath)
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

        public static void ReadCSVFile(int countSpeciality, List<string> filePaths, List<List<RatingCalculator.Entrant>> edbo)
        {
            //зчитування даних з файлів у загальний список абітурієнтів
            foreach (string filePath in filePaths)
            {
                using (StreamReader reader = new(filePath, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine()!;
                        string[] fields = line.Split(';');//Розділити рядок на поля
                                                          //прибираю значення контрактників та квотників
                        if (fields[1] == "1" || fields[1] == "2" || fields[1] == "3" || fields[1] == "4" || fields[1] == "5")
                        {
                            RatingCalculator.Entrant newEntrant = new(fields[0], int.Parse(fields[1]), double.Parse(fields[2]), fields[3], int.Parse(fields[4]), int.Parse(fields[5]));
                            edbo[countSpeciality].Add(newEntrant);
                        }
                    }
                }
                countSpeciality--;
            }
        }
    }
}