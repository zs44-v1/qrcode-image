using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;


namespace GenQRCode
{
    public enum qrPosition
    {
        none, right, top, left, bottom, center
    }
    public enum imageExtension
    {
        Horizontal, Vertical
    }

    public enum ErrorCode
    {
        low,
        med,
        quart,
        high
    }
    public partial class GenQRCode
    {
        /// <summary>
        /// 
        /// </summary>
        private string _qrData;
        private string _outpath;
        private Size _qrSize;
        private List<InfoLineText> _lineList;
        private qrPosition _qrcLoc;
        private BarcodeWriter _qrcodegen = new BarcodeWriter();
        private EncodingOptions _encodeopt;
        private Size _imgSize;
        private float _dpiResolution = 300;
        private bool _resizeFonts = true;
        private string _fileName;

        private const int _cstDpiResolution = 300;
        private const int _qrcSquareSize = 600;
        public int imgStep = 5;



        public string fileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public string qrData
        {
            get { return _qrData; }
            set { _qrData = value; }
        }

        public string outPath
        {
            get { return _outpath; }
            set { _outpath = value; }
        }

        public Size qrSize
        {
            get { return _qrSize; }
            set { _qrSize = value; }
        }



        public Size imgSize
        {
            get { return _imgSize; }
            set { _imgSize = value; }
        }

        public int dpiResolution
        {
            get
            { return Convert.ToInt32(Math.Round(_dpiResolution,MidpointRounding.AwayFromZero)); }
            set
            { _dpiResolution = (float)value; }
        }

        public bool resizeFont
        {
            get { return _resizeFonts; }
            set { _resizeFonts = value; }

        }

        public List<InfoLineText> textLines
        {
            get { return _lineList; }
            set { _lineList = value; }
        }

        public qrPosition qrLocation
        {
            get { return _qrcLoc; }
            set { _qrcLoc = value; }
        }

        public Bitmap outfile;
        public Bitmap imageFile;



        /// <summary>
        /// GenQRCode constructor
        /// </summary>
        /// <param name="qrdata">Data string in qr code</param>
        /// <param name="outpath">Path string where the image will be saved</param>
        /// <param name="fName"></param>
        public GenQRCode(string qrdata, string outpath, string fName) : this(qrdata, outpath, fName, _qrcSquareSize)
        {
        }

        /// <summary>
        /// GenQRCode constructor
        /// </summary>
        /// <param name="qrdata">Data string in qr code</param>
        /// <param name="outpath">Path string where the image will be saved</param>
        /// <param name="fName"></param>
        /// <param name="qrcSize">QR code size in pixels, given as a point struct</param>
        public GenQRCode(string qrdata, string outpath, string fName, int qrcSize) : this(qrdata, outpath, fName, qrcSize, _cstDpiResolution)
        {
        }

        /// <summary>
        /// GenQRCode constructor
        /// </summary>
        /// <param name="qrdata">Data string in qr code</param>
        /// <param name="outpath">Path string where the image will be saved</param>
        /// <param name="fName"></param>
        /// <param name="qrcSize">QR code size in pixels, given as a point struct</param>
        /// <param name="imgResolution">Image resolution in dpi</param>
        public GenQRCode(string qrdata, string outpath, string fName, int qrcSize, int imgResolution) : this (qrdata,outpath, fName, qrcSize,imgResolution, false)
        {
        }

        /// <summary>
        /// GenQRCode constructor
        /// </summary>
        /// <param name="qrdata">Data string in qr code</param>
        /// <param name="outpath">Path string where the image will be saved</param>
        /// <param name="fName"></param>
        /// <param name="qrcSize">QR code size in pixels, given as a point struct</param>
        /// <param name="imgResolution">Image resolution in dpi</param>
        /// <param name="fontResize"></param>
        public GenQRCode(string qrdata, string outpath, string fName, int qrcSize, int imgResolution, bool fontResize) 
            : this(qrdata, outpath, fName, qrcSize, imgResolution, fontResize, null)
        {
        }
        /// <summary>
        /// GenQRCode constructor
        /// </summary>
        /// <param name="qrdata">Data string in qr code</param>
        /// <param name="outpath">Path string where the image will be saved</param>        
        /// <param name="fName"></param>
        /// <param name="qrcSize">QR code size in pixels, given as a point struct</param>
        /// <param name="imgResolution">Image resolution in dpi</param>
        /// <param name="fontResize"></param>
        /// <param name="lineList">List of additional text to write on image</param>
        public GenQRCode(string qrdata, string outpath, string fName, int qrcSize, int imgResolution, bool fontResize, List<InfoLineText> lineList) : 
            this(qrdata, outpath, fName, qrcSize, imgResolution, fontResize, lineList, qrPosition.none)
        {
        }

