using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;

namespace CurveFitting
{
    public partial class CurveFittingCalculate
    {
        public List<Point> Points { get; set; }

        private List<Point> insPois;// 补点后的点集

        private List<double[]> parms;// 每两个点间的曲线参数

        public CurveFittingCalculate(string path)
        {
            ReadingData(path);
            parms = new List<double[]>();
        }

        public List<Point> GetFittingPoints(bool isClose, int insPoiCount, out List<double[]> parms)
        {
            List<Point> fitPois = new List<Point>();
            this.parms.Clear();
            ComplementPoint(isClose);
            int i1 = 0, i2 = 1;
            while (true)
            {
                if (i1 > Points.Count - 1 || i2 > Points.Count - 1)
                {
                    break;
                }

                fitPois.AddRange(GetCurvePoint(i1++, i2++, 100));
            }
            // 如果是闭合曲线就再闭合到起始点
            if (isClose)
            {
                fitPois.AddRange(GetCurvePoint(i1, 0, insPoiCount));
            }
            parms = this.parms;
            return fitPois;
        }

        // 补充点
        private void ComplementPoint(bool isClose)
        {
            insPois = new List<Point>();

            if (isClose)
            {
                Point[] lastPoi = Points.Skip(Points.Count - 2).Take(2).ToArray();
                insPois.Add(lastPoi[0]);
                insPois.Add(lastPoi[1]);
                insPois.AddRange(Points);
                insPois.Add(Points[0]);
                insPois.Add(Points[1]);

                insPois.Add(Points[2]);
            }
            else
            {
                Point[] lastPoi = Points.Skip(Points.Count - 3).Take(3).Reverse().ToArray();
                Func<Point, Point, Point, Point> newPoi = (p1, p2, p3) => new Point()
                {
                    X = p3.X - 3 * p2.X + 3 * p1.X,
                    Y = p3.Y - 3 * p2.Y + 3 * p1.Y,
                };
                Point A = newPoi(Points[0], Points[1], Points[2]);
                Point B = newPoi(A, Points[0], Points[1]);
                Point C = newPoi(lastPoi[0], lastPoi[1], lastPoi[2]);
                Point D = newPoi(C, lastPoi[0], lastPoi[1]);

                insPois.Add(B);
                insPois.Add(A);
                insPois.AddRange(Points);
                insPois.Add(C);
                insPois.Add(D);
            }
        }

        // 计算梯度
        private void GetGrad(int insIndex, out double xGrad, out double yGrad)
        {
            Point[] p = insPois.Skip(insIndex).Take(5).ToArray();

            double[] a = new double[4], b = new double[4];
            for (int i = 0; i < 4; i++)
            {
                a[i] = p[i + 1].X - p[i].X;
                b[i] = p[i + 1].Y - p[i].Y;
            }
            double w2 = Abs(a[2] * b[3] - a[3] * b[2]);
            double w3 = Abs(a[0] * b[1] - a[1] * b[0]);
            double a0 = w2 * a[1] + w3 * a[2];
            double b0 = w2 * b[1] + w3 * b[2];
            xGrad = a0 / Sqrt(a0 * a0 + b0 * b0);
            yGrad = b0 / Sqrt(a0 * a0 + b0 * b0);
        }

        // 计算曲线上的所有点
        private IEnumerable<Point> GetCurvePoint(int i0, int i1, int poiCount)
        {
            GetGrad(i0, out double cos0, out double sin0);
            GetGrad(i1, out double cos1, out double sin1);
            Point p0 = Points[i0], p1 = Points[i1];
            double r = Sqrt(Pow(p1.X - p0.X, 2) + Pow(p1.Y - p0.Y, 2));
            // 计算曲线参数
            double
                E0 = p0.X,
                E1 = r * cos0,
                E2 = 3 * (p1.X - p0.X) - r * (cos1 + 2 * cos0),
                E3 = -2 * (p1.X - p0.X) + r * (cos1 + cos0),

                F0 = p0.Y,
                F1 = r * sin0,
                F2 = 3 * (p1.Y - p0.Y) - r * (sin1 + 2 * sin0),
                F3 = -2 * (p1.Y - p0.Y) + r * (sin1 + sin0);
            parms.Add(new double[] { E0, E1, E2, E3, F0, F1, F2, F3 });
            // 计算拟合点坐标
            return from i in Enumerable.Range(1, poiCount)
                   let z = 1.0 / i
                   select new Point
                   {
                       X = E0 + E1 * z + E2 * z * z + E3 * z * z * z,
                       Y = F0 + F1 * z + F2 * z * z + F3 * z * z * z
                   };
        }

        private void ReadingData(string path)
        {
            Points = new List<Point>();
            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    string[] split = line.Split(',');
                    Point point = new Point()
                    {
                        ID = int.Parse(split[0]),
                        X = double.Parse(split[1]),
                        Y = double.Parse(split[2])
                    };
                    Points.Add(point);
                }
            }
        }
    }
}
