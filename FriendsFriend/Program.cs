using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using VkNet;
using VkNet.Model;

namespace FriendsFriend
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dataSetPath = @"data\IDList.csv";
            List<User> users = new List<User>();
            var api = new VkApi();

            var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ",", Encoding = Encoding.UTF8 };

            using (var fileReader = File.OpenText(dataSetPath))

            using (var csvResult = new CsvReader(fileReader, config))
            {
                csvResult.Read();
                csvResult.ReadHeader();

                while (csvResult.Read())
                    users.Add(new User(csvResult.GetField<string>("Ваш ID в VK")));
                
            }


            api.Authorize(new ApiAuthParams
            {
                AccessToken = File.ReadAllText(@"data\VKToken.txt")
            });

            var res = api.Groups.Get(new GroupsGetParams());

            Console.WriteLine(res.TotalCount);

        }
    }

    class User
    {
        private string _id;
        private string _name;
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