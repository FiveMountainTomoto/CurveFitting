using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Point = CurveFitting.CurveFittingCalculate.Point;

namespace CurveFitting
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CurveFittingCalculate curFit;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ReadingDataButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opfdl = new OpenFileDialog()
            {
                Title = "读取数据文件",
                RestoreDirectory = true,
                Multiselect = false,
                Filter = "文本文档(*.txt)|*.txt"
            };
            if (opfdl.ShowDialog() == true)
            {
                curFit = new CurveFittingCalculate(opfdl.FileName);
                IEnumerable<string> strs = from p in curFit.Points
                                           select $"{p.ID}\t{p.X}\t{p.Y}";
                dataTextBox.Text = "ID,\tX,\tY\n";
                dataTextBox.Text += string.Join("\n", strs);
            }
        }

        private void CloseCurveButton_Click(object sender, RoutedEventArgs e)
        {
            // 两点间插入 10 个点
            List<Point> pois = curFit.GetFittingPoints(true, 10, out List<double[]> parms);
            OutPut(pois, parms);
        }
        private void UnclosedCurveButton_Click(object sender, RoutedEventArgs e)
        {
            List<Point> pois = curFit.GetFittingPoints(false, 10, out List<double[]> parms);
            OutPut(pois, parms);
        }

        private void OutPut(List<Point> pois, List<double[]> parms)
        {
            // 输出参数
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("起点ID\t终点ID\tE0\tE1\tE2\tE3\tF0\tF1\tF2\tF3");
            int i = 0;
            foreach (double[] p in parms)
            {
                if (i == 13)
                {
                    sb.AppendLine($"{i}\t0\t{p[0]:F3}\t{p[1]:F3}\t{p[2]:F3}\t{p[3]:F3}\t{p[4]:F3}\t{p[5]:F3}\t{p[6]:F3}\t{p[7]:F3}");// 闭合曲线的最后一个结果用的
                }
                else
                {
                    sb.AppendLine($"{i}\t{++i}\t{p[0]:F3}\t{p[1]:F3}\t{p[2]:F3}\t{p[3]:F3}\t{p[4]:F3}\t{p[5]:F3}\t{p[6]:F3}\t{p[7]:F3}");
                }
            }
            resultTextBox.Text = sb.ToString();

            // 输出曲线拟合点
            IEnumerable<string> poiStrs = from p in pois select $"{p.X:F3},{p.Y:F3}";
            curPoiTextBox.Text = string.Join("\n", poiStrs);

            tabControl.SelectedIndex = 1;
        }

        private void SaveResultButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfdl = new SaveFileDialog()
            {
                Title = "保存结果为文本文档",
                RestoreDirectory = true,
                Filter = "文本文档(*.txt)|*.txt"
            };
            if (sfdl.ShowDialog() == true)
            {
                File.WriteAllText(sfdl.FileName, resultTextBox.Text);
            }
        }

        private void SaveCurPoiButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfdl = new SaveFileDialog()
            {
                Title = "保存曲线拟合点为文本文档",
                RestoreDirectory = true,
                Filter = "文本文档(*.txt)|*.txt"
            };
            if (sfdl.ShowDialog() == true)
            {
                File.WriteAllText(sfdl.FileName, curPoiTextBox.Text);
            }
        }
    }
}
