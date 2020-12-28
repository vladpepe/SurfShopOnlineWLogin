using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using OnlineShoppingStore.DAL;
using OnlineShoppingStore.Models.Home;
using OnlineShoppingStore.ViewModels;


namespace OnlineShoppingStore.Models
{
    public class Gmail
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        //Send email to confirm that the order is successufuly done (not shipped yet)
        public static void SendEmail(CreateOrderViewModel model ,string products )
        {
            MailMessage mm = new MailMessage("pepesurfshop@gmail.com", model.Member.EmailId);
            mm.Body = "Hi "  + model.Member.FristName + " " + model.Member.LastName + "\n \n" +
                "You have successfully made a order on PepeSurfShop! \n \n " +
                "Here you can see the details of your order: \n " +               
                "\n" +
                "\n" +
                "Shipping adress : " + model.ShippingDetails.Adress + "\n"+
                "City : " + model.ShippingDetails.City + "\n" +
                "Country : " + model.ShippingDetails.Country + "\n" +
                "\n" +
                "\n" + products +
                
                "\n" +
                "Total price : " + model.ShippingDetails.AmountPaid.ToString() + "\n" +
                "";
            
            mm.Subject = "Your PepeSurfShop Order";
            mm.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("pepesurfshop@gmail.com", "Qwerty.1234");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mm);
        }


        //Send email to confirm that the order is shipped
        public static void SendEmail(Tbl_Members member, Tbl_Orders order)
        {
            MailMessage mm = new MailMessage("pepesurfshop@gmail.com", member.EmailId);
            mm.Body = "Hi " + member.FristName + " " + member.LastName + "\n \n" +
                "You order on PepeSurfShop was already shipped \n \n " +
                "\n" +
                "Now It's up to the postal service to carry your products ! \n" +
                "\n" +
                "\n" +
                "Order ID : " + order.OrderId + "\n" +
                "Your order stauts : " + order.OrderStatus + "\n" +
                "\n" +
                "\n" +
                "Please do not hesitate to contact us for any issue ! \n" +
                "\n" +
                "\n" +
                "Best regards\n" +
                "PepeSurfShop\n" +
                "";

            mm.Subject = "Your PepeSurfShop Order Shipped";
            mm.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("pepesurfshop@gmail.com", "Qwerty.1234");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mm);
        }

    }

}
