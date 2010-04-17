# MultiFormPoster is a .NET 3.5 class library for creating form POSTs  

## License: Apache License 2.0  

### Key Features

* Multi-Part form posts
* Basic HTTP Auth

### Basic Usage
	// If you're going to be uploading a file, you'll need to first read it in as a byte array
	FileStream fs = new FileStream("c:\\test.jpg", FileMode.Open, FileAccess.Read);
	byte[] data = new byte[fs.Length];
	fs.Read(data, 0, data.Length);
	fs.Close();

	// Generate post objects
	Dictionary<string, object> postParameters = new Dictionary<string, object>();

	// Adding a file as the post parameter Be sure to add the content-type as well
	postParameters.Add("file", new FileParameter(data, "test.jpg", "image/jpeg"));

	// Adding a normal name/value post parameter
	postParameters.Add("name", "FileName");

	// Create request and receive response
	string postURL = "http://linktotheplaceyouwanttopost.com/link.html";

	// Create an HttpWebResponse object and use either the MultipartFormDataPost WITH Basic HttpAuth params or without
	HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, "UserName", "Password", postParameters);

	// Process response
	StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
	string fullResponse = responseReader.ReadToEnd();
	webResponse.Close();
	// Do something with fullResponse. This may contain xml, json, or whatever the API you're posting to is using.
 
### Todo:
* Add ActiveDirectry Auth
* Add cookie container (some sites authorize you based on the cookie they already dropped)

### Credits:
	This class is based heavily on a blog post by Brian Grinstead which you may find here:
	[http://www.briangrinstead.com/blog/multipart-form-post-in-c][1]

	I've taken his example and blown it out a bit to add some functionality such as basic http auth functions.
	
   [1]: http://www.briangrinstead.com/blog/multipart-form-post-in-c