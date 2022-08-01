using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OyunMagazasi
{
    public partial class frm_yorumEkle : Form
    {
        public frm_yorumEkle()
        {
            InitializeComponent();
        }
        NpgsqlConnection baglanti = new NpgsqlConnection("server=localHost; port=5432; Database=DbOyunMagazasi; user ID=postgres; password=halzey28");

        private void btn_yorumekle_Click(object sender, EventArgs e)
        {
            if (rtb_yorum.Text == "")
            {
                MessageBox.Show("Yorum giriniz");
                return;
            }
           //string sorgu1 = "(select oyunid from oyun where oyunad='" + frm_home.oyun_ad + "')";
           // string sorgu2 = "(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "')";
            // string sorgu = "insert into yorum (icerik,oyunid,kullaniciid) values ('" + rtb_yorum.Text.Trim() + "'," + sorgu1 + "," + sorgu2 + ")";
           // string sorgu = "insert into yorum (icerik,oyunid,kullaniciid) values ('" + rtb_yorum.Text.Trim() + "'," + sorgu1 + "," + sorgu2 + ")";
       
              try
             {
                baglanti.Open();
                NpgsqlCommand komut = new NpgsqlCommand("call yorumekle(:p1, :p2, :p3)", baglanti);// yordam(procedure)
                komut.CommandType = CommandType.Text;
                komut.Parameters.AddWithValue("p1", DbType.String).Value = rtb_yorum.Text;
                komut.Parameters.AddWithValue("p2", DbType.String).Value = frm_home.oyun_ad;
                komut.Parameters.AddWithValue("p3", DbType.String).Value = frm_giris_kayit.kul_Ad;
                komut.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("Kayıt eklendi");
                this.Close();
              }
              catch
              {
                  MessageBox.Show("Kayıt eklenemedi");
              }
        }
    }
}
