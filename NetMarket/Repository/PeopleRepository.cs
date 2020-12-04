using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NetMarket.Entities;
using NetMarket.Models;
using NetMarket.Validations;
using NetMarket.ViewModels.Employee;

namespace NetMarket.Repository
{
    public class PeopleRepository
    {
        private NetMarketDbContext _netMarketDbContext;
        private readonly Dictionary<string, Func<string, object, string>> _actionsUpdate;
        private IMemoryCache _cache;

        public PeopleRepository(NetMarketDbContext netMarketDbContext, IMemoryCache cache)
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
            _cache = cache;
        }

        private List<People> GetPeople()
        {
            if (!_cache.TryGetValue("people", out List<People> list))
            {
                list = _netMarketDbContext.People.ToList();
                _cache.Set("people", list, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(28000)));
            }
            return list;
        }

        public People CheckData(string login, string password)
        {
            var usersForThisParameters = (from human in GetPeople()
                                          where (human.Login == login || human.Email == login) && (human.Password == password)
                                          select human).ToList();
            return usersForThisParameters.Count == 0 ? null : usersForThisParameters[0];
        }

        public void AddHuman(string login, string email, string password, string name, string surname, string middleName, string numberPhone, int roleId)
        {
            _cache.Remove("people");
            People user = new People();
            user.Login = login;
            user.Email = email;
            user.Password = password;
            user.Name = name;
            user.Surname = surname;
            user.MiddleName = middleName;
            user.PhoneNumber = numberPhone;
            user.RoleId = roleId;
            _netMarketDbContext.People.Add(user);
            _netMarketDbContext.SaveChanges();
        }

        public bool IsUniqueLogin(string login , string email)
        {
            if (_netMarketDbContext.People.Any())
            {
                var usersForThisParameter = (from human in GetPeople()
                        where human.Login == login || human.Email == email
                        select human).ToList();
                return usersForThisParameter.Count == 0;
            }

            return true;
        }

        public Guid GetUserId(string login)
        {
            return (from human in GetPeople()
                where human.Login == login
                select human).ToList()[0].Id;
        }

        public People GetUser(string login)
        {
            return (from human in GetPeople()
                where human.Login == login
                select human).ToList()[0];
        }

        public List<EmployeeViewModel> GetEmployees()
        {
            return (from human in GetPeople()
                where human.RoleId == 1 || human.RoleId == 2
                select new EmployeeViewModel
                {
                    Id = human.Id,
                    Surname = human.Surname,
                    Name = human.Name,
                    MiddleName = human.MiddleName ?? "",
                    Email = human.Email,
                    PhoneNumber = human.PhoneNumber,
                    Role = human.RoleId
                }).ToList();
        }

        public List<EmployeeViewModel> GetEmployees(string search)
        {
            return (from human in GetPeople()
                where (human.RoleId == 1 || human.RoleId == 2) && (human.Name.Contains(search) || human.Surname.Contains(search) || human.MiddleName != null && human.MiddleName.Contains(search))
                select new EmployeeViewModel
                {
                    Id = human.Id,
                    Surname = human.Surname,
                    Name = human.Name,
                    MiddleName = human.MiddleName ?? "",
                    Email = human.Email,
                    PhoneNumber = human.PhoneNumber,
                    Role = human.RoleId
                }).ToList();
        }

        public async Task<string> UpdateAsync(string login, string typeOfUpdate, string data, string additionalData)
        {
            _cache.Remove("users");
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
            if ((from human in _netMarketDbContext.People
                where human.Login == (string)newLogin
                select human).ToList().Count == 0)
            {
                var user = _netMarketDbContext.People.Find(GetUserId(login));
                user.Login = (string)newLogin;
                _netMarketDbContext.People.Update(user);
                return "good";
            }

            return "bad";
        }

        private string EmailUpdate(string login, object newEmail)
        {
            if (EmailValidation.IsValid((string)newEmail))
            {
                if ((from human in _netMarketDbContext.People
                     where human.Email == (string)newEmail
                    select human).ToList().Count == 0)
                {
                    var human = _netMarketDbContext.People.Find(GetUserId(login));
                    human.Email = (string)newEmail;
                    _netMarketDbContext.People.Update(human);
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
            if ((from human in _netMarketDbContext.People
                 where human.Login == login && human.Password == passwords[1]
                select human).ToList().Count == 1)
            {
                var human = _netMarketDbContext.People.Find(GetUserId(login));
                human.Password = passwords[0];
                _netMarketDbContext.People.Update(human);
                return "good";
            }
            return "Пароль не изменён, так как текущий пароль введён неправильно!";
        }

        private string NameUpdate(string login, object name)
        {
            var human = _netMarketDbContext.People.Find(GetUserId(login));
            human.Name = (string)name;
            _netMarketDbContext.People.Update(human);
            return "good";
        }

        private string SurnameUpdate(string login, object surname)
        {
            var human = _netMarketDbContext.People.Find(GetUserId(login));
            human.Name = (string)surname;
            _netMarketDbContext.People.Update(human);
            return "good";
        }

        private string MiddleNameUpdate(string login, object middleName)
        {
            var human = _netMarketDbContext.People.Find(GetUserId(login));
            human.MiddleName = (string)middleName;
            _netMarketDbContext.People.Update(human);
            return "good";
        }

        private string PhoneNumberUpdate(string login, object phoneNumber)
        {
            _cache.Remove("people");
            string response = "good";
            if (phoneNumber != null)
            {
                if (PhoneNumberValidation.IsValid((string)phoneNumber))
                {
                    if ((from human in _netMarketDbContext.People
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

            if (phoneNumber == null && (from human in _netMarketDbContext.People
                where human.Login == login
                select human).ToList()[0].RoleId != 3)
            {
                response = "Некорректный номер телефона!";
            }

            if (response == "good")
            {
                var human = _netMarketDbContext.People.Find(GetUserId(login));
                human.PhoneNumber = (string)phoneNumber;
                _netMarketDbContext.People.Update(human);
            }
            return response;
        }

        public async Task EmployeeRoleUpdateAsync(Guid userId, int roleId)
        {
            _cache.Remove("people");
            var human = _netMarketDbContext.People.Find(userId);
            human.RoleId = roleId;
            _netMarketDbContext.People.Update(human);
            await _netMarketDbContext.SaveChangesAsync();
        }

        public void ClearCache()
        {
            _cache.Remove("people");
        }
    }
}