using Microsoft.EntityFrameworkCore;
using SpecEntityNetCore3.Domain;
using SpecEntityNetCore3.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecEntityNetCore3
{
    public class DBUser
    {
        public int UserId { get; set; }
        public string  Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime Birthday { get; set; }
    }

    public class Repository : IRepository
    {
        private UserDbContext _context;

        public Repository(UserDbContext context)
        {
            _context = context;
        }

        public void Add(User user)
        {
            var dbUser = new DBUser()
            {
                Name = user.Name,
                Gender = user.Gender,
                Birthday = user.Birthday
            };

            _context.Users.Add(dbUser);
            _context.SaveChanges();
        }


        public IReadOnlyCollection<User> GetUsers(IUserSpecification spec)
        {
            var expr = spec.ToEFExpression();
            var result =
                _context.Users.Where(expr)
                .AsEnumerable()
                .Select(u => new User(u.Name, u.Gender, u.Birthday))
                .ToArray();

            return result;
        }

    }

    public class UserDbContext : DbContext
    {
        public DbSet<DBUser> Users { get; set; }
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuider)
        {
            modelBuider.Entity<DBUser>(e =>
            {
                e.ToTable("tbUser").HasKey(x => x.UserId);
                e.Property(x => x.UserId).ValueGeneratedOnAdd();
            });
            base.OnModelCreating(modelBuider);
        }
    }
}
