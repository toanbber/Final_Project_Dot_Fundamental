using Final_Project_Dot_Fundamental;
using Final_Project_Dot_Fundamental.Model;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

internal class CsvDataProvider<T> : IDataProvider<T> where T : Titanic, new()
{
    private readonly ILogger<CsvDataProvider<T>> _logger;

    public CsvDataProvider(ILogger<CsvDataProvider<T>> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<T>> ReadAsync(string path)
    {
        if (!File.Exists(path))
        {
            _logger.LogError("CSV file not found: {Path}", path);
            throw new FileNotFoundException($"CSV file not found: {path}");
        }

        var passengers = new List<T>();
        using var parser = new TextFieldParser(path);
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;
        parser.ReadLine();

        while (!parser.EndOfData)
        {
            string[] fields = parser.ReadFields()!;

            T p = new T
            {
                Pclass = int.Parse(fields[2]),
                Name = string.IsNullOrWhiteSpace(fields[3]) ? null : fields[3],
                Sex = string.IsNullOrWhiteSpace(fields[4]) ? null : fields[4],
                Age = string.IsNullOrWhiteSpace(fields[5]) ? (double?)null : double.Parse(fields[5], CultureInfo.InvariantCulture),
                SibSp = int.Parse(fields[6]),
                Parch = int.Parse(fields[7]),
                Ticket = string.IsNullOrWhiteSpace(fields[8]) ? null : fields[8],
                Fare = string.IsNullOrWhiteSpace(fields[9]) ? 0 : double.Parse(fields[9], CultureInfo.InvariantCulture),
                Cabin = string.IsNullOrWhiteSpace(fields[10]) ? null : fields[10],
                Embarked = string.IsNullOrWhiteSpace(fields[11]) ? null : fields[11]
            };

            passengers.Add(p);
        }

        return passengers;
    }
}
