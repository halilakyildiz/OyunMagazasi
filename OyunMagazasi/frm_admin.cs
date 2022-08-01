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
    public partial class frm_admin : Form
    {
        NpgsqlConnection baglanti = new NpgsqlConnection("server=localHost; port=5432; Database=DbOyunMagazasi; user ID=postgres; password=halzey28");
        int x = 0;
        void OyunListele()
        {
            string sorgu = "select oyunad,satisadeti from oyun order by oyunid";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dgv_magaza.DataSource = ds.Tables[0];
        }
        void YapimciListele()
        {
            combo_yapimci.Items.Clear();
            string sorgu = "select * from oyunyapimcisi";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                combo_yapimci.Items.Add(dr["yapimciadi"].ToString());
            }
            combo_yapimci.Items.Insert(0, "Yapımcı Seç");
            combo_yapimci.SelectedIndex = 0;
        }
        void ToplamOyunSayisiGetir()
        {
            string sorgu = "select * from toplamoyun";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                lbl_toplamoyun.Text = (dr["oyunsayisi"].ToString());
            }
        }
        public frm_admin()
        {
            InitializeComponent();
            OyunListele();
            YapimciListele();
            dgv_magaza.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_magaza.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void frm_admin_Load(object sender, EventArgs e)
        {
            ToplamOyunSayisiGetir();
        }

        private void dgv_magaza_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string secilen = dgv_magaza.CurrentRow.Cells[0].Value.ToString();
            lbl_secilen.Text = secilen;
            tb_guncelle.Text = lbl_secilen.Text;
        }
        List<string> KategoriGetir()
        {
            List<string> kategoriler = new List<string>();
            if (cb_acikdunya.Checked == true)
                kategoriler.Add("Açık Dünya");
            if (cb_aksiyon.Checked == true)
                kategoriler.Add("Aksiyon");
            if (cb_korku.Checked == true)
                kategoriler.Add("Korku");
            if (cb_fantastik.Checked == true)
                kategoriler.Add("Fantastik");
            if (cb_fps.Checked == true)
                kategoriler.Add("FPS");
            if (cb_macera.Checked == true)
                kategoriler.Add("Macera");
            if (cb_strateji.Checked == true)
                kategoriler.Add("Strateji");
            if (cb_yaris.Checked == true)
                kategoriler.Add("Yarış");
            return kategoriler;
        }
        bool CheckBoxKontrol()
        {
            if (cb_acikdunya.Checked == true)
                return true;
            else if (cb_aksiyon.Checked == true)
                return true;
            else if (cb_korku.Checked == true)
                return true;
            else if (cb_fantastik.Checked == true)
                return true;
            else if (cb_fps.Checked == true)
                return true;
            else if (cb_macera.Checked == true)
                return true;
            else if (cb_strateji.Checked == true)
                return true;
            else if (cb_yaris.Checked == true)
                return true;
            return false;
        }
        void EkleElemanlariSıfırla()
        {
            tb_fiyat.Text = "";
            combo_yapimci.SelectedIndex = 0;

            cb_acikdunya.Checked = false;
            cb_aksiyon.Checked = false;
            cb_korku.Checked = false;
            cb_fantastik.Checked = false;
            cb_fps.Checked = false;
            cb_macera.Checked = false;
            cb_strateji.Checked = false;
            cb_yaris.Checked = false;

        }
        bool OyunKontrol(string oyunadi)
        {
            baglanti.Open();
            string sorgu = "select *from oyun where oyunad='" + oyunadi + "'";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            NpgsqlDataReader reader = komut.ExecuteReader();

            while (reader.HasRows)
            {
                reader.Read();
                baglanti.Close();
                return true;
            }
                baglanti.Close();
                return false;           
            
        }
        private void btn_ekle_Click(object sender, EventArgs e)
        {
            string ad = tb_ekle.Text;
            int satis_adet = 0;
            if (CheckBoxKontrol() == false)
            {
                MessageBox.Show("En az bir adet kategori seçmelisiniz");
                return;
            }
            if(combo_yapimci.SelectedItem.ToString()=="Yapımcı Seç")
            {
                MessageBox.Show("Geçerli bir yapımcı seçin");
                return;
            }
            if (tb_fiyat.Text == "")
            {
                MessageBox.Show("Fiyat giriniz");
                return;
            }
            string para = tb_fiyat.Text.ToString();
            int sonuc = para.IndexOf(",");
            if (sonuc != -1)
            {
                MessageBox.Show("Ondalık sayıyı nokta kullanarak girin");
                return;
            }

            if (OyunKontrol(tb_ekle.Text.Trim()))
            {
                MessageBox.Show("Bu isimde oyun zaten mevcut");
                return;
            }
            List<string> kategoriler = KategoriGetir();
            if (ad != "")
            {
                baglanti.Open();
                string sorgu = "insert into oyun (oyunad,satisadeti,yapimciid,fiyat) values (@p1,@p2,(select yapimciid from oyunyapimcisi where yapimciadi='"+combo_yapimci.SelectedItem.ToString()+"'),"+tb_fiyat.Text.ToString()+")";
                NpgsqlCommand cmd = new NpgsqlCommand(sorgu, baglanti);
                cmd.Parameters.AddWithValue("@p1", ad);
                cmd.Parameters.AddWithValue("@p2", satis_adet);
                cmd.ExecuteNonQuery();
                baglanti.Close();               
                tb_ekle.Text = "";                

                for (int i = 0; i < kategoriler.Count; i++)
                {
                    baglanti.Open();
                    string oyunid_altsorgu = "(select oyunid from oyun where oyunad='" + ad + "')";
                    string kategoriid_altsorgu = "(select kategoriid from kategori where kategoriad='" + kategoriler[i] + "')";
                    sorgu = "insert into kategorioyun (oyunid,kategoriid) values (" + oyunid_altsorgu + "," + kategoriid_altsorgu + ")";
                    cmd = new NpgsqlCommand(sorgu, baglanti);
                    cmd.ExecuteNonQuery();
                    baglanti.Close();
                }
                    MessageBox.Show("Kayıt Eklendi");
                    OyunListele();
                    ToplamOyunSayisiGetir();
                EkleElemanlariSıfırla();
            }
            else
            {
                MessageBox.Show("Bir oyun adı giriniz");
            }
        }

        private void btn_guncelle_Click(object sender, EventArgs e)
        {
            if (lbl_secilen.Text != "")
            {
                lbl_guncelle_uyari.Text = "Kutucuğa yeni veriyi girin";

                
                string sorgu = "";
                if (tb_guncelle.Text != lbl_secilen.Text)
                {
                    baglanti.Open();
                    if (lbl_secilen.Text != "")
                        sorgu = "update oyun set oyunad='" + tb_guncelle.Text + "' " +
                            "where oyunid=(select oyunid from oyun where oyunad='" + lbl_secilen.Text + "')";
                    else
                        MessageBox.Show("Bir oyun seciniz");
                    NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
                    komut.ExecuteNonQuery();
                    baglanti.Close();
                    OyunListele();
                    MessageBox.Show("Kayıt Güncellendi");
                }
                else
                    MessageBox.Show("Veride değişiklik yapmadınız");              
               
            }
            else
                MessageBox.Show("Bir oyun seçiniz");
        }

        private void btn_sil_Click(object sender, EventArgs e)
        {
            if (lbl_secilen.Text != "")
            {
                baglanti.Open();

                // oyuna ait yorumları sil
                NpgsqlCommand komut = new NpgsqlCommand("call yorumsil(:p1)", baglanti);// yordam(procedure)
                komut.CommandType = CommandType.Text;
                komut.Parameters.AddWithValue("p1", DbType.String).Value = lbl_secilen.Text;
                komut.ExecuteNonQuery();
                baglanti.Close();

                // cok satilanalrdan sil
                baglanti.Open();
                komut = new NpgsqlCommand("call coksatilan_sil(:p1)", baglanti);// yordam(procedure)
                komut.CommandType = CommandType.Text;
                komut.Parameters.AddWithValue("p1", DbType.String).Value = lbl_secilen.Text;
                komut.ExecuteNonQuery();
                baglanti.Close();

                // oyuna ait kategorileri sil
                baglanti.Open();
                komut = new NpgsqlCommand("call kategorisil(:p1)", baglanti);// yordam(procedure)
                komut.CommandType = CommandType.Text;
                komut.Parameters.AddWithValue("p1", DbType.String).Value = lbl_secilen.Text;
                komut.ExecuteNonQuery();
                baglanti.Close();

                // oyunu satın alan kişilerden oyunu sil
                baglanti.Open();
                komut = new NpgsqlCommand("call oyuniade(:p1)", baglanti);// yordam(procedure)
                komut.CommandType = CommandType.Text;
                komut.Parameters.AddWithValue("p1", DbType.String).Value = lbl_secilen.Text;
                komut.ExecuteNonQuery();
                baglanti.Close();

                // son olarak oyunu sil
                baglanti.Open();
                komut = new NpgsqlCommand("call oyunsil(:p1)", baglanti);// yordam(procedure)
                komut.CommandType = CommandType.Text;
                komut.Parameters.AddWithValue("p1", DbType.String).Value = lbl_secilen.Text;
                komut.ExecuteNonQuery();
                baglanti.Close();

                OyunListele();
                MessageBox.Show("Kayıt silindi");
                tb_guncelle.Text = "";
                baglanti.Close();
                ToplamOyunSayisiGetir();
            }
            else
                MessageBox.Show("Bir oyun seçiniz");
            
        }

        private void btn_yapimciEkle_Click(object sender, EventArgs e)
        {
            frm_yapimciEkle yapimci_frm = new frm_yapimciEkle();
            yapimci_frm.ShowDialog();
            YapimciListele();
        }
       
    }
}
