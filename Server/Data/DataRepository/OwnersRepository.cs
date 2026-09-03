using Core.Models;
using Core.Repository;
using Core.Resources;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataRepository
{
    public class OwnersRepository : IOwnersRepository
    {
        private readonly Context _dbContext;
        public OwnersRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Owners?> Add(Owners obj)
        {
            await _dbContext.Owners.AddAsync(obj);
            await _dbContext.SaveChangesAsync();
            return obj;
        }
        public async Task<bool> Delete(int id)
        {
                var owner = await _dbContext.Owners.FindAsync(id);
                if (owner == null)
                {
                    return false; 
                }

                _dbContext.Owners.Remove(owner);

                await _dbContext.SaveChangesAsync();
                return true;
        }
        public async Task<List<Owners?>> GetAll()
        {
            return await _dbContext.Owners.ToListAsync<Owners?>();
        }
        public async Task<Owners?> GetById(int id)
        {
            return await _dbContext.Owners.FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<Owners?> Update(int id, Owners obj)
        {
            var existingOwner = await _dbContext.Owners.FindAsync(id);

            if (existingOwner == null) return null;

            existingOwner.Email = obj.Email;
            existingOwner.FirstName = obj.FirstName;
            existingOwner.LastName = obj.LastName;
            existingOwner.PhoneNumber = obj.PhoneNumber;

            // מעדכנים את הסיסמה רק אם נשלחה סיסמה חדשה בפועל - אחרת עדכון פרופיל רגיל
            // (כמו שינוי שם/טלפון) היה מאפס את הסיסמה הקיימת של המשתמש.
            if (!string.IsNullOrWhiteSpace(obj.Password))
            {
                existingOwner.Password = obj.Password;
            }

            await _dbContext.SaveChangesAsync();
            return existingOwner;
        }
        public async Task<Owners?> GetByEmail(string email)
        {
            return await _dbContext.Owners
                .FirstOrDefaultAsync(o => o.Email == email);
        }
    }
}
