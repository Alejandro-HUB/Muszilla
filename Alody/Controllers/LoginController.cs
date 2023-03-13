using Alody.Models;
using AutoMapper;
using DBContext.Models;
using Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Requests;
using Responses;
using System.Threading.Tasks;
using ViewModels;

namespace Alody.Controllers
{
    public class LoginController : Controller
    {
        private IMapper _mapper;
        private MuszillaDbContext _dbContext;
        public LoginController(IMapper mapper, MuszillaDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

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
