using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Data;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;

namespace Wpf
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        BitmapImage img;
        List<Image> pictureBox1 = new List<Image>();
        List<TextBox> textList = new List<TextBox>();
        Image selDPB = null;
        TextBox selTB = null;
        System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
        BitmapImage myBitmapImage = new BitmapImage();

        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private TextBox CreateTB()
        {
            TextBox tb = new TextBox();
            tb.ContextMenu = null;
            tb.AcceptsReturn = true;
            tb.Height = Double.NaN;
            tb.Width = Double.NaN;
            tb.MinWidth = 50;
            tb.MouseLeftButtonDown += new MouseButtonEventHandler(TBFocusEvent);
            tb.GotFocus += new RoutedEventHandler(TBFocusEvent);
            tb.LostFocus += new RoutedEventHandler(TBLostFocus);
            tb.Background = Brushes.Transparent;
            tb.SetValue(DraggableExtender.CanDragProperty, true);
            return tb;
        }

        public string ImageToString(ImageSource source)
        {
            byte[] data;
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)source));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return Convert.ToBase64String(data);
        }

        public ImageSource StringToImage(string imageString)
        {
            byte[] binaryData = Convert.FromBase64String(imageString);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();
            return bi;
        }

        private void TBFocusEvent(object sender, EventArgs e)
        {
            selTB = (TextBox)sender;
            selTB.Focus();
            selTB.BorderThickness = new Thickness(1);
            var control = sender as UIElement;
            if (control != null && control.GetType() == typeof(TextBox))
            {
                canvas1.Children.Remove(control);
                canvas1.Children.Add(control);
            }
        }

        private void PBFocusEvent(object sender, EventArgs e)
        {
            selDPB = (Image)sender;
            selDPB.Focus();
            var control = sender as UIElement;
            if (control != null && control.GetType() == typeof(TextBox))
            {
                canvas1.Children.Remove(control);
                canvas1.Children.Add(control);
            }
        }

        private void TBLostFocus(object sender, EventArgs e)
        {
            selTB = (TextBox)sender;
            selTB.BorderThickness = new Thickness(0);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                if (filename.Remove(0, filename.Length - 2) == "ac")
                {
                    ObjSer os = new ObjSer();
                    pictureBox1.Clear();
                    Stream TestFileStream = File.OpenRead(filename);
                    BinaryFormatter deserializer = new BinaryFormatter();
                    os = (ObjSer)deserializer.Deserialize(TestFileStream);
                    TestFileStream.Close();
                    for (int i = 0; i < os.GetTextEnumerator(); i++)
                    {
                        textList.Add(new TextBox());
                        textList.Last().AcceptsReturn = true;
                        textList.Last().Height = Double.NaN;
                        textList.Last().Width = Double.NaN;
                        textList.Last().MinWidth = 50;
                        textList.Last().MouseLeftButtonDown += new MouseButtonEventHandler(TBFocusEvent);
                        textList.Last().LostFocus += new RoutedEventHandler(TBLostFocus);
                        textList.Last().Background = Brushes.Transparent;
                        textList.Last().SetValue(DraggableExtender.CanDragProperty, true);
                        textList.Last().Text = os.GetFromTextList(i).Text;
                        textList.Last().FontFamily = new FontFamily(os.GetFromTextList(i).FontName);
                        textList.Last().FontSize = os.GetFromTextList(i).FontSize;
                        BrushConverter bc = new BrushConverter();
                        textList.Last().Foreground = (Brush)bc.ConvertFrom(os.GetFromTextList(i).FontColor);
                        canvas1.Children.Add(textList.Last());
                        Canvas.SetLeft(textList.Last(), os.GetFromTextList(i).X);
                        Canvas.SetTop(textList.Last(), os.GetFromTextList(i).Y);
                    }
                    for (int i = 0; i < os.GetPicEnumerator(); i++)
                    {
                        pictureBox1.Add(new Image());
                        pictureBox1.Last().MouseLeftButtonDown += new MouseButtonEventHandler(PBFocusEvent);
                        pictureBox1.Last().Source = StringToImage(os.GetFromPicList(i).PicArray);
                        pictureBox1.Last().SetValue(DraggableExtender.CanDragProperty, true);
                        canvas1.Children.Add(pictureBox1.Last());
                        Canvas.SetLeft(pictureBox1.Last(), os.GetFromPicList(i).X);
                        Canvas.SetTop(pictureBox1.Last(), os.GetFromPicList(i).Y);
                    }
                }
                else
                {
                    pictureBox1.Add(new Image());
                    var uri = new Uri(filename);
                    img = new BitmapImage(uri);
                    pictureBox1.Last().MouseLeftButtonDown += new MouseButtonEventHandler(PBFocusEvent);
                    pictureBox1.Last().Width = img.Width;
                    pictureBox1.Last().Height = img.Height;
                    pictureBox1.Last().Source = img;
                    pictureBox1.Last().SetValue(DraggableExtender.CanDragProperty,true);
                    while (pictureBox1.Last().Width > canvas1.Width || pictureBox1.Last().Height > canvas1.Height)
                    {
                        pictureBox1.Last().Width -= 10;
                        pictureBox1.Last().Height -= 10;
                    }
                    canvas1.Children.Add(pictureBox1.Last());
                }
            }
                
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            textList.Add(CreateTB());
            canvas1.Children.Add(textList.Last());
        }

        private void CreateSaveBitmap(Canvas canvas, string filename)
        {
            Transform transform = canvas.LayoutTransform;
            canvas.LayoutTransform = null;

            // fix margin offset as well
            Thickness margin = canvas.Margin;
            Thickness marginOld = canvas.Margin;
            canvas.Margin = new Thickness(0, 0, 0, 0);
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             (int)canvas.Width, (int)canvas.Height,
             96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            renderBitmap.Render(canvas);

            string strFilExtn = filename.Remove(0, filename.Length - 3);
                // Save file
            switch (strFilExtn)
            {
                case "bmp":
                    {
                        BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                        using (FileStream file = File.Create(filename))
                        {
                            encoder.Save(file);
                        }
                        break;
                    }
                case "jpg":
                    {
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                        using (FileStream file = File.Create(filename))
                        {
                            encoder.Save(file);
                        }
                        break;
                    }
                case "gif":
                    {
                        GifBitmapEncoder encoder = new GifBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                        using (FileStream file = File.Create(filename))
                        {
                            encoder.Save(file);
                        }
                        break;
                    }
                case "tif":
                    {
                        TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                        using (FileStream file = File.Create(filename))
                        {
                            encoder.Save(file);
                        }
                        break;
                    }
                case "png":
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                        using (FileStream file = File.Create(filename))
                        {
                            encoder.Save(file);
                        }
                        break;
                    }
            }
            canvas.Margin = marginOld;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            canvas1.InvalidateVisual();
            Microsoft.Win32.SaveFileDialog savedialog = new Microsoft.Win32.SaveFileDialog();
            savedialog.Title = "Сохранить картинку как ...";
            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.Filter =
                "AC File(*.ac)|*.ac|" +
                "Bitmap File(*.bmp)|*.bmp|" +
                "GIF File(*.gif)|*.gif|" +
                "JPEG File(*.jpg)|*.jpg|" +
                "TIF File(*.tif)|*.tif|" +
                "PNG File(*.png)|*.png";
            if (savedialog.ShowDialog() == true)
            {
                string fileName = savedialog.FileName;
                string strFilExtn = fileName.Remove(0, fileName.Length - 3);
                // Save file
                switch (strFilExtn)
                {
                    case "bmp":
                        CreateSaveBitmap(canvas1, fileName);
                        break;
                    case "jpg":
                        CreateSaveBitmap(canvas1, fileName);
                        break;
                    case "gif":
                        CreateSaveBitmap(canvas1, fileName);
                        break;
                    case "tif":
                        CreateSaveBitmap(canvas1, fileName);
                        break;
                    case "png":
                        CreateSaveBitmap(canvas1, fileName);
                        break;
                    case ".ac":
                        ObjSer os = new ObjSer();
                        foreach (Image pic in pictureBox1)
                        {
                            Point relativePoint = pic.TransformToAncestor(this).Transform(new Point(0, 0));
                            SerPic sp = new SerPic(relativePoint.X, relativePoint.Y, pic.Width, pic.Height, ImageToString(pic.Source));
                            os.AddToPicList(sp);
                        }
                        foreach (TextBox text in textList)
                        {
                            Point relativePoint = text.TransformToAncestor(this).Transform(new Point(0, 0));
                            SerText st = new SerText(relativePoint.X, relativePoint.Y, text.FontFamily.Source, text.FontSize, text.Foreground.ToString(), text.Text);
                            os.AddToTextList(st);
                        }
                        Stream TestFileStream = File.Create(fileName);
                        BinaryFormatter serializer = new BinaryFormatter();
                        serializer.Serialize(TestFileStream, os);
                        TestFileStream.Close();
                        break;
                }
            }
        }

        private Brush ChooseColor()
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.FullOpen = true;
            colorDialog.ShowDialog();
            System.Drawing.Color color1 = colorDialog.Color;
            Color color = Colors.Red;
            color.A = color1.A;
            color.R = color1.R;
            color.G = color1.G;
            color.B = color1.B;
            Brush brush = new SolidColorBrush(color);
            return brush;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            canvas1.Background = ChooseColor();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (selDPB != null)
            {
                pictureBox1.Remove(selDPB);
                canvas1.Children.Remove(selDPB);
            }
            else
                MessageBox.Show("Выбирите Картинку на форме!!!");
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = @"help.chm";
            proc.Start();
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            DoPreview("n");
            //Transform transform = canvas1.LayoutTransform;
            //canvas1.LayoutTransform = null;

            //// fix margin offset as well
            //Thickness margin = canvas1.Margin;
            //Thickness marginOld = canvas1.Margin;
            //canvas1.Margin = new Thickness(0, 0, 0, 0);
            //RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
            // (int)canvas1.Width, (int)canvas1.Height,
            // 96d, 96d, PixelFormats.Pbgra32);
            //// needed otherwise the image output is black
            //canvas1.Measure(new Size((int)canvas1.Width, (int)canvas1.Height));
            //canvas1.Arrange(new Rect(new Size((int)canvas1.Width, (int)canvas1.Height)));

            //renderBitmap.Render(canvas1);
            //var vis = new DrawingVisual();
            //var dc = vis.RenderOpen();
            //dc.DrawImage(renderBitmap, new Rect { Width = renderBitmap.Width, Height = renderBitmap.Height });
            //dc.Close();
            //canvas1.Margin = marginOld;
            //PrintDialog printDialog1 = new PrintDialog();
            //if (printDialog1.ShowDialog() == true)
            //{
            //    printDialog1.PrintVisual(vis, "Image");
            //}
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            if (selTB != null)
            {
                selTB.Foreground = ChooseColor();
                using (System.Windows.Forms.FontDialog fd = new System.Windows.Forms.FontDialog())
                {
                    try
                    {
                        fd.AllowScriptChange = false;
                        fd.AllowSimulations = false;
                        if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            FontFamilyConverter ffc = new FontFamilyConverter();
                            selTB.FontSize = fd.Font.Size;
                            selTB.FontFamily = (FontFamily)ffc.ConvertFromString(fd.Font.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Not a truetype font
                        MessageBox.Show("Шрифт не изменен");
                    }
                }
            }
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            if (selDPB!=null && selDPB.Width > 10 && selDPB.Height > 10)
            {
                selDPB.Width -= 10;
                selDPB.Height -= 10;
            }
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            if (selDPB != null)
            {
                selDPB.Width += 10;
                selDPB.Height += 10;
            }
        }

        public struct Data
        {
            public string a { get; set; }
            public string b { get; set; }
            public string c { get; set; }
        }

        private void button11_Click(object sender, RoutedEventArgs e)
        {
            Grid table = new Grid();
            table.SetValue(DraggableExtender.CanDragProperty, true);
            for (int i = 0; i < 5; i++)
            {
                RowDefinition row = new RowDefinition();
                table.RowDefinitions.Add(row);
                for (int j = 0; j < 5; j++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = GridLength.Auto;
                    table.ColumnDefinitions.Add(column);
                    TextBox tb = new TextBox();
                    tb.ContextMenu = null;
                    tb.MinWidth = 50;
                    tb.Text = "";
                    Grid.SetRow(tb, i);
                    Grid.SetColumn(tb, j);
                    table.Children.Add(tb);
                }

            }
            canvas1.Children.Add(table);
        }

        private string _previewWindowXaml =
    @"<Window
        xmlns                 ='http://schemas.microsoft.com/netfx/2007/xaml/presentation'
        xmlns:x               ='http://schemas.microsoft.com/winfx/2006/xaml'
        Title                 ='Print Preview - @@TITLE'
        Height                ='800'
        Width                 ='600'
        WindowStartupLocation ='CenterOwner'
        WindowState='Maximized'>
        <DocumentViewer Name='dv1'/>
     </Window>";

        internal void DoPreview(string title)
        {
            string fileName = System.IO.Path.GetRandomFileName();
            try
            {
                // write the XPS document
                using (XpsDocument doc = new XpsDocument(fileName, FileAccess.ReadWrite))
                {
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                    writer.Write(canvas1);
                }

                // Read the XPS document into a dynamically generated
                // preview Window 
                using (XpsDocument doc = new XpsDocument(fileName, FileAccess.Read))
                {
                    FixedDocumentSequence fds = doc.GetFixedDocumentSequence();

                    string s = _previewWindowXaml;
                    s = s.Replace("@@TITLE", title.Replace("'", "&apos;"));

                    using (var reader = new System.Xml.XmlTextReader(new StringReader(s)))
                    {
                        Window preview = System.Windows.Markup.XamlReader.Load(reader) as Window;

                        DocumentViewer dv1 = LogicalTreeHelper.FindLogicalNode(preview, "dv1") as DocumentViewer;
                        dv1.Document = fds as IDocumentPaginatorSource;


                        preview.ShowDialog();
                    }
                }
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                    }
                }
            }
        } 
        //for DataGrid table
        //DataGrid table = new DataGrid();
        //List<TextBox> tableData = new List<TextBox>(5);
        //table.ItemsSource = tableData;
        ////for (int i = 0; i < 5; i++)
        ////{
        ////    table.Columns.Add(new DataGridTextColumn());
        ////    table.CanUserAddRows = true;
        ////    table.CanUserDeleteRows = true;
        ////    table.IsReadOnly = false;
        ////}
        //canvas1.Children.Add(table);
    }
}
