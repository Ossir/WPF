using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;

namespace Wpf
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        int zoomPercent = 1;
        BitmapImage img;
        List<Image> pictureBox1 = new List<Image>();
        List<TextBox> textList = new List<TextBox>();
        List<Grid> gridList = new List<Grid>();
        Image selDPB = null;
        TextBox selTB = null;
        Grid selGrid = null;
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

        private TextBox CreateTableTB()
        {
            TextBox tb = new TextBox();
            tb.ContextMenu = null;
            tb.AcceptsReturn = true;
            tb.Height = Double.NaN;
            tb.Width = Double.NaN;
            tb.MouseLeftButtonDown += new MouseButtonEventHandler(TBFocusEvent);
            tb.GotFocus += new RoutedEventHandler(TBFocusEvent);
            tb.MinWidth = 50;
            tb.Background = Brushes.Transparent;
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
            FrameworkElement element = sender as FrameworkElement;
            var par = element.Parent as UIElement;
            selGrid = null;
            selDPB = null;
            selTB = (TextBox)sender;
            selTB.Focus();
            selTB.BorderThickness = new Thickness(1);
            var control = sender as UIElement;
            if (control != null && control.GetType() == typeof(TextBox) && par.GetType() != typeof(Grid))
            {
                canvas1.Children.Remove(control);
                canvas1.Children.Add(control);
            }else
                if (control != null && control.GetType() == typeof(TextBox) && par.GetType() == typeof(Grid))
                {
                    selGrid = (Grid)par;
                    selGrid.Focus();
                }
        }

        private void PBFocusEvent(object sender, EventArgs e)
        {
            selGrid = null;
            selTB = null;
            selDPB = (Image)sender;
            selDPB.Focus();
            //var control = sender as UIElement;
            //if (control != null && control.GetType() == typeof(TextBox))
            //{
            //    canvas1.Children.Remove(control);
            //    canvas1.Children.Add(control);
            //}
        }

        private void GridFocusEvent(object sender, EventArgs e)
        {
            selTB = null;
            selDPB = null;
            selGrid = (Grid)sender;
            selGrid.Focus();
            //var control = sender as UIElement;
            //if (control != null && control.GetType() == typeof(TextBox))
            //{
            //    canvas1.Children.Remove(control);
            //    canvas1.Children.Add(control);
            //}
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
                    for (int i = 0; i < os.GetPicEnumerator(); i++)
                    {
                        pictureBox1.Add(new Image());
                        pictureBox1.Last().MouseLeftButtonDown += new MouseButtonEventHandler(PBFocusEvent);
                        pictureBox1.Last().Source = StringToImage(os.GetFromPicList(i).PicArray);
                        pictureBox1.Last().SetValue(DraggableExtender.CanDragProperty, true);
                        var transform = pictureBox1.Last().RenderTransform as TranslateTransform;
                        if (transform == null)
                        {
                            transform = new TranslateTransform();
                            pictureBox1.Last().RenderTransform = transform;
                        }
                        transform.X = os.GetFromPicList(i).X - canvas1.Margin.Left;
                        transform.Y = os.GetFromPicList(i).Y - canvas1.Margin.Top;
                        pictureBox1.Last().RenderTransform = transform;
                        canvas1.Children.Add(pictureBox1.Last());
                    }
                    for (int i = 0; i < os.GetTextEnumerator(); i++)
                    {
                        textList.Add(CreateTB());
                        textList.Last().Text = os.GetFromTextList(i).Text;
                        textList.Last().FontFamily = new FontFamily(os.GetFromTextList(i).FontName);
                        textList.Last().FontSize = os.GetFromTextList(i).FontSize;
                        BrushConverter bc = new BrushConverter();
                        textList.Last().Foreground = (Brush)bc.ConvertFrom(os.GetFromTextList(i).FontColor);
                        var transform = textList.Last().RenderTransform as TranslateTransform;
                        if (transform == null)
                        {
                            transform = new TranslateTransform();
                            textList.Last().RenderTransform = transform;
                        }
                        transform.X = os.GetFromTextList(i).X - canvas1.Margin.Left;
                        transform.Y = os.GetFromTextList(i).Y - canvas1.Margin.Top;
                        textList.Last().RenderTransform = transform;
                        canvas1.Children.Add(textList.Last());
                    }
                    for (int i = 0; i < os.GetGridEnumerator(); i++)
                    {
                        gridList.Add(new Grid());
                        gridList.Last().SetValue(DraggableExtender.CanDragProperty, true);
                        for (int f = 0; f < os.GetFromGridList(i).rows; f++)
                        {
                            RowDefinition row = new RowDefinition();
                            gridList.Last().RowDefinitions.Add(row);
                            for (int j = 0; j < os.GetFromGridList(i).columns; j++)
                            {
                                ColumnDefinition column = new ColumnDefinition();
                                column.Width = GridLength.Auto;
                                gridList.Last().ColumnDefinitions.Add(column);
                                TextBox tb = CreateTableTB();
                                Grid.SetRow(tb, f);
                                Grid.SetColumn(tb, j);
                                gridList.Last().Children.Add(tb);
                            }

                        }
                        int k = 0;
                        foreach (TextBox t in gridList.Last().Children)
                        {                           
                            t.Text = os.GetFromGridList(i).text[k];
                            k++;
                        }

                        var transform = gridList.Last().RenderTransform as TranslateTransform;
                        if (transform == null)
                        {
                            transform = new TranslateTransform();
                            gridList.Last().RenderTransform = transform;
                        }
                        transform.X = os.GetFromGridList(i).X - canvas1.Margin.Left;
                        transform.Y = os.GetFromGridList(i).Y - canvas1.Margin.Top;
                        gridList.Last().RenderTransform = transform;
                        canvas1.Children.Add(gridList.Last());
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
                    double WidthPercent = img.Width * zoomPercent / 100;
                    double HeightPercent = img.Height * zoomPercent / 100;
                    while (pictureBox1.Last().Width > canvas1.Width || pictureBox1.Last().Height > canvas1.Height)
                    {
                        pictureBox1.Last().Width -= WidthPercent;
                        pictureBox1.Last().Height -= HeightPercent;
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
                        foreach (Grid table in gridList)
                        {
                            List<string> text = new List<string>();
                            foreach (TextBox t in table.Children)
                            {
                                text.Add(t.Text);
                            }
                            Point relativePoint = table.TransformToAncestor(this).Transform(new Point(0, 0));
                            SerGrid sg = new SerGrid(relativePoint.X, relativePoint.Y, table.RowDefinitions.Count, table.ColumnDefinitions.Count / table.RowDefinitions.Count, text);
                            os.AddToGridList(sg);
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
            //else
            //    MessageBox.Show("Выберите картинку на форме");
            if (selTB != null)
            {
                textList.Remove(selTB);
                canvas1.Children.Remove(selTB);
            }
            //else
            //    MessageBox.Show("Выберите текст для удаления");
            if (selGrid != null)
            {
                gridList.Remove(selGrid);
                canvas1.Children.Remove(selGrid);
            }
            //else
            //    MessageBox.Show("Выберите таблицу для удаления");
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
            Window2 tableDialog = new Window2();
            tableDialog.ShowDialog();
            if (tableDialog.yesButton)
            {
                gridList.Add(new Grid());
                gridList.Last().MouseLeftButtonDown += new MouseButtonEventHandler(GridFocusEvent);
                gridList.Last().SetValue(DraggableExtender.CanDragProperty, true);
                for (int i = 0; i < tableDialog.rows; i++)
                {
                    RowDefinition row = new RowDefinition();
                    gridList.Last().RowDefinitions.Add(row);
                    for (int j = 0; j < tableDialog.columns; j++)
                    {
                        ColumnDefinition column = new ColumnDefinition();
                        column.Width = GridLength.Auto;
                        gridList.Last().ColumnDefinitions.Add(column);
                        TextBox tb = CreateTableTB();
                        Grid.SetRow(tb, i);
                        Grid.SetColumn(tb, j);
                        gridList.Last().Children.Add(tb);
                    }

                }
                canvas1.Children.Add(gridList.Last());
            }
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
    }
}
