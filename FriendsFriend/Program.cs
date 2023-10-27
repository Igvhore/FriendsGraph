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
                AccessToken = token
            });

            static async Task<List<User>> GetFriendsAsync(HttpClient client, string token, long id)
            {
                string querry = await client.GetStringAsync(string.Format($"https://api.vk.com/method/friends.get?user_id={id}&access_token={token}&v=5.131 HTTP/1.1"));
                querry = querry.Remove(0, 34);
                querry = querry.Replace("]}}", "");
                Console.WriteLine($"{querry}");
                List<User> friends = new List<User>();
                string[] s = querry.Split(',');
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

            Console.WriteLine("Пользователь авторизован");
            User.NormilizeID(users, api);
            Console.WriteLine("ID нормализованы");
            users = users.Where(n => n.Id_as_number != 0).ToList();
            Console.WriteLine("Список создан");        

            int i = 0;

            foreach (User user in users)
            {
                user.friends = await GetFriendsAsync(client, token, user.Id_as_number);
                Thread.Sleep(300);
                foreach (User friend in user.friends)
                {
                    File.AppendAllText(@"data\TwoLayers.txt", $"{user.Id_as_number}:{friend.Id_as_number}\n");
                    i++;
                }
                    
            }
            Console.Write("Было записано строк на 0-1 уровне: ");
            Console.Write($"{i}\n");
            Console.Write("Было юзеров 0-1: ");
            Console.Write($"{users.Count}\n");

            users = users.Where(n => n.friends.Count != 0).ToList();

            Console.Write("Стало юзеров 0-1: ");
            Console.Write($"{users.Count}\n");

            List<long> usersOfSecondLayer = new List<long>();

            i= 0;
            foreach (User user in users)
            {
                foreach (User friend in user.friends)
                {
                    friend.friends = await GetFriendsAsync(client, token, friend.Id_as_number);
                    Thread.Sleep(300);
                    foreach (User f in friend.friends)
                    {
                        if (friend.friends.Count != 0)
                        {
                            File.AppendAllText(@"data\TwoLayers.txt", $"{friend.Id_as_number}:{f.Id_as_number}\n");
                            usersOfSecondLayer.Add(f.Id_as_number);
                            i++;
                        }
                    }
                        
                }
            }
            Console.WriteLine("Было записано строк на 1-2 уровне: ");
            Console.Write($"{i}\n");

            i = 0;
            int a = 0;
            foreach (User user in users)
            {
                foreach (User friend in user.friends)
                {
                    a++;
                    foreach (User f in friend.friends)
                    {
                        
                        f.friends = await GetFriendsAsync(client, token, f.Id_as_number);
                        Thread.Sleep(300);
                        foreach(User fr3 in f.friends)
                        {
                            if ((f.friends.Count != 0) && (usersOfSecondLayer.Contains(fr3.Id_as_number)))
                            {
                                File.AppendAllText(@"data\TwoLayers.txt", $"{f.Id_as_number}:{fr3.Id_as_number}\n");
                                i++;
                            }
                                
                        }
                    }
                    Console.WriteLine($"Был записан 3 слой для {a} из {users.Count}");
                }
            }

            Console.Write("Было записано строк на 2-3 уровне: ");
            Console.Write(i);
        }
    }
}