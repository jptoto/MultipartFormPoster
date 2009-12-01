using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace ThirtyPoints.MultiFormPoster {
    public static class FormUpload {
        private static readonly Encoding encoding = Encoding.UTF8;
       
        
        /// <summary>
        /// Call the multi part form poster without any auth
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="postParameters"></param>
        /// <returns></returns>
        public static HttpWebResponse MultipartFormDataPost(string postUrl, Dictionary<string, object> postParameters) {
            string formDataBoundary = "-----------------------------28947758029299";
            string jp;
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);
            return PostForm(postUrl, contentType, formData, null, null);
        }

        /// <summary>
        /// Call the multi part form poster with HTTP Auth params
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="httpAuthUser"></param>
        /// <param name="httpAuthPassword"></param>
        /// <param name="postParameters"></param>
        /// <returns></returns>
        public static HttpWebResponse MultipartFormDataPost(string postUrl, string httpAuthUser, string httpAuthPassword, Dictionary<string, object> postParameters) {
            string formDataBoundary = "-----------------------------28947758029299";
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);
            return PostForm(postUrl, contentType, formData, httpAuthUser, httpAuthPassword);
        }




        private static HttpWebResponse PostForm(string postUrl, string contentType, byte[] formData, string httpAuthUser, string httpAuthPassword) {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null) {
                throw new NullReferenceException("Request is not a valid http request.");
            }

            // Set up the request properties
            request.Method = "POST";
            request.ContentType = contentType;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;  // We need to count how many bytes we're sending.
            
            // If we need to support httpauth we add that on here
            if (httpAuthUser != null && httpAuthPassword != null)
            {
                request.Credentials = new NetworkCredential(httpAuthUser, httpAuthPassword);
                request.PreAuthenticate = true;
            }

            using (Stream requestStream = request.GetRequestStream()) {
                // Push it out there
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }
            
            return request.GetResponse() as HttpWebResponse;
            
        }


        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary) {
            Stream formDataStream = new System.IO.MemoryStream();

            foreach (var param in postParameters) {
                if (param.Value is FileParameter) {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, header.Length);

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                    string lineEnding = "\r\n";
                    formDataStream.Write(encoding.GetBytes(lineEnding), 0, lineEnding.Length);

                } else {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, postData.Length);
                }
            }

            // Add the end of the request
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, footer.Length);

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
    }
}
