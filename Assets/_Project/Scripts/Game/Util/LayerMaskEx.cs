using UnityEngine;

namespace Util
{
    public static class LayerMaskEx
    {
        public static bool Contains(this LayerMask self, int layer)
        {
            var layerBit = 1 << layer;

            return (self.value & layerBit) == layerBit;
        }


        public static int FirstLayerSet(this LayerMask self)
        {
            if (self.value == 0) return 0;

            int layer = 0;
            while ((self.value & (1 << layer)) == 0)
            {
                ++layer;
            }

            return layer;
        }
    }
}