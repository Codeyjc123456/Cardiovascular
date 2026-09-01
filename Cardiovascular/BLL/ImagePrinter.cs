using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace Cadio.BLL
{
    public static class ImagePrinter
    {
        public static void PrintImagesWithDialog(IList<string> imagePaths)
        {
            if (imagePaths == null || imagePaths.Count == 0) return;

            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true) return;

            // 用户在系统对话框里选择的打印队列
            var selectedQueueFullName = printDialog.PrintQueue?.FullName;

            var doc = new PrintDocument { DocumentName = "Image Print" };

            // 尽量把选中的打印机映射到 GDI+ 的 PrinterName
            if (!string.IsNullOrWhiteSpace(selectedQueueFullName))
            {
                foreach (string p in PrinterSettings.InstalledPrinters)
                {
                    if (selectedQueueFullName.EndsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        selectedQueueFullName.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        doc.PrinterSettings.PrinterName = p;
                        break;
                    }
                }
            }

            int index = 0;

            doc.BeginPrint += (_, __) => index = 0;

            doc.PrintPage += (s, e) =>
            {
                if (index >= imagePaths.Count)
                {
                    e.HasMorePages = false;
                    return;
                }

                using var img = LoadImageSafe(imagePaths[index++]);

                // 可打印区域（已包含页边距）
                Rectangle bounds = e.PageBounds;

                // 等比缩放并居中
                Rectangle target = GetScaledRect(img.Width, img.Height, bounds);

                // 背景填充（避免透明 PNG 打印底色不一致）
                using (var bg = new SolidBrush(Color.White))
                    e.Graphics.FillRectangle(bg, bounds);

                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                e.Graphics.DrawImage(img, target);

                e.HasMorePages = index < imagePaths.Count;
            };

            doc.Print();
        }

        private static Bitmap LoadImageSafe(string path)
        {
            // 从文件流读入，避免文件占用导致失败
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var img = System.Drawing.Image.FromStream(fs);
            return new Bitmap(img);
        }

        private static Rectangle GetScaledRect(int srcW, int srcH, Rectangle dstBounds)
        {
            if (srcW <= 0 || srcH <= 0) return dstBounds;
            float scale = Math.Min(
                (float)dstBounds.Width / srcW,
                (float)dstBounds.Height / srcH);
            int w = (int)Math.Round(srcW * scale);
            int h = (int)Math.Round(srcH * scale);

            int x = dstBounds.X + (dstBounds.Width - w) / 2;
            int y = dstBounds.Y + (dstBounds.Height - h) / 2;
            return new Rectangle(x, y, w, h);
        }
    }
}
