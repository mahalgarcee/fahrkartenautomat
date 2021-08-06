using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FahrkartenautomatUi
{

    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
            (
                int nLeft,
                int nTop,
                int nRight,
                int nBottom,
                int nWidthEllipse,
                int nHeightEllipse
            );
        public static List<string> kommentare;
        public static List<string> buchungenZeile;
        private static Dictionary<int, Buchung> buchungen;
        public static string bestandMuenzen;
        public static void dateiEinlesen(string Weg)
        {

            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            try
            {
                string[] lines = File.ReadAllLines(Weg);

                if (lines.Length < 5)
                {
                    //form1.richTextBox2.AppendText("Zu wenig Zeilen",Color.Red);
                    MessageBox.Show("Zu wenig Zeilen", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i > 3)
                        {
                            buchungenZeile.Add(lines[i]);
                        }
                        if (i == 3)
                        {
                            bestandMuenzen = lines[i];
                        }
                        if (i < 3)
                        {
                            kommentare.Add(lines[i]);
                        }
                    }
                }

            }
            catch (IOException e)
            {
                //form1.richTextBox2.AppendText("Datei konnte nicht geöffnet sein", Color.Red);
                MessageBox.Show("Datei konnte nicht geöffnet sein", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        public static void buchungErstellen(List<string> buchungenZeile)
        {
            int i = 0;
            foreach (string buchungZeil in buchungenZeile)
            {
                Buchung buchung = new Buchung(buchungZeil);
                buchungen.Add(i, buchung);
                i++;
            }
        }

        internal static Dictionary<int, Buchung> Buchungen { get => buchungen; set => buchungen = value; }

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button3.Hide();
            richTextBox2.Hide();
            button1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button1.Width, button1.Height, 40, 40));
            button2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button2.Width, button2.Height, 40, 40));
            richTextBox1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, richTextBox1.Width, richTextBox1.Height, 30, 30));
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string dateiName = openFileDialog1.FileName;
            richTextBox1.Text = dateiName;
            richTextBox1.SelectAll();
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String dateiName = richTextBox1.Text;
            if (!String.IsNullOrEmpty(dateiName))
            {
                button1.Hide();
                button2.Hide();
                richTextBox1.Hide();
                button3.Show();
                richTextBox2.Show();
                richTextBox2.Clear();
                kommentare = new List<string>();
                buchungenZeile = new List<string>();
                buchungen = new Dictionary<int, Buchung>();
                // Eingabedatei lesen
                dateiEinlesen(dateiName);
                if (File.ReadAllLines(dateiName).Length >= 5)
                {
                    //Ausgabe schreiben in der Oberfläche
                    richTextBox2.AppendText(string.Format("{0}", string.Join("\n", kommentare)));
                    richTextBox2.AppendText("\n");
                    buchungErstellen(buchungenZeile);
                    Automat automat = new Automat(bestandMuenzen, buchungen);
                    automat.buchungenTesten();

                    //Ausgabe schreiben in der Datei
                    string path = @"Ausgabe.txt";
                    String[] lines = richTextBox2.Text.Split('\n');
                    if (!File.Exists(path))
                    {
                        var myFile= File.Create(path);
                        myFile.Close();
                        TextWriter tw = new StreamWriter(path);
                        for (int i = 0; i < lines.Length; i++)
                        {
                            tw.WriteLine(lines[i]);
                        }
                        tw.Close();

                    }
                    else
                    {
                        System.IO.File.WriteAllText(@"Ausgabe.txt", string.Empty);
                        TextWriter tw = new StreamWriter(path);
                        for (int i = 0; i < lines.Length; i++)
                        {
                            tw.WriteLine(lines[i]);
                        }
                        tw.Close();
                    }


                }
            }
            else
            {
                MessageBox.Show("Datei nicht vorhanden!!", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            button1.Show();
            button2.Show();
            richTextBox1.Show();
            button3.Hide();
            richTextBox2.Hide();

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }

}
