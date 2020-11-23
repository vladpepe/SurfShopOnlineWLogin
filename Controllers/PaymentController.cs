using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShoppingStore.Models;
using PayPal.Api;

namespace OnlineShoppingStore.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult PaymentWithPapal()
        {

            APIContext apicontext = PaypalConfiguration.GetAPIContext();
            try
            {

                string PayerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(PayerId))
                {
                    string baseURi = Request.Url.Scheme + "://" + Request.Url.Authority +
                                     "/PaymentWithPapal/PaymentWithPapal?";

                    var Guid = Convert.ToString((new Random()).Next(10000000));
                    var createdPayment = CreatePayment(apicontext, baseURi + "guid=" + Guid);

                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectURL = string.Empty;

                    while (links.MoveNext())
                    {
                        Links link = links.Current;

                        if (link.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectURL = link.href;
                        }


                    }
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPaymnt = ExecutePayment(apicontext, PayerId, Session[guid] as string);


                    if (executedPaymnt.ToString().ToLower() != "approved")
                    {
                        return View("FailureView");
                    }

                }
            }
            catch (Exception ex)
            {

                PaypalLogger.Log("Error: " + ex.Message);
                return View("FailureView");


                //throw;
            }
            return View("SuccessView");

        }

        private object ExecutePayment(APIContext apicontext, string payerId, string PaymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            payment = new Payment() { id = PaymentId };
            return payment.Execute(apicontext, paymentExecution);
        }

        private PayPal.Api.Payment payment;

        private Payment CreatePayment(APIContext apicontext, string redirectURl)
        {

            var ItemLIst = new ItemList() { items = new List<Item>() };

            
           
                List<Models.Home.Item> cart = (List<Models.Home.Item>)(Session["cart"]);
                foreach (var item in cart)
                {
                    ItemLIst.items.Add(new Item()
                    {
                        name = item.Product.ProductName,
                        currency = "USD",
                        price = item.Product.Price.ToString(),
                        quantity = item.Product.Quantity.ToString(),
                        sku = "sku"
                    });


                }


                var payer = new Payer() { payment_method = "paypal" };

                var redirUrl = new RedirectUrls()
                {
                    cancel_url = redirectURl + "&Cancel=true",
                    return_url = redirectURl
                };

                var details = new Details()
                {
                    tax = "1",
                    shipping = "2",
                    subtotal = Session["SesTotal"].ToString()
                };

                var amount = new Amount()
                {
                    currency = "USD",
                    total = (Convert.ToDouble(details.tax) + Convert.ToDouble(details.shipping) + Convert.ToDouble(details.subtotal)).ToString(),
                    details = details
                };

                var transactionList = new List<Transaction>();
                transactionList.Add(new Transaction()
                {
                    description = "Transaction Description",
                    invoice_number = Convert.ToString((new Random()).Next(100000)),
                    amount = amount,
                    item_list = ItemLIst

                });

                payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirUrl
                };
            



            return payment.Create(apicontext);

        }
    }
}