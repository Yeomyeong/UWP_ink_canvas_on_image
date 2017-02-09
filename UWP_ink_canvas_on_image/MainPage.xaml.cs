using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using System.Numerics;
using Windows.UI.Input.Inking;

namespace UWP_ink_canvas_on_image
{
    public sealed partial class MainPage : Page
    {
        private float firstImageWidth;
        private float firstImageHeight;
        private int imageWidth;
        private int imageHeight;

        public MainPage()
        {
            this.InitializeComponent();
            ink.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Touch;
            firstImageWidth = (float)image.ActualWidth;
            firstImageHeight = (float)image.ActualHeight;
            imageWidth = (int)image.ActualWidth;
            imageHeight = (int)image.ActualHeight;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // grab output file
            StorageFolder storageFolder = KnownFolders.SavedPictures;
            var file = await storageFolder.CreateFileAsync("output.jpg", CreationCollisionOption.ReplaceExisting);

            CanvasDevice device = new CanvasDevice();
            device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, imageWidth, imageHeight, 96);
            

            // grab your input file from Assets folder
            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
            var inputFile = await assets.GetFileAsync("sample.jpg");
            using (var ds = renderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.White);
                var image = await CanvasBitmap.LoadAsync(device, inputFile.Path);
                
                // draw your image first
                ds.DrawImage(image, new Rect(new Point(0,0), new Point(imageWidth, imageHeight))); //Rect
                // then draw contents of your ink canvas over it
                ds.DrawInk(ink.InkPresenter.StrokeContainer.GetStrokes());
            }

            // save results
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await renderTarget.SaveAsync(fileStream, CanvasBitmapFileFormat.Jpeg, 1f);
            }
        }

        private void App_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            setFirstImageSize();
            imageWidth = (int)image.ActualWidth;
            imageHeight = (int)image.ActualHeight;

            if (firstImageWidth > 0 && firstImageHeight > 0)
            {
                
                var widthScale = imageWidth / firstImageWidth;
                var heightScale = imageHeight / firstImageHeight;
                
                Debug.WriteLine("image size : " + imageWidth + " x " + imageHeight);
                Debug.WriteLine("resize scale : " + widthScale + " x " + heightScale);
                if (widthScale != 1)
                {
                    foreach (var stroke in ink.InkPresenter.StrokeContainer.GetStrokes())
                    {
                        stroke.PointTransform = Matrix3x2.CreateScale(widthScale, heightScale);
                        Debug.WriteLine(stroke.PointTransform.ToString());
                    }
                }
            }
        }

        private void setFirstImageSize ()
        {
            if (firstImageWidth == 0)
                firstImageWidth = (int)image.ActualWidth;
            if (firstImageHeight == 0)
                firstImageHeight = (int)image.ActualHeight;

        }
    }
}
