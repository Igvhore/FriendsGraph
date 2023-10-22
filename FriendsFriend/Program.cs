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
            users = Reader.ReadUserInfo(users,dataSetPath);
            Console.BackgroundColor = ConsoleColor.Green;
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
            Console.BackgroundColor = ConsoleColor.White;
            static async Task<List<User>> ProcessRepositoriesAsync(HttpClient client, string token, long id)
            {
                string querry = await client.GetStringAsync(string.Format("https://api.vk.com/method/friends.get?user_id={0}&access_token={1}&v=5.131 HTTP/1.1", id, token));
                querry = querry.Remove(0, 34);
                querry = querry.Replace("]}}", "");
                List<User> friends = new List<User>();
                string[] s = querry.Split(',');
                //ТУТ СВОЮ ДИРЕКТОРИЮ УКАЖИ - ПОЛЬЗОВАТЕЛЬ ЗАПИСЫВАЕТСЯ, ДРУЗЬЯ НЕТ. А ЕЩЕ ЭТА ХУЙНЯ ОЧ ТОРМОЗИТ ЗАПРОСЫ. СЕЛЕКТ ТОРМОЗИТ МЕНЬШЕ ВСЕГО, НО НЕ ЗАВЕЛСЯ
                File.AppendAllText(@"C:\Users\Илья\Desktop\0-1слой.txt", "Пользователь:");
                File.AppendAllText(@"C:\Users\Илья\Desktop\0-1слой.txt", id.ToString());
                File.AppendAllText(@"C:\Users\Илья\Desktop\0-1слой.txt", "Дружит с:");
                foreach (string c in s)
                {
                    try
                    {
                        friends.Add(new User(Int64.Parse(c)));
                        //НИ ЧЕРЕЗ СЕЛЕКТ, НИ ТАК ЭТА СТРОЧКА НЕ ПАШЕТ
                       // File.AppendAllText(@"C:\Users\Илья\Desktop\0-1слой.txt", friends[friends.Count].Id);
                    }
                    catch { }
                }
               //ЭТА СТРОЧКА ТОЖЕ НЕ ПАШЕТ
               // File.AppendAllLines(@"C:\Users\Илья\Desktop\0-1слой.txt", friends.Select(n => Convert.ToString(n.Id_as_number))); // 
                return friends;
            }

            foreach (var user in users)
            {
                user.friends = await ProcessRepositoriesAsync(client, token, user.Id_as_number);
                Thread.Sleep(45);
            }

            users = users.Where(n => n.friends.Count != 0).ToList();
            int i = 1;
            foreach (var user in users)
            {
                Console.WriteLine("~~~~~~~~~~1 слой~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                foreach (var friend in user.friends)
                {
                    Console.WriteLine("Получен пользователь 1го слоя под номером: {0}", i);
                    friend.friends = await ProcessRepositoriesAsync(client, token, friend.Id_as_number);
                    Thread.Sleep(45);
                    i++;
                }
            }
            Console.WriteLine(i);
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            i = 1;
            foreach (var user in users)
            {
                Console.WriteLine("~~~~~~~~~~~~2 слой~~~~~~~~~~~~~~~~~~~~~~~~~");
                foreach (var friend in user.friends)
                {
                    Console.WriteLine("Получен пользователь 2го слоя под номером: {0}", i);
                    friend.friends = await ProcessRepositoriesAsync(client, token, friend.Id_as_number);
                    Thread.Sleep(45);
                    i++;
                }
            }

            foreach (var user in users)
            {
                foreach(var friend in user.friends)
                {
                    foreach(var f in friend.friends)
                        Console.WriteLine(f.Id_as_number);
                }
            }
        }
    }
}

/*
 * 4520686918,hezzerd
4520686962,igwhore
4520686996,531619927
4520687102,194848002
4520687265,mra4naya_jiu4hoctb
4520687484,https://vk.com/id_yasno
4520687874,alex_bayir
4520688223,308412461
452068827,232210943
4520688804,https://vk.com/p_i_n_a_c_o_l_a_d_a
4520689685,madbunny1
4520692516,139939428
4520699518,scndjk
4520703117,54705450
4520742024,https://vk.com/valdrg
4520844951,id_totktosmog
4520860352,totara_jokiyskiy
4520870776,alonepomahtik
4520977175,315590903
4521085143,Ye1isha
4521150571,ensssomhet
4520687928,https://vk.com/kvonker
452069765,denilai
,@id184267947
*/