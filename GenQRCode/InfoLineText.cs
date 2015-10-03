using System.Drawing;

namespace GenQRCode
{
    public class InfoLineText
    {
        private string _textData;
        private Font _fontstyle;
        private SizeF _pixelDimensions;

        /// <summary>
        /// InfoLineText constructor
        /// </summary>
        /// <param name="text">string</param>
        /// <param name="fontstyle">font</param>
        public InfoLineText(string text, Font fontstyle)
        {
            textData = text;
            fontStyle = fontstyle;
        }
        /// <summary>
        /// InfoLineText constructor
        /// </summary>
        /// <param name="text">string</param>
        public InfoLineText(string text)
        {
            textData = text;
            fontStyle = new Font(FontFamily.GenericSansSerif, 60, GraphicsUnit.Point);
        }
        /// <summary>
        /// Text string
        /// </summary>
        public string textData
        {
            get
            {
                return _textData;
            }
            set
            {
                _textData = value;
            }
        }
        
        /// <summary>
        /// Font style
        /// </summary>
        public Font fontStyle
        {
            get
            {
                return _fontstyle;
            }
            set
            {
                _fontstyle = value;
            }
        }
        /// <summary>
        /// Text string pixel dimensions
        /// </summary>
        public SizeF pxDimensions
        {
            get
            {
                return _pixelDimensions;
            }
            internal set
            {
                _pixelDimensions = value;
            }
        }
    }
}

