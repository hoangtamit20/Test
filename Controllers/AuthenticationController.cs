using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetShop.Configurations;
using PetShop.Entity;

namespace PetShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly JwtConfig _jwtConfig;
        public AuthenticationController(UserManager<NguoiDung> userManager, JwtConfig jwtConfig) => (_userManager, _jwtConfig) = (userManager, jwtConfig);
    }
}