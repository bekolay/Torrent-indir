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
    public class HomeController : Controller
    {
        private BerkayEntities db = new BerkayEntities();

        public ActionResult Index([Bind(Include = "Blogİd,BlogAdi,BlogUrl,BlogAciklama,BlogYazi,BlogTarih,Kullaniciİd,BlogAnahtar,BlogAktifMi,BlogResim,BlogGösterimSayisi,BlogKategoriİd")]int? page, Bloglar bloglar)
        {

            int pageIndex = page ?? 1;
            int dataCount = 6;
            var blog = db.Bloglars.Include(b => b.Kullanici).Where(p => p.BlogAktifMi).OrderByDescending(p => p.BlogTarih);
            return View(blog.ToPagedList(pageIndex, dataCount));
        }

        
        public ActionResult Ara(string kelime, int? page)
        {
            int pageIndex = page ?? 1;
            int dataCount = 6;
            ViewBag.Kelime = kelime;
            return View(db.Bloglars.Include(b => b.Kullanici).Where(p => p.BlogAktifMi).OrderByDescending(p => p.BlogAdi.Contains(kelime)).ToPagedList(pageIndex, dataCount));
        }

        public ActionResult Contact()
        {

            return View();
        }

        public ActionResult Telif()
        {
            return View();
        }
    }
}