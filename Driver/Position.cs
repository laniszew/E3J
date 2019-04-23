using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Driver
{
    public class Position
    {
        public int ID { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; } 
        public double A { get; private set; }
        public double B { get; private set; }
        public double L1 { get; private set; }
        public E3JManipulator.GrabE Grab { get; private set; }

        protected Position()
        {
            // empty constructor for factory methods
        }

        public static Position CreatePosition(int id, double X = 0, double Y = 0,
            double Z = 0, double A = 0, double B = 0 , double L1 = 0, 
            E3JManipulator.GrabE grab = E3JManipulator.GrabE.Open)
        {
            return new Position()
            {
                X = X,
                Y = Y,
                Z = Z,
                A = A,
                B = B,
                L1 = L1,
                Grab = grab
            };
        }

        /// <summary>
        /// Creates position from WH (Where) command response
        /// </summary>
        /// <param name="position"></param>
        public static Position FromWHResponse(string position)
        {
            var splitted = position.Split(',');

            // Validate input
            if (!Regex.IsMatch(position, @"^((\+|-)(\d+.\d+,)){7}[R|L],[A|B],[O|C]$") || splitted.Length != 10)
            {
                Console.Error.WriteLine("Could not create position because the input is invalid.");
                return null;
            }

            return new Position()
            {
                X = double.Parse(splitted[0], CultureInfo.InvariantCulture),
                Y = double.Parse(splitted[1], CultureInfo.InvariantCulture),
                Z = double.Parse(splitted[2], CultureInfo.InvariantCulture),
                A = double.Parse(splitted[3], CultureInfo.InvariantCulture),
                B = double.Parse(splitted[4], CultureInfo.InvariantCulture),
                L1 = double.Parse(splitted[5], CultureInfo.InvariantCulture),
                Grab = splitted[9].Equals("O") ? E3JManipulator.GrabE.Open : E3JManipulator.GrabE.Closed
            };
        }
    }
}
