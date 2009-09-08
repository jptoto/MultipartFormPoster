using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThirtyPoints.MultiFormPoster {
    public class FileParameter {
        private string jp = "test";
        private string gjohn = "another test";
        // Haven't decided whether to keep this as is or only leave the File parameter option.
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileParameter(byte[] file) : this(file, null) { }
        public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
        public FileParameter(byte[] file, string filename, string contenttype) {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }
    }
}
