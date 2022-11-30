using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikesRoadServer
{
    class StreamSaver
    {
        public static void SaveStreamToFile(string path,Stream stream)
        {
            byte[] bytes = GetStreamBytes(stream);
            File.WriteAllBytes(path, bytes);
        }
        public static byte[] GetStreamBytes(Stream stream)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(stream))
            {
                bytes = br.ReadBytes((int)stream.Length);
            }

            return bytes;
        }
    }
}
