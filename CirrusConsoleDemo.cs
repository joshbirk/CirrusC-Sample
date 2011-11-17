using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Web;
using System.Net;
using System.IO;

namespace CirrusConsoleDemo
{
    class CirrusConsoleDemo
    {
        private const string oauthendpoint = "https://login.salesforce.com/services/oauth2/token";
        private const string oauthoptions = "grant_type=password";

        public string token;
        public string instance_url;
        public Dictionary<string, string> properties;

        public CirrusConsoleDemo() {}

        static void Main(string[] args)
        {
            CirrusConsoleDemo c = new CirrusConsoleDemo();
            c.init();
            c.login();
            c.insertItem();
        }

        public void init()
        {
            properties = new Dictionary<string, string>();
            foreach (String row in File.ReadAllLines("buyerapp.txt"))
            {
                properties.Add(row.Split('=')[0], row.Split('=')[1]);
            }
            Console.Write("Logging in for "+properties["username"]);
        }

        public void login()
        {
            string postData = oauthoptions + "&client_id=" + properties["consumerkey"] + "&client_secret=" + properties["privatekey"] + "&username=" + properties["username"] + "&password=" + properties["password"];
            string responseFromServer = doHTTPRequest(oauthendpoint, postData, "", false);
            string[] data = responseFromServer.Split(':');
            token = data[7];
            token = token.Substring(1, token.Length - 1);
            token = token.Replace("\"}", "");

            instance_url = data[5];
            instance_url = instance_url.Substring(2, instance_url.Length - 2);
            instance_url = instance_url.Substring(0, instance_url.IndexOf("\""));
        }

        public void insertItem()
        {
            string endpoint = "https://" + instance_url + "/services/data/v" + properties["api"] + "/sobjects/Merchandise__c";
            string postData;
            if(properties["merchandise_price"] != "") {
              postData = "{\"Name\" : \"" + properties["merchandise_name"] + "\", \"Price__c\" : \"" + properties["merchandise_price"] + "\"}";
            }
            else
            {
                postData = "{\"Name\" : \"" + properties["merchandise_name"] + "\"}";
            }
            string responseFromServer = doHTTPRequest(endpoint, postData, token, true);
            Console.Write(responseFromServer);
        }

        public string doHTTPRequest(string endpoint, string postData, string token, Boolean isJSON)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            if (isJSON)
            {
                request.ContentType = "application/json";
            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded";
            } 
            if (token != "")
            {
                request.Headers["Authorization"] = "OAuth " + token;
            }

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response;
            try {
                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                return responseFromServer;
            } catch (System.Net.WebException error) {
                response = error.Response;
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();  
                return responseFromServer;  
            }
            
            

            
        }

        

        
    }
}
