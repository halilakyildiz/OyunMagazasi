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
    public partial class frm_home : Form
    {
        public static frm_giris_kayit giris_form = null;
        public static string oyun_ad = "";
        public frm_home()
        {
            InitializeComponent();
        }
        NpgsqlConnection baglanti = new NpgsqlConnection("server=localHost; port=5432; Database=DbOyunMagazasi; user ID=postgres; password=halzey28");
        private void tab_magaza_Click(object sender, EventArgs e)
        {
        }
        void OyunlarimListele()
        {
            string sorgu = "select oyun.oyunad from kullanicioyun inner join oyun on oyun.oyunid=kullanicioyun.oyunid where kullaniciid=(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad+ "')  order by kul_oyun_id";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dgv_oyunlarim.DataSource = ds.Tables[0];
        }
        void MagazaLisele()
        {
            string sorgu = "select oyunad,satisadeti,fiyat from oyun order by oyunid";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dgv_magaza.DataSource = ds.Tables[0];
        }
        void CokSatilanlarListele()
        {
            string sorgu = "select oyun.oyunad,oyun.satisadeti from coksatilanlar inner join oyun on oyun.oyunid=coksatilanlar.oyunid ";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dgv_coksatilanlar.DataSource = ds.Tables[0];
        }
        void ProfilGetir()
        {
            baglanti.Open();
            string sorgu = "select * from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "'";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            NpgsqlDataReader reader = komut.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                string ad = reader["ad"].ToString();
                string soyad = reader["soyad"].ToString();
                string kul_adi = reader["kullaniciadi"].ToString();
                string parola = reader["parola"].ToString();

                tb_profil_ad.Text = ad;
                tb_profil_soyad.Text = soyad;
                tb_profil_kulAdi.Text = kul_adi;
                tb_profil_parola.Text = parola;
            }
            baglanti.Close();           
        }
        void IletisimGetir()
        {
            baglanti.Open();
            string alt_sorgu = "(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "')";
            string sorgu = "select * from iletisim where kullaniciid=" + alt_sorgu + "";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            NpgsqlDataReader reader = komut.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                string email = reader["email"].ToString();
                string adres = reader["adres"].ToString();

                tb_iletisim_email.Text = email;
                rtb_iletisim_adres.Text = adres;
            }
            baglanti.Close();
        }
        private void btn_cikis_Click(object sender, EventArgs e)
        {
            this.Close();           
        }
      
        void BakiyeGetir()
        {
            string sorgu = "select bakiye from cuzdan where kullaniciid=(select kullaniciid from kullanici where kullaniciadi='"+frm_giris_kayit.kul_Ad+"')";
            baglanti.Open();
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            NpgsqlDataReader reader = komut.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                lbl_bakiye.Text = reader["bakiye"].ToString();
            }
            baglanti.Close();
        }
        private void frm_home_Load(object sender, EventArgs e)
        {
            MagazaLisele();
            OyunlarimListele();
            ProfilGetir();
            IletisimGetir();
            BakiyeGetir();
            CokSatilanlarListele();

            dgv_magaza.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_magaza.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv_oyunlarim.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_oyunlarim.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv_coksatilanlar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_coksatilanlar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

      

        private void dgv_magaza_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }
        bool OyunKontrol(string oyunadi)
        {
            baglanti.Open();
            string sorgu = "select kullanici.kullaniciadi from kullanicioyun " +
                "inner join kullanici on kullanicioyun.kullaniciid=kullanici.kullaniciid where oyunid=(select oyunid from oyun where oyunad='"+oyunadi+"')";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            NpgsqlDataReader reader = komut.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string kul_adi = reader["kullaniciadi"].ToString();
                    if (kul_adi == frm_giris_kayit.kul_Ad)
                    {
                        baglanti.Close();
                        return true;
                    }
                }          

            }
            baglanti.Close();
            return false;
        }
        private void btn_oyunal_Click(object sender, EventArgs e)
        {
            if (lbl_secilen.Text == "")
            {
                MessageBox.Show("Bir oyun seçin");
                return;
            }
            if (OyunKontrol(lbl_secilen.Text))
            {
                MessageBox.Show("Bu oyuna sahipsniz");
                return;
            }


            try
            {
                int oyun_id = 0, kullanici_id = 0;
                double bakiye = 0;
                double oyun_fiyati = 0;

                baglanti.Open();
                string sorgu = "select * from oyun where oyunad='"+lbl_secilen.Text+"'";
                NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
                NpgsqlDataReader reader = komut.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    oyun_fiyati = Convert.ToDouble(reader["fiyat"]);
                    oyun_id = Convert.ToInt32(reader["oyunid"]);
                }
                baglanti.Close();

                baglanti.Open();
                sorgu = "select * from cuzdan oyun where kullaniciid=(select kullaniciid from kullanici where kullaniciadi='"+frm_giris_kayit.kul_Ad+"')";
                komut = new NpgsqlCommand(sorgu, baglanti);
                reader = komut.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    bakiye = Convert.ToDouble(reader["bakiye"]);
                    kullanici_id = Convert.ToInt32(reader["kullaniciid"]);
                }
                baglanti.Close();

                if (oyun_fiyati > bakiye)
                {
                    MessageBox.Show("Bakiyeniz Yetersiz. Yükleme yapınız");
                    return;
                }
                
                //string sorgu1 = "(select oyunid from oyun where oyunad='" + lbl_secilen.Text + "')";
                //string sorgu2 = "(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "')";
                sorgu = "insert into kullanicioyun (oyunid,kullaniciid) values (" + oyun_id.ToString() + "," + kullanici_id.ToString() + ")";
                baglanti.Open();
                komut = new NpgsqlCommand(sorgu, baglanti);
                komut.ExecuteNonQuery();
                baglanti.Close();

                baglanti.Open();
               // string kullanici_altsorgu = "(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "')";
                //string oyunfiyat_altsorgu = "(select fiyat from oyun where oyunad='" + lbl_secilen.Text + "')";
                sorgu = "update cuzdan set bakiye=bakiye-" + oyun_fiyati.ToString() + " where kullaniciid=" + kullanici_id.ToString() + "";
                komut = new NpgsqlCommand(sorgu, baglanti);
                komut.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Kayıt eklendi");

                OyunlarimListele();
                BakiyeGetir();
                MagazaLisele();
                CokSatilanlarListele();
            }

           catch
            {
                MessageBox.Show("Kayıt eklenemedi");
            }
        }

        private void btn_oyunlarim_yenile_Click(object sender, EventArgs e)
        {
            OyunlarimListele();
        }

        private void dgv_magaza_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string secilen = dgv_magaza.CurrentRow.Cells[0].Value.ToString();
            lbl_secilen.Text = secilen;
        }

        private void tab_oyunlarim_Click(object sender, EventArgs e)
        {

        }

        private void dgv_oyunlarim_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string secilen = dgv_oyunlarim.CurrentRow.Cells[0].Value.ToString();
            lbl_oyunlarim_secilen.Text = secilen;
        }

        private void btn_yorumekle_Click(object sender, EventArgs e)
        {
            oyun_ad = lbl_oyunlarim_secilen.Text;
            if (oyun_ad == "")
            {
                MessageBox.Show("Bir oyun seçin");
                return;
            }
            frm_yorumEkle yorumekle = new frm_yorumEkle();
            yorumekle.ShowDialog();
        }

        private void tab_profil_Click(object sender, EventArgs e)
        {

        }

        private void btn_profil_guncelle_Click(object sender, EventArgs e)
        {
            string ad = tb_profil_ad.Text;
            string soyad = tb_profil_soyad.Text;
            string kull_adi = tb_profil_kulAdi.Text;
            string parola = tb_profil_parola.Text;

            try
            {
                string sorgu = "update kullanici set ad='"+ad+"',soyad='"+soyad+"',kullaniciadi='"+kull_adi+"',parola='"+parola+"' where " +
                "kullaniciid=(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "')";
                baglanti.Open();
                NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
                komut.ExecuteNonQuery();
                baglanti.Close();
                ProfilGetir();
                MessageBox.Show("Kayıt güncellendi");
            }
            catch
            {
                MessageBox.Show("Kayıt güncellenemedi");
            }
            
        }

        private void btn_iletisim_guncelle_Click(object sender, EventArgs e)
        {
            string email = tb_iletisim_email.Text;
            string adres = rtb_iletisim_adres.Text;

            try
            {
                string sorgu = "update iletisim set email='"+email+"',adres='"+adres+"' where " +
                "kullaniciid=(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "')";
                baglanti.Open();
                NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
                komut.ExecuteNonQuery();
                baglanti.Close();
                IletisimGetir();
                MessageBox.Show("Kayıt güncellendi");
            }
            catch
            {
                MessageBox.Show("Kayıt güncellenemedi");
            }
        }

        private void btn_oyundetay_Click(object sender, EventArgs e)
        {
            oyun_ad = lbl_secilen.Text;
            if (oyun_ad == "")
            {
                MessageBox.Show("Bir oyun seçin");
                return;
            }
            frm_oyunDetay oyundetay = new frm_oyunDetay();
            oyundetay.ShowDialog();
        }

        private void btn_hesapsil_Click(object sender, EventArgs e)
        {
            baglanti.Open();

            // yorum sil
            NpgsqlCommand komut = new NpgsqlCommand("delete from yorum where kullaniciid=(select kullaniciid from kullanici where kullaniciadi='"+frm_giris_kayit.kul_Ad+"')", baglanti);            
            komut.ExecuteNonQuery();
            baglanti.Close();

            // alınan oyun sil
            baglanti.Open();
            komut = new NpgsqlCommand("delete from kullanicioyun where kullaniciid=(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "')", baglanti);
            komut.ExecuteNonQuery();
            baglanti.Close();

            // cüzdan sil
            baglanti.Open();
            komut = new NpgsqlCommand("delete from cuzdan where kullaniciid=(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "')", baglanti);
            komut.ExecuteNonQuery();
            baglanti.Close();

            // iletişim sil
            baglanti.Open();
            komut = new NpgsqlCommand("delete from iletisim where kullaniciid=(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "')", baglanti);
            komut.ExecuteNonQuery();
            baglanti.Close();

            // kullanıcı sil
            baglanti.Open();
            komut = new NpgsqlCommand("delete from kullanici where  kullaniciid=(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "')",baglanti);
            komut.ExecuteNonQuery();
            baglanti.Close();
            
            MessageBox.Show("Kayıt silindi");
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tb_bakiye.Text == "")
            {
                MessageBox.Show("Yüklemek istediğiniz miktarı girin");
                return;
            }
            string para = tb_bakiye.Text.ToString();
            int sonuc = para.IndexOf(",");
            if (sonuc != -1)
            {
                MessageBox.Show("Ondalık sayıyı nokta kullanarak girin");
                return;
            }
            if (Convert.ToInt32(tb_bakiye.Text) < 0)
            {
                MessageBox.Show("Geçerli bir miktar giriniz");
                return;
            }
             try
             { 
                baglanti.Open();
                string sorgu = "update cuzdan set bakiye= bakiye + (" + para + ") where kullaniciid=(select kullaniciid from kullanici where kullaniciadi='" + frm_giris_kayit.kul_Ad + "')";
                NpgsqlCommand cmd = new NpgsqlCommand(sorgu, baglanti);
                cmd.ExecuteNonQuery();                
                MessageBox.Show("Yükleme başarılı");               
                tb_bakiye.Text = "";
                baglanti.Close();
                BakiyeGetir();
             }
             catch (Exception)
             {
                baglanti.Close();
                MessageBox.Show("Yükleme yapılamadı");
             }
            
        }
    }
}
