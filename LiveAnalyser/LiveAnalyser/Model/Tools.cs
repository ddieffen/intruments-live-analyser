using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Drawing;
using ZedGraph;



namespace LiveAnalyser.Model
{
    public static class Tools
    {
        /// <summary>
        /// Converts UNIX timestamp to DateTime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime FromPOSIX(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// Converts DateTime to UNIX timestamp
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long ToPOSIX(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return (long)diff.TotalSeconds;
        }

        /// <summary>
        /// Perform a Clone of the object
        /// </summary>
        /// <typeparam name="T">The type of object being cloned.</typeparam>
        /// <param name="RealObject">The object instance to clone.</param>
        /// <returns>The cloned object.</returns>
        public static T Clone<T>(T RealObject)
        {
            // No need to serialize (or clone) null object, simply return null (or the object itself)
            if (Object.ReferenceEquals(RealObject, null))
                return default(T);
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be marked Serializable", "RealObject");
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        public static class CompDecomp
        {
            internal static long Decompress(Stream inp, Stream outp)
            {
                byte[] buf = new byte[1000];
                long nBytes = 0;
                inp.Position = 0;

                // Decompress the contents of the input file
                using (inp = new DeflateStream(inp, CompressionMode.Decompress))
                {
                    int len;
                    while ((len = inp.Read(buf, 0, buf.Length)) > 0)
                    {
                        // Write the data block to the decompressed output stream
                        outp.Write(buf, 0, len);
                        nBytes += len;
                    }
                }
                // Done
                return nBytes;
            }
            internal static long Compress(Stream inp, Stream outp)
            {
                byte[] buf = new byte[1000];
                long nBytes = 0;
                inp.Position = 0;

                // Compress the contents of the input file
                using (outp = new DeflateStream(outp, CompressionMode.Compress, true))
                {
                    int len;
                    while ((len = inp.Read(buf, 0, buf.Length)) > 0)
                    {
                        // Write the data block to the compressed output stream
                        outp.Write(buf, 0, len);
                        nBytes += len;
                    }
                }
                // Done
                return nBytes;
            }
        }

        public static double[] LeastSquares(PointPairList pts)
        {
            if (pts.Count > 0)
            {
                double sumXiyi = 0;
                double sumXi = 0;
                double sumYi = 0;
                double sumXiSquare = 0;
                for (int i = 0; i < pts.Count; i++)
                {
                    sumXiyi += pts[i].X * pts[i].Y;
                    sumXi += pts[i].X;
                    sumYi += pts[i].Y;
                    sumXiSquare += Math.Pow(pts[i].X, 2);
                }

                double beta = (sumXiyi - ((1 / pts.Count) * sumXi * sumYi)) / (sumXiSquare - (1 / pts.Count) * Math.Pow(sumXi, 2));
                double alpha = (sumYi / pts.Count) - (beta * sumXi / pts.Count);

                return new double[2] { alpha, beta };
            }
            return new double[2] { 0, 0 };
        }

        internal static IPointList CreateLinear(double[] ab1, PointPairList ppl1)
        {
            PointPairList returned = new PointPairList();
            if (ppl1.Count != 0 && ab1.Count() == 2)
            {
                double minX = 0;
                double maxX = ppl1.OrderBy(item => item.X).Last().X;

                returned.Add(minX, ab1[1] * minX + ab1[0]);
                returned.Add(maxX, ab1[1] * maxX + ab1[0]);
            }
            return returned;
        }
    }
}
