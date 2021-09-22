using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Drawing;
using System;

namespace RenamePhoto
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        Thread thr;

        private void Select_File_CSV(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            
            ofd.FileName = "Csv_file";
            ofd.Filter = "CSV Formated (.csv) | *.csv";
            ofd.DefaultExt = ".csv";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PathFileCSV.Text = ofd.FileName;

            }
        }

        private string select_directory()
        {
            var dlg = new  FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var folder = dlg.SelectedPath;
                return folder.ToString();
            }
            else return null;
        }

        private void PathDirectoryPhotoIn(object sender, RoutedEventArgs e)
        {
            photoIn.Text = select_directory();
        }

        private void PathDirectoryPhotoOut(object sender, RoutedEventArgs e)
        {
            photoOut.Text = select_directory();
        }

        private bool CheckTextBox(System.Windows.Controls.TextBox textBox){
            if (textBox.Text == "") return false;
            else return true;
        }

        private bool CheckedIsNull() {
            if (!CheckTextBox(PathFileCSV) || !CheckTextBox(photoIn) || !CheckTextBox(photoOut))
            {
                System.Windows.MessageBox.Show("Заполните все пути", "Error");
                go.IsEnabled = true;
                terminate.IsEnabled = false;
                return false;
            }
            return true;
        }

        private List<DataMap> create_list_DataMap()
        {
            if (!Dispatcher.Invoke(() => CheckedIsNull()))
            {
                thr.Abort();
            }

            string pattern = @"\b\d+\b";
            List<DataMap> dataMaps = new List<DataMap>();
            try
            {
                using (var reading = new StreamReader(@PathFileCSV.Text))
                {

                    while (!reading.EndOfStream)
                    {
                        var line = reading.ReadLine();
                        var values = line.Split(';');

                        Match match_inetelct = Regex.Match(values[0], pattern, RegexOptions.IgnoreCase);
                        Match match_one_c = Regex.Match(values[1], pattern, RegexOptions.IgnoreCase);

                        if (match_inetelct.Success && match_one_c.Success)
                        {
                            dataMaps.Add(new DataMap(values[0], values[1]));
                        }

                    }
                }
            }
            catch (IOException ex)
            {
                DialogResult dr = (DialogResult)System.Windows.MessageBox.Show(ex.Message, "Error!!", MessageBoxButton.OK);
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    return create_list_DataMap();
                }

            }


            progress.Maximum = dataMaps.Count();
            return dataMaps;
        }

        private bool check_path_file(DataMap dataMap)
        {
            if (File.Exists(System.IO.Path.Combine(@photoIn.Text, dataMap.tabn_one_c + ".jpg")))
                return true;
            else return false;
        }

        private string create_path_file(string val, System.Windows.Controls.TextBox textBox)
        {
            return System.IO.Path.Combine(@textBox.Text, val + ".jpg");
        }
        private string create_path_file_bmp(string val, System.Windows.Controls.TextBox textBox)
        {
            return System.IO.Path.Combine(@textBox.Text, val + ".bmp");
        }
        private void rename_files()
        {
            if (!Dispatcher.Invoke(() => CheckedIsNull()))
            {
                thr.Abort();
            }

            var list = Dispatcher.Invoke(() => create_list_DataMap());
            foreach (DataMap dataMap in list)
            {
                if (Dispatcher.Invoke(() => check_path_file(dataMap)))
                {
                    if (!File.Exists(Dispatcher.Invoke(() => create_path_file(dataMap.id_intelect, photoOut))))
                    {
                        using (Image fs = Image.FromFile(Dispatcher.Invoke(() => create_path_file(dataMap.tabn_one_c, photoIn))))
                        {
                            Console.WriteLine("Width: {0}, Height: {1}", fs.Width, fs.Height);
                            
                            if (fs.Width > fs.Height)
                                fs.RotateFlip(RotateFlipType.Rotate90FlipNone);

                            fs.Save(Dispatcher.Invoke(() => create_path_file_bmp(dataMap.id_intelect, photoOut)), System.Drawing.Imaging.ImageFormat.Bmp);
                        }
                        Dispatcher.Invoke(() => progress.Value += 1);
                    }
                }

            }

            go.IsEnabled = true;
        }
        
        private void Goes(object sender, RoutedEventArgs e)
        {
            go.IsEnabled = false;
            terminate.IsEnabled = true;
            thr = new Thread(new ThreadStart(rename_files));
            thr.Start();
        }

        private void TerminateThread(object sender, RoutedEventArgs e)
        {
            thr.Abort();
            go.IsEnabled = true;
            terminate.IsEnabled = false;
            progress.Value = 0;
        }
    }
}