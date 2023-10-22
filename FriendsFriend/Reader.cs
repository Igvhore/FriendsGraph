using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace FriendsFriend
{
    internal class Reader
    {
        public static List<User> ReadUserInfo(List<User> users, string dataSetPath)
        {
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
            
            return users;
        }
    }
}