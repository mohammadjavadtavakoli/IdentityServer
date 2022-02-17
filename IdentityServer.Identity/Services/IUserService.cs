using IdentityServer.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Identity.Services
{
    public interface IUserService
    {
        Users GetUserByUserNameAndPassword(string username, string password);
    }
}
