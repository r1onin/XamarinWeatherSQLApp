using Android.App;
using Android.Widget;
using Android.OS;
using System.Json;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;
using AndroidSQLite.Resources.Model;
using AndroidSQLite.Resources.DataHelper;
using AndroidSQLite.Resources;
using Android.Util;

namespace AndroidSQLite
{
    [Activity(Label = "AndroidSQLite", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        ListView lstData;
        List<Report> lstSource = new List<Report>();
        DataBase db;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Create DataBase
            db = new DataBase();
            db.createDataBase();
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Log.Info("DB_PATH", folder);

            lstData = FindViewById<ListView>(Resource.Id.listView);

            var edtLat = FindViewById<EditText>(Resource.Id.edtLat);
            var edtLong = FindViewById<EditText>(Resource.Id.edtLong);

            var btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            var btnEdit = FindViewById<Button>(Resource.Id.btnEdit);
            var btnDelete = FindViewById<Button>(Resource.Id.btnDelete);

            //LoadData
            LoadData();

            //Event
            btnAdd.Click += async delegate
            {
                Report report = new Report()
                {
                    Latitude = edtLat.Text,
                    Longitude = edtLong.Text,
                    Json = (await FetchWeatherAsync("http://api.geonames.org/findNearByWeatherJSON?lat=" +
                             edtLat.Text +
                             "&lng=" +
                             edtLong.Text +
                             "&username=onin")).ToString()
                };
                db.insertIntoTableReport(report);
                LoadData();
            };

            btnEdit.Click += async delegate {
                Report report = new Report()
                {
                    Id=int.Parse(edtLat.Tag.ToString()),
                    Latitude = edtLat.Text,
                    Longitude = edtLong.Text,
                    Json = (await FetchWeatherAsync("http://api.geonames.org/findNearByWeatherJSON?lat=" +
                             edtLat.Text +
                             "&lng=" +
                             edtLong.Text +
                             "&username=onin")).ToString()
                };
                db.updateTableReport(report);
                LoadData();
            };

            btnDelete.Click += delegate {
                Report report = new Report()
                {
                    Id = int.Parse(edtLat.Tag.ToString()),
                    Latitude = edtLat.Text,
                    Longitude = edtLong.Text
                };
                db.deleteTableReport(report);
                LoadData();
            };

            lstData.ItemClick += (s,e) =>{
                //Set background for selected item
                for(int i = 0; i < lstData.Count; i++)
                {
                    if (e.Position == i)
                        lstData.GetChildAt(i).SetBackgroundColor(Android.Graphics.Color.DarkGray);
                    else
                        lstData.GetChildAt(i).SetBackgroundColor(Android.Graphics.Color.Transparent);

                }

                //Binding Data
                var txtCoord = e.View.FindViewById<TextView>(Resource.Id.textCoord);
                var txtJson = e.View.FindViewById<TextView>(Resource.Id.textJson);

                edtLat.Text = txtCoord.Text.Split(',')[0];
                edtLat.Tag = e.Id;
                edtLong.Text = txtCoord.Text.Split(',')[1];
                
                

            };

        }

        private void LoadData()
        {
            lstSource = db.selectTableReport();
            var adapter = new ListViewAdapter(this, lstSource);
            lstData.Adapter = adapter;
        }

        // Gets weather data from the passed URL.
        private async Task<JsonValue> FetchWeatherAsync(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                    // Return the JSON document:
                    return jsonDoc;
                }
            }
        }
    }
}