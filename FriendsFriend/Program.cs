namespace FriendsFriend
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }

    class User
    {
        private string _id;
        private string _name;

        public string Id { get { return _id; } }
        public string Name { get { return _name; } }
        
        public User()
        {
            _id = "NONE";
            _name = "EMPTY";
        }

        public User(string id, string name)
        {
            _id = id;
            _name = name;
        }

        public void SetName (string NewName) => _name = NewName;
        public void SetID(string NewID) => _id = NewID;

    }
}