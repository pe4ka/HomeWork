﻿using System.Linq;
using System.Threading.Tasks;
using HomeWork.Models;
using HomeWork.Repositories;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Data.Entity;
using X.PagedList;
using System.Data.Entity.Core.Objects;

namespace HomeWork.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        //Add
        #region Add
        public async Task<bool> AddUserAsync(RegisterViewModel model)
        {
            string nameCountry = string.Empty;
            string nameCity = string.Empty;

            if (!IsRelationship(model.City, model.Country, out nameCity, out nameCountry)) return false;
        
            var newUser = new UserInfo()
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Age = model.Age,
                Country = nameCountry,
                City = nameCity,
                Details = model.Detalis,
                PhoneNumber = model.PhoneNumber
            };

            await userRepository.AddAsync(newUser);

            return true;
        }
        #endregion

        //Delete
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await userRepository.FindUserAsync(id);
            if (user == null) return false;

            await userRepository.DeleteAsync(id);
            return true;
        }
        //Get Cities
        public async Task<SelectList> GetCitiesAsync(int idCountry, string selected)
        {
            var listCities = await userRepository.GetCities().OrderBy(c => c.Name).
                Where(i => i.Country_ID.ID == idCountry).
                ToListAsync();

            if (!string.IsNullOrEmpty(selected))
            {
                var temp = listCities.First();
                var indexSelect = listCities.FindIndex(c => c.Name == selected);
                listCities[0] = listCities.FirstOrDefault(c => c.Name == selected);
                listCities[indexSelect] = temp;
            }

            var selectList = new SelectList(listCities, "ID", "Name");

            return selectList;

        }
        //Get Countries
        public async Task<SelectList> GetCountriesAsync(string selected)
        {
            var listCountries = await userRepository.GetCountries().OrderBy(c => c.NameCountry).ToListAsync();

            if (!string.IsNullOrEmpty(selected))
            {
                var temp = listCountries.First();
                var indexSelect = listCountries.FindIndex(c => c.NameCountry == selected);
                listCountries[0] = listCountries.FirstOrDefault(c => c.NameCountry == selected);
                listCountries[indexSelect] = temp;
            }

            var selectList = new SelectList(listCountries, "ID", "NameCountry");

            return selectList;
        }



        public async Task<UserInfo> GetUserAsync(int id)
        {
            var user = await userRepository.FindUserAsync(id);

            return user;
        }

        public async Task<IEnumerable<UserInfo>> GetUsersForPageList(int page, int pageSize, string sort, string search)
        {
            IEnumerable<UserInfo> users = null;

            if (!string.IsNullOrEmpty(search))
            {
                users = await userRepository.Get.OrderBy(p => p.Id).Where(s => s.FirstName.Contains(search)
                                        || s.LastName.Contains(search)).ToPagedListAsync(page, pageSize);
            }
            else
            {
                switch (sort)
                {
                    case "name_desc":
                        users = await userRepository.Get.OrderByDescending(p => p.FirstName).ToPagedListAsync(page, pageSize);
                        break;
                    default:  //ascending
                        users = await userRepository.Get.OrderBy(p => p.FirstName).ToPagedListAsync(page, pageSize);
                        break;
                        //...
                }
            }

            return users;
        }
        //UPDATE
        #region Update
        public async Task<bool> UpdateUserAsync(EditViewModel model)
        {

            string nameCountry = string.Empty;
            string nameCity = string.Empty;
          
            if (!IsRelationship(model.City, model.Country, out nameCity, out nameCountry)) return false;

            var user = await userRepository.FindUserAsync(model.Id);

            if (user == null) return false;

            UserInfo updateUser = new UserInfo()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Age = model.Age,
                PhoneNumber = model.PhoneNumber,
                Email = user.Email,
                Password = user.Password,
                City = nameCity,
                Country = nameCountry,
                Details = model.Detalis,
            };

            await userRepository.UpdateAsync(updateUser);

            return true;
        }
        #endregion

        public async Task<int> CountUsersAsync()
        {
            return await userRepository.Get.CountAsync();
        }

        private bool IsRelationship(int idCity, int idCountry, out string nameCity, out string nameCountry)
        {
            nameCity = string.Empty;
            nameCountry = string.Empty;

            var country = userRepository.GetCountries().Where(c => c.ID == idCountry).Any();

            if (!country) return false;

            bool result = userRepository.GetCities().Where(n => n.ID == idCity).Any(id => id.Country_ID.ID == idCountry);

            if (result)
            {
                nameCountry = userRepository.GetCountries().
                Where(c => c.ID == idCountry).
                Single().NameCountry;

                nameCity = userRepository.GetCities().
                Where(c => c.ID == idCity).
                Single().Name;

                return true;
            }
            return false;
        }

    }
}