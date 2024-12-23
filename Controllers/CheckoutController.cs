using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plantshop.Data;
using Plantshop.Services.Interfaces;
using Plantshop.ViewModels;
using PlantShop.Models;
using SQLitePCL;

namespace Plantshop.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly UserManager<AppUser> _userManager;

        public CheckoutController(IOrderService orderService, ICartService cartService, UserManager<AppUser> userManager)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _cartService.GetCartItemsAsync(user.Id);
            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            var viewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = await _cartService.GetCartTotalAsync(user.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!ModelState.IsValid)
            {
                model.CartItems = await _cartService.GetCartItemsAsync(user.Id);
                model.TotalAmount = await _cartService.GetCartTotalAsync(user.Id);
                return View("Index", model);
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(user.Id, model);
                return RedirectToAction(nameof(Confirmation), new { orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                model.CartItems = await _cartService.GetCartItemsAsync(user.Id);
                model.TotalAmount = await _cartService.GetCartTotalAsync(user.Id);
                return View("Index", model);
            }
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order == null || order.UserId != user.Id)
                return NotFound();

            return View(order);
        }
    }
}
