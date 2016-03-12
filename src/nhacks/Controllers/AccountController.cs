using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using nhacks.Models;
using nhacks.ViewModels.Account;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;

namespace nhacks.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // POST: api/Account/login
        [HttpPost("login")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken] //TODO: Not sure if needed
        public async Task<IActionResult> PostApplicationUser([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return new HttpStatusCodeResult(StatusCodes.Status202Accepted);
                    //TODO: Return some data about a user?
                }
                if (result.RequiresTwoFactor)
                {
                    //FIXME
                    //return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return HttpBadRequest(model); //TODO: Not sure if this works
                }
            }

            // If we got this far, something failed, redisplay form
            return HttpBadRequest(model); //TODO: Not sure if this works
        }

        // POST: api/Account/signup
        [HttpPost("signup")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken] //TODO: Not sure if neede
        public async Task<IActionResult> PostApplicationUser([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return new HttpStatusCodeResult(StatusCodes.Status201Created);
                //return CreatedAtRoute("GetApplicationUser", new { id = user.Id }, user); //FIXME: returns too much information
            }
            else
            {
                if (ApplicationUserExists(user.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    return new HttpStatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
        }

        // POST: /Account/LogOff
        [HttpPost("logout")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return new HttpStatusCodeResult(StatusCodes.Status200OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUser.Count(e => e.Id == id) > 0;
        }
    }
}