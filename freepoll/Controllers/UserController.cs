using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using freepoll.Models;
using freepoll.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace freepoll.Controllers
{
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly FreePollDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(FreePollDBContext dBContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("userregistration")]
        [HttpPut]

        public IActionResult RegisterPollUser([FromBody] UserViewModel newUser)
        {
            int result = 0;
            UserResponseViewModel dynamicresponse = new UserResponseViewModel();
            User olduser = _dBContext.User.Where(x => x.Email == newUser.userEmail.ToLower()).FirstOrDefault();
            if (olduser != null)
            {
                User user = new User();
                user.Name = newUser.userName;
                user.Email = newUser.userEmail;
                user.Password = newUser.password;
                user.Github = newUser.github;
                user.Google = newUser.google;
                user.Facebook = newUser.facebook;
                user.CreatedTime = DateTime.UtcNow;
                user.UserGuid = Guid.NewGuid().ToString();

                _dBContext.User.Add(user);
                result = _dBContext.SaveChanges();

                if (result > 0)
                {
                    dynamicresponse.UserGuid = user.UserGuid;
                }
                else
                {
                    return BadRequest("Registration Failed . Please try again");
                    
                }
            }
            else
            {
                return BadRequest("User already exists");
            }

            return Ok(dynamicresponse);
        }

        [Route("userlogin")]
        [HttpGet]
        public IActionResult UserLogin(LoginViewModel logindetails)
        {
            User user = new User();


            if(string.IsNullOrEmpty(logindetails.loginpassword))
            {
                user = _dBContext.User.Where(x => x.Email == logindetails.loginemail.ToLower() && x.Password == logindetails.loginpassword.ToLower()).FirstOrDefault();
            }
            else
            {
                if(logindetails.platformdetail.platform.ToLower() == "google")
                {
                    user = _dBContext.User.Where(x => x.Email == logindetails.loginemail.ToLower() && x.Google == logindetails.platformdetail.platformid.ToLower()).FirstOrDefault();
                }
                else if (logindetails.platformdetail.platform.ToLower() == "github")
                {
                    user = _dBContext.User.Where(x => x.Email == logindetails.loginemail.ToLower() && x.Github == logindetails.platformdetail.platformid.ToLower()).FirstOrDefault();
                }
                else if (logindetails.platformdetail.platform.ToLower() == "facebook")
                {
                    user = _dBContext.User.Where(x => x.Email == logindetails.loginemail.ToLower() && x.Facebook == logindetails.platformdetail.platformid.ToLower()).FirstOrDefault();
                }

            }

            UserResponseViewModel login = new UserResponseViewModel();

            if (user != null)
            {
                login.UserGuid = user.UserGuid;
            }
            else
            {
                return BadRequest("Invalid Login");
            }

            return Ok(login);
        }


        //[Route("userpoll/{userGuid}")]


    }
}
