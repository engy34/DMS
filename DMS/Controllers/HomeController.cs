using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS.Controllers
{

    public class HomeController : Controller
    {
        ModelDataContext db = new Models.ModelDataContext();
        public ActionResult login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }

        public string getUsers(string email, string password)
        {
            var model = (from f in db.Customers

                         where (email != "" ? f.Email == email : 1 == 1 && password != "" ? f.Password == password : 1 == 1)
                         select new { f.CustCode ,f.Username }
                       ).FirstOrDefault();

            if (model != null)
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string rws = serializer.Serialize(model);
                return rws;

            }
            else
                return null;
        }
        [HttpPost]
        public void save(string username, string password, byte sex,string email,string phone)
        {
            var custId = 0;
            Customer obj = db.Customers.SingleOrDefault(s => s.Email == email );// Request.Cookies["BranchId"].Value
            if (obj == null)
                obj = new Customer();
            var id = (from u in db.Customers
                      select new { u.CustCode }).OrderByDescending(u => Convert.ToInt32(u.CustCode)).ToList().FirstOrDefault();
            if (id != null) { custId =Convert.ToInt32( id.CustCode)+1;  }
            else { custId = 1; }
            obj = new Customer()
            { Email = email,
            Password=password,
            Phone=phone,
            CustDescArab="",
            CustDescEng=username,
            sex=sex,
            Username=username,
            CustCode=custId.ToString(),
             
            };

            db.Customers.InsertOnSubmit(obj);
            db.SubmitChanges();

        }

    }
}