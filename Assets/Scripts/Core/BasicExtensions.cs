namespace BasicExtensions
{
    public static class BasicExtensions
    {
        public static bool IsWithin(this int value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        }
    }
}
