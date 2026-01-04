using AuthWebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthWebAPI.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        //add user set aka user table
        public DbSet<User> Users{ get; set; }         
    }
}