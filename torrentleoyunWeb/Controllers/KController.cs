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
    public class KController : Controller
    {
        // GET: Kategoriler
        public ActionResult Index([Bind(Include = "BlogKategoriİd,BlogKategoriAdi,BlogKategoriUrl,BlogKategoriTarih")] BlogKategori Kategori, string id, int? page)
        {
            int _page = page ?? 1;
            using (BerkayEntities db = new BerkayEntities())
            {
                //BlogKategori k = db.BlogKategori.Include(p => p.Kullanic).FirstOrDefault(p => p.KategoriAktif && p.KategoriUrl == id);
                var k = db.BlogKategoris.Include(p => p.Bloglars).FirstOrDefault(p => p.BlogKategoriUrl == id);
                ViewBag.Kategori = k.BlogKategoriAdi;
                ViewBag.url = k.BlogKategoriUrl;


                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                return View(k.Bloglars.Where(p => p.BlogAktifMi).OrderByDescending(p => p.BlogTarih).ToPagedList(_page, 7));
            }
        }
    }
}