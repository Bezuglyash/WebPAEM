using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetMarket.Entities;
using NetMarket.Models;

namespace NetMarket.Repository
{
    public class UserRepository
    {
        private NetMarketDbContext _netMarketDbContext;
        private static User _userInput;

        public UserRepository(NetMarketDbContext netMarketDbContext)
        {
            _netMarketDbContext = netMarketDbContext;
        }

        public bool IsTrueData(string login, string password)
        {
            var usersForThisParameters = (from human in _netMarketDbContext.Users
                                          where (human.Login == login || human.Email == login) && (human.Password == password)
                                          select human).ToList();
            _userInput = usersForThisParameters.Count != 0 ? usersForThisParameters[0] : null;
            return _userInput != null;
        }

        public string GetFullName()
        {
            return _userInput.Name + " " + _userInput.Surname;
        }

        public string GetLogin()
        {
            return _userInput.Login;
        }

        public string GetPassword()
        {
            return _userInput.Password;
        }

        public string GetName()
        {
            return _userInput.Name;
        }

        public string GetSurname()
        {
            return _userInput.Surname;
        }

        public string GetNumberPhone()
        {
            return _userInput.PhoneNumber;
        }

        public string GetEmail()
        {
            return _userInput.Email;
        }

        public void AddUser(string login, string email, string password, string name, string surname, string middleName, string numberPhone, int roleId)
        {
            User user = new User();
            user.Login = login;
            user.Email = email;
            user.Password = password;
            user.Name = name;
            user.Surname = surname;
            user.MiddleName = middleName;
            user.PhoneNumber = numberPhone;
            user.RoleId = roleId;
            _netMarketDbContext.Users.Add(user);
            _netMarketDbContext.SaveChanges();
        }

        /*public void UpdateUser(string login, string password, string name, string surname, string numberPhone, string email)
        {
            int index = GetIndexById(GetId());
            _users[index].Login = login;
            _users[index].Password = password;
            _users[index].Name = name;
            _users[index].Surname = surname;
            _users[index].PhoneNumber = numberPhone;
            _users[index].Email = email;
            _userInput = _users[index];
            Task.Run(() => UpdateUserDb(_users[index]));
        }*/

        public bool IsUniqueLogin(string text)
        {
            if (_netMarketDbContext.Users.Any())
            {
                var usersForThisParameter = (from human in _netMarketDbContext.Users
                        where human.Login == text || human.Email == text
                        select human).ToList();
                return usersForThisParameter.Count == 0 ? true : false;
            }

            return true;
        }

        private void UpdateUserDb(User userRewrite)
        {
           // User user = _netMarketDbContext.Users.Find(GetId());
            //user = userRewrite;
            //_netMarketDbContext.SaveChanges();
        }

        /*private int GetIndexById(int id)
        {
            int index = 0;
            foreach (var user in _users)
            {
                if (user.Id == id)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }*/
    }
}