using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Data
{
    public static class DbContextExtensions
    {
        public static void Seed(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                     new User { UserName="zviki", PasswordHash="asdasd123", PhoneNumber = "599522728", PhoneNumberConfirmed =true, EmailConfirmed = true, Balance = 999999 },
                };
                var _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                foreach (var user in users)
                {
                    var pwd = user.PasswordHash;
                    user.PasswordHash = null;
                    _userManager.CreateAsync(user, pwd).Wait();
                }
            }
            if (!context.Jackpots.Any())
            {
                context.Jackpots.Add(new Jackpot
                {

                });
                context.SaveChanges();
            }
        }
    }
}
