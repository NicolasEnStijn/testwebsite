using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using WebApplication2;

namespace WebApplication2
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private remoteprojects rp;
        private bool Ipadres;

        protected void Page_Load(object sender, EventArgs e)
        {

            string xml = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\remoteprojects.xml");
            var catalog1 = xml.ParseXML<remoteprojects>();
            rp = (remoteprojects)catalog1;
            int i;
            for (i = 0; i < rp.projects.GetLength(0); i++)
            {
                TableRow tRow = new TableRow();
                Table1.Rows.Add(tRow);
                Button btn = new Button();
                btn.ID = i.ToString();
                btn.Click += new EventHandler(SelectButton_Click);
                TableCell tcb = new TableCell();
                tcb.Controls.Add(btn);
                tRow.Cells.Add(tcb);
                TableCell tcn = new TableCell();
                tcn.Text = rp.projects[i].name;
                tRow.Cells.Add(tcn);
                TableCell tc = new TableCell();
                tc.Text = rp.projects[i].municipality;
                tRow.Cells.Add(tc);
            }
           
        }

        protected void SelectButton_Click(object sender, EventArgs e)
        {
            bool status = COMPortHelper.IsServiceRunning();
            Button button = sender as Button;
            if (rp.projects[Convert.ToInt32(button.ID)].ipaddress.Equals(""))
            {
                TextBox1.Text = rp.projects[Convert.ToInt32(button.ID)].hostname; // als IP-adres leeg is gebruik hostname
                Ipadres = false;
            }
            else
            {
                TextBox1.Text = rp.projects[Convert.ToInt32(button.ID)].ipaddress;
                Ipadres = true;
            }
            TextBox2.Text = rp.projects[Convert.ToInt32(button.ID)].programtolaunch; // vul in de textbox in welk programma we gebruiken
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if(Ipadres)
            COMPortHelper.changecomport(TextBox2.Text, TextBox1.Text); // vul het juist IP-adres in bij de juiste comport
            else
            COMPortHelper.changehostname(TextBox2.Text, TextBox1.Text); // vul  het juiste hostname  in bij de juiste comport
        }
    }

    internal static class ParseHelpers
    {

        public static Stream ToStream(this string @this)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static T ParseXML<T>(this string @this) where T : class
        {
            var reader = XmlReader.Create(@this.Trim().ToStream(), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document });
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }
    }


}