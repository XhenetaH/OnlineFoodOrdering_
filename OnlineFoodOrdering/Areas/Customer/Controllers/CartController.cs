using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrdering.Data;
using OnlineFoodOrdering.Models;
using OnlineFoodOrdering.Models.ViewModels;
using OnlineFoodOrdering.Utility;
using Stripe;

namespace OnlineFoodOrdering.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }
        public CartController(ApplicationDbContext db,IEmailSender emailSender)
        {
            this._db = db;
            this._emailSender = emailSender;
        }
        public async Task<IActionResult> Index()
        {
            detailCart = new OrderDetailsCart()
            {
                Order = new Models.Order()
            };
            detailCart.Order.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);
            if(cart !=null)
            {
                detailCart.listCart = cart.ToList();
            }
            foreach(var list in detailCart.listCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == list.MenuItemId);
                detailCart.Order.OrderTotal = detailCart.Order.OrderTotal + (list.MenuItem.Price * list.Count);
                list.MenuItem.Description = SD.ConvertToRawHtml(list.MenuItem.Description);
                if(list.MenuItem.Description.Length>100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
            }
            detailCart.Order.OrderTotalOriginal = detailCart.Order.OrderTotal;

            if(HttpContext.Session.GetString(SD.ssCouponCode)!=null)
            {
                detailCart.Order.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == detailCart.Order.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.Order.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.Order.OrderTotalOriginal);
            }
            return View(detailCart);

        }

        public async Task<IActionResult> SingleSummary(int id, int count)
        {
            if (id == null)
                return NotFound();
            else
            {
                SingleSummaryViewModel vm = new SingleSummaryViewModel()
                {
                    MenuItem = await _db.MenuItem.Where(m => m.Id == id).FirstOrDefaultAsync(),
                    Count = count,
                    Order = new Models.Order()

                };
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ApplicationUser applicationUser = await _db.ApplicationUser.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();
              
                vm.Order.OrderTotalOriginal = vm.MenuItem.Price;
                vm.Order.PickupName = applicationUser.Name;
                vm.Order.PickupLastName = applicationUser.LastName;
                vm.Order.PhoneNumber = applicationUser.PhoneNumber;
                vm.Order.PickUpTime = DateTime.Now;
                return View(vm);
            }
           
        }
        public async Task<IActionResult> Summary()
        {
            detailCart = new OrderDetailsCart()
            {
                Order = new Models.Order()
            };
            detailCart.Order.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = await _db.ApplicationUser.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();
            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);
            if (cart != null)
            {
                detailCart.listCart = cart.ToList();
            }
            foreach (var list in detailCart.listCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == list.MenuItemId);
                detailCart.Order.OrderTotal = detailCart.Order.OrderTotal + (list.MenuItem.Price * list.Count);
               
            }
            detailCart.Order.OrderTotalOriginal = detailCart.Order.OrderTotal;
            detailCart.Order.PickupName = applicationUser.Name;
            detailCart.Order.PickupLastName = applicationUser.LastName;
            detailCart.Order.PhoneNumber = applicationUser.PhoneNumber;
            detailCart.Order.PickUpTime = DateTime.Now;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailCart.Order.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == detailCart.Order.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.Order.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.Order.OrderTotalOriginal);
            }
            return View(detailCart);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            detailCart.listCart = await _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();

            detailCart.Order.PaymentStatus = SD.PaymentStatusPending;
            detailCart.Order.OrderDate = DateTime.Now;
            detailCart.Order.UserId = claim.Value;
            detailCart.Order.Status = SD.PaymentStatusPending;
            detailCart.Order.PickUpTime = Convert.ToDateTime(detailCart.Order.PickUpDate.ToShortDateString() + " " + detailCart.Order.PickUpTime.ToShortTimeString());
           
            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _db.Order.Add(detailCart.Order);
            await _db.SaveChangesAsync();

            detailCart.Order.OrderTotalOriginal = 0;

            foreach (var item in detailCart.listCart)
            {
                item.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = detailCart.Order.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                detailCart.Order.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                _db.OrderDetails.Add(orderDetails);

            }
           

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailCart.Order.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == detailCart.Order.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.Order.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.Order.OrderTotalOriginal);
            }
            else
            {
                detailCart.Order.OrderTotal = detailCart.Order.OrderTotalOriginal;
            }
            detailCart.Order.CouponCodeDiscount = detailCart.Order.OrderTotalOriginal - detailCart.Order.OrderTotal;

            _db.ShoppingCart.RemoveRange(detailCart.listCart);
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);
            await _db.SaveChangesAsync();

            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(detailCart.Order.OrderTotal * 100),
                Currency = "eur",
                Description = "Order ID : " + detailCart.Order.Id,
                Source = stripeToken
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            if(charge.BalanceTransactionId == null)
            {
                detailCart.Order.PaymentStatus = SD.PaymentStatusRejected;
            }
            else
            {
                detailCart.Order.TransactionId = charge.BalanceTransactionId;
            }
            if (charge.Status.ToLower() == "succeeded")
            {
                await _emailSender.SendEmailAsync(_db.Users.Where(u=>u.Id==claim.Value).FirstOrDefault().Email,"Ogani - Order Created "+ detailCart.Order.Id.ToString(),"Order has been submitted successfully.");


                detailCart.Order.PaymentStatus = SD.PaymentStatusApproved;
                detailCart.Order.Status = SD.StatusSubmitted;
            }
            else
            {
                detailCart.Order.PaymentStatus = SD.PaymentStatusRejected; 
            }
            await _db.SaveChangesAsync();

            return RedirectToAction("Confirm", "Order", new { id=detailCart.Order.Id });

        }





        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);
            if(cart.Count == 1)
            {
                _db.ShoppingCart.Remove(cart);
                await _db.SaveChangesAsync();

                var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);
            cart.Count += 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);
            _db.ShoppingCart.Remove(cart);
            await _db.SaveChangesAsync();

            var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddCoupon()
        {
            if(detailCart.Order.CouponCode == null)
            {
                detailCart.Order.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, detailCart.Order.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {
           
            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }
        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var claimsIndentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIndentity.FindFirst(ClaimTypes.NameIdentifier);

            List<OrderDetailsViewModel> orderDetailsList = new List<OrderDetailsViewModel>();

            List<Models.Order> orderList = await _db.Order.Include(o => o.ApplicationUser).Where(u => u.UserId == claim.Value).ToListAsync();

            foreach(Models.Order item in orderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    Order = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderDetailsList.Add(individual);
            }

            return View(orderDetailsList);
        }


    }
}