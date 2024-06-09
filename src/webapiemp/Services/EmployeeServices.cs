using System.Linq;
using Microsoft.EntityFrameworkCore;
using webapiemp.Models;

namespace webapiemp.Services
{
    public class EmployeeServices
    {
        public static async Task<List<User>> GetUsers(ApplicationDbContext context)
        {
            return await context.Users.ToListAsync();
        }

        public static async Task<User> GetUser(int id, ApplicationDbContext context)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public static async Task AddUser(User user, ApplicationDbContext context)
        {
            context.Users.Add(new User
            {
                //Name = user.Name,
                Email = user.Email,
            });
            await context.SaveChangesAsync();
        }

        public static async Task UpdateUser(User user, ApplicationDbContext context)
        {
            var dbuser = await GetUser(user.Id, context);
            //dbuser.Name = user.Name;
            dbuser.Email = user.Email;
            await context.SaveChangesAsync();
        }

        public static async Task DeleteUser(int id, ApplicationDbContext context)
        {
            var user = await GetUser(id, context);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        
    }
}
