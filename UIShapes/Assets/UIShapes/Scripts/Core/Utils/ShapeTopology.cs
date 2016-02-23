namespace Scripts.Core.Tools.Render
{
    public class ShapeTopology
    {
        public static int[] TringleFan(int total)
        {
            var indices = new int[total*3];

            for (var i = 0; i < total; i++)
            {
                indices[3 * i] = 0;
                indices[3 * i + 1] = 2 + i;
                indices[3 * i + 2] = 1 + i;
            }

            return indices;
        }        
        
        public static int[] TringleQuad(int total)
        {
            var indices = new int[total*6];

            for (var i = 0; i < total*6; i += 6)
            {
                indices[i] = i;
                indices[i + 1] = i + 1;
                indices[i + 2] = i + 2;                
                indices[i + 3] = i + 2;
                indices[i + 4] = i + 3;
                indices[i + 5] = i;
            }

            return indices;
        }

        public static int[] TringleStrip(int total)
        {
            var indices = new int [total * 3];

            for (var i = 0; i < total; i++)
            {
                if (i%2 == 0)
                {
                    indices[3*i] = i;
                    indices[3*i + 1] = i + 1;
                    indices[3*i + 2] = i + 2;
                }
                else
                {
                    indices[3 * i] = i;
                    indices[3 * i + 1] = i + 2;
                    indices[3 * i + 2] = i + 1;
                }
            }

            return indices;
        }
    }
}
