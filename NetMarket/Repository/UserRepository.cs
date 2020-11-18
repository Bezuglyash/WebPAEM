using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetMarket.Entities;
using NetMarket.Models;

namespace NetMarket.Repository
{
    public class UserRepository
    {
        private NetMarketDbContext _netMarketDbContext;
        private readonly Dictionary<string, Func<string, object, string>> _actionsUpdate;

        public UserRepository(NetMarketDbContext netMarketDbContext)
        {
            _netMarketDbContext = netMarketDbContext;
            _actionsUpdate = new Dictionary<string, Func<string, object, string>>();
            _actionsUpdate.Add("login", LoginUpdate);
            _actionsUpdate.Add("email", EmailUpdate);
            _actionsUpdate.Add("password", PasswordUpdate);
            _actionsUpdate.Add("name", NameUpdate);
            _actionsUpdate.Add("surname", SurnameUpdate);
            _actionsUpdate.Add("middleName", MiddleNameUpdate);
            _actionsUpdate.Add("phoneNumber", PhoneNumberUpdate);
        }

        public User CheckData(string login, string password)
        {
            var usersForThisParameters = (from human in _netMarketDbContext.Users
                                          where (human.Login == login || human.Email == login) && (human.Password == password)
                                          select human).ToList();
            return usersForThisParameters.Count == 0 ? null : usersForThisParameters[0];
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

        public bool IsUniqueLogin(string login , string email)
        {
            if (_netMarketDbContext.Users.Any())
            {
                var usersForThisParameter = (from human in _netMarketDbContext.Users
                        where human.Login == login && human.Email == email
                        select human).ToList();
                return usersForThisParameter.Count == 0;
            }

            return true;
        }

        public Guid GetUserId(string login)
        {
            return (from human in _netMarketDbContext.Users
                where human.Login == login
                select human).ToList()[0].Id;
        }

        public User GetUser(string login)
        {
            return (from human in _netMarketDbContext.Users
                where human.Login == login
                select human).ToList()[0];
        }

        public async Task<string> UpdateAsync(string login, string type, string data, string additionalData)
        {
            string response;
            if (additionalData == null)
            {
                response = _actionsUpdate[type](login, data);
            }
            else
            {
                response = _actionsUpdate[type](login, new List<string>
                {
                    data,
                    additionalData
                });
            }
            await _netMarketDbContext.SaveChangesAsync();
            return response;
        }

        private string LoginUpdate(string login, object newLogin)
        {
            if ((from human in _netMarketDbContext.Users
                where human.Login == (string)newLogin
                select human).ToList().Count == 0)
            {
                var user = _netMarketDbContext.Users.Find(GetUserId(login));
                user.Login = (string)newLogin;
                _netMarketDbContext.Users.Update(user);
                return "good";
            }

            return "bad";
        }

        private string EmailUpdate(string login, object newEmail)
        {
            if ((from human in _netMarketDbContext.Users
                where human.Email == (string)newEmail
                 select human).ToList().Count == 0)
            {
                var user = _netMarketDbContext.Users.Find(GetUserId(login));
                user.Email = (string)newEmail;
                _netMarketDbContext.Users.Update(user);
                return "good";
            }

            return "bad";
        }

        private string PasswordUpdate(string login, object passwordData)
        {
            return "";
        }

        private string NameUpdate(string login, object name)
        {
            return "";
        }

        private string SurnameUpdate(string login, object surname)
        {
            return "";
        }

        private string MiddleNameUpdate(string login, object middleName)
        {
            return "";
        }

        private string PhoneNumberUpdate(string login, object phoneNumber)
        {
            return "";
        }

        private void UpdateUserDb(User userRewrite)
        {
            //User user = _netMarketDbContext.Users.Find(GetId());
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