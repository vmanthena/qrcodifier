﻿using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Reflection;

using DYMO.Label.Framework;

using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Controls;

namespace DynamicDevices.QRCodifier
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        private BitmapImage _currentQRImage;
        private ILabel _label = Label.Open(Application.GetResourceStream(new Uri("BarcodeAsImage.label", UriKind.Relative)).Stream);

        public Window1()
        {
            InitializeComponent();

            Title += " v" + Assembly.GetEntryAssembly().GetName().Version;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PrintersComboBox.ItemsSource = Framework.GetLabelWriterPrinters();
            PrintersComboBox.SelectedIndex = 0;

            MakeLabel(this, null);
        }

        private void PrintBarcodeAsImageButton_Click(object sender, RoutedEventArgs e)
        {

            // print
            try
            {
                _label.Print(PrintersComboBox.Text);

                MessageBox.Show(this, "Printed!");
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(this, "Error printing: " + ex.Message);
            }

        }

        private void MakeLabel(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Prevent exceptions
            if (!this.IsLoaded)
                return;

            // - calculate QR image
            using (var ms = new MemoryStream())
            {
                var encoder = new QrEncoder();
                var qrCode = encoder.Encode(QRCodeURI.Text);
                var renderer = new Renderer(5);
                renderer.WriteToStream(qrCode.Matrix, ms, ImageFormat.Png);

                ms.Seek(0, SeekOrigin.Begin);

                // - set data as a barcode image
                _label.SetImagePngData("Image", ms);
            }

            // - set text
            _label.SetObjectText("TextLine1", TextLine1.Text);
            _label.SetObjectText("TextLine2", TextLine2.Text);
            _label.SetObjectText("URL", QRCodeURI.Text);

            // - render label
            var data = _label.RenderAsPng(null, null);

            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(data);
            bi.EndInit();

            // - set it into the UI image 
            QRCodeImage.Source = bi;
        }
    }
}
