using System;
using System.Collections.Generic;
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

namespace DataObject.Generator
{
    using System.Diagnostics;
    using System.Data;
    using Core.Comm;
    using Core;
    using Core.Collection;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.ContentRendered += MainWindow_ContentRendered;
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            LoadDataBases();
        }

        private void LoadDataBases()
        {
            try
            {
                cbDbs.ItemsSource = DBEngine.All();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"错误",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            LoadDataBases();
        }


        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                GeneratorDocument document = new GeneratorDocument();
                string path = document.Save(tvTables.SelectedItem as DatatableItem);

                if(MessageBox.Show("文件已成功生成，是否打开输出目录？","完成",MessageBoxButton.OKCancel,MessageBoxImage.Information,MessageBoxResult.OK) == MessageBoxResult.OK)
                {
                    using (Process p = new Process())
                    {
                        p.StartInfo.FileName = "explorer.exe";
                        p.StartInfo.Arguments = @" /select, " + path;
                        p.Start();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }


        private void cbDbs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ISqlObject obj = e.AddedItems[0] as ISqlObject;
            tvTables.ItemsSource = obj.Children;
        }

        private void tvTables_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
            string tableName = tvTables.SelectedValue.ToString();
            string lastfix = "Entity";
            string className = tableName + lastfix;
            ISqlObject obj = e.NewValue as ISqlObject;
            dgFields.ItemsSource = obj.Children;

            if (obj.Children.Count > 0)
            {
                btnGen.IsEnabled = true;
            }
        }

        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("1.0.0.1","版本");
        }
    }
}