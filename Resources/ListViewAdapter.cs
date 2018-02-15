using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidSQLite.Resources.Model;
using Java.Lang;

namespace AndroidSQLite.Resources
{
    public class ViewHolder : Java.Lang.Object
    {
        public TextView coord { get; set; }
        public TextView json { get; set; }
    }
    public class ListViewAdapter:BaseAdapter
    {
        private Activity activity;
        private List<Report> lstReport;
        public ListViewAdapter(Activity activity, List<Report> lstReport)
        {
            this.activity = activity;
            this.lstReport = lstReport;
        }

        public override int Count
        {
            get
            {
                return lstReport.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return lstReport[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.list_view_dataTemplate, parent, false);

            var txt1 = view.FindViewById<TextView>(Resource.Id.textCoord);
            var txt2 = view.FindViewById<TextView>(Resource.Id.textJson);

            txt1.Text = lstReport[position].Latitude+','+ lstReport[position].Longitude;
            txt2.Text = lstReport[position].Json;

            return view;
        }
    }
}