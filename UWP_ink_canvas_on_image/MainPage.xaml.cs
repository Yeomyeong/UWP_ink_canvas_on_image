using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿은 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 에 문서화되어 있습니다.

namespace UWP_ink_canvas_on_image
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ink.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Touch;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // grab output file

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            var file = await storageFolder.CreateFileAsync("output.jpg", CreationCollisionOption.ReplaceExisting);

            CanvasDevice device = new CanvasDevice();
            device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, (int)ink.ActualWidth, (int)ink.ActualHeight, 96);

            // grab your input file from Assets folder
            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
            var inputFile = await assets.GetFileAsync("sample.jpg");
            using (var ds = renderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.White);
                var image = await CanvasBitmap.LoadAsync(device, inputFile.Path);
                // draw your image first
                ds.DrawImage(image);
                // then draw contents of your ink canvas over it
                ds.DrawInk(ink.InkPresenter.StrokeContainer.GetStrokes());
            }

            // save results
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await renderTarget.SaveAsync(fileStream, CanvasBitmapFileFormat.Jpeg, 1f);
            }
        }
    }
}
