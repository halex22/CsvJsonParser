using System.Text;
using System.Text.Json;
using CsvJsonParser;

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = $@"{args[0]}";

        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found");
            return;
        }

        string extension = Path.GetExtension(filePath);
        string directory = Path.GetDirectoryName(filePath);


        if (extension == ".csv")
        {
            using (StreamReader sr = new(filePath))
            {
                var keysLine = sr.ReadLine();
                string[] keys = keysLine.Split(",");
                List<Person> people = new();
                //foreach (string key in keys) Console.WriteLine(key);


                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    string[] values = line.Split(",");
                    Person person = new Person
                    {
                        id = values[0],
                        first_name = values[1],
                        last_name = values[2],
                        email = values[3],
                        gender = values[4],
                        ip_address = values[5],
                    };
                    people.Add(person);
                }

                string jsonString = JsonSerializer.Serialize(people, new JsonSerializerOptions { WriteIndented = true });
                //File.WriteAllText("mockData.json", jsonString);
                string destinationPath = $@"{directory}\results.json";

                if(File.Exists(destinationPath)) File.Delete(destinationPath);
                File.WriteAllText(destinationPath, jsonString);
                //Console.WriteLine(jsonString);
            }
        }
        else
        {
            string jsonString = File.ReadAllText(filePath);
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;
            StringBuilder csvBuilder = new();
            List<string> columns = [];  

            // get the first element to find the header
            if (root.EnumerateArray().First().ValueKind == JsonValueKind.Object)
            {                
                foreach (JsonProperty prop in root.EnumerateArray().First().EnumerateObject())
                {
                    columns.Add(prop.Name);
                }
                csvBuilder.AppendLine(string.Join(",", columns));
            }

            foreach (var obj in root.EnumerateArray())
            {
                if (obj.ValueKind == JsonValueKind.Object)
                {
                    List<string> values = [];
                    foreach (JsonProperty prop in obj.EnumerateObject())
                    {
                        values.Add(prop.Value.ToString());
                    }

                    csvBuilder.AppendLine(string.Join(",", values));
                }
            }

            Console.WriteLine(csvBuilder.ToString());
            string destinationPath = Path.Combine(directory, "result.csv");
            File.WriteAllText(destinationPath, csvBuilder.ToString());
        }
    }
}