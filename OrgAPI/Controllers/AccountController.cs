using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrgAPI.ViewModel;

namespace OrgAPI.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signManager;

        public AccountController(SignInManager<IdentityUser> _signInManager,UserManager<IdentityUser> _userManager)
        {
            signManager = _signInManager;
            userManager = _userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Registerviewmodel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser()
                    {
                        UserName = model.UserName,
                        Email = model.Email
                    };

                    var userResult = await userManager.CreateAsync(user, model.password);
                    if (userResult.Succeeded)
                    {
                        return Ok(user);
                    }
                    else
                    {
                        foreach(var error in userResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }

                }
                return BadRequest(ModelState.Values);
            }
            catch (Exception E)
            {
                ModelState.AddModelError("", E.Message);
                return BadRequest(ModelState.Values);
            }
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if(ModelState.IsValid)
            {
                var signInResult = await signManager.PasswordSignInAsync(model.UserName, model.password, false, false);
                if(signInResult.Succeeded)
                {
                    return Ok();
                }
            }
            return BadRequest(ModelState);
        }

        public async Task<IActionResult> SignOut()
        {
            await signManager.SignOutAsync();
            return NoContent();
        }


    }

}