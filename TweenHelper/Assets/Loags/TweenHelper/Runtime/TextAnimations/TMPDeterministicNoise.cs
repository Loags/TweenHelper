namespace LB.TweenHelper
{
    internal static class TMPDeterministicNoise
    {
        public static float Sample(int seed, int character, int timeSlice, int channel)
        {
            unchecked
            {
                uint value = unchecked((uint)seed * 374761393u + (uint)character * 668265263u + (uint)timeSlice * 2246822519u + (uint)channel * 3266489917u);
                value = (value ^ (value >> 13)) * 1274126177;
                value ^= value >> 16;
                return (value & 0x00ffffff) / 16777215f;
            }
        }
    }
}
