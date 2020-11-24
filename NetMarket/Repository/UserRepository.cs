using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetMarket.Entities;
using NetMarket.Models;
using NetMarket.Validations;

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

        public async Task<string> UpdateAsync(string login, string typeOfUpdate, string data, string additionalData)
        {
            string response;
            if (additionalData == null)
            {
                response = _actionsUpdate[typeOfUpdate](login, data);
            }
            else
            {
                response = _actionsUpdate[typeOfUpdate](login, new List<string>
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
            if (EmailValidation.IsValid((string)newEmail))
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

                return "Этот email уже занят!";
            }
            return "Некорректный email!";
        }

        private string PasswordUpdate(string login, object passwordData)
        {
            var passwords = passwordData as List<string>;
            passwords[0] = Encryption.Encryption.GetHash(passwords[0]);
            passwords[1] = Encryption.Encryption.GetHash(passwords[1]);
            if ((from human in _netMarketDbContext.Users
                where human.Login == login && human.Password == passwords[1]
                select human).ToList().Count == 1)
            {
                var user = _netMarketDbContext.Users.Find(GetUserId(login));
                user.Password = passwords[0];
                _netMarketDbContext.Users.Update(user);
                return "good";
            }
            return "Пароль не изменён, так как текущий пароль введён неправильно!";
        }

        private string NameUpdate(string login, object name)
        {
            var user = _netMarketDbContext.Users.Find(GetUserId(login));
            user.Name = (string)name;
            _netMarketDbContext.Users.Update(user);
            return "good";
        }

        private string SurnameUpdate(string login, object surname)
        {
            var user = _netMarketDbContext.Users.Find(GetUserId(login));
            user.Name = (string)surname;
            _netMarketDbContext.Users.Update(user);
            return "good";
        }

        private string MiddleNameUpdate(string login, object middleName)
        {
            var user = _netMarketDbContext.Users.Find(GetUserId(login));
            user.MiddleName = (string)middleName;
            _netMarketDbContext.Users.Update(user);
            return "good";
        }

        private string PhoneNumberUpdate(string login, object phoneNumber)
        {
            string response = "good";
            if (phoneNumber != null)
            {
                if (PhoneNumberValidation.IsValid((string)phoneNumber))
                {
                    if ((from human in _netMarketDbContext.Users
                        where human.PhoneNumber == (string)phoneNumber
                        select human).ToList().Count != 0)
                    {
                        response = "Этот номер телефона уже занят!";
                    }
                }
                else
                {
                    response = "Некорректный номер телефона!";
                }
            }
            if (response == "good")
            {
                var user = _netMarketDbContext.Users.Find(GetUserId(login));
                user.PhoneNumber = (string)phoneNumber;
                _netMarketDbContext.Users.Update(user);
            }
            return response;
        }
    }
}