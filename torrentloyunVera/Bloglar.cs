//------------------------------------------------------------------------------
// <auto-generated>
//    Bu kod bir şablondan oluşturuldu.
//
//    Bu dosyada el ile yapılan değişiklikler uygulamanızda beklenmedik davranışa neden olabilir.
//    Kod yeniden oluşturulursa, bu dosyada el ile yapılan değişikliklerin üzerine yazılacak.
// </auto-generated>
//------------------------------------------------------------------------------

namespace torrentloyunVera
{
    using System;
    using System.Collections.Generic;
    
    public partial class Bloglar
    {
        public Bloglar()
        {
            this.BlogYorums = new HashSet<BlogYorum>();
        }
    
        public int Blogİd { get; set; }
        public string BlogAdi { get; set; }
        public string BlogUrl { get; set; }
        public string BlogAciklama { get; set; }
        public string BlogYazi { get; set; }
        public System.DateTime BlogTarih { get; set; }
        public int Kullaniciİd { get; set; }
        public string BlogAnahtar { get; set; }
        public bool BlogAktifMi { get; set; }
        public string BlogResim { get; set; }
        public int BlogGösterimSayisi { get; set; }
        public int BlogKategoriİd { get; set; }
    
        public virtual BlogKategori BlogKategori { get; set; }
        public virtual Kullanici Kullanici { get; set; }
        public virtual ICollection<BlogYorum> BlogYorums { get; set; }
    }
}