        /// <summary>
        /// GenQRCode constructor
        /// </summary>
        /// <param name="qrdata">Data string in qr code</param>
        /// <param name="outpath">Path string where the image will be saved</param>
        /// <param name="fName"></param>
        /// <param name="qrcSize">QR code size in pixels, given as a point struct</param>
        /// <param name="imgResolution">Image resolution in dpi</param>
        /// <param name="fontResize"></param>
        /// <param name="lineList">List of additional text to write on image</param>
        /// <param name="qrPos">Location of the qrcode square on the image, enum for each location</param>
        public GenQRCode(string qrdata, string outpath, string fName, int qrcSize, int imgResolution, bool fontResize, List<InfoLineText> lineList, qrPosition qrPos)
        {
            qrData = qrdata;
            outPath = outpath;
            fileName = fName;
            qrSize = new Size(qrcSize, qrcSize);
            dpiResolution = imgResolution;
            resizeFont = fontResize;
            textLines = lineList;
            if (textLines == null || textLines.Count == 0)
            {
                qrLocation = qrPosition.none;
            }
            else
            {
                qrLocation = qrPos;
            }
            SetTextSize();
        }



        /// <summary>
        /// Remove any invalid character from the filename.
        /// </summary>
        /// <param name="fileName">string to check and strip of invalid characters</param>
        /// <returns>string</returns>
        private static string GetValidFileName(string fName)
        {
            // remove any invalid character from the filename.
            return Regex.Replace(fName.Trim(), "[^A-Za-z0-9-_.]+", "_");
        }

        /// <summary>
        /// Set font size to keep all text inside image width/height.
        /// </summary>
        public void makeQRCode(ErrorCode errorlevel)
        {
            ZXing.QrCode.Internal.ErrorCorrectionLevel EClevel = ZXing.QrCode.Internal.ErrorCorrectionLevel.L;

            _encodeopt = new EncodingOptions
            {
                Height = qrSize.Height,
                Width = qrSize.Width,
                Margin = 1,
                PureBarcode = true
            };
            switch (errorlevel)
            {
                case ErrorCode.low:
                    {
                        EClevel = ZXing.QrCode.Internal.ErrorCorrectionLevel.L;
                        break;
                    }
                case ErrorCode.med:
                    {
                        EClevel = ZXing.QrCode.Internal.ErrorCorrectionLevel.M;
                        break;
                    }
                case ErrorCode.quart:
                    {
                        EClevel = ZXing.QrCode.Internal.ErrorCorrectionLevel.Q;
                        break;
                    }
                case ErrorCode.high:
                    {
                        EClevel = ZXing.QrCode.Internal.ErrorCorrectionLevel.H;
                        break;
                    }
                default:
                    break;
            }
            _encodeopt.Hints.Add(EncodeHintType.ERROR_CORRECTION, EClevel);
            _qrcodegen.Options = _encodeopt;
            _qrcodegen.Format = BarcodeFormat.QR_CODE;
            if (String.IsNullOrEmpty(qrData) != true)
            {
                outfile = _qrcodegen.Write(qrData);
            }

        }

