using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;
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
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace FriendsFriend
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string dataSetPath = @"data\IDList.csv";
            List<User> users = new List<User>();
            List<User> usersFirstLayer = new List<User>();
            List<User> usersSecondLayer = new List<User>();
            VkApi api = new VkApi();
            string token1 = File.ReadAllText(@"C:\Users\Илья\Source\Repos\Igwhore\FriendsGraph\FriendsFriend\data\VKToken.txt");

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
            Console.WriteLine("Файл запарсен");
            api.Authorize(new ApiAuthParams
            {
                AccessToken = File.ReadAllText(@"C:\Users\Илья\Source\Repos\Igwhore\FriendsGraph\FriendsFriend\data\VKToken.txt") //@"data\VKToken.txt"
            });
            Console.WriteLine("Пользователь авторизован");

            List<long> zeroLayerUsersIDs = api.Users.Get(users.Select(n => n.Id)).Select(t => t.Id).ToList();
            File.AppendAllText(@"C:\Users\Илья\Desktop\0-1слой.txt", "Первый слой:");
            File.AppendAllLines(@"C:\Users\Илья\Desktop\0-1слой.txt", zeroLayerUsersIDs.Select(n => Convert.ToString(n)));
            File.AppendAllText(@"C:\Users\Илья\Desktop\0-1слой.txt", "Первый слой подробнее:");

            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            static async Task ProcessRepositoriesAsync(HttpClient client, string token1, long id)
            {
                var json = await client.GetStringAsync(
                    string.Format("https://api.vk.com/method/friends.get?user_id={0}&access_token={1}&v=5.131 HTTP/1.1", id, token1));
                Console.WriteLine(json);
            }


            foreach (var userId in zeroLayerUsersIDs)
            {
                Console.WriteLine(userId);

                await ProcessRepositoriesAsync(client, token1, userId);
                Thread.Sleep(100);
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
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
