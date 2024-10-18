using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SharpShell.Attributes;
using SharpShell.SharpThumbnailHandler;

namespace GnfThumbnailSharp
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.FileExtension, ".gnf")]
    [DisplayName("GnfThumbnailHandler")]
    [Guid("5F5F476E-6654-6875-6D62-6E61696C5F5F")]
    public class GnfThumbnailHandler : SharpThumbnailHandler, IDisposable
    {
        public GnfThumbnailHandler()
        {
        }
        public void Dispose()
        {
        }

        /// <summary>
        /// Gets the thumbnail image.
        /// </summary>
        /// <param name="width">The width of the image that should be returned.</param>
        /// <returns>
        /// The image for the thumbnail.
        /// </returns>
        protected override Bitmap GetThumbnailImage(uint width)
        {
            try
            {
                int lenght = (int)SelectedItemStream.Length;
                if (SelectedItemStream.Length > 0x3200000)
                {
                    lenght = 0x3200000;
                }

                byte[] arr = new byte[lenght];
                SelectedItemStream.Read(arr, 0, arr.Length);
                var ms = new MemoryStream(arr);

                var gnf = new Scarlet.IO.ImageFormats.GNF();
                gnf.Open(ms, Scarlet.IO.Endian.LittleEndian);
                var bitmap = gnf.GetBitmap(0, 0);
                ms.Close();

                int newHeight = bitmap.Height;
                int newWidth = bitmap.Width;

                if (newWidth > width || newWidth > width)
                {
                    newWidth = (int)width;
                    double aspectRatio = (double)bitmap.Height / bitmap.Width;
                    newHeight = (int)(width * aspectRatio);
                  
                    if (newHeight > width)
                    {
                        double aspectRatio2 = (double)bitmap.Width / bitmap.Height;
                        newWidth = (int)(width * aspectRatio2);
                        newHeight = (int)width;
                    }

                    if (newHeight < 4)
                    {
                        newHeight = 4;
                    }
                    if (newWidth < 4)
                    {
                        newWidth = 4;
                    }

                    Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(() => false);
                    var imag = bitmap.GetThumbnailImage((int)newWidth, (int)newHeight, myCallback, IntPtr.Zero);
                    bitmap = (Bitmap)imag;
                }
            
                return bitmap;
            }
            catch (Exception)
            {
                var bitmap = new Bitmap((int)width, (int)width);
                var g = Graphics.FromImage(bitmap);
                g.FillRectangle(Brushes.Black, 0, 0, width, width);
                Pen p = new Pen(Color.FromArgb(0xC0, 0x00, 0x00), (int)Math.Sqrt(width));
                g.DrawLine(p, -1, -1, width , width);
                g.DrawLine(p, -1, width, width, -1);
                return bitmap;
            }
        }


    }
}