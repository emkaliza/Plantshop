using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlantShop.Models;
using PlantShop.ViewModels;
using System.Security.Claims;

namespace PlantShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Json(new { success = false, errors = new[] { "Користувача з таким email не знайдено." } });
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName,
                    model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Користувач {user.Email} успішно увійшов в систему.");
                    if (user.Role == "Admin")
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin", new { area = "Admin" }) });
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Customer", new { area = "Customer"}) });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning($"Обліковий запис користувача {user.Email} заблоковано.");
                    return Json(new { success = false, errors = new[] { "Обліковий запис заблоковано. Спробуйте пізніше." } });
                }

                return Json(new { success = false, errors = new[] { "Невірний пароль." } });
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, errors = errors });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return Json(new { success = false, errors = new[] { "Користувач з таким email вже існує." } });
                }

                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.Name,
                    EmailConfirmed = true // Тимчасово, доки не буде налаштовано електронну пошту
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Створено нового користувача {user.Email}.");
                    await _userManager.AddToRoleAsync(user, "Customer");
                    // Автоматичний вхід після реєстрації
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                }

                var errors = result.Errors.Select(e => e.Description);
                return Json(new { success = false, errors = errors });
            }

            var modelErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, errors = modelErrors });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Користувач вийшов із системи.");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogWarning("Не вдалося отримати зовнішню інформацію для входу.");
                return RedirectToAction("Login");
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                _logger.LogInformation($"Користувач увійшов через {info.LoginProvider}.");
                return RedirectToAction("Index", "Home");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            // Якщо користувача немає, створюємо нового
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = name,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    _logger.LogError($"Помилка при створенні користувача для {info.LoginProvider}.");
                    return RedirectToAction("Login");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                }
            }

            // Додаємо зовнішній логін до користувача
            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (addLoginResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation($"Створено нового користувача через {info.LoginProvider}.");
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Не розкриваємо інформацію про існування користувача
                    return Json(new { success = true });
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, code = code }, Request.Scheme);

                // TODO: Додати надсилання email із посиланням для скидання пароля
                _logger.LogInformation($"Створено запит на скидання пароля для користувача {user.Email}.");

                return Json(new { success = true });
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, errors = errors });
        }
    }
}