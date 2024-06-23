using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Data.SqlClient;

namespace txtbox_xml
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection con= new SqlConnection("SQL DATA SOURCE BİLGİLERİNİZİ BURAYA GİRİN");
        BindingSource bs = new BindingSource();

        XmlDocument doc = new XmlDocument();
        



        public void listele()
        {
            SqlDataAdapter verial= new SqlDataAdapter("select * from human", con);
            DataTable dt= new DataTable();
            verial.Fill(dt);
            bs.DataSource = dt;
            dataGridView1.DataSource = bs;

        }
        public void list()
        {
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.Columns.Add("Kimlik");
            listView1.Columns.Add("Ad");
            listView1.Columns.Add("Soyad");
            listView1.Columns.Add("Adres");

        }

        public void clear()
        {
            var textboxs= Controls.OfType<TextBox>();
            foreach(TextBox textbox in textboxs)
            {
                textbox.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Adım 1: Xml dosyasını oluştur ve load et
            doc.Load(Application.StartupPath + "\\isim.xml");

            //Adım 2: Oluşturulan dosya üzerinden yeni bir element oluştur ve özellik olarak (setAttribute()) primary değerini ata
            XmlElement human = doc.CreateElement("human");
            human.SetAttribute("kimlik", textBox1.Text);

            //Adım 3: Gerekli node'ları oluşturulan dosya üzerinden oluştur ve innertextile değerleri ata. Element altına append et
            XmlNode ad = doc.CreateNode(XmlNodeType.Element, "ad","");
            ad.InnerText= textBox2.Text;
            human.AppendChild(ad);

            XmlNode soyad = doc.CreateNode(XmlNodeType.Element, "soyad", "");
            soyad.InnerText = textBox3.Text;
            human.AppendChild(soyad);

            XmlNode adres = doc.CreateNode(XmlNodeType.Element, "adres", "");
            adres.InnerText = textBox4.Text;
            human.AppendChild(adres);

            //Adım 4: Elementi dosya altına append et
            doc.DocumentElement.AppendChild(human);

            //Adım 5: dosyası save et
            doc.Save(Application.StartupPath + "\\isim.xml");


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            list();
            listele();
            dataGridView1.ReadOnly= true;
            dataGridView2.ReadOnly= true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ListViewItem item=  new ListViewItem(textBox1.Text);
            item.SubItems.Add(textBox2.Text);
            item.SubItems.Add(textBox3.Text);
            item.SubItems.Add(textBox4.Text);
            
            listView1.Items.Add(item);
            clear();


        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                doc.Load(Application.StartupPath + "\\isim.xml");

                foreach(ListViewItem item in listView1.Items)
                {
                    XmlElement human = doc.CreateElement("human");
                    human.SetAttribute("kimlik", item.SubItems[0].Text);

                    XmlNode ad = doc.CreateNode(XmlNodeType.Element, "ad", "");
                    ad.InnerText = item.SubItems[1].Text;
                    human.AppendChild(ad);

                    XmlNode soyad = doc.CreateNode(XmlNodeType.Element, "soyad", "");
                    soyad.InnerText = item.SubItems[2].Text;
                    human.AppendChild(soyad);

                    XmlNode adres = doc.CreateNode(XmlNodeType.Element, "adres", "");
                    adres.InnerText = item.SubItems[3].Text;
                    human.AppendChild(adres);

                    doc.DocumentElement.AppendChild(human);
                }
                doc.Save(Application.StartupPath + "\\isim.xml");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("insert into human(kimlik, ad, soyad, adres) values(@kimlik, @ad, @soyad, @adres)", con);
            cmd.Parameters.AddWithValue("@kimlik", textBox1.Text);
            cmd.Parameters.AddWithValue("@ad", textBox2.Text);
            cmd.Parameters.AddWithValue("@soyad", textBox3.Text);
            cmd.Parameters.AddWithValue("@adres", textBox4.Text);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            clear();
            listele();



        }

        private void button5_Click(object sender, EventArgs e)
        {
            SqlCommand xmlcmd = new SqlCommand("select * from human", con);
            con.Open();
            SqlDataReader oku = xmlcmd.ExecuteReader();

            doc.Load(@Application.StartupPath + "\\isim.xml");

            while (oku.Read())
            {
                XmlElement human = doc.CreateElement("human");
                human.SetAttribute("kimlik", oku["kimlik"].ToString());

                string[] columns = { "ad", "soyad", "adres" }; 
                foreach (string i in columns) 
                {
                    XmlNode node = doc.CreateNode(XmlNodeType.Element, i, "");
                    node.InnerText = oku[i].ToString();
                    human.AppendChild(node);
                }
                doc.DocumentElement.AppendChild(human);
            }

            con.Close();
            doc.Save(@Application.StartupPath + "\\isim.xml");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                //Adım 1: Dosyayı yükle
                doc.Load(@Application.StartupPath + "\\isim.xml");
                //Adım 2: Dosya içeriğinin null olma olasılığını kontrol et
                if (doc.DocumentElement != null)
                {
                    //Adım 3: Dataset oluştur ve datasetin readxml özelliği ile dosyayı oku
                    //XmlNodeReader kullanma sebebimiz veriyi şerit benzeri bir yapıda okumak içindir. Yani dizi şeklinde
                    DataSet ds = new DataSet();
                    ds.ReadXml(new XmlNodeReader(doc));
                    //Şuan dataset içerisinde xml dosyasındaki tablomuz var bunu verisetinden veritablosuna aktarmamız gerekiyor

                    //Adım 4: Tablomuzun kolon isimlerini içeren bir dizi oluştur
                    string[] columnorder = { "kimlik", "ad", "soyad", "adres"};

                    //Adım 5: Datatable oluştur ve foreach ile dizideki okolon isimlerini datatable ın kolonlarına ekle
                    DataTable dt = new DataTable();
                    foreach (var columnname in columnorder)
                    {
                        dt.Columns.Add(columnname, typeof(string));
                        //Columns.Add() methodu 2 parametre alır birincisi veri, 
                    }

                    //Adım 6: Dataset içerisindeki tablomuzu yeni oluşturduğumuz datatable ın satırlarına aktarıyoruz
                    //Tables[0] dememizin sebebi xml dosyamızın içinde şuan tek tablo var
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        //Yeni bir row oluşturup ona verilerimizin bulunduğu tablodan newrow diyerek atama yapıyoruz
                        DataRow drw = dt.NewRow();
                        foreach (var columnname in columnorder)
                        {   //drw yeni oluşturduğumuz datarowu temsil ederken row dataset içindeki tablomuzdaki rowu temsil eder
                            //Temel olarak yaptığı şey verilerin bulunduğu satırdaki verileri yeni oluşturduğumuz satırlara kopyalamaktır.
                            drw[columnname] = row[columnname];
                        }
                        //kopyalamalar bitince satırı datatable'a ekliyoruz
                        dt.Rows.Add(drw);
                    }

                    dataGridView2.DataSource = dt;

                    for (int i = 0; i < dataGridView2.Columns.Count; i++)
                    {
                        dataGridView2.Columns[i].HeaderText = dt.Columns[i].ColumnName;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
