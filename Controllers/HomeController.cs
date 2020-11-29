using OnlineShoppingStore.DAL;
using OnlineShoppingStore.Models.Home;
using OnlineShoppingStore.Repository;
using OnlineShoppingStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace OnlineShoppingStore.Controllers
{
    public class HomeController : Controller
    {
        dbMyOnlineShoppingEntities ctx = new dbMyOnlineShoppingEntities();
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        
        public ActionResult Index(string search, int? page)
        {
            FormsAuthentication.SignOut();
            HomeIndexViewModel model = new HomeIndexViewModel();
            return View(model.CreateModel(search, 12, page));     // the second parameter of the CreateModel() is the number of products in a page
        }

        public ActionResult Login()
        {
            FormsAuthentication.SignOut();
            return View();
        }

        // It takes the password inserted by the user and converts it in MD5 string. Then it searches in the DB if there is a equal password MD5.
        //If in the DB exists the same password (Valid Login) it redirect you to the Admin side. 
        [HttpPost]
        public ActionResult Login(Tbl_Admin tbl,string returnUrl)
        {
            dbMyOnlineShoppingEntities db = new dbMyOnlineShoppingEntities();
            tbl.Password = Tbl_Admin.MD5Hash(tbl.Password);
            var dataItem = db.Tbl_Admin.Where(x => x.UserName == tbl.UserName && x.Password == tbl.Password).SingleOrDefault();
            if (dataItem != null)
            {
                FormsAuthentication.SetAuthCookie(dataItem.UserName, false);
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                         && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);    
                }
                else
                {
                    //Valid login and password 
                    return Redirect("/Admin/Product");       
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid user/pass");
                return View();
            }
        }

        //Sign out the admin. Now you can't visit the Admin part until you dont do the login again
        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

        public ActionResult AddAdmin()
        {
            FormsAuthentication.SignOut();
            return View();
        }


        //Add a new admin to the Database -- In real live need to take out this option since it doesnt make sense to add the admin whenever you want
        [HttpPost]
        public ActionResult AddAdmin(Tbl_Admin tbl)
        {
            
            if (tbl.Password != null && tbl.UserName != null)
            {
                dbMyOnlineShoppingEntities db = new dbMyOnlineShoppingEntities();
                tbl.Password = Tbl_Admin.MD5Hash(tbl.Password);
                var dataItem = db.Tbl_Admin.Where(x => x.UserName == tbl.UserName).SingleOrDefault();

                if (dataItem == null) { 
                    _unitOfWork.GetRepositoryInstance<Tbl_Admin>().Add(tbl);
                    return RedirectToAction("Login");
                }
            }
            return RedirectToAction("AddAdmin");
        }

        public ActionResult Checkout()
        {
            return View();
        }

        public ActionResult CheckoutDetails()
        {
            if (Session["cart"] != null)
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var checkCart = 0;
                foreach (var item in cart)
                {
                    if (item.Quantity >0)
                    {

                        checkCart = 1;
                    }
                }
                if(checkCart == 0)
                {
                   return RedirectToAction("Index", "Home");
                }
                
            }

            return View();
            
                
        }


        public ActionResult CreateOrder()
        {
            return View();
        }

        public ActionResult Location()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult PayOrder()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CreateOrder(CreateOrderViewModel orderViewModel)
        {
            if (orderViewModel != null)
            {
                dbMyOnlineShoppingEntities db = new dbMyOnlineShoppingEntities();
                var mem = _unitOfWork.GetRepositoryInstance<Tbl_Members>().GetAllRecords();
                var maxMemberId = 0;

                foreach (var item in mem)
                {
                    if(item.MemberId > maxMemberId)
                    {
                        maxMemberId = item.MemberId + 1;
                    }
                    
                }
                Tbl_Members person = new Tbl_Members();
                person.FristName = orderViewModel.Member.FristName;
                person.LastName = orderViewModel.Member.LastName;
                person.MemberId = maxMemberId;
                person.CreatedOn = DateTime.Now;
                person.EmailId = orderViewModel.Member.EmailId;
                person.IsActive = true;
                person.IsDelete = false;
                person.Password = "1234";
                person.ModifiedOn = DateTime.Now;
                _unitOfWork.GetRepositoryInstance<Tbl_Members>().Add(person);


                var shippingDet = _unitOfWork.GetRepositoryInstance<Tbl_ShippingDetails>().GetAllRecords();
                var maxShipId = 0;
                foreach (var item in shippingDet)
                {
                    if (item.ShippingDetailId > maxShipId)
                    {
                        maxShipId = item.ShippingDetailId + 1;
                    }
                }

                var orders = _unitOfWork.GetRepositoryInstance<Tbl_Orders>().GetAllRecords();
                var maxOrderId = 0;
                foreach (var item in orders)
                {
                    if (item.OrderId > maxOrderId)
                    {
                        maxOrderId = item.OrderId + 1;
                    }
                }

                Tbl_ShippingDetails shippingDetails = new Tbl_ShippingDetails();
                shippingDetails.ShippingDetailId = maxShipId;
                shippingDetails.Adress = orderViewModel.ShippingDetails.Adress;
                shippingDetails.City = orderViewModel.ShippingDetails.City;
                shippingDetails.ZipCode = orderViewModel.ShippingDetails.ZipCode;
                shippingDetails.Country = orderViewModel.ShippingDetails.Country;
                shippingDetails.State = orderViewModel.ShippingDetails.State;
                shippingDetails.MemberId = maxMemberId;
                string AmountPaid = Session["SesTotal"].ToString();
                shippingDetails.AmountPaid = Decimal.Parse(AmountPaid);
                shippingDetails.PaymentType = "Cash";
                //shippingDetails.OrderId = maxOrderId;
                _unitOfWork.GetRepositoryInstance<Tbl_ShippingDetails>().Add(shippingDetails);

                Tbl_Orders order = new Tbl_Orders();
                order.MemberId = person.MemberId;
                order.OrderStatus = "First Step";
                order.ShippingDetailsId = shippingDetails.ShippingDetailId;
                order.OrderId = maxOrderId;
                _unitOfWork.GetRepositoryInstance<Tbl_Orders>().Add(order);

                
                List<Item> cart = (List<Item>)Session["cart"];
            
                foreach (var item in cart)
                {
                    if (item.Quantity > 0)
                    {
                        Tbl_OrderProducts orderProducts = new Tbl_OrderProducts();
                        orderProducts.ProductId = item.Product.ProductId;
                        orderProducts.Quantity = item.Quantity;
                        orderProducts.OrderId = order.OrderId;
                        _unitOfWork.GetRepositoryInstance<Tbl_OrderProducts>().Add(orderProducts);
                    }
                }
            }

            return RedirectToAction("PayOrder","Home");
        }

       



        public ActionResult DecreaseQty(int productId)
        {
            if (Session["cart"] != null)
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = ctx.Tbl_Product.Find(productId);
                foreach (var item in cart)
                {
                    if (item.Product.ProductId == productId)
                    {
                        int prevQty = item.Quantity;
                        if (prevQty > 0)
                        {
                            item.Quantity--;
                        }
                        else
                        {
                            cart.Remove(item);
                        }
                        break;
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect("Checkout");
        }

        public ActionResult IncreaseQty(int productId)
        {
            if (Session["cart"] != null)
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = ctx.Tbl_Product.Find(productId);
                foreach (var item in cart)
                {
                    if (item.Product.ProductId == productId)
                    {

                        int prevQty = item.Quantity;
                        if (prevQty < product.Quantity)
                        {

                            item.Quantity++;
                        }   
                        break;
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect("Checkout");
        }

        //Add the product to the cart. If there is already other article with the same name it just increase the quantity
        public ActionResult AddToCart(int productId, string url)
        {
            if (Session["cart"] == null)
            {
                List<Item> cart = new List<Item>();
                var product = ctx.Tbl_Product.Find(productId);
                cart.Add(new Item()
                {
                    Product = product,
                    Quantity = 1
                });
                Session["cart"] = cart;
            }
            else
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var count = cart.Count();
                var product = ctx.Tbl_Product.Find(productId);
                for (int i = 0; i < count; i++)
                {
                    if (cart[i].Product.ProductId == productId)
                    {
                        int prevQty = cart[i].Quantity;
                        cart.Remove(cart[i]);
                        cart.Add(new Item()
                        {
                            Product = product,
                            Quantity = prevQty + 1
                        });
                        break;
                    }
                    else
                    {
                        var prd = cart.Where(x => x.Product.ProductId == productId).SingleOrDefault();
                        if (prd == null)
                        {
                            cart.Add(new Item()
                            {
                                Product = product,
                                Quantity = 1
                            });
                        }
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect("Index");
        }
        
        public ActionResult RemoveFromCart(int productId)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            foreach (var item in cart)
            {
                if (item.Product.ProductId == productId)
                {
                    cart.Remove(item);
                    break;
                }
            }
            Session["cart"] = cart;
            return Redirect("Index");
        }
    }
}