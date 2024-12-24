using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plantshop.Data;
using Plantshop.Services.Interfaces;
using PlantShop.Models;
using System.Runtime.InteropServices;

namespace Plantshop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<AppUser> _userManager;

        public CartController(ICartService cartService, UserManager<AppUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        // Метод для відображення кошика
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _cartService.GetCartItemsAsync(user.Id);
            return View(cartItems);
        }

        //Метод для відображення кількості товарів в кошику
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCartItemsCount()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = true, isAuthenticated = false });
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                {
                    return Json(new { success = false, message = "Користувача не знайдено", isAuthenticated = false });
            }
            var count = await _cartService.GetCartItemsCountAsync(user.Id);
           
            return Json(new { success = true, count, isAuthenticated = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity)
        {
            try
            {
                // Перевіряємо авторизацію
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Будь ласка, авторизуйтесь" });
                }

                // Додаємо товар до кошика
                await _cartService.AddToCartAsync(id, user.Id, quantity);

                // Отримуємо оновлену кількість товарів
                var itemsCount = await _cartService.GetCartItemsCountAsync(user.Id);

                // Повертаємо успішний результат
                return Json(new { success = true, message = "Товар додано до кошика", itemsCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BuyNow(int id, int quantity)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Будь ласка, авторизуйтесь" });
                }

                // Очищаємо кошик і додаємо товар
                await _cartService.ClearCartAsync(user.Id);
                await _cartService.AddToCartAsync(id, user.Id, quantity);

                // Перенаправляємо на оформлення замовлення
                return Json(new { success = true, redirectUrl = "/Checkout" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Оновлення кількості товару
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Будь ласка, авторизуйтесь" });
                }

                // Перевірка мінімальної кількості
                if (quantity < 1)
                {
                    return Json(new { success = false, message = "Кількість повинна бути більше 0" });
                }

                // Перевірка доступної кількості товару
                /*var availableStock = await _cartService.GetAvailableStockAsync(cartItemId);
                if (quantity > availableStock)
                {
                    return Json(new { success = false, message = $"Доступно тільки {availableStock} одиниць" });
                }*/

                await _cartService.UpdateCartItemQuantityAsync(cartItemId, quantity);
                var cartTotal = await _cartService.GetCartTotalAsync(user.Id);
                var itemsCount = await _cartService.GetCartItemsCountAsync(user.Id);

                return Json(new
                {
                    success = true,
                    total = cartTotal,
                    itemsCount,
                    cartItemId,
                    quantity,
                    message = "Кількість оновлено"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        /*[HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Будь ласка, авторизуйтесь" });
                }

                if (quantity < 1)
                {
                    return Json(new { success = false, message = "Кількість повинна бути більше 0" });
                }

                await _cartService.UpdateCartItemQuantityAsync(cartItemId, quantity);
                var cartTotal = await _cartService.GetCartTotalAsync(user.Id);
                var itemsCount = await _cartService.GetCartItemsCountAsync(user.Id);

                return Json(new
                {
                    success = true,
                    total = cartTotal,
                    itemsCount,
                    message = "Кількість оновлено"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }*/

        // Видалення товару з кошика
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Будь ласка, авторизуйтесь" });
                }

                await _cartService.RemoveCartItemAsync(cartItemId);
                var cartTotal = await _cartService.GetCartTotalAsync(user.Id);
                var itemsCount = await _cartService.GetCartItemsCountAsync(user.Id);

                return Json(new
                {
                    success = true,
                    total = cartTotal,
                    itemsCount,
                    message = "Товар видалено з кошика"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}