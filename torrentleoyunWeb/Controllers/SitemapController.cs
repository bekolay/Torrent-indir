using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using torrentloyunVera;

namespace torrentleoyunWeb.Controllers
{
    public class SitemapController : Controller
    {
        private BerkayEntities db = new BerkayEntities();
        // GET: Sitemap
        public ActionResult Index()
        {
            Response.Clear();
            //Response.ContentTpye ile bu Action'ýn View'ýný XML tabanlý olarak ayarlýyoruz.
            Response.ContentType = "text/xml";
            XmlTextWriter xr = new XmlTextWriter(Response.OutputStream, Encoding.UTF8);
            xr.WriteStartDocument();
            xr.WriteStartElement("urlset");//urlset etiketi açýyoruz
            xr.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            xr.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xr.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/siteindex.xsd");
            /* sitemap dosyamýzýn olmazsa olmazýný ekledik. Þemasý bu dedik buraya kadar.  */

            //Burada veritabanýmýzdaki Personelleri SiteMap'e ekliyoruz.
            var s = db.Bloglars.OrderByDescending(p=>p.BlogTarih).Where(p => p.BlogAktifMi == true);
            foreach (var a in s)
            {
                xr.WriteStartElement("url");
                xr.WriteElementString("loc", "https://torrenttenidir.com/blog/" + a.BlogUrl);
                xr.WriteElementString("lastmod", a.BlogTarih.ToString("yyyy-MM-dd"));
                xr.WriteElementString("priority", "0.9");
                xr.WriteElementString("changefreq", "daily");
                xr.WriteEndElement();
            }
            var d = db.BlogKategoris.OrderByDescending(p => p.BlogKategoriTarih);
            foreach (var b in d)
            {
                xr.WriteStartElement("url");
                xr.WriteElementString("loc", "https://torrenttenidir.com/K/" + b.BlogKategoriUrl);
                xr.WriteElementString("lastmod", b.BlogKategoriTarih.ToString("yyyy-MM-dd"));
                xr.WriteElementString("priority", "0.9");
                xr.WriteElementString("changefreq", "daily");
                xr.WriteEndElement();
            }

            xr.WriteStartElement("url");
            xr.WriteElementString("loc", "https://torrenttenidir.com/");
            xr.WriteElementString("lastmod", "2020-05-29");
            xr.WriteElementString("changefreq", "daily");
            xr.WriteElementString("priority", "1");
            xr.WriteEndElement();

            xr.WriteEndDocument();
            //urlset etiketini kapattýk
            xr.Flush();
            xr.Close();
            Response.End();
            return View();
        }
    }
}