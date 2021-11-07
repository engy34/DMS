using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
namespace DMS.Controllers
{
    public class ShoppingController : Controller
    {
        ModelDataContext db = new Models.ModelDataContext();
        // GET: Shopping
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Mobile()
        {
            return View();
        }
        public ActionResult Fashion()
        {
            return View();
        }
        public ActionResult AddItem()
        {
            return View();
        }
        public ActionResult Cart()
        {
            return View();
        }
        [HttpPost]
        public string GetItemList(byte type)
        {
            if (type == 0)
            {
                var model = (from u in db.Items
                             join i in db.images on u.id equals  i.itemid into ui
                             from i in ui.DefaultIfEmpty()
                             select new {u.id, u.ItemName, u.ItemPrice, u.itemColor,u.Description,im=i.imagePath}).ToList();
           

              
                if (model != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string rws = serializer.Serialize(model);
                    return rws;
                }
                else
                    return null;
            }
            if (type == 1) {
                var model = (from u in db.Items
                             join i in db.images on u.id equals i.itemid into ui
                             from i in ui.DefaultIfEmpty()
                             where u.Category=="1"
                             select new { u.id, u.ItemName, u.ItemPrice, u.itemColor, u.Description,im=i.imagePath }).ToList();


         
                if (model != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string rws = serializer.Serialize(model);
                    return rws;
                }
                else
                    return null;


            }
            if (type == 2)
            {
                var model = (from u in db.Items
                             join i in db.images on u.id equals i.itemid into ui
                             from i in ui.DefaultIfEmpty()
                             where u.Category == "0"
                             select new { u.id, u.ItemName, u.ItemPrice, u.itemColor, u.Description,im=i.imagePath }).ToList();



                if (model != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string rws = serializer.Serialize(model);
                    return rws;
                }
                else
                    return null;


            }
            else return null;
        
        }

    [HttpPost]
    public string GetOrders(string custid) {
         
                var model = (from u in db.Order_Details
                             join i in db.Order_Headers on u.id equals i.orderId into ui
                             from i in ui.DefaultIfEmpty()
                             where i.CustomerId==custid
                             select new { u.id, u.itemName, u.ItemPrice, u.Qty, u.TotalPrice,date=i.OrderDate.Value.ToShortDateString() }).ToList();



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
        public string GetUnits()
        {
            
                var model = (from u in db.UOMs

                             select new { u.id, u.UOM1 }).ToList();
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
        public void save(string itemName, string desc, string Category, float price, string qty,string unit,string color,float tax)
        {

          
            var itemId = 0;
            Item obj = new Item(); 
           
            var id = (from u in db.Items
                      select new { u.id }).OrderByDescending(u => Convert.ToInt32(u.id)).ToList().FirstOrDefault();
            if (id != null) { itemId = Convert.ToInt32(id.id)+1; }
            else { itemId = 1; }
            obj = new Item()
            {
              id=itemId.ToString(),
              ItemName=itemName,
              Category=Category,
              Description=desc,
              itemColor=color,
              itemTax=tax,
              QTY=qty,
              UOM=unit,
              ItemPrice=price,
              imageByte="",
              imagePath="",

            };

            db.Items.InsertOnSubmit(obj);
            db.SubmitChanges();

        }
        [HttpPost]
        public void AddCart(string itemid, string userid) {
            var headerid = 0;
            var orderId = 0;
            Order_Detail obj = new Order_Detail();
            Order_Header obj2 = new Order_Header();
            var id = (from u in db.Order_Details
                      select new { u.id}).OrderByDescending(u => Convert.ToInt32(u.id)).ToList().FirstOrDefault();
            if (id != null) { orderId = Convert.ToInt32(id.id) + 1; }
            else { orderId = 1; }

            var id2 = (from h in db.Order_Headers
                       select new { h.id }).OrderByDescending(u => Convert.ToInt32(u.id)).ToList().FirstOrDefault();
            if (id2 != null) { headerid = Convert.ToInt32(id2.id) + 1; }
            else { headerid = 1; }
            var detail = (from i in db.Items
                          where i.id == itemid
                          select new { i.ItemPrice, i.ItemName, i.QTY, i.itemTax, i.UOM }).SingleOrDefault();
            obj = new Order_Detail()
            { id= orderId.ToString(),
                ItemId = itemid,
                Discount = 0,
                itemName = detail.ItemName == null ? "" : detail.ItemName,
              
                UOM = detail.UOM == null ? "" : detail.UOM,
                ItemPrice=detail.ItemPrice==0?0:detail.ItemPrice,
                Qty=detail.QTY==null?0:double.Parse( detail.QTY),
               Tax=detail.itemTax==0?0:detail.itemTax,
               TotalPrice=detail.ItemPrice-detail.itemTax,
               custid=userid,

            };

            db.Order_Details.InsertOnSubmit(obj);
            obj2 = new Order_Header()
            { id = headerid.ToString(),
                CustomerId = userid,
                DiscountCode = "",
                DiscountValue = 0,
                OrderDate = DateTime.Now,
                RequestDate = DateTime.Now,
                DueDate = DateTime.Now,
                Status = "",
                TaxCode = "",
                TaxValue = detail.itemTax,
                orderId = orderId.ToString(),
                TotalPrice=obj.TotalPrice,
            };

            db.Order_Headers.InsertOnSubmit(obj2);


            db.SubmitChanges();

        }
        [HttpPost]
        public JsonResult upload( HttpPostedFileWrapper ImageFile)
        {
            var imgid = 0;
            var itemid = 0;
            var file = ImageFile;
            var item = (from l in db.Items

                        select new {l.id }).OrderByDescending(u=>Convert.ToInt32( u.id)).ToList().FirstOrDefault();
            if (item== null) { itemid = 1; }
            else itemid = Convert.ToInt32(item.id) + 1;
            var id = (from l in db.images

                        select new { l.id }).OrderByDescending(u => Convert.ToInt32(u.id)).ToList().FirstOrDefault();
            if (id == null) { imgid = 1; }
            else imgid =Convert.ToInt32( id.id)+1;
            byte[] imagebyte = null;
            if (file != null)
            {
               
                file.SaveAs(Server.MapPath("/Images/" + file.FileName));
                BinaryReader reader = new BinaryReader(file.InputStream);
                imagebyte = reader.ReadBytes(file.ContentLength);
                image img = new image();
                img.imageByte = imagebyte;
                img.imagePath = "/Images/" + file.FileName;
                img.itemid = itemid.ToString();
                img.id = imgid.ToString();
                
                db.images.InsertOnSubmit(img);
                db.SubmitChanges();

            }
            return Json(file.FileName, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public int deletefromCart(string id,string custid)
        {
            

            Order_Detail objM = db.Order_Details.Where(w => w.id ==id&&w.custid==custid ).FirstOrDefault();
            db.Order_Details.DeleteOnSubmit(objM);
            Order_Header objH = db.Order_Headers.Where(w => w.id == id && w.CustomerId == custid).FirstOrDefault();
            db.Order_Headers.DeleteOnSubmit(objH);
            db.SubmitChanges();

            return 1;
        }
        [HttpPost]
        public int deleteItem(string id, string custid) {

            Item item = db.Items.Where(w => w.id == id).FirstOrDefault();
            db.Items.DeleteOnSubmit(item);
            image img = db.images.Where(w => w.itemid== id ).FirstOrDefault();
            db.images.DeleteOnSubmit(img);
            db.SubmitChanges();

            return 1;
        }
    }
}