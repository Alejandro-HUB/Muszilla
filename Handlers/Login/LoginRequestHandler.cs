using DBContext.Models;
using Microsoft.IdentityModel.Tokens;
using Requests;
using Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Handlers
{
    public class LoginRequestHandler
    {
        MuszillaDbContext _dbContext;
        public LoginRequestHandler(MuszillaDbContext dbContext)
        { 
            _dbContext = dbContext;
        }
        public LoginResponse Handle(LoginRequest request)
        {
            var response = new LoginResponse();

            if (!string.IsNullOrEmpty(request?.userModel?.Email) 
                && !string.IsNullOrEmpty(request?.userModel?.Password))
            {
                try
                {
                    var user = _dbContext.Users.Where(x => x.Email.ToLower() == request.userModel.Email.ToLower()
                    && x.Password == request.userModel.Password)?.FirstOrDefault();
                    if (user != null)
                    {
                        if (user.IsGoogleUser != 1)
                        {
                            return new LoginResponse()
                            {
                                IsSuccess = true,
                                Message = "User Athenticated"
                            };
                        }
                    }
                }
                catch (Exception e)
                {

                    return new LoginResponse()
                    {
                        IsSuccess = false,
                        Message = $"Failed to Authenticate User, EX: {e}"
                    };
                }
            }


            return new LoginResponse()
            {
                IsSuccess = false,
                Message = "Failed to Authenticate User"
            };
        }
    }  
}

