using VkNet;
using VkNet.Model;
using System.Net.Http.Headers;

namespace FriendsFriend
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string dataSetPath = @"data\IDList.csv";
            List<User> users = new List<User>();
            VkApi api = new VkApi();
            string token = File.ReadAllText(@"data\VKToken.txt");


            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            users = Reader.ReadUserInfo(users, dataSetPath);
            Console.WriteLine("Файл прочитан");

            api.Authorize(new ApiAuthParams
            {
                AccessToken = File.ReadAllText(@"data\VKToken.txt")
            });

            Console.WriteLine("Пользователь авторизован");
            User.NormilizeID(users, api);
            Console.WriteLine("ID нормализованы");
            users = users.Where(n => n.Id_as_number != 0).ToList();
            Console.WriteLine("Список создан");
            static async Task<List<User>> GetFriendsAsync(HttpClient client, string token, long id)
            {
                string querry = await client.GetStringAsync(string.Format("https://api.vk.com/method/friends.get?user_id={0}&access_token={1}&v=5.131 HTTP/1.1", id, token));
                querry = querry.Remove(0, 34);
                querry = querry.Replace("]}}", "");
                List<User> friends = new List<User>();
                string[] s = querry.Split(',');
                //File.AppendAllText(@"D:\VisualStudio\Projects\FriendsVK\FriendsGraph\AllLayers.txt", "Пользователь: ");
                //File.AppendAllText(@"D:\VisualStudio\Projects\FriendsVK\FriendsGraph\AllLayers.txt", id.ToString());
                //File.AppendAllText(@"D:\VisualStudio\Projects\FriendsVK\FriendsGraph\AllLayers.txt", "\nДружит с:\n");
                foreach (string c in s)
                {
                    try
                    {
                        
                        friends.Add(new User(Int64.Parse(c)));
                    }
                    catch { }
                }
                return friends;
            }

            foreach (var user in users)
            {
                user.friends = await GetFriendsAsync(client, token, user.Id_as_number);
                File.AppendAllText(@"D:\VisualStudio\Projects\FriendsVK\FriendsGraph\AllNewLayers.txt", user.Id_as_number.ToString() + " , ");
                File.AppendAllLines(@"D:\VisualStudio\Projects\FriendsVK\FriendsGraph\AllNewLayers.txt", user.friends.Select(n => Convert.ToString(n.Id_as_number)));
                Thread.Sleep(45);
            }

            /*foreach (User user in users)
            {
                foreach (var friend in user.friends)
                    File.AppendAllText(@"data\AllLayers.txt", $"{user.Id_as_number}:{friend.Id_as_number}\n");
            }*/

            users = users.Where(n => n.friends.Count != 0).ToList();
            foreach (var user in users)
            {
                foreach (var friend in user.friends)
                {
                    friend.friends = await GetFriendsAsync(client, token, friend.Id_as_number);
                    File.AppendAllLines(@"D:\VisualStudio\Projects\FriendsVK\FriendsGraph\AllNewLayers.txt", friend.friends.Select(n => Convert.ToString(n.Id_as_number)));
                    Thread.Sleep(45);
                }
            }

            /* foreach (User user in users)
             {
                 foreach (var friend in user.friends)
                 {
                     foreach (var f in friend.friends)
                         File.AppendAllText(@"data\AllLayers.txt", $"{friend.Id_as_number}:{f.Id_as_number}\n");
                 }
             }*/

            foreach (User user in users)
            {
                File.AppendAllText(@"data\Vertex.txt", $"{user.Id_as_number.ToString()}\n");
                foreach (var friend in user.friends)
                {
                    File.AppendAllText(@"data\Vertex.txt", $"{friend.Id_as_number.ToString()}\n");

                    foreach (var f in friend.friends)
                        File.AppendAllText(@"data\Vertex.txt", $"{f.Id_as_number.ToString()}\n");
                }
            }
        }
    }
}

/*
 * 
*/