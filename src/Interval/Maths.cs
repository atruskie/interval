namespace Math
{
    public static class Maths
    {
        public static double Center(double a, double b)
        {
            return ((a > b ? (a - b) : (b - a)) * 0.5) + a;
        }
    }
}