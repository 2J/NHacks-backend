using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace nhacks.Services
{
    public class Kairos
    {
        const string AppId = "37fb60a3";//TODO: get this from app settings
        const string AppKey = "9c8346c689cfe6f40b06e86538ae7d6d";

        public HttpResponseMessage Enroll(string imgData, string subjectId, string galleryName, string selector = "FACE", string symmetricFill = "true")
        {
            var request = new HttpClient();
            var values = new Dictionary<string, string>();
            values.Add("image", imgData);
            values.Add("subject_id", subjectId);
            values.Add("gallery_name", galleryName);
            values.Add("selector", selector);
            values.Add("symmetricFill", symmetricFill);

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");
            request.DefaultRequestHeaders.Add("app_id", AppId);
            request.DefaultRequestHeaders.Add("app_key", AppKey);
            var result = request.PostAsync("https://api.kairos.com/enroll", content).Result;
            return result;
        }

        public HttpResponseMessage Recognize(string imgData, string galleryName) //TODO: test
        {
            var request = new HttpClient();
            var values = new Dictionary<string, string>();
            values.Add("image", imgData);
            values.Add("gallery_name", galleryName);

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");
            request.DefaultRequestHeaders.Add("app_id", AppId);
            request.DefaultRequestHeaders.Add("app_key", AppKey);
            var result = request.PostAsync("https://api.kairos.com/recognize", content).Result;
            return result;
        }

        public HttpResponseMessage Detect(string imgData, string selector = "FACE")
        {
            var request = new HttpClient();
            var values = new Dictionary<string, string>();
            values.Add("image", imgData);
            values.Add("selector", selector);

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");
            request.DefaultRequestHeaders.Add("app_id", AppId);
            request.DefaultRequestHeaders.Add("app_key", AppKey);
            var result = request.PostAsync("https://api.kairos.com/detect", content).Result;
            return result;
        }

        public HttpResponseMessage GalleryListAll()
        {
            var request = new HttpClient();
            var values = new Dictionary<string, string>();
            var content = new FormUrlEncodedContent(values); //do not need this
            request.DefaultRequestHeaders.Add("app_id", AppId);
            request.DefaultRequestHeaders.Add("app_key", AppKey);
            var result = request.PostAsync("https://api.kairos.com/gallery/list_all", content).Result;
            return result;
        }

        public HttpResponseMessage GalleryView(string galleryName)
        {
            var request = new HttpClient();
            var values = new Dictionary<string, string>();
            values.Add("gallery_name", galleryName);

            var content = new FormUrlEncodedContent(values);
            request.DefaultRequestHeaders.Add("app_id", AppId);
            request.DefaultRequestHeaders.Add("app_key", AppKey);
            var result = request.PostAsync("https://api.kairos.com/gallery/view", content).Result;
            return result;
        }

        public HttpResponseMessage GalleryRemove(string galleryName)
        {
            var request = new HttpClient();
            var values = new Dictionary<string, string>();
            values.Add("gallery_name", galleryName);

            var content = new FormUrlEncodedContent(values);
            request.DefaultRequestHeaders.Add("app_id", AppId);
            request.DefaultRequestHeaders.Add("app_key", AppKey);
            var result = request.PostAsync("https://api.kairos.com/gallery/remove", content).Result;
            return result;
        }

        public HttpResponseMessage GalleryRemoveSubject(string galleryName, string subjectId) //TODO
        {
            var request = new HttpClient();
            var values = new Dictionary<string, string>();
            values.Add("gallery_name", galleryName);
            values.Add("subject_id", subjectId);

            var content = new FormUrlEncodedContent(values);
            request.DefaultRequestHeaders.Add("app_id", AppId);
            request.DefaultRequestHeaders.Add("app_key", AppKey);
            var result = request.PostAsync("https://api.kairos.com/gallery/remove_subject", content).Result;
            return result;
        }
    }
}
