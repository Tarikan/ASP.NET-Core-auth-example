using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using LoginApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;

namespace LoginApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Auth")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;
        
        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,   
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        
        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = new User { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
 
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    return Ok(user);
                }

                // If we got this far, something failed, redisplay form
            return BadRequest("Username already exist");
        }
        
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return Ok("Successfully logged in");
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return BadRequest("Account locked out");
            }
            
            return BadRequest("Invalid username or/and password ");
        }
    }
}