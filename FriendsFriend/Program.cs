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

            List<long> zeroLayerUsersIDs = api.Users.Get(users.Select(n => n.Id)).Select(t => t.Id).ToList();


            for (int i = 0; i < zeroLayerUsersIDs.Count; i++)
                users[i].friends = api.Friends.Get(new FriendsGetParams { UserId = zeroLayerUsersIDs[i]}).Select(n => n.Id).ToList();
        }

    }
}