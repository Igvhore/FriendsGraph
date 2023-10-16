using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendsFriend
{
    internal class User
    {
        private string _id;
        private string _name;
        public List<long> friends;

        public string Id { get { return _id; } }
        public string Name { get { return _name; } }

        public User()
        {
            _id = "NONE";
            _name = "EMPTY";
            friends = new List<long>();
        }
        public User(string id)
        {
            _id = id;
            _name = "EMPTY";\
            friends = new List<long>();
        }
        public User(string id, string name)
        {
            _id = id;
            _name = name;
            friends = new List<long>();
        }
        public User(string id, string name, List<long> friends)
        {
            _id = id;
            _name = name;
            friends = new List<long>();
        }

        public void SetName(string NewName) => _name = NewName;
        public void SetID(string NewID) => _id = NewID;
        public string CorrectVkId(string id) => id.Trim('@').Replace("https://vk.com/", "");

    }
}
