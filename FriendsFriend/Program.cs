using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace FriendsFriend
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dataSetPath = @"data\IDList.csv";
            List<User> users = new List<User>();
            List<User> usersFirstLayer = new List<User>();
            List<User> usersSecondLayer = new List<User>();
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

            List<long> zeroLayerUsersIDs = api.Users.Get(users.Select(n => n.Id)).Select(t => t.Id).ToList();
            File.AppendAllText(@"C:\Users\Илья\Desktop\0-1слой.txt", "Первый слой:");
            File.AppendAllLines(@"C:\Users\Илья\Desktop\0-1слой.txt", zeroLayerUsersIDs.Select(n => Convert.ToString(n)));
            File.AppendAllText(@"C:\Users\Илья\Desktop\0-1слой.txt", "Первый слой подробнее:");
            int i_1 = 0;
            foreach (var currentUser in users)
            {
                Cicle_1:
                try
                {
                    currentUser.friends = api.Friends.Get(new FriendsGetParams { UserId = zeroLayerUsersIDs[i_1] }).Select(n => n.Id).ToList();
                    Console.WriteLine("Чел с 0 уровня " + currentUser.Id + " дружит с " + currentUser.friends.Count);
                    Thread.Sleep(1000);
                    File.AppendAllText(@"C:\Users\Илья\Desktop\0-1слой.txt", zeroLayerUsersIDs[i_1].ToString());
                   
                    foreach (long friend in currentUser.friends)
                    {
                   
                        usersFirstLayer.Add(new User(friend.ToString()));
                        //File.AppendAllText(@"C:\Users\Илья\Desktop\0-1слой.txt", "Друзья:");
                        //File.AppendAllLines(@"C:\Users\Илья\Desktop\0-1слой.txt", currentUser.friends.Select(n => Convert.ToString(n)));
                    }
                    i_1++;
                }
                catch
                {
                    i_1++;
                    goto Cicle_1;
                }
                //api.Users.Get(new long[] { friend }).FirstOrDefault().FirstName + " " + api.Users.Get(new long[] { friend }).FirstOrDefault().LastName
            }

            int i_2 = 0;
            
            foreach (var currentUser in usersFirstLayer)
            {
                Cicle_2:
                try
                {
                    currentUser.friends = api.Friends.Get(new FriendsGetParams { UserId = Convert.ToInt64(usersFirstLayer[i_2].Id) }).Select(n => n.Id).ToList();
                    Console.WriteLine("Чел с 1го слоя " + currentUser.Id + " дружит с " + currentUser.friends.Count);
                    Thread.Sleep(2500);
                    foreach (long friend in currentUser.friends)
                    {
                        usersSecondLayer.Add(new User(friend.ToString()));
                    }
                    i_2++;
                }
                catch
                {
                    i_2++;
                    goto Cicle_2;
                }  
            }


        }

        class User
        {
            private string _id;
            private string _name;
            public List<long> friends = new List<long>();

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