using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Core.Tools.DotShape
{
    public class DotFactory
    {
        public List<Dot> CreateCircle(Vector3 position, float radius, int total)
        {
            var points = DotShapeUtils.GetCircle(position, radius, total);
            var items = DotShapeUtils.CreateNodes(points, 0.005f);

            var indexes = new List<int>();

            FillIndexes(indexes, 1, total);
            FillIndexes(indexes, 2, total);
            FillIndexes(indexes, 3, total);
            FillIndexes(indexes, 4, total);

            DotShapeUtils.Join(items, indexes, 2.5f);

            return items.Select(item => item.AddComponent<Dot>()).ToList();
        }

        public void FillIndexes(List<int> indexes, int shift, int total)
        {
            for (var i = 0; i < total; i++)
            {
                indexes.Add(i);
                indexes.Add((i + shift)%total);
            }
        }
    }
}