        public string GetFileName(string filename)
        {
            string fileName = GetValidFileName(filename + "_" + qrSize.Width.ToString() + "x" + qrSize.Height.ToString() + "_" + dpiResolution.ToString() +"_"+ _encodeopt.Hints[EncodeHintType.ERROR_CORRECTION] + ".png");
            return fileName;
        }
        public string WriteImage()
        {
            string imgFileName = GetFileName(fileName);
            switch (qrLocation)
            {
                case qrPosition.none:
                    {
                        outfile.SetResolution(dpiResolution, dpiResolution);
                        break;
                    }
                case qrPosition.right:
                    {
                        SetImageSize(imageExtension.Horizontal);
                        imageFile = new Bitmap(imgSize.Width, imgSize.Height);

                        SolidBrush brush = new SolidBrush(Color.Black);
                        SolidBrush bgBrush = new SolidBrush(Color.White);
                        StringFormat format = new StringFormat() { Alignment = StringAlignment.Center };
                        Graphics grImg = Graphics.FromImage(imageFile);
                        grImg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        // filling background with white color
                        grImg.FillRectangle(bgBrush, 0, 0, imageFile.Width, imageFile.Height);
                        // drawing your generated image over new one
                        grImg.DrawImage(outfile, new Point((imageFile.Width - qrSize.Width), (imageFile.Height-qrSize.Height)/2));
                        // drawing text
                        int yOffset = 0;
                        foreach (var textItem in textLines)
                        {
                            grImg.DrawString(textItem.textData, textItem.fontStyle, brush, (imageFile.Width - qrSize.Width)/2, yOffset, format);
                            yOffset += (int)(Math.Round(textItem.pxDimensions.Height,MidpointRounding.AwayFromZero));
                        }
                        imageFile.SetResolution(dpiResolution, dpiResolution);
                        //string imgFileName = GetFileName(fileName);
                        ////string fileName = GetValidFileName(qrData + "_" + qrSize.Width.ToString() + "x" + qrSize.Height.ToString() + "_" + _encodeopt.Hints[EncodeHintType.ERROR_CORRECTION] + ".png");
                        //imageFile.Save(System.IO.Path.Combine(outPath, imgFileName), System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                case qrPosition.top:
                    {
                        SetImageSize(imageExtension.Vertical);
                        imageFile = new Bitmap(imgSize.Width, imgSize.Height);
                        SolidBrush brush = new SolidBrush(Color.Black);
                        SolidBrush bgBrush = new SolidBrush(Color.White);
                        StringFormat format = new StringFormat() { Alignment = StringAlignment.Center };
                        Graphics grImg = Graphics.FromImage(imageFile);
                        grImg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        // filling background with white color
                        grImg.FillRectangle(bgBrush, 0, 0, imageFile.Width, imageFile.Height);
                        // drawing your generated image over new one
                        grImg.DrawImage(outfile, new Point((imgSize.Width - qrSize.Width) / 2, 0));
                        // drawing text
                        int yOffset = qrSize.Height;
                        float yOffsetF = 0;
                        foreach (var textItem in textLines)
                        {
                            grImg.DrawString(textItem.textData, textItem.fontStyle, brush, imageFile.Width / 2, yOffset, format);
                            yOffsetF += textItem.pxDimensions.Height;
                            yOffset = qrSize.Height + (int)(Math.Round(yOffsetF,MidpointRounding.AwayFromZero));

                        }
                        imageFile.SetResolution(dpiResolution, dpiResolution);
                        //string imgFileName = GetFileName(fileName);
                        //imageFile.Save(System.IO.Path.Combine(outPath, imgFileName), System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                case qrPosition.left:
                    {
                        SetImageSize(imageExtension.Horizontal);
                        try
                        {
                            imageFile = new Bitmap(imgSize.Width, imgSize.Height);
                        }
                        catch (Exception errorArg)
                        {
                            MessageBox.Show(errorArg.Message);
                            throw;
                        }
                        SolidBrush brush = new SolidBrush(Color.Black);
                        SolidBrush bgBrush = new SolidBrush(Color.White);
                        StringFormat format = new StringFormat() { Alignment = StringAlignment.Center };
                        Graphics grImg = Graphics.FromImage(imageFile);
                        grImg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        // filling background with white color
                        grImg.FillRectangle(bgBrush, 0, 0, imageFile.Width, imageFile.Height);
                        // drawing your generated image over new one
                        grImg.DrawImage(outfile, new Point(0, (imageFile.Height - qrSize.Height) / 2));
                        // drawing text
                        int yOffset = 0;
                        foreach (var textItem in textLines)
                        {
                            grImg.DrawString(textItem.textData, textItem.fontStyle, brush, (imageFile.Width + qrSize.Width) / 2, yOffset, format);
                            yOffset += (int)(Math.Round(textItem.pxDimensions.Height,MidpointRounding.AwayFromZero));
                        }
                        imageFile.SetResolution(dpiResolution, dpiResolution);
                        //string imgFileName = GetFileName(fileName);
                        ////string fileName = GetValidFileName(qrData + "_" + qrSize.Width.ToString() + "x" + qrSize.Height.ToString() + "_" + _encodeopt.Hints[EncodeHintType.ERROR_CORRECTION] + ".png");
                        //imageFile.Save(System.IO.Path.Combine(outPath, imgFileName), System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                case qrPosition.bottom:
                    {
                        SetImageSize(imageExtension.Vertical);
                        imageFile = new Bitmap(imgSize.Width, imgSize.Height);
                        SolidBrush brush = new SolidBrush(Color.Black);
                        SolidBrush bgBrush = new SolidBrush(Color.White);
                        StringFormat format = new StringFormat() { Alignment = StringAlignment.Center };
                        Graphics grImg = Graphics.FromImage(imageFile);
                        grImg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        // filling background with white color
                        grImg.FillRectangle(bgBrush, 0, 0, imageFile.Width, imageFile.Height);
                        // drawing your generated image over new one
                        grImg.DrawImage(outfile, new Point((imgSize.Width - qrSize.Width)/2, (imgSize.Height - qrSize.Height)));
                        // drawing text
                        int yOffset = 0;
                        float yOffsetF = 0;
                        foreach (var textItem in textLines)
                        {
                            grImg.DrawString(textItem.textData, textItem.fontStyle, brush, imageFile.Width / 2, yOffset, format);
                            yOffsetF += textItem.pxDimensions.Height;
                            yOffset = (int)(Math.Round(yOffsetF,MidpointRounding.AwayFromZero));
                        }
                        imageFile.SetResolution(dpiResolution, dpiResolution);
                        //string imgFileName = GetFileName(fileName);
                        ////string fileName = GetValidFileName(qrData + "_" + qrSize.Width.ToString() + "x" + qrSize.Height.ToString() + "_" + _encodeopt.Hints[EncodeHintType.ERROR_CORRECTION] + ".png");
                        //imageFile.Save(System.IO.Path.Combine(outPath, imgFileName), System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                case qrPosition.center:
                    {
                        SetImageSize(imageExtension.Vertical);
                        imageFile = new Bitmap(imgSize.Width, imgSize.Height);
                        SolidBrush brush = new SolidBrush(Color.Black);
                        SolidBrush bgBrush = new SolidBrush(Color.White);
                        StringFormat format = new StringFormat() { Alignment = StringAlignment.Center };
                        Graphics grImg = Graphics.FromImage(imageFile);
                        grImg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        // filling background with white color
                        grImg.FillRectangle(bgBrush, 0, 0, imageFile.Width, imageFile.Height);

                        float yOffsetTopMaxF = 0;
                        int topLines = (int)(Math.Floor(textLines.Count / 2.0));
                        for (int i = 0; i < topLines; i++) {
                            yOffsetTopMaxF += textLines[i].pxDimensions.Height;
                        }
                        // drawing your generated image over new one
                        grImg.DrawImage(outfile, new Point((imgSize.Width - qrSize.Width) / 2, (int)yOffsetTopMaxF));
                        
                        int yOffset = 0;

                        // drawing text
                        for (int i = 0; i < topLines; i++)
                        {
                            grImg.DrawString(textLines[i].textData, textLines[i].fontStyle, brush, imageFile.Width / 2, yOffset, format);
                            yOffset += (int)Math.Round(textLines[i].pxDimensions.Height,MidpointRounding.AwayFromZero);
                        }
                        yOffset = imageFile.Height;
                        for (int i = textLines.Count - 1; i >= topLines; i--)
                        {
                            yOffset -= (int)Math.Round(textLines[i].pxDimensions.Height,MidpointRounding.AwayFromZero);
                            grImg.DrawString(textLines[i].textData, textLines[i].fontStyle, brush, imageFile.Width / 2, yOffset, format);
                        }
                        imageFile.SetResolution(dpiResolution, dpiResolution);
                        //string imgFileName = GetFileName(fileName);
                        ////string fileName = GetValidFileName(qrData + "_" + qrSize.Width.ToString() + "x" + qrSize.Height.ToString() + "_" + _encodeopt.Hints[EncodeHintType.ERROR_CORRECTION] + ".png");
                        //imageFile.Save(System.IO.Path.Combine(outPath, imgFileName), System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    }
                default:
                    break;

            }
            if(qrLocation == qrPosition.none)
            {
                outfile.Save(System.IO.Path.Combine(outPath, imgFileName), System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                imageFile.Save(System.IO.Path.Combine(outPath, imgFileName), System.Drawing.Imaging.ImageFormat.Png); 
            }
            return System.IO.Path.Combine(outPath, imgFileName);

        }

    }
}