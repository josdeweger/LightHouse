namespace LightHouse.Delcom.SignalLight
{
    public static class Extensions
    {
        public static byte[] PadRight(this byte[] bytes, int nrOfBytes = 8)
        {
            var newArray = new byte[nrOfBytes];

            for (var i = 0; i < bytes.Length; i++)
            {
                newArray[i] = bytes[i];
            }

            var nextIndex = bytes.Length;
            var lastIndex = nrOfBytes - 1;

            for (var j = nextIndex; j <= lastIndex; j++)
            {
                newArray[j] = 0;
            }

            return newArray;
        }
    }
}
