using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenQRCode
{
    class cFileNameStatus
    {
        private string _fileName;

        public string fileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private bool _autoFileName;

        public bool autoFileName
        {
            get { return _autoFileName; }
            set { _autoFileName = value; }
        }

        private bool _imgParameters;

        public bool imgParameters
        {
            get { return _imgParameters; }
            set { _imgParameters = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public cFileNameStatus():this(null,true)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fname"></param>
        public cFileNameStatus(string fname):this (fname, false)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="autoName"></param>
        public cFileNameStatus(string fname, bool autoName):this(fname, autoName,false)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="autoName"></param>
        /// <param name="imgParam"></param>
        public cFileNameStatus(string fname, bool autoName, bool imgParam)
        {
            fileName = fname;
            autoFileName = autoName;
            imgParameters = imgParam;
        }

    }
}
