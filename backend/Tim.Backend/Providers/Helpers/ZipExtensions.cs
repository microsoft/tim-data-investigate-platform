// <copyright file="ZipExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Helpers
{
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Helper functions for object conversion to and from compressed base64.
    /// Based on <see href="https://stackoverflow.com/questions/7343465/compression-decompression-string-with-c-sharp/64582157">stackoverflow</see>.
    /// </summary>
    public static class ZipExtensions
    {
        /// <summary>
        /// Converts an object to json and compresses it.
        /// </summary>
        /// <param name="obj">Object to convert to data.</param>
        /// <returns>Compressed data.</returns>
        public static byte[] ObjectToByteArray(this object obj)
        {
            return Encoding.UTF8.GetBytes(obj.ToJson()).Compress();
        }

        /// <summary>
        /// Decompress json and convert to an object.
        /// </summary>
        /// <typeparam name="T">Type to convert to.</typeparam>
        /// <param name="data">Compressed data.</param>
        /// <returns>Coverted object.</returns>
        public static T ObjectFromByteArray<T>(this byte[] data)
        {
            return Encoding.UTF8.GetString(data.Decompress()).FromJson<T>();
        }

        /// <summary>
        /// Compress a byte array.
        /// </summary>
        /// <param name="data">Byte array to compress.</param>
        /// <returns>Compressed byte array.</returns>
        public static byte[] Compress(this byte[] data)
        {
            using var sourceStream = new MemoryStream(data);
            using var destinationStream = new MemoryStream();
            sourceStream.CompressTo(destinationStream);
            return destinationStream.ToArray();
        }

        /// <summary>
        /// Decompress a byte array.
        /// </summary>
        /// <param name="data">Byte array to decompress.</param>
        /// <returns>Decompressed byte array.</returns>
        public static byte[] Decompress(this byte[] data)
        {
            using var sourceStream = new MemoryStream(data);
            using var destinationStream = new MemoryStream();
            sourceStream.DecompressTo(destinationStream);
            return destinationStream.ToArray();
        }

        /// <summary>
        /// Compress a stream.
        /// </summary>
        /// <param name="stream">Stream to compress.</param>
        /// <param name="outputStream">Stream to write to.</param>
        public static void CompressTo(this Stream stream, Stream outputStream)
        {
            using var gzipStream = new GZipStream(outputStream, CompressionMode.Compress);
            stream.CopyTo(gzipStream);
            gzipStream.Flush();
        }

        /// <summary>
        /// Decompress a stream.
        /// </summary>
        /// <param name="stream">Stream to decompress.</param>
        /// <param name="outputStream">Stream to write to.</param>
        public static void DecompressTo(this Stream stream, Stream outputStream)
        {
            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
            gzipStream.CopyTo(outputStream);
        }

        /// <summary>
        /// Convert an object to json.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Json string.</returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Convert json to a typed object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="data">Json to convert.</param>
        /// <returns>Object from json.</returns>
        public static T FromJson<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
