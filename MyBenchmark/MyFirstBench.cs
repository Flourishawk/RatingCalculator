using BenchmarkDotNet.Attributes;
using RatingCalculator;
using BenchmarkDotNet.Diagnosers;
using System.Diagnostics.CodeAnalysis;
using static RatingCalculator.RatingCreator;


namespace MyBenchmark
{
    [MemoryDiagnoser]
    public class MyFirstBench
    {
        private int CountSpeciality { get; set; }
        private List<string>? FilePaths { get; set; }

        [NotNull] private List<List<Entrant>> Edbo { get; set; } = new List<List<Entrant>>();
        [NotNull] private List<Entrant> StudentsRating { get; set; } = new List<Entrant>();
        [NotNull] private string RatingFilePath { get; set; } = "";

        [GlobalSetup]
        public void Setup()
        {
            // підготовка змінних для ReadCSVFile
            string baseImportFilePath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "Import");
            FilePaths = new(Directory.GetFiles(baseImportFilePath, "*.csv"));
            if (FilePaths is null)
            {
                throw new MyCustomException("Неможливий filePath");
            }

            CountSpeciality = FilePaths.Count;
            if (CountSpeciality == 0)
            {
                throw new MyCustomException("Кількість спеціальностей = 0");
            }
            for (int a = 0; a < CountSpeciality; a++)
            {
                Edbo.Add(new List<Entrant>());
            }
            CountSpeciality--;
            string baseExportFilePath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "Export\\");
            RatingFilePath = baseExportFilePath + "Rating.csv";
            List<Entrant> edboRating = MergeLists(Edbo);//зібрав усіх студентів в єдиний List
            SortListByRating(edboRating);//відосртував по рейтингу
            StudentsRating = TrimRating(edboRating);
        }

        [Benchmark]
        public void BenchReadCsvFile()
        {
            CSVEditor.ReadCSVFile(CountSpeciality, FilePaths!, Edbo);
        }

        [Benchmark]
        public void BenchCreateCsvFile()
        {
            CSVEditor.CreateCSVFile(StudentsRating, RatingFilePath);
        }
    }
}
