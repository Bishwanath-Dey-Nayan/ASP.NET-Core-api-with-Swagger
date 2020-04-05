using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
                        var roleResult = await userManager.AddToRoleAsync(user, "User");
                        if (roleResult.Succeeded)
                        {
                            return Ok(user);
                        }
                       
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
                    var user = await userManager.FindByNameAsync(model.UserName);
                    var roles = await userManager.GetRolesAsync(user);
                    IdentityOptions identityOptions = new IdentityOptions();

                    //Adding data to the token
                    var claims = new Claim[]
                    {
                           new Claim(identityOptions.ClaimsIdentity.UserIdClaimType,user.Id),
                           new Claim(identityOptions.ClaimsIdentity.UserNameClaimType, user.UserName),
                           new Claim(identityOptions.ClaimsIdentity.RoleClaimType,roles[0])
                    };

                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This-is-my-sceret-key"));
                    var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
                    var jwt = new JwtSecurityToken(signingCredentials: signingCredentials,expires:DateTime.Now.AddMinutes(30),claims:claims);
                    return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            await signManager.SignOutAsync();
            return NoContent();
        }


    }

}