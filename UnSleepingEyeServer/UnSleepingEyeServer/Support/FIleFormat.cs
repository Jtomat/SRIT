using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace UnSleepingEyeServer.Support
{
    public static class FileFormat
    {
        static int MimeSampleSize = 256;

        static string DefaultMimeType = "application/octet-stream";

        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        static extern int FindMimeFromData(IntPtr pBC,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.I1, SizeParamIndex=3)]
            byte[] pBuffer,
            int cbSize,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
            int dwMimeFlags,
            out IntPtr ppwzMimeOut,
            int dwReserved);

        public static string[] GetMimeFromBytes(byte[] data)
        {
            try
            {
                IntPtr mimeType;
                FindMimeFromData((IntPtr)0, null, data, MimeSampleSize, null, 0, out mimeType, 0);
                // Marshal.FreeCoTaskMem(mimeType);
                //var mimePointer = new IntPtr(mimeType);
                var mime = Marshal.PtrToStringUni(mimeType);
                Marshal.FreeCoTaskMem(mimeType);

                return new[] { ReadebleFormat(mime), FormatClass(mime) } ??
                    new[] { ReadebleFormat(DefaultMimeType), FormatClass(DefaultMimeType) };
            }
            catch
            {
                return new[] { ReadebleFormat(DefaultMimeType), FormatClass(DefaultMimeType) };
            }
        }

        public static string ReadebleFormat(string MimeType)
        {
            if (MimeType.Contains("png"))
            {
                MimeType = "png";
            }
            switch (MimeType)
            {
                case "application/x-zip-compressed":
                    return "office-x";
                case "text/plain":
                    return "txt";
                case "application/pdf":
                    return "pdf";
                case "image/bmp":
                    return "bmp";
                case "application/msword":
                    return "doc";
                case "image/gif":
                    return "gif";
                case "image/jpeg":
                    return "jpeg";
                case "audio/mpeg":
                    return "mp3";
                case "video/mpeg":
                    return "mpeg";
            }
            return MimeType;
        }
        public static string FormatClass(string MimeType)
        {

            return MimeType.Substring(0, MimeType.LastIndexOf("/"));
        }

        public static string[] GetContentFrom(byte[] file, string FormatClass)
        {
            switch (FormatClass)
            {
                case "application":
                    try
                    {
                        return ExctractFromDocX(file);
                    }
                    catch
                    {
                        return new string[] { };
                    }
                case "text":
                    return ExtractFromTXT(file);
            }

            return null;
        }
        static string[] ExtractFromTXT(byte[] file) {
            using (var strReader = new StreamReader(new MemoryStream(file)))
            {
                var resultStr = strReader.ReadToEnd();
                return resultStr.Split(new char[] {'\n','\r' }, StringSplitOptions.RemoveEmptyEntries);
            }

        }
        static string[] ExctractFromDocX(byte[] file)
        {
            var doc = WordprocessingDocument.Open(new MemoryStream(file), true);
            var res = new List<string>();// doc.MainDocumentPart.Document.Body.InnerText.Split(
                                         //new char[]{'\n','\r'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var cont in doc.MainDocumentPart.Document.Body.OfType<Paragraph>())
            {
                var data = cont.InnerText;
                if (string.IsNullOrEmpty(data))
                    continue;
                res.Add(data);
            }

            return res.ToArray();
        }
    }
}
