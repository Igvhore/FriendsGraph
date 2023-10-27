using VkNet;

namespace FriendsFriend
{
    internal class User
    {
        private string _id;
        private long _id_as_number;
        private string _name;

        public List<User> friends = new List<User>();
        public string Id { get { return _id; } }
        public string Name { get { return _name; } }
        public long Id_as_number { get { return _id_as_number; } set { _id_as_number = value; } }

        public User()
        {
            _id = "NONE";
            _id_as_number = 0;
            _name = "EMPTY";
            friends = new List<User>();
        }
        public User(string id)
        {
            _id = id;
            _id_as_number = 0;
            _name = "EMPTY";
            friends = new List<User>();
        }
        public User(long id)
        {
            _id = "";
            _id_as_number = id;
            _name = "EMPTY";
            friends = new List<User>();
        }
        public User(string id, string name)
        {
            _id = id;
            _id_as_number = 0;
            _name = name;
            friends = new List<User>();
        }
        public User(string id, string name, List<User> Friends)
        {
            _id = id;
            _id_as_number = 0;
            _name = name;
            friends = Friends;
        }

        public void SetName(string NewName) => _name = NewName;
        public void SetID(string NewID) => _id = NewID;
        public void SetNumberID(long NewNumberID) => _id_as_number = NewNumberID;
        public string CorrectVkId(string id) => id.Trim('@').Replace("https://vk.com/", "");
        public static void NormilizeID(List<User> users, VkApi api)
        {
            foreach (User user in users)
            {
                try
                {
                    user.Id_as_number = api.Users.Get(new List<string>() { user.Id }).ToList().First().Id;
                }
                catch (InvalidOperationException e)
                {
                    user.Id_as_number = 0;
                }
                finally
                {
                    Thread.Sleep(400);
                }
            }
        }
       
    }
}
