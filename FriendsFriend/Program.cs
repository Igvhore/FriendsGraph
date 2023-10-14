using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Utils;

namespace FriendsFriend
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dataSetPath = @"data\IDList.csv";
            List<User> users = new List<User>();
            VkApi api = new VkApi();


            CsvConfiguration config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ",", Encoding = Encoding.UTF8 };

            using (StreamReader fileReader = File.OpenText(dataSetPath))

            using (CsvReader csvResult = new CsvReader(fileReader, config))
            {
                csvResult.Read();
                csvResult.ReadHeader();

                while (csvResult.Read())
                    users.Add(new User(csvResult.GetField<string>("Ваш ID в VK")));

                foreach (var user in users)
                    user.SetID(user.CorrectVkId(user.Id));
            }

            api.Authorize(new ApiAuthParams
            {
                AccessToken = File.ReadAllText(@"data\VKToken.txt")
            });

            List<long> firstLayerUsersIDs = api.Users.Get(users.Select(n => n.Id)).Select(t => t.Id).ToList();

        }

        class User
        {
            private string _id;
            private string _name;
            public VkCollection<User> friends;

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

            public void SetName(string NewName) => _name = NewName;
            public void SetID(string NewID) => _id = NewID;
            public string CorrectVkId(string id) => id.Trim('@').Replace("https://vk.com/", "");

        }

    }
}