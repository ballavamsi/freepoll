using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using freepoll.Models;
using freepoll.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using freepoll.Common;
using static freepoll.Common.ResponseMessages;

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

        //[Route("register")]
        //[HttpPut]

        //public IActionResult RegisterPollUser([FromBody] UserViewModel newUser)
        //{
        //    int result = 0;
        //    UserResponseViewModel userResponseViewModel = new UserResponseViewModel();
        //    User olduser = _dBContext.User.Where(x => x.Email == newUser.userEmail.ToLower()).FirstOrDefault();
        //    if (olduser != null)
        //    {
        //        User user = new User();
        //        user.Name = newUser.userName;
        //        user.Email = newUser.userEmail;
        //        //user.Password = newUser.password;
        //        user.PhotoUrl = newUser.profileUrl;
        //        user.Github = newUser.github;
        //        user.Google = newUser.google;
        //        user.Facebook = newUser.facebook;
        //        user.CreatedTime = DateTime.UtcNow;
        //        user.Status = 1;
        //        user.UserGuid = Guid.NewGuid().ToString();

        //        _dBContext.User.Add(user);
        //        result = _dBContext.SaveChanges();

        //        if (result > 0)
        //        {
        //            userResponseViewModel.profileUrl = user.PhotoUrl;
        //            userResponseViewModel.userName = user.Name;
        //            userResponseViewModel.userEmail = user.Email;
        //            userResponseViewModel.UserGuid = user.UserGuid;
        //        }
        //        else
        //        {
        //            return BadRequest("Registration Failed . Please try again");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("User already exists");
        //    }

        //    return Ok(userResponseViewModel);
        //}

        [Route("login")]
        [HttpPost]
        public IActionResult UserLogin([FromBody]LoginViewModel logindetails)
        {
            User user = new User();
            UserResponseViewModel userResponseViewModel = new UserResponseViewModel();

            List<User> users = new List<User>();
            users = _dBContext.User.ToList();

            user = users.Where(x => x.Email == logindetails.email.ToLower()).FirstOrDefault();

            

            if(user == null)
            {
                user = new User();
                user.Name = logindetails.name;
                user.Email = logindetails.email;
                user.Password = string.Empty;
                user.PhotoUrl = logindetails.platformdetail.platformImage;
                user.Github = logindetails.platformdetail.platform == "github" ? logindetails.platformdetail.platformid : string.Empty;
                user.Google = logindetails.platformdetail.platform == "google" ? logindetails.platformdetail.platformid : string.Empty;
                user.Facebook = logindetails.platformdetail.platform == "facebook" ? logindetails.platformdetail.platformid : string.Empty;
                user.CreatedTime = DateTime.UtcNow;
                user.Status = 1;
                user.UserGuid = Guid.NewGuid().ToString();

                _dBContext.User.Add(user);
                int result = _dBContext.SaveChanges();
            }

            if (user.Status == 0)
                return BadRequest(Messages.Inactiveusererror);

            if(logindetails.platformdetail.platform.ToLower() == "google" && string.IsNullOrEmpty(user.Google))
            {
                user.Google = logindetails.platformdetail.platformid;
                _dBContext.User.Update(user);
                _dBContext.SaveChanges();
            }
            if (logindetails.platformdetail.platform.ToLower() == "facebook" && string.IsNullOrEmpty(user.Facebook))
            {
                user.Facebook = logindetails.platformdetail.platformid;
                _dBContext.User.Update(user);
                _dBContext.SaveChanges();
            }
            if (logindetails.platformdetail.platform.ToLower() == "github" && string.IsNullOrEmpty(user.Github))
            {
                user.Github = logindetails.platformdetail.platformid;
                _dBContext.User.Update(user);
                _dBContext.SaveChanges();
            }

            if (logindetails.platformdetail.platform.ToLower() == "google" && user.Google == logindetails.platformdetail.platformid ||
               logindetails.platformdetail.platform.ToLower() == "github" && user.Github == logindetails.platformdetail.platformid ||
               logindetails.platformdetail.platform.ToLower() == "facebook" && user.Facebook == logindetails.platformdetail.platformid)
            {
                userResponseViewModel.profileUrl = user.PhotoUrl;
                userResponseViewModel.userName = user.Name;
                userResponseViewModel.userEmail = user.Email;             
                string encryptedString = Security.Encrypt(user.UserGuid);
                userResponseViewModel.UserGuid = encryptedString;

            }
            else
            {
                return BadRequest(Messages.Inactiveusererror);
            }
            return Ok(userResponseViewModel);
        }
    }
}
