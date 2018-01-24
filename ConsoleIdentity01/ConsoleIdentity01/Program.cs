using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Data.Entity.Infrastructure.Annotations;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleIdentity01
{
    class Program
    {
        static void Main(string[] args)
        {


            //var path = @"C:\Users\amk\Downloads\Backup\68356029.pdf";
            //path = Path.GetDirectoryName(path) + @"\" + "Sample.jpg";

            var i = 10;


            //*********************************************
            //var userStore = new UserStore<IdentityUser>();
            //var userManager = new UserManager<IdentityUser>(userStore);

            //var creationResult = userManager.Create(new IdentityUser("Ajay@Yahoo.com"), "Password123!");

            //Console.WriteLine($"Created: {creationResult.Succeeded}");
            //*********************************************

            //var userName = "Ajay@Yahoo.com";
            //var userPassword = "Password123!";

            //var userStore = new UserStore<IdentityUser>();
            //var userManager = new UserManager<IdentityUser>(userStore);


            //var user = userManager.FindByName(userName);
            //var claimResult = userManager.AddClaim(user.Id, new System.Security.Claims.Claim("given_name", "Ajay"));

            //Console.WriteLine($"Claim: {claimResult.Succeeded}");
            //*********************************************

            //var userName = "Ajay@Yahoo.com";
            //var userPassword = "Password123!";

            //var userStore = new UserStore<IdentityUser>();
            //var userManager = new UserManager<IdentityUser>(userStore);

            //var user = userManager.FindByName(userName);

            //var isMatch = userManager.CheckPassword(user, userPassword);

            //Console.WriteLine($"Password Match: {isMatch}");
            //*********************************************

            var userName = "Ajay@Yahoo.com";
            var userPassword = "Password123!";

            var userStore = new CustomUserStore(new CustomUserDbContext());
            var userManager = new UserManager<CustomUser, int>(userStore);

            var creationResult = userManager.Create(new CustomUser {UserName = userName }, userPassword);

            Console.WriteLine($"Created: {creationResult.Succeeded}");
            var user = userManager.FindByName(userName);

            var isMatch = userManager.CheckPassword(user, userPassword);

            Console.WriteLine($"Password Match: {isMatch}");
        }

        public class CustomUser: IUser<int>
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string PasswordHash { get; set; }
        }

        public class CustomUserDbContext: DbContext
        {
            public CustomUserDbContext(): base("DefaultConnection") { }

            public DbSet<CustomUser> Users { get; set; }


            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                var user = modelBuilder.Entity<CustomUser>();
                user.ToTable("Users");
                user.HasKey(x => x.Id);
                user.Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                user.Property(x => x.UserName).IsRequired().HasMaxLength(256)
                    .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));


                base.OnModelCreating(modelBuilder);

            }
        }

        public class CustomUserStore : IUserPasswordStore<CustomUser, int>
        {
            private readonly CustomUserDbContext context;
            public CustomUserStore(CustomUserDbContext context)
            {
                this.context = context;
            }
            public Task CreateAsync(CustomUser user)
            {
                context.Users.Add(user);
                return context.SaveChangesAsync();
            }

            public Task DeleteAsync(CustomUser user)
            {
                context.Users.Remove(user);
                return context.SaveChangesAsync();
            }

            public void Dispose()
            {
                context.Dispose();
            }

            public Task<CustomUser> FindByIdAsync(int userId)
            {
                return context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            }

            public Task<CustomUser> FindByNameAsync(string userName)
            {
                return context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            }

            public Task<string> GetPasswordHashAsync(CustomUser user)
            {
                return Task.FromResult(user.PasswordHash);
            }

            public Task<bool> HasPasswordAsync(CustomUser user)
            {
                return Task.FromResult(user.PasswordHash != null);
            }

            public Task SetPasswordHashAsync(CustomUser user, string passwordHash)
            {
                user.PasswordHash = passwordHash;
                return Task.CompletedTask;
            }

            public Task UpdateAsync(CustomUser user)
            {
                context.Users.Attach(user);
                return context.SaveChangesAsync();
            }
        }
    }
}
