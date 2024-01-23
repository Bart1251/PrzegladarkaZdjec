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
        List<string> paths = new List<string>();
        BitmapImage displayedImage = new BitmapImage();
        int displayedImageIndex = 0;
        int size = 100;
        Rotation rotation = 0;

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
                paths.Clear();
                rotation = 0;
                string[] extensions = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };
                foreach(string extension in extensions)
                    paths.AddRange(Directory.GetFiles(dialog.SelectedPath, "*." + extension));

                if(paths.Count > 0)
                    DisplayImage(0);
                else
                {
                    displayedImageIndex = 0;
                    Image.Source = null;
                }
            }
        }

        private void ShowPrevious(object sender, EventArgs e)
        {
            if (displayedImageIndex > 0)
            {
                rotation = 0;
                DisplayImage(displayedImageIndex - 1);
            }
        }

        private void Rotate(object sender, EventArgs e)
        {
            if (paths.Count == 0) return;
            rotation = (Rotation)((int)(rotation + 1) % 4);
            DisplayImage(displayedImageIndex);
        }

        private void ShowNext(object sender, EventArgs e)
        {
            if (displayedImageIndex < paths.Count - 1)
            {
                rotation = 0;
                DisplayImage(displayedImageIndex + 1);
            }
        }

        private void WindowResize(object sender, EventArgs e)
        {
            if (paths.Count > 0)
                ResizeImage();
        }

        private void ZoomOut(object sender, EventArgs e)
        {
            if (paths.Count == 0) return;
            if ((int)Math.Round(size / 20.0) * 20 > 20)
            {
                FitScreenBtn.IsEnabled = true;
                OriginalSizeBtn.IsEnabled = true;
                size = (int)Math.Round(size / 20.0) * 20;
                size -= 20;
                Image.Width = displayedImage.Width * size / 100;
                Image.Height = displayedImage.Height * size / 100;
                Size.Content = size.ToString() + "%";
            }
        }

        private void ZoomIn(object sender, EventArgs e)
        {
            if (paths.Count == 0) return;
            if ((int)Math.Round(size / 20.0) * 20 < 200)
            {
                FitScreenBtn.IsEnabled = true;
                OriginalSizeBtn.IsEnabled = true;
                size = (int)Math.Round(size / 20.0) * 20;
                size += 20;
                Image.Width = displayedImage.Width * size / 100;
                Image.Height = displayedImage.Height * size / 100;
                Size.Content = size.ToString() + "%";
            }
        }

        private void OriginalSize(object sender, RoutedEventArgs e)
        {
            if (paths.Count == 0) return;
            OriginalSizeBtn.IsEnabled = false;
            FitScreenBtn.IsEnabled = true;
            ResizeImage();
        }

        private void FitScreen(object sender, EventArgs e)
        {
            if (paths.Count == 0) return;
            FitScreenBtn.IsEnabled = false;
            OriginalSizeBtn.IsEnabled = true;
            ResizeImage();
        }

        private void OriginalFit()
        {
            Image.Width = displayedImage.Width;
            Image.Height = displayedImage.Height;
            size = 100;
        }

        private void ResizeFit()
        {
            if (displayedImage.Width > ImageGrid.ActualWidth - 60)
            {
                Image.Width = ImageGrid.ActualWidth - 60;
                if (displayedImage.Height * (ImageGrid.ActualWidth - 60) / displayedImage.Width > ImageGrid.ActualHeight)
                {
                    Image.Height = ImageGrid.ActualHeight;
                    Image.Width = displayedImage.Width * ImageGrid.ActualHeight / displayedImage.Height;
                    size = (int)(ImageGrid.ActualHeight / displayedImage.Height * 100);
                }
                else
                {
                    Image.Height = displayedImage.Height * (ImageGrid.ActualWidth - 60) / displayedImage.Width;
                    size = (int)((ImageGrid.ActualWidth - 60) / displayedImage.Width * 100);
                }
            }
            else if (displayedImage.Height > ImageGrid.Height)
            {
                Image.Height = ImageGrid.ActualHeight;
                if (displayedImage.Width * ImageGrid.ActualHeight / displayedImage.Height > ImageGrid.ActualWidth - 60)
                {
                    Image.Width = ImageGrid.ActualWidth - 60;
                    Image.Height = displayedImage.Height * (ImageGrid.ActualWidth - 60) / displayedImage.Width;
                    size = (int)((ImageGrid.ActualWidth - 60) / displayedImage.Width * 100);
                }
                else
                {
                    Image.Width = displayedImage.Width * ImageGrid.ActualHeight / displayedImage.Height;
                    size = (int)(ImageGrid.ActualHeight / displayedImage.Height * 100);
                }
            }
            else
            {
                if (ImageGrid.ActualHeight > displayedImage.Height * (ImageGrid.ActualWidth - 60) / displayedImage.Width)
                {
                    Image.Width = ImageGrid.ActualWidth;
                    Image.Height = displayedImage.Height * (ImageGrid.ActualWidth - 60) / displayedImage.Width;
                    size = (int)((ImageGrid.ActualWidth - 60) / displayedImage.Width * 100);
                }
                else
                {
                    Image.Height = ImageGrid.ActualHeight;
                    Image.Width = displayedImage.Width * ImageGrid.ActualHeight / displayedImage.Height;
                    size = (int)(ImageGrid.ActualHeight / displayedImage.Height * 100);
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
            displayedImageIndex = i;
            displayedImage = new BitmapImage();
            displayedImage.BeginInit();
            displayedImage.UriSource = new Uri(paths[i]);
            displayedImage.Rotation = rotation;
            displayedImage.EndInit();
            Image.Source = displayedImage;
            Name.Content = paths[i].Split("\\").Last();
            FitScreenBtn.IsEnabled = false;
            OriginalSizeBtn.IsEnabled = true;
            ResizeImage();
        }

    }
}
