using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Threading;
using System.Diagnostics;

namespace MaterialsManager
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

        public void GoToBz2(string path)
        {
            FileInfo fileToBeZipped = new FileInfo(path);
            FileInfo zipFileName = new FileInfo(string.Concat(fileToBeZipped.FullName, ".bz2"));
            using (FileStream fileToBeZippedAsStream = fileToBeZipped.OpenRead())
            {
                using (FileStream zipTargetAsStream = zipFileName.Create())
                {
                    BZip2.Compress(fileToBeZippedAsStream, zipTargetAsStream, true, 4096);
                }
            }
        }
        public void ExtractBz2(string path)
        {
            using (FileStream fileToDecompressAsStream = new FileInfo(path).OpenRead())
            {
                string[] f = path.Split('.');
                string decompressedFileName = null;
                for (int i = 0; i < f.Length; i++)
                {
                    if ((f.Length - 2) == i)
                    {
                        decompressedFileName += $".";
                    }

                    if ((f.Length - 1) != i)
                    {
                        decompressedFileName += f[i];
                    }
                }

                using (FileStream decompressedStream = File.Create(decompressedFileName))
                {
                    BZip2.Decompress(fileToDecompressAsStream, decompressedStream, true);
                }
            }
        }
        private void ButtonToBz2Run_Click(object sender, RoutedEventArgs e)
        {
            string path = TextBoxToBz2DirInput.Text;
            bool ?removeoriginalfile = CheckBoxToBz2RemoveOriginalFile.IsChecked;
            if (path.Length == 0)
            {
                MessageBox.Show("Path is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(!Directory.Exists(path))
            {
                MessageBox.Show("Path not exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new Thread(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    TextBoxToBz2DirInput.IsEnabled = ButtonToBz2Run.IsEnabled = false;
                    foreach (string item in Directory.GetFiles(path))
                    {
                        GoToBz2(item);
                        if ((bool)removeoriginalfile)
                        {
                            File.Delete(item);
                        };
                    }

                    Mouse.OverrideCursor = null;
                    TextBoxToBz2DirInput.IsEnabled = ButtonToBz2Run.IsEnabled = true;
                });
            }).Start();
        }
        private void ButtonUnBz2Run_Click(object sender, RoutedEventArgs e)
        {
            string path = TextBoxUnBz2DirInput.Text;
            bool? removeoriginalfile = CheckBoxUnBz2RemoveOriginalFile.IsChecked;
            if (path.Length == 0)
            {
                MessageBox.Show("Path is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(path))
            {
                MessageBox.Show("Path not exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new Thread(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    TextBoxUnBz2DirInput.IsEnabled = ButtonUnBz2Run.IsEnabled = false;
                    foreach (string item in Directory.GetFiles(path))
                    {
                        try
                        {
                            ExtractBz2(item);
                        }
                        catch (NotSupportedException)
                        {
                            MessageBox.Show("Bz2 files not found!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if ((bool)removeoriginalfile)
                        {
                            File.Delete(item);
                        }
                    }

                    Mouse.OverrideCursor = null;
                    TextBoxUnBz2DirInput.IsEnabled = ButtonUnBz2Run.IsEnabled = true;
                });
            }).Start();
        }
        private void ButtonToBz2OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.SelectedPath = Directory.GetCurrentDirectory();
            bool? success = dialog.ShowDialog();
            if (success == true)
            {
                TextBoxToBz2DirInput.Text = dialog.SelectedPath;
            }
        }
        private void ButtonUnBz2OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.SelectedPath = Directory.GetCurrentDirectory();
            bool? success = dialog.ShowDialog();
            if (success == true)
            {
                TextBoxUnBz2DirInput.Text = dialog.SelectedPath;
            }
        }

        private void HyperLinkAboutAuthorVk_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://vk.com/winsomequill";
            ProcessStartInfo sInfo = new ProcessStartInfo(url)
            {
                UseShellExecute = true,
            };
            Process.Start(sInfo);
        }

        private void HyperLinkAboutGitHub_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/WinsomeQuill/MaterialsManager";
            ProcessStartInfo sInfo = new ProcessStartInfo(url)
            {
                UseShellExecute = true,
            };
            Process.Start(sInfo);
        }
    }
}
