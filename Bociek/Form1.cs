namespace Bociek
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Globalization;
    using ExtensionMethods;
    public partial class Form1 : Form
    {
        String Logs;
        String Log;
        static String DateFormat = "dd.MM.yyyy HH:mm:ss";
        String Today;

        public Form1()
        {
            InitializeComponent();

            LogsInit();
            Log = null;

            Today = DateTime.Today.ToString();
            textBoxStart.Text = Today;
            textBoxStop.Text = DateFormat;

            textBoxHowLong.Text = "0:0:0";
        }

        private void LogsInit()
        {
            Logs = "Kto? | Co robi? | Od kiedy? | Do kiedy? | Jak długo? | Dodatkowe informacje";
            listBoxLogs.Items.Add(Logs);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxStop.Text == DateFormat)
                textBoxStop.Text = null;

            if (AllIsOk())
            {
                // Log = "Kto? | Co robi? | Od kiedy? | Do kiedy? | Jak długo? | Dodatkowe informacje"
                Log = comboBoxWho.SelectedItem.ToString() + " | " 
                    + comboBoxWhatDo.SelectedItem.ToString() + " | "
                    + textBoxStart.Text + " | ";

                if (textBoxStop.Text.Length > 10)//tekst krótszy od 10 oznacza ze nie podano prawidlowego
                {
                        Log += textBoxStop.Text + " | " + HowLong() + " | ";
                        textBoxStart.Text = textBoxStop.Text;
                }
                else
                    if (textBoxHowLong.Text.Length < 5)
                    {
                        Log = Log + " | " + " | ";
                    }
                    else
                    {
                        String temp = DateOfEnd();
                        Log = Log + temp + " | " + textBoxHowLong.Text + " | ";
                        textBoxStart.Text = temp;
                    }

                Log = Log + textBoxExtraInfo.Text;
                listBoxLogs.Items.Add(Log);

                textBoxStop.Text = null;

                SaveLog(Log);

                textBoxHowLong.Text = type[type.Length - 1];

                listBoxLogs.TopIndex = listBoxLogs.Items.Count - 1;

            }
            else
            {
                MessageBox.Show("Podaj wymagane informacje");
            }
        }

        String[] type;
        private String DateOfEnd()
        {
            var mydateStart = textBoxStart.Text.ToString();
            var dateStart = mydateStart.ToDateTime(DateFormat);

            TimeSpan howLong;

           type = textBoxHowLong.Text.Split(' ');
            if (type.Length == 1)
            {
                String[] howLongTab = textBoxHowLong.Text.Split(':');
                howLong = new TimeSpan(
                    Int32.Parse(howLongTab[0]), //godziny
                    Int32.Parse(howLongTab[1]), //minuty
                    Int32.Parse(howLongTab[2]) //sekundy
                    );
            }
            else
            {
                String[] Start = type[0].Split(':');
                TimeSpan howLongStart = new TimeSpan(
                    Int32.Parse(Start[0]), //godziny
                    Int32.Parse(Start[1]), //minuty
                    Int32.Parse(Start[2]) //sekundy
                    );

                
                String[] Stop = type[1].Split(':');

                TimeSpan howLongStop = new TimeSpan(
                    Int32.Parse(Stop[0]), //godziny
                    Int32.Parse(Stop[1]), //minuty
                    Int32.Parse(Stop[2]) //sekundy
                    );

                howLong = (howLongStop - howLongStart);
                textBoxHowLong.Text = howLong.Hours.ToString() + ":" + howLong.Minutes.ToString() + ":" + howLong.Seconds.ToString();
            }

            var dateEnd = mydateStart.ToDateTime(DateFormat) + howLong;
            return dateEnd.ToString();
        }

        private bool AllIsOk()
        {
            if (comboBoxWhatDo.SelectedItem == null)
                return false;

            if (comboBoxWho.SelectedItem == null)
                return false;

            if (textBoxStart.Text.Length < 10)
                return false;

            if (textBoxStop.Text == Today)
                return false;

            return true;
        }

        private String HowLong()
        {
            var mydateStart = textBoxStart.Text.ToString();
            var dateStart = mydateStart.ToDateTime(DateFormat);

            var mydateStop = textBoxStop.Text.ToString();
            var dateStop = mydateStop.ToDateTime(DateFormat);

            TimeSpan howLong = dateStop - dateStart;

            return howLong.Hours.ToString() + ":" + howLong.Minutes.ToString() + ":" + howLong.Seconds.ToString();
        }

        private void SaveLog(String Log)
        {
            StreamWriter SW;
            SW = File.AppendText("Informacje o bocianach - program.txt");
            SW.WriteLine(Log);
            SW.Close();
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = theDialog.FileName;

                string[] filelines = File.ReadAllLines(filename);

                listBoxLogs.Items.Clear();
                LogsInit();

                foreach (String elem in filelines)
                {
                    Log = Environment.NewLine + elem;
                    listBoxLogs.Items.Add(Log);
                }

                listBoxLogs.TopIndex = listBoxLogs.Items.Count - 1;
            }
        }

        private void buttonDate_Click(object sender, EventArgs e)
        {
            textBoxStop.Text = textBoxStart.Text;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxStop.Text = null;
        }

        private void buttonClear2_Click(object sender, EventArgs e)
        {
            textBoxHowLong.Text = null;
        }

        private void listBoxLogs_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sb1 = new StringBuilder();
                foreach (object row in listBoxLogs.SelectedItems)
                {
                    String[] rowTab = row.ToString().Split('|');

                    for (int i = 1; i < rowTab[3].ToString().Length-1; i++)
                    {
                        sb1.Append(rowTab[3].ToString()[i]);
                    }

                    sb.Append(sb1);
                    sb.AppendLine();
                }
                sb.Remove(sb.Length - 1, 1); // Just to avoid copying last empty row
                Clipboard.SetData(System.Windows.Forms.DataFormats.Text, sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

namespace ExtensionMethods
{
    using System;
    using System.Globalization;

    public static class DateTimeExtensions
    {
        public static DateTime ToDateTime(this string s,
                  string format = "ddMMyyyy", string cultureString = "en-US")
        {
            try
            {
                var r = DateTime.ParseExact(
                    s: s,
                    format: format,
                    provider: CultureInfo.GetCultureInfo(cultureString));
                return r;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (CultureNotFoundException)
            {
                throw; // Given Culture is not supported culture
            }
        }
    }
}