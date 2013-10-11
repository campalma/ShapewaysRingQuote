/*
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;


namespace iMaterialseSimpleUploadClient
{
    public class SimpleUploadClient
    {
        string _uploadUrl = "http://i.materialise.com/upload";
        MultipartFormDataContent _uploadContent = new MultipartFormDataContent();
        Guid _productTypeID;

        public SimpleUploadClient(Guid productTypeID)
        {
            _productTypeID = productTypeID;
        }

        public string Upload(string filePath, Guid? materialID = null, Guid? colorID = null)
        {
            if (!File.Exists(filePath))
            {
                throw new IOException("Provided file wasn't found.");
            }

            string uri;
            using (var client = CreateHttpClient())
            {
                var uploadContent = new MultipartFormDataContent();

                RegisterProductType(client, uploadContent, _productTypeID);
                RegisterModelFile(client, uploadContent, filePath);

                if (materialID.HasValue)
                {
                    RegisterMaterial(client, uploadContent, materialID.Value);
                    if (colorID.HasValue)
                    {
                        RegisterColor(client, uploadContent, colorID.Value);
                    }
                }

                uri = RequestRedirectionUrl(client, uploadContent);
            }
            return uri;

        }

        private string RequestRedirectionUrl(HttpClient client, HttpContent content)
        {
            var response = client.PostAsync(_uploadUrl, content).Result;
            return response.Headers.Location.ToString();
        }

        private void RegisterProductType(HttpClient client, MultipartFormDataContent uploadContent, Guid productTypeID)
        {
            var content = new StringContent(productTypeID.ToString());
            content.Headers.ContentType = null;

            uploadContent.Add(content, "\"plugin\"");
        }

        private void RegisterColor(HttpClient client, MultipartFormDataContent uploadContent, Guid colorID)
        {
            var content = new StringContent(colorID.ToString());
            content.Headers.ContentType = null;

            uploadContent.Add(content, "\"colourID\"");
        }

        private void RegisterMaterial(HttpClient client, MultipartFormDataContent uploadContent, Guid materialID)
        {
            var content = new StringContent(materialID.ToString());
            content.Headers.ContentType = null;

            uploadContent.Add(content, "\"materialID\"");
        }

        private void RegisterModelFile(HttpClient client, MultipartFormDataContent uploadContent, string filePath)
        {
            var fileStream  = File.OpenRead(filePath);
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

            uploadContent.Add(streamContent, "\"file\"");
            streamContent.Headers.ContentDisposition.FileName = "\"" + Path.GetFileName(filePath) + "\"";
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient(
                new HttpClientHandler
                {
                    //...disabling autoredirects
                    AllowAutoRedirect = false
                });

            return httpClient;
        }
    }
}

*/