﻿using VkNet;
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
            
            api.Authorize(new ApiAuthParams
            {
                AccessToken = File.ReadAllText(@"data\VKToken.txt")
            });

            User.NormilizeID(users, api);

            users = users.Where(n => n.Id_as_number != 0).ToList();

            static async Task<List<User>> ProcessRepositoriesAsync(HttpClient client, string token, long id)
            {
                string querry = await client.GetStringAsync(string.Format("https://api.vk.com/method/friends.get?user_id={0}&access_token={1}&v=5.131 HTTP/1.1", id, token));
                querry = querry.Remove(0, 34);
                querry = querry.Replace("]}}", "");
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

            foreach (var user in users)
            {
                user.friends = await ProcessRepositoriesAsync(client, token, user.Id_as_number);
                Thread.Sleep(500);
            }

            users = users.Where(n => n.friends.Count != 0).ToList();

            foreach (var user in users)
            {
                foreach (var friend in user.friends)
                {
                    friend.friends = await ProcessRepositoriesAsync(client, token, friend.Id_as_number);
                    Thread.Sleep(500);
                }
            }

            foreach (var user in users)
            {
                foreach (var friend in user.friends)
                {
                    friend.friends = await ProcessRepositoriesAsync(client, token, friend.Id_as_number);
                    Thread.Sleep(500);
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