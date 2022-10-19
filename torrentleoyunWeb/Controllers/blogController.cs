using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using torrentloyunVera;
using WebMatrix.WebData;

namespace torrentleoyunWeb.Controllers
{
    public class BlogController : Controller
    {
        // GET: Kategori
        private BerkayEntities db = new BerkayEntities();

        public ActionResult Detay(string id)
        {
            var bloglar = db.Bloglars
             .Include(p => p.BlogYorums)
             .Include(p => p.Kullanici)
             .FirstOrDefault(p => p.BlogAktifMi && p.BlogUrl == id);


            ViewBag.Yorumlar = bloglar.BlogYorums.Where(p=>p.BlogAkitf == true).ToArray();
            //ViewBag.blogid = bloglar.BlogİD;
            //ViewBag.KullaniciId = WebSecurity.GetUserId(User.Identity.Name);
            ViewBag.Blogİd = bloglar.Blogİd;
            bloglar.BlogGösterimSayisi += 1;
            db.SaveChanges();
            return View(bloglar);
        }

        public JsonResult yorumGonder([Bind(Include = "BlogYorumYazi,BlogYorumAdi,BlogTarih,BlogEposta,BlogAkitf,Blogİd")] BlogYorum blogYorum)
        {



            string kullaniciAdi = blogYorum.BlogYorumAdi;
            string Mesaj = blogYorum.BlogYorumYazi;
            string mail = blogYorum.BlogEposta;
            if (kullaniciAdi == null)
            {

                if (mail == null)
                {
                    ModelState.AddModelError("", "Hata");
                }

                ModelState.AddModelError("", "Hata Oluştu");

            }
            else
            {
                blogYorum.BlogAkitf = false;
                blogYorum.BlogTarih = DateTime.Now;
                db.BlogYorums.Add(blogYorum);
                db.SaveChanges();
                ModelState.AddModelError("", "Başarılı bir şekilde gönderildi.");
            }


            return Json(false, JsonRequestBehavior.AllowGet);
        }

    }
}