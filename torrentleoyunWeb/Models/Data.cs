using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using torrentloyunVera;

namespace torrentleoyunWeb.Models
{
    public static class Data
    {

        public static torrentloyunVera.Bloglar[] Gosterim
        {
            get
            {
                using (BerkayEntities db = new BerkayEntities())
                {
                    return db.Bloglars.Include(p => p.BlogKategori).Include(p => p.Kullanici).OrderByDescending(p => p.BlogGösterimSayisi).Where(p => p.BlogAktifMi).Take(5).ToArray();
                }
            }
        }

        public static torrentloyunVera.BlogKategori[] Kategoriler
        {
            get
            {
                using (BerkayEntities db = new BerkayEntities())
                {
                    return db.BlogKategoris.OrderByDescending(p => p.BlogKategoriİd).ToArray();
                }
            }
        }

    }
}