using IdentityServer.Identity.Data.Application.Context;
using IdentityServer.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _Context;

        public UserService(ApplicationDbContext context)
        {
            _Context = context;
        }

        public Users GetUserByUserNameAndPassword(string username, string password)
        {
            return _Context.Users.FirstOrDefault(x => x.UserName == username && x.password == password);
        }
    }
}
