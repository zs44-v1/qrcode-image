using System;
using System.Drawing;


namespace GenQRCode
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GenQRCode
    {
        /// <summary>
        /// 
        /// </summary>
        private void SetTextSize()
        {
            switch (qrLocation)
            {
                case qrPosition.none:
                    break;
                case qrPosition.right:
                    {
                        SetFontSize(imageExtension.Horizontal);
                        break;
                    }
                case qrPosition.top:
                    {
                        SetFontSize(imageExtension.Vertical);
                        break;
                    }
                case qrPosition.left:
                    {
                        SetFontSize(imageExtension.Horizontal);
                        break;
                    }
                case qrPosition.bottom:
                    {
                        SetFontSize(imageExtension.Vertical);
                        break;
                    }
                case qrPosition.center:
                    {
                        SetFontSize(imageExtension.Vertical);
                        break;
                    }
                default:
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flow"></param>
        private void SetFontSize(imageExtension flow)
        {
            float _fontSize;
            Bitmap _dummyImg;
            Graphics _dummyG;

            _dummyImg = new Bitmap(1, 1);
            _dummyG = Graphics.FromImage(_dummyImg);

            if (resizeFont)
            {
                switch (flow)
                {
                    case imageExtension.Horizontal:
                        {
                            float maxTextHeight = 0;
                            foreach (var textItem in textLines)
                            {
                                SizeF _stringSize = _dummyG.MeasureString(textItem.textData, textItem.fontStyle);
                                maxTextHeight += _stringSize.Height;
                                textItem.pxDimensions = _stringSize;
                            }
                            while (maxTextHeight > qrSize.Height)
                            {
                                maxTextHeight = 0;
                                foreach (var textItem in textLines)
                                {
                                    _fontSize = textItem.fontStyle.Size;
                                    _fontSize -= (float)0.1;
                                    if (_fontSize < 0.5)
                                        throw new ArgumentException("Too many text lines to render with " + qrSize.Height + "px QR-Code" + "\n Cannot use a smaller font size\n");

                                    textItem.fontStyle = new Font(textItem.fontStyle.FontFamily, _fontSize, textItem.fontStyle.Style);

                                    SizeF _stringSize = _dummyG.MeasureString(textItem.textData, textItem.fontStyle);
                                    textItem.pxDimensions = _stringSize;
                                    maxTextHeight += _stringSize.Height;
                                }
                            }
                        }
                        break;
                    case imageExtension.Vertical:
                        foreach (var textItem in textLines)
                        {
                            _fontSize = textItem.fontStyle.Size;
                            SizeF _stringSize = _dummyG.MeasureString(textItem.textData, textItem.fontStyle);
                            while (_stringSize.Width > qrSize.Width)
                            {
                                _fontSize -= (float)0.1;
                                if (_fontSize < 0.5)
                                {
                                    throw new ArgumentException("Text line too long to render with " + qrSize.Height + "px QR-Code" + "\n Cannot use a smaller font size\n");
                                }
                                textItem.fontStyle = new Font(textItem.fontStyle.FontFamily, _fontSize, textItem.fontStyle.Style);
                                _stringSize = _dummyG.MeasureString(textItem.textData, textItem.fontStyle);
                            }
                            textItem.pxDimensions = _stringSize;
                        }
                        break;
                    default:
                        break;
                } 
            }
            else
        	{
                foreach (var textItem in textLines)
                {
                    SizeF _stringSize = _dummyG.MeasureString(textItem.textData, textItem.fontStyle);
                    textItem.pxDimensions = _stringSize;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tFlow"></param>
        private void SetImageSize(imageExtension tFlow)
        {
            switch (tFlow)
            {
                #region imageExtensionVertical
                case imageExtension.Vertical:
                    {
                        float maxHeight = 0;
                        Bitmap _dummyImg = new Bitmap(1, 1);
                        Graphics _dummyG = Graphics.FromImage(_dummyImg);
                        Size temp = qrSize;
                        foreach (var textItem in textLines)
                        {
                            if (String.IsNullOrEmpty(textItem.textData) == false)
                            {
                                SizeF measure = _dummyG.MeasureString(textItem.textData, textItem.fontStyle);
                                maxHeight += measure.Height;
                                if (temp.Width < (int)measure.Width)
                                {
                                    temp.Width = (int)(measure.Width);
                                }
                            }
                        }
                        temp.Height += (int)(Math.Round((maxHeight / imgStep), MidpointRounding.AwayFromZero) * imgStep);
                        if (resizeFont)
                            temp.Width = qrSize.Width;
                        imgSize = temp;

                        break;
                    }
                #endregion
                #region imageExtensionHorizontal
                case imageExtension.Horizontal:
                    {
                        float totalHeight = 0;
                        float maxWidth = 0;
                        Bitmap _dummyImg = new Bitmap(1, 1);
                        Graphics _dummyG = Graphics.FromImage(_dummyImg);

                        Size temp = qrSize;
                        foreach (var textItem in textLines)
                        {
                            if (String.IsNullOrEmpty(textItem.textData) == false)
                            {
                                //tempWidth = _dummyG.MeasureString(textItem.textData, textItem.fontStyle).Width;
                                //if (tempWidth > maxWidth)
                                //{
                                //    maxWidth = tempWidth;
                                //}
                                SizeF measure = _dummyG.MeasureString(textItem.textData, textItem.fontStyle);
                                if (measure.Width > maxWidth)
                                {
                                    maxWidth = measure.Width;
                                }
                                totalHeight += measure.Height;
                            }
                        }
                        temp.Width += (int)(Math.Round((maxWidth / imgStep), MidpointRounding.AwayFromZero) * imgStep);
                        if (totalHeight > temp.Height)
                        {
                            temp.Height = (int)(Math.Round((totalHeight / imgStep), MidpointRounding.AwayFromZero) * imgStep);
                        }
                        imgSize = temp;
                        break;
                    }

                #endregion
                default:
                    break;
            }
        }    
    }
}
