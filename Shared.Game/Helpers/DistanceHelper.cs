using System;
using System.Windows;

namespace Shared.Game.Helpers
{
    public static class DistanceHelper
    {
        public static double GetDistance(Point a, Point b) => Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

        //ToDo verify if is this correct for all cases
        public static Point GetUnitDistance(Point a, Point b)
        {
            Point result;

            result = new Point(a.X-b.X,a.Y-b.Y);

            return result;
        }

        public static double GetDistance(double a, double b) => Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
    }
}