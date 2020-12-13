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
    /// <summary>
    /// Класс-репозиторий для работы с пользователями на уровне доступа к данным
    /// </summary>
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

        /// <summary>
        /// Метод проверки логина и пароля для идентификации
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>При успешной проверки - данные пользователя, в потивном случае - null</returns>
        public People CheckData(string login, string password)
        {
            var usersForThisParameters = (from human in GetPeople()
                                          where (human.Login == login || human.Email == login) && (human.Password == password)
                                          select human).ToList();
            return usersForThisParameters.Count == 0 ? null : usersForThisParameters[0];
        }

        /// <summary>
        /// Метод добваления нового пользователя
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="email">Email</param>
        /// <param name="password">Пароль</param>
        /// <param name="name">Имя</param>
        /// <param name="surname">Фамилия</param>
        /// <param name="middleName">Отчество</param>
        /// <param name="numberPhone">Номер телефона</param>
        /// <param name="roleId">ID права доступа</param>
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

        /// <summary>
        /// Метод проверки на уникальность логина и email
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="email">Email</param>
        /// <returns>True, если данные уникальны, иначе - false</returns>
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

        /// <summary>
        /// Метод получения ID пользователя
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns>ID пользователя</returns>
        public Guid GetUserId(string login)
        {
            return (from human in GetPeople()
                where human.Login == login
                select human).ToList()[0].Id;
        }

        /// <summary>
        /// Метод получения данных о клиенте
        /// </summary>
        /// <param name="login">Логин клиента</param>
        /// <returns>Клиент</returns>
        public People GetUser(string login)
        {
            return (from human in GetPeople()
                where human.Login == login
                select human).ToList()[0];
        }

        /// <summary>
        /// Метод получения списка персонала интернет-магазина
        /// </summary>
        /// <returns>Список персонала</returns>
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

        /// <summary>
        /// Метод получения списка персонала интернет-магазина (вызывается при поиске)
        /// </summary>
        /// <param name="search">Поиск</param>
        /// <returns>Список персонала</returns>
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

        /// <summary>
        /// Метод обновления данных о пользователе
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="typeOfUpdate">Тип обновления (login, password, email, name, surname, middleName, phoneNumber)</param>
        /// <param name="data">Обновленные данные</param>
        /// <param name="additionalData">Дополнительные данные (при изменении пароля нужен текущий для проверки)</param>
        /// <returns>Возвращается Good, если изменения успешно сохранены, иначе - bad</returns>
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

        /// <summary>
        /// Метод обновления прав доступа персонала
        /// </summary>
        /// <param name="userId">ID персонала</param>
        /// <param name="roleId">ID новых прав доступа</param>
        /// <returns></returns>
        public async Task EmployeeRoleUpdateAsync(Guid userId, int roleId)
        {
            _cache.Remove("people");
            var human = _netMarketDbContext.People.Find(userId);
            human.RoleId = roleId;
            _netMarketDbContext.People.Update(human);
            await _netMarketDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Метод очистки кеша пользователей
        /// </summary>
        public void ClearCache()
        {
            _cache.Remove("people");
        }
    }
}