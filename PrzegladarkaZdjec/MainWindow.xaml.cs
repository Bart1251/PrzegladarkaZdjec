using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace PrzegladarkaZdjec
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<BitmapImage> images = new List<BitmapImage>();
        List<string> paths = new List<string>();
        int displayedImage = 0;
        int size = 100;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenDirectory(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Wybierz folder";
            dialog.UseDescriptionForTitle = true;
            
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                images.Clear();
                paths.Clear();
                string[] extensions = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };
                foreach(string extension in extensions)
                    paths.AddRange(Directory.GetFiles(dialog.SelectedPath, "*." + extension));

                foreach(string path in paths)
                {
                    images.Add(new BitmapImage(new Uri(path)));
                }
                if(images.Count > 0)
                    DisplayImage(0);
                else
                {
                    displayedImage = 0;
                    Image.Source = null;
                }
            }
        }

        private void ShowPrevious(object sender, EventArgs e)
        {
            if (displayedImage > 0)
                DisplayImage(displayedImage - 1);
        }

        private void ShowNext(object sender, EventArgs e)
        {
            if (displayedImage < images.Count - 1)
                DisplayImage(displayedImage + 1);
        }

        private void WindowResize(object sender, EventArgs e)
        {
            if (images.Count > 0)
                ResizeImage();
        }

        private void ZoomOut(object sender, EventArgs e)
        {
            if (images.Count == 0) return;
            if ((int)Math.Round(size / 20.0) * 20 > 20)
            {
                FitScreenBtn.IsEnabled = true;
                size = (int)Math.Round(size / 20.0) * 20;
                size -= 20;
                Image.Width = images[displayedImage].Width * size / 100;
                Image.Height = images[displayedImage].Height * size / 100;
                Size.Content = size.ToString() + "%";
            }
        }

        private void ZoomIn(object sender, EventArgs e)
        {
            if (images.Count == 0) return;
            if ((int)Math.Round(size / 20.0) * 20 < 200)
            {
                FitScreenBtn.IsEnabled = true;
                size = (int)Math.Round(size / 20.0) * 20;
                size += 20;
                Image.Width = images[displayedImage].Width * size / 100;
                Image.Height = images[displayedImage].Height * size / 100;
                Size.Content = size.ToString() + "%";
            }
        }

        private void OriginalSize(object sender, RoutedEventArgs e)
        {
            if (images.Count == 0) return;
            OriginalSizeBtn.IsEnabled = false;
            FitScreenBtn.IsEnabled = true;
            ResizeImage();
        }

        private void FitScreen(object sender, EventArgs e)
        {
            if (images.Count == 0) return;
            FitScreenBtn.IsEnabled = false;
            OriginalSizeBtn.IsEnabled = true;
            ResizeImage();
        }

        private void OriginalFit()
        {
            Image.Width = images[displayedImage].Width;
            Image.Height = images[displayedImage].Height;
            size = 100;
        }

        private void ResizeFit()
        {
            if (images[displayedImage].Width > ImageGrid.ActualWidth - 60)
            {
                Image.Width = ImageGrid.ActualWidth - 60;
                if (images[displayedImage].Height * (ImageGrid.ActualWidth - 60) / images[displayedImage].Width > ImageGrid.ActualHeight)
                {
                    Image.Height = ImageGrid.ActualHeight;
                    Image.Width = images[displayedImage].Width * ImageGrid.ActualHeight / images[displayedImage].Height;
                    size = (int)(ImageGrid.ActualHeight / images[displayedImage].Height * 100);
                }
                else
                {
                    Image.Height = images[displayedImage].Height * (ImageGrid.ActualWidth - 60) / images[displayedImage].Width;
                    size = (int)((ImageGrid.ActualWidth - 60) / images[displayedImage].Width * 100);
                }
            }
            else if (images[displayedImage].Height > ImageGrid.Height)
            {
                Image.Height = ImageGrid.ActualHeight;
                if (images[displayedImage].Width * ImageGrid.ActualHeight / images[displayedImage].Height > ImageGrid.ActualWidth - 60)
                {
                    Image.Width = ImageGrid.ActualWidth - 60;
                    Image.Height = images[displayedImage].Height * (ImageGrid.ActualWidth - 60) / images[displayedImage].Width;
                    size = (int)((ImageGrid.ActualWidth - 60) / images[displayedImage].Width * 100);
                }
                else
                {
                    Image.Width = images[displayedImage].Width * ImageGrid.ActualHeight / images[displayedImage].Height;
                    size = (int)(ImageGrid.ActualHeight / images[displayedImage].Height * 100);
                }
            }
            else
            {
                if (ImageGrid.ActualHeight < images[displayedImage].Height * (ImageGrid.ActualWidth - 60) / images[displayedImage].Width)
                {
                    Image.Width = ImageGrid.ActualWidth;
                    Image.Height = images[displayedImage].Height * (ImageGrid.ActualWidth - 60) / images[displayedImage].Width;
                    size = (int)((ImageGrid.ActualWidth - 60) / images[displayedImage].Width * 100);
                }
                else
                {
                    Image.Height = ImageGrid.ActualHeight;
                    Image.Width = images[displayedImage].Width * ImageGrid.ActualHeight / images[displayedImage].Height;
                    size = (int)(ImageGrid.ActualHeight / images[displayedImage].Height * 100);
                }
            }
        }

        private void ResizeImage()
        {
            if (!FitScreenBtn.IsEnabled)
                ResizeFit();
            if (!OriginalSizeBtn.IsEnabled)
                OriginalFit();

            Size.Content = size.ToString() + "%";
        }

        private void DisplayImage(int i)
        {
            displayedImage = i;
            Image.Source = images[i];
            Name.Content = paths[i].Split("\\").Last();
            FitScreenBtn.IsEnabled = false;
            OriginalSizeBtn.IsEnabled = true;
            ResizeImage();
        }

    }
}
