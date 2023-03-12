using Alody.Models;
using AutoMapper;
using DBContext.Models;
using Handlers;
using Microsoft.AspNetCore.Mvc;
using Requests;
using Responses;
using System.Threading.Tasks;
using ViewModels;

namespace Alody.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private MapperConfiguration _mapperConfiguration;
        private Mapper _mapper;
        private MuszillaDbContext _dbContext;
        public LoginController(MapperConfiguration mapperConfiguration, MuszillaDbContext dbContext)
        {
            _mapperConfiguration = mapperConfiguration;
            _mapper = new Mapper(mapperConfiguration);
            _dbContext = dbContext;
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(UserModel user)
        {
            LoginRequestHandler handler = new LoginRequestHandler(_dbContext);

            //Build Request
            LoginRequest request = new LoginRequest();
            request.userModel = _mapper.Map<UserViewModel>(user);

            //Response
            LoginResponse response = handler.Handle(request);

            if (response.IsSuccess)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }

        [HttpPost]
        [Route("GoogleLogin")]
        public IActionResult Login(UserModel user)
        {
            LoginRequestHandler handler = new LoginRequestHandler(_dbContext);

            //Build Request
            LoginRequest request = new LoginRequest();
            request.userModel = _mapper.Map<UserViewModel>(user);

            //Response
            LoginResponse response = handler.Handle(request);

            if (response.IsSuccess)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }
    }
}
