// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("4+Z1fATSGr2CDMQddzQQ3DV10M55WD35u177kYg/GQTHW4KJn61kLWyE+tT0ubta3XK5nETGKdO5IHPMCf7IoUfI5inAiXz+tNiqLSZkhfrY0RD94Dn2QOmJ66lzEVKA8U45ByJaTYUdtWvcF8gHQfMB9Zvmy9FtVF5Typ0mg6bp5JdpvbBJk/5FD01x8vzzw3Hy+fFx8vLzS3tBZ7wIIFmDKl6qSQPsbvJVOpYljP1OcHGE8ctZK5QGWEDdhJtcQ1q2xUuMQBg7MvEGL0px2GvOe/ZpE1BfpX3dzX1G+R6YvPgk+Jl8ixCZ7kvyTy2ciV+3vTNd68bvEqCOHVyTCtuHskbDcfLRw/71+tl1u3UE/vLy8vbz8KtRFmkL4eW5fvHw8vPy");
        private static int[] order = new int[] { 5,11,3,10,7,7,10,11,8,13,10,13,12,13,14 };
        private static int key = 243;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
