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


namespace torrentleoyunWeb.Controllers
{
    public class HesapController : Controller
    {
        // GET: Hesap

        private BerkayEntities db = new BerkayEntities();
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Kullanicilar()
        {
            ViewBag.Aktifolmayan = db.Kullanicis.Where(p => p.KullaniciAktif == false);

            return View(db.Kullanicis.ToList());
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                //string password = Tools.Md5(model.Password);
                string password = model.Sifre.Md5();

                Kullanici user = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == model.KullaniciAdi && p.KullaniciSifre == password);

                if (user == null)
                {
                    ViewBag.Error = "Kullanıcı Adınız veya Şifreniz Hatalı";
                    return View();
                }

                if (user.KullaniciAktif == false)
                {
                    ViewBag.Error = "Banlandınız acaba kim bilir ne yaptınız :)";
                    return View();
                }


                FormsAuthentication.RedirectFromLoginPage(user.KullaniciAdi.ToString(), model.BeniHatirla);
            }

            return Redirect(ReturnUrl ?? "/Hesap/AnaSayfa");
        }

        public ActionResult UyeOl()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UyeOl([Bind(Include = "Kullaniciİd,KullaniciAdi,KullaniciAdSoyad,KullaniciTarih,KullaniciSifre,KullaniciSifreTekrar,KullaniciDogum,KullaniciEposta,KullaniciGuvenlik,KullaniciAktif,KullaniciUrl")] Kullanici kullanici)
        {
            var Kullanici = db.Kullanicis.Where(a => a.KullaniciEposta == kullanici.KullaniciEposta).FirstOrDefault();
            //eğer yoksa kaydı ekle
            if (Kullanici == null)
            {
                if (kullanici.KullaniciSifre == null)
                {
                    ModelState.AddModelError("", "Şifre Girilmedi");
                    return View();
                }
                if (kullanici.KullaniciSifreTekrar == null)
                {
                    ModelState.AddModelError("", "Şifre Girilmedi");
                    return View();
                }
                if (kullanici.KullaniciAdi == null)
                {
                    ModelState.AddModelError("", "Kullanıcı Adı Girilmedi");
                    return View();
                }
                if (kullanici.KullaniciAdSoyad == null)
                {
                    ModelState.AddModelError("", "Ad Soyad Girilmedi");
                    return View();
                }
                if (kullanici.KullaniciEposta == null)
                {
                    ModelState.AddModelError("", "E posta Girilmedi");
                    return View();
                }
                if (kullanici.KullaniciSifre != kullanici.KullaniciSifreTekrar)
                {
                    ModelState.AddModelError("", "Şifreler Aynı Değil ");
                    return View();
                }
                if (kullanici.KullaniciGuvenlik != kullanici.KullaniciGuvenlik)
                {
                    ModelState.AddModelError("", "Şifreler Aynı Değil ");
                    return View();
                }
                kullanici.KullaniciGuvenlik = kullanici.KullaniciGuvenlik.Md5();
                kullanici.KullaniciUrl = kullanici.KullaniciAdi.Seo();
                kullanici.KullaniciAktif = true;
                kullanici.KullaniciSifreTekrar = kullanici.KullaniciSifreTekrar.Md5();
                kullanici.KullaniciSifre = kullanici.KullaniciSifre.Md5();
                kullanici.KullaniciTarih = DateTime.Now;
                ////Guid nesnesini benzersiz dosya adı oluşturmak için tanımladık ve Replace ile aradaki “-” işaretini atıp yan yana yazma işlemi yaptık.
                //string DosyaAdi = Guid.NewGuid().ToString().Replace(kullanici.KullaniciResim, "");
                ////Kaydetceğimiz resmin uzantısını aldık.
                //string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
                //string TamYolYeri = "~/images/" + DosyaAdi + uzanti;
                ////Eklediğimiz Resni Server.MapPath methodu ile Dosya Adıyla birlikte kaydettik.
                //Request.Files[0].SaveAs(Server.MapPath(TamYolYeri));
                ////Ve veritabanına kayıt için dosya adımızı değişkene aktarıyoruz.

                kullanici.KullaniciUrl = kullanici.KullaniciAdi.Seo();
                db.Kullanicis.Add(kullanici);
                db.SaveChanges();
                Roles.AddUserToRole(kullanici.KullaniciAdi, "Kullanıcı");
                ModelState.AddModelError("", "Kayıt Başarılı");
                return View();
            }
            //kullanıcı daha önceden varsa hata mesajı gönder..
            else
            {
                ModelState.AddModelError("", "E-posta adresi bulunmakta.");
                return View();
            }
        }

        [Authorize(Roles = "Admin,Kullanici")]
        public ActionResult AnaSayfa()
        {
            return View();
        }

        // GET: VeraTTAdmin/Kullanicis

        [Authorize(Roles = "Admin,Kullanici")]
        public ActionResult Ayarlar()
        {
            Kullanici kullanici3 = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == p.KullaniciAdi);

            Session["Kullanici3"] = kullanici3.KullaniciAdSoyad;


            int k = WebSecurity.GetUserId(User.Identity.Name);
            var kullanici = db.Kullanicis.FirstOrDefault(x => x.Kullaniciİd == k);
            if (kullanici == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(kullanici);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Ayarlar")]
        public ActionResult Ayarlar2([Bind(Include = "Kullaniciİd,KullaniciAdi,KullaniciAdSoyad,KullaniciTarih,KullaniciSifre,KullaniciSifreTekrar,KullaniciDogum,KullaniciEposta,KullaniciGuvenlik,KullaniciAktif,KullaniciUrl")] Kullanici kullanici)
        {

            if (kullanici.KullaniciAdi == null)
            {
                Kullanici kullanici3 = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == p.KullaniciAdi);

                Session["Kullanici3"] = kullanici3.KullaniciAdSoyad;

                ModelState.AddModelError("", "Ad Soyad Yazılmadı");
                return View();
            }

            if (kullanici.KullaniciAdSoyad == null)
            {
                Kullanici kullanici3 = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == p.KullaniciAdi);

                Session["Kullanici3"] = kullanici3.KullaniciAdSoyad;

                ModelState.AddModelError("", "Ad Soyad Yazılmadı");
                return View();
            }
            if (kullanici.KullaniciSifre == null)
            {
                Kullanici kullanici3 = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == p.KullaniciAdi);

                Session["Kullanici3"] = kullanici3.KullaniciAdi;


                ModelState.AddModelError("", "Şifre Yazılmadı");
                return View();
            }
            if (kullanici.KullaniciSifre == null)
            {

                Kullanici kullanici3 = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == p.KullaniciAdi);

                Session["Kullanici3"] = kullanici3.KullaniciAdi;


                ModelState.AddModelError("", "Şifre Tekrar Yazılmadı");
                return View();
            }
            if (kullanici.KullaniciEposta == null)
            {
                Kullanici kullanici3 = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == p.KullaniciAdi);

                Session["Kullanici3"] = kullanici3.KullaniciAdi;


                ModelState.AddModelError("", "E posta Yazılmadı");
                return View();
            }
            if (kullanici.KullaniciSifreTekrar != kullanici.KullaniciSifreTekrar)
            {
                Kullanici kullanici3 = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == p.KullaniciAdi);

                Session["Kullanici3"] = kullanici3.KullaniciAdi;

                ModelState.AddModelError("", "Şifreler Aynı Değil ");
                return View();
            }
            if (ModelState.IsValid)
            {
                ////Guid nesnesini benzersiz dosya adı oluşturmak için tanımladık ve Replace ile aradaki “-” işaretini atıp yan yana yazma işlemi yaptık.
                //string DosyaAdi = Guid.NewGuid().ToString().Replace(kullanici.KullaniciResim, "");
                ////Kaydetceğimiz resmin uzantısını aldık.
                //string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
                //string TamYolYeri = "/Images/" + DosyaAdi + uzanti;
                ////Eklediğimiz Resni Server.MapPath methodu ile Dosya Adıyla birlikte kaydettik.
                //Request.Files[0].SaveAs(Server.MapPath(TamYolYeri));
                //Ve veritabanına kayıt için dosya adımızı değişkene aktarıyoruz.
                //kullanici.KullaniciResim = DosyaAdi + uzanti;
                //kullanici.KullaniciKategoriİd = 3;
                kullanici.KullaniciEposta = kullanici.KullaniciEposta;
                kullanici.KullaniciAktif = true;
                kullanici.KullaniciSifreTekrar = kullanici.KullaniciSifreTekrar.Md5();
                kullanici.KullaniciSifre = kullanici.KullaniciSifre.Md5();
                kullanici.KullaniciTarih = DateTime.Now;
                db.Entry(kullanici).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(kullanici);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            Kullanici kullanici3 = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == p.KullaniciAdi);

            Session["Kullanici3"] = kullanici3.KullaniciAdi;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kullanici kullanici = db.Kullanicis.Find(id);
            if (kullanici == null)
            {
                return HttpNotFound();
            }
            return View(kullanici);
        }


        // POST: VeraTTAdmin/Kullanicis/Edit/5
        // Aşırı gönderim saldırılarından korunmak için, lütfen bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için https://go.microsoft.com/fwlink/?LinkId=317598 sayfasına bakın.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Kullaniciİd,KullaniciAdi,KullaniciAdSoyad,KullaniciTarih,KullaniciSifre,KullaniciSifreTekrar,KullaniciDogum,KullaniciEposta,KullaniciGuvenlik,KullaniciAktif,KullaniciUrl")] Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                kullanici.KullaniciSifreTekrar = kullanici.KullaniciSifreTekrar.Md5();
                kullanici.KullaniciSifre = kullanici.KullaniciSifre.Md5();
                kullanici.KullaniciTarih = DateTime.Now;
                db.Entry(kullanici).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Kullanicilar");
            }
            return View(kullanici);
        }

        [Authorize(Roles = "Kullanici")]
        public ActionResult BloglarKullanici()
        {
            int k = WebSecurity.GetUserId(User.Identity.Name);
            var bloglar1 = db.Bloglars.Where(x => x.Kullaniciİd == k && x.BlogAktifMi == true).ToList();
            var bloglar = db.Bloglars.Include(b => b.BlogKategori).Include(b => b.Kullanici);
            ViewBag.Aktifblog = db.Bloglars.Include(b => b.BlogKategori).Where(p => p.BlogAktifMi == false && p.Kullaniciİd == k).ToList();
            ViewBag.Yorumlar = bloglar.ToList();
            return View(bloglar1);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Bloglar()
        {
            var bloglar = db.Bloglars.Include(b => b.BlogKategori).Where(p => p.BlogAktifMi == true);
            ViewBag.Aktifblog = db.Bloglars.Include(b => b.BlogKategori).Where(p => p.BlogAktifMi == false).ToList();
            return View(bloglar.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlogEkle()
        {
            ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
            ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = "Admin")]
        public ActionResult BlogEkle([Bind(Include = "Blogİd,BlogAdi,BlogUrl,BlogAciklama,BlogYazi,BlogTarih,Kullaniciİd,BlogAnahtar,BlogAktifMi,BlogResim,BlogGösterimSayisi,BlogKategoriİd")] Bloglar bloglar)
        {
            var Kullanici = db.Bloglars.Where(a => a.BlogAdi == bloglar.BlogAdi).FirstOrDefault();
            //eğer yoksa kaydı ekle
            if (Kullanici == null)
            {
                if (bloglar.BlogResim == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Resim Seçilmedi");
                    return View();
                }
                if (bloglar.BlogAciklama == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Açıklama Yazılmadı");
                    return View();
                }
                if (bloglar.BlogKategoriİd == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Kategori seçilmedi");
                    return View();
                }
                if (bloglar.BlogAdi == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Blog adı yazılmadı");
                    return View();
                }
                if (bloglar.BlogAnahtar == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Blog etiket yazılmadı");
                    return View();
                }
                if (bloglar.BlogYazi == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Blog yazı yazılmadı");
                    return View();
                }
                //eğer dosya gelmişse işlemleri yap
                if (Request.Files.Count > 0)
                {
                    string DosyaAdi = Guid.NewGuid().ToString().Replace(bloglar.BlogResim, "");
                    //Kaydetceğimiz resmin uzantısını aldık.
                    string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
                    string TamYolYeri = "/images/" + DosyaAdi + uzanti;
                    //Eklediğimiz Resni Server.MapPath methodu ile Dosya Adıyla birlikte kaydettik.
                    //Ve veritabanına kayıt için dosya adımızı değişkene aktarıyoruz.
                    if (ModelState.IsValid)
                    {
                        bloglar.BlogResim = DosyaAdi + uzanti;
                        bloglar.Kullaniciİd = WebSecurity.GetUserId(User.Identity.Name);
                        bloglar.BlogUrl = bloglar.BlogAdi.Seo();
                        int gösterim = bloglar.BlogGösterimSayisi = 0;
                        bloglar.BlogTarih = DateTime.Now;
                        bloglar.BlogAktifMi = true;
                        db.Bloglars.Add(bloglar);
                        Request.Files[0].SaveAs(Server.MapPath(TamYolYeri));
                        db.SaveChanges();
                        return RedirectToAction("Bloglar");
                    }
                }
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                db.Bloglars.Add(bloglar);
                db.SaveChanges();
                ModelState.AddModelError("", "Kayıt Başarılı");
                return View();
            }
            //kullanıcı daha önceden varsa hata mesajı gönder..
            else
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Blog adı daha önce girilmiş");
                return View();
            }
        }

        // GET: YonetimOzbasoft/Bloglars/Edit/5

        [Authorize(Roles = "Admin")]
        public ActionResult BlokEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bloglar bloglar = db.Bloglars.Find(id);
            Kullanici kullanici = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == p.KullaniciAdi);

            Session["Kullanici"] = kullanici.KullaniciAdSoyad;
            if (bloglar == null)
            {
                return HttpNotFound();
            }
            ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
            ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
            return View(bloglar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = "Admin")]
        public ActionResult BlokEdit([Bind(Include = "Blogİd,BlogAdi,BlogUrl,BlogAciklama,BlogYazi,BlogTarih,Kullaniciİd,BlogAnahtar,BlogAktifMi,BlogResim,BlogGösterimSayisi,BlogKategoriİd")] Bloglar bloglar)
        {
            if (bloglar.BlogAciklama == null)
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Açıklama Yazılmadı");
                return View();
            }
            if (bloglar.BlogKategoriİd == null)
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Kategori seçilmedi");
                return View();
            }
            if (bloglar.BlogAdi == null)
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Blog adı yazılmadı");
                return View();
            }
            if (bloglar.BlogAnahtar == null)
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Blog etiket yazılmadı");
                return View();
            }
            if (bloglar.BlogYazi == null)
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Blog yazı yazılmadı");
                return View();
            }
            //string DosyaAdi = Guid.NewGuid().ToString().Replace(bloglar.BlogResim, "");
            ////Kaydetceğimiz resmin uzantısını aldık.
            //string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
            //string TamYolYeri = "/Images/" + DosyaAdi + uzanti;
            ////Eklediğimiz Resni Server.MapPath methodu ile Dosya Adıyla birlikte kaydettik.
            //Request.Files[0].SaveAs(Server.MapPath(TamYolYeri));
            ////Ve veritabanına kayıt için dosya adımızı değişkene aktarıyoruz.
            if (ModelState.IsValid)
            {
                bloglar.Blogİd = bloglar.Blogİd;
                bloglar.BlogResim = bloglar.BlogResim;
                bloglar.BlogUrl = bloglar.BlogAdi.Seo();
                //bloglar.BlogResim = DosyaAdi + uzanti;
                bloglar.BlogTarih = bloglar.BlogTarih;
                db.Entry(bloglar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Bloglar");
            }
            return View(bloglar);
        }

        [Authorize(Roles = "Admin,Kullanici")]
        public ActionResult BlogEkleKullanici()
        {
            ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
            ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
            return View();
        }

        // POST: Bloglars/Create
        // Aşırı gönderim saldırılarından korunmak için, lütfen bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için https://go.microsoft.com/fwlink/?LinkId=317598 sayfasına bakın.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = "Admin,Kullanici")]
        public ActionResult BlogEkleKullanici([Bind(Include = "Blogİd,BlogAdi,BlogUrl,BlogAciklama,BlogYazi,BlogTarih,Kullaniciİd,BlogAnahtar,BlogAktifMi,BlogResim,BlogGösterimSayisi,BlogKategoriİd")] Bloglar bloglar)
        {
            var Kullanici = db.Bloglars.Where(a => a.BlogAdi == bloglar.BlogAdi).FirstOrDefault();
            //eğer yoksa kaydı ekle
            if (Kullanici == null)
            {
                if (bloglar.BlogResim == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Resim Seçilmedi");
                    return View();
                }
                if (bloglar.BlogAciklama == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Açıklama Yazılmadı");
                    return View();
                }
                if (bloglar.BlogKategoriİd == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Kategori seçilmedi");
                    return View();
                }
                if (bloglar.BlogAdi == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Blog adı yazılmadı");
                    return View();
                }
                if (bloglar.BlogAnahtar == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Blog etiket yazılmadı");
                    return View();
                }
                if (bloglar.BlogYazi == null)
                {
                    ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                    ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                    ModelState.AddModelError("", "Blog yazı yazılmadı");
                    return View();
                }
                //eğer dosya gelmişse işlemleri yap
                if (Request.Files.Count > 0)
                {
                    string DosyaAdi = Guid.NewGuid().ToString().Replace(bloglar.BlogResim, "");
                    //Kaydetceğimiz resmin uzantısını aldık.
                    string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
                    string TamYolYeri = "/images/" + DosyaAdi + uzanti;
                    //Eklediğimiz Resni Server.MapPath methodu ile Dosya Adıyla birlikte kaydettik.
                    //Ve veritabanına kayıt için dosya adımızı değişkene aktarıyoruz.
                    if (ModelState.IsValid)
                    {
                        bloglar.BlogResim = DosyaAdi + uzanti;
                        bloglar.Kullaniciİd = WebSecurity.GetUserId(User.Identity.Name);
                        bloglar.BlogUrl = bloglar.BlogAdi.Seo();
                        int gösterim = bloglar.BlogGösterimSayisi = 0;
                        bloglar.BlogTarih = DateTime.Now;
                        bloglar.BlogAktifMi = false;
                        db.Bloglars.Add(bloglar);
                        Request.Files[0].SaveAs(Server.MapPath(TamYolYeri));
                        db.SaveChanges();
                        return RedirectToAction("BloglarKullanici");
                    }
                }
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                db.Bloglars.Add(bloglar);
                db.SaveChanges();
                ModelState.AddModelError("", "Kayıt Başarılı");
                return View();
            }
            //kullanıcı daha önceden varsa hata mesajı gönder..
            else
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Blog adı daha önce girilmiş");
                return View();
            }
        }

        // GET: YonetimOzbasoft/Bloglars/Edit/5

        [Authorize(Roles = "Kullanici")]
        public ActionResult BlokEditKullanici(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bloglar bloglar = db.Bloglars.Find(id);
            Kullanici kullanici = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == p.KullaniciAdi);

            Session["Kullanici"] = kullanici.KullaniciAdSoyad;
            if (bloglar == null)
            {
                return HttpNotFound();
            }
            ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
            ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
            return View(bloglar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = "Kullanici")]
        public ActionResult BlokEditKullanici([Bind(Include = "Blogİd,BlogAdi,BlogUrl,BlogAciklama,BlogYazi,BlogTarih,Kullaniciİd,BlogAnahtar,BlogAktifMi,BlogResim,BlogGösterimSayisi,BlogKategoriİd")] Bloglar bloglar)
        {
            if (bloglar.BlogAciklama == null)
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Açıklama Yazılmadı");
                return View();
            }
            if (bloglar.BlogKategoriİd == null)
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Kategori seçilmedi");
                return View();
            }
            if (bloglar.BlogAdi == null)
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Blog adı yazılmadı");
                return View();
            }
            if (bloglar.BlogAnahtar == null)
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Blog etiket yazılmadı");
                return View();
            }
            if (bloglar.BlogYazi == null)
            {
                ViewBag.BlogKategoriİd = new SelectList(db.BlogKategoris, "BlogKategoriİd", "BlogKategoriAdi");
                ViewBag.Kullaniciİd = new SelectList(db.Kullanicis, "Kullaniciİd", "KullaniciAdi");
                ModelState.AddModelError("", "Blog yazı yazılmadı");
                return View();
            }
            //string DosyaAdi = Guid.NewGuid().ToString().Replace(bloglar.BlogResim, "");
            ////Kaydetceğimiz resmin uzantısını aldık.
            //string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
            //string TamYolYeri = "/Images/" + DosyaAdi + uzanti;
            ////Eklediğimiz Resni Server.MapPath methodu ile Dosya Adıyla birlikte kaydettik.
            //Request.Files[0].SaveAs(Server.MapPath(TamYolYeri));
            ////Ve veritabanına kayıt için dosya adımızı değişkene aktarıyoruz.
            if (ModelState.IsValid)
            {
                bloglar.Blogİd = bloglar.Blogİd;
                bloglar.BlogResim = bloglar.BlogResim;
                bloglar.BlogUrl = bloglar.BlogAdi.Seo();
                //bloglar.BlogResim = DosyaAdi + uzanti;
                bloglar.BlogTarih = DateTime.Now;
                db.Entry(bloglar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("BloglarKullanici");
            }
            return View(bloglar);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Kategoriler()
        {
            return View(db.BlogKategoris.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlogKategoriEkle()
        {
            return View();
        }

        // POST: BlogKategoris/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult BlogKategoriEkle([Bind(Include = "BlogKategoriİd,BlogKategoriAdi,BlogKategoriUrl,BlogKategoriTarih")] BlogKategori blogKategori)
        {
            if (blogKategori.BlogKategoriAdi == null)
            {
                ModelState.AddModelError("", "Kategori adı girilmedi");
                return View();
            }

            if (ModelState.IsValid)
            {
                blogKategori.BlogKategoriUrl = blogKategori.BlogKategoriAdi.Seo();
                blogKategori.BlogKategoriTarih = DateTime.Now;
                db.BlogKategoris.Add(blogKategori);
                db.SaveChanges();
                return RedirectToAction("Kategoriler");
            }

            return View(blogKategori);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlogKategoriDüzenle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogKategori blogKategori = db.BlogKategoris.Find(id);
            if (blogKategori == null)
            {
                return HttpNotFound();
            }
            return View(blogKategori);
        }

        // POST: BlogKategoris/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult BlogKategoriDüzenle([Bind(Include = "BlogKategoriİd,BlogKategoriAdi,BlogKategoriUrl,BlogKategoriTarih")] BlogKategori blogKategori)
        {
            if (blogKategori.BlogKategoriAdi == null)
            {
                ModelState.AddModelError("", "Kategori adı girilmedi");
                return View();
            }

            if (ModelState.IsValid)
            {
                blogKategori.BlogKategoriUrl = blogKategori.BlogKategoriAdi.Seo();
                blogKategori.BlogKategoriTarih = DateTime.Now;
                db.Entry(blogKategori).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Kategoriler");
            }
            return View(blogKategori);
        }

        // GET: BlogKategoris/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult BlogKategoriSil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogKategori blogKategori = db.BlogKategoris.Find(id);
            if (blogKategori == null)
            {
                return HttpNotFound();
            }
            return View(blogKategori);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Yorumlar()
        {
            var blogYorums = db.BlogYorums.Include(b => b.Bloglar).Where(p=>p.BlogAkitf == true);
            ViewBag.Aktifblog = db.BlogYorums.Include(b => b.Bloglar).Where(p => p.BlogAkitf == false).ToList();
            return View(blogYorums.ToList());
        }

        // GET: BlogYorums/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult YorumEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogYorum blogYorum = db.BlogYorums.Find(id);
            if (blogYorum == null)
            {
                return HttpNotFound();
            }
            ViewBag.Blogİd = new SelectList(db.Bloglars, "Blogİd", "BlogAdi", blogYorum.Blogİd);
            return View(blogYorum);
        }

        // POST: BlogYorums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult YorumEdit([Bind(Include = "BlogYorumİd,BlogYorumYazi,BlogYorumAdi,BlogTarih,BlogEposta,BlogAkitf,Blogİd")] BlogYorum blogYorum)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blogYorum).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Yorumlar");
            }
            ViewBag.Blogİd = new SelectList(db.Bloglars, "Blogİd", "BlogAdi", blogYorum.Blogİd);
            return View(blogYorum);
        }

        // GET: BlogKategoris/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult YorumSil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogYorum Yorum = db.BlogYorums.Find(id);
            if (Yorum == null)
            {
                return HttpNotFound();
            }
            return View(Yorum);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("YorumSil")]
        [ValidateAntiForgeryToken]
        public ActionResult YorumSil2(int id)
        {
            BlogYorum blogYorum = db.BlogYorums.Find(id);
            db.BlogYorums.Remove(blogYorum);
            db.SaveChanges();
            return RedirectToAction("Yorumlar");
        }


        [Authorize(Roles = "Admin")]
        // POST: BlogKategoris/Delete/5
        [HttpPost, ActionName("BlogKategoriSil")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogKategori blogKategori = db.BlogKategoris.Find(id);
            db.BlogKategoris.Remove(blogKategori);
            db.SaveChanges();
            return RedirectToAction("Kategoriler");
        }

        public ActionResult Cikis()
        {
            Kullanici kullanici3 = db.Kullanicis.FirstOrDefault(p => p.KullaniciAdi == p.KullaniciAdi);

            Session["Kullanici3"] = kullanici3.KullaniciAdi;

            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
