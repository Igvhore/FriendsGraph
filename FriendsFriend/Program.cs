using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using System.IO;

namespace FriendsFriend
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dataSetPath = @"data\IDList.csv";
            List<string> lines = new List<string>();
            List<User> users = new List<User>();

            using (StreamReader reader = new StreamReader(File.OpenRead(dataSetPath)))
            {
                string target = "Отметка времени";
                int? targetPosition = null;
                string line;

                List<string> collected = new List<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] split = line.Split(',');
                    collected.Clear();

                    for (int i = 0; i < split.Length; i++)
                    {
                        if (string.Equals(split[i], target, StringComparison.OrdinalIgnoreCase))
                        {
                            targetPosition = i;
                            break;
                        }
                    }

                    for (int i = 0; i < split.Length; i++)
                    {
                        if (targetPosition != null && i == targetPosition.Value) continue;

                        if (split[i] == "Ваш ID в VK")
                            collected.Add("Id");
                        else
                            collected.Add(split[i]);

                    }

                    lines.Add(string.Join(",", collected));
                }
            }
            using (StreamWriter writer = new StreamWriter(dataSetPath, false))
            {
                foreach (string line in lines)
                    writer.WriteLine(line);

            }          

            using (var reader = new StreamReader(dataSetPath))

            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                IEnumerable<User> records = csv.GetRecords<User>();

                foreach (IEnumerable<User> record in records)
                    Console.WriteLine(record);
            }
        }
    }

    class User
    {
        private string _id;

        private string _name;

        [Name("Id")]
        public string Id { get { return _id; } }

        public string Name { get { return _name; } }
        
        public User()
        {
            _id = "NONE";
            _name = "EMPTY";
        }
        public User(string id)
        {
            _id = id;
            _name = "EMPTY";
        }
        public User(string id, string name)
        {
            _id = id;
            _name = name;
        }

        public void SetName (string NewName) => _name = NewName;
        public void SetID(string NewID) => _id = NewID;

    }

}