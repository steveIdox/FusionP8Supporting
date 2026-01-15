using Aspose.Pdf.Operators;
using Files.idox.eim.fusionp8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// uses aspose

namespace Stamping
{
    public class Stamp
    {
        public static byte[] ApplyText(string file, string text, Common.Objects.StampConfiguration stampConfiguration)
        {
            if (!System.IO.File.Exists(file))
            {
                Logging.Log.Error("No such file [" + file + "]", "Stamping");
                return null;
            }

            Aspose.Pdf.License license = new Aspose.Pdf.License();
            license.SetLicense(System.Configuration.ConfigurationManager.AppSettings["Aspose:License"]);

            var pdf = new Aspose.Pdf.Document(file);
            
            var stamp = new Aspose.Pdf.TextStamp(text);
            if (stampConfiguration.HorizontalAlignment == Common.Objects.HorizontalStampAlignment.Center) 
                stamp.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center;
            else if (stampConfiguration.HorizontalAlignment == Common.Objects.HorizontalStampAlignment.Right)
                stamp.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Right;
            else stamp.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Left;

            if(stampConfiguration.VerticalAlignment == Common.Objects.VerticalStampAlignment.Middle)
                stamp.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Center;
            else if (stampConfiguration.VerticalAlignment == Common.Objects.VerticalStampAlignment.Top)
                stamp.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top;
            else stamp.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Bottom;
            
            stamp.Opacity = stampConfiguration.Opacity;
            stamp.RotateAngle = stampConfiguration.Rotation;

            stamp.TextState.Font = Aspose.Pdf.Text.FontRepository.FindFont(stampConfiguration.FontName);
            stamp.TextState.FontSize = stampConfiguration.FontSize;
            stamp.TextState.ForegroundColor = HexToColor(stampConfiguration.FontColor);

            foreach (int iPage in stampConfiguration.Pages)
                pdf.Pages[iPage].AddStamp(stamp);

            string tempFile = FileHelper.GetTempFilePathFromInput(file);
            pdf.Save(tempFile);
            return FileHelper.bytesFromFile(tempFile);
        }
        private static Aspose.Pdf.Color HexToColor(string hex)
        {
            hex = hex.TrimStart('#');
            double r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255.0;
            double g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255.0;
            double b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255.0;
            return Aspose.Pdf.Color.FromRgb(r, g, b);
        }


        public static byte[] ApplyImage(string file, string image, Common.Objects.StampConfiguration stampConfiguration)
        {
            if (!System.IO.File.Exists(file))
            {
                Logging.Log.Error("No such file [" + file + "]", "Stamping");
                return null;
            }

            if (!System.IO.File.Exists(image))
            {
                Logging.Log.Error("No such image file [" + image + "]", "Stamping");
                return null;
            }

            Aspose.Pdf.License license = new Aspose.Pdf.License();
            license.SetLicense(System.Configuration.ConfigurationManager.AppSettings["Aspose:License"]);

            var pdf = new Aspose.Pdf.Document(file);
            
            var stamp = new Aspose.Pdf.ImageStamp(image);
            if (stampConfiguration.HorizontalAlignment == Common.Objects.HorizontalStampAlignment.Center) 
                stamp.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center;
            else if (stampConfiguration.HorizontalAlignment == Common.Objects.HorizontalStampAlignment.Right)
                stamp.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Right;
            else stamp.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Left;

            if(stampConfiguration.VerticalAlignment == Common.Objects.VerticalStampAlignment.Middle)
                stamp.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Center;
            else if (stampConfiguration.VerticalAlignment == Common.Objects.VerticalStampAlignment.Top)
                stamp.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top;
            else stamp.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Bottom;
            
            stamp.Opacity = stampConfiguration.Opacity;
            stamp.RotateAngle = stampConfiguration.Rotation;

            pdf.Pages[1].AddStamp(stamp);
            string tempFile = FileHelper.GetTempFilePathFromInput(file);
            pdf.Save(tempFile);
            return FileHelper.bytesFromFile(tempFile);
        }
    }
}
