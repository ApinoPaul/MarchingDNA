﻿using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace MyCustomPlugins
{
    public class MyCustomPluginsInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "MyCustomPlugins";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("1c0b833b-1a43-4bb7-ac7d-e528f43f4d82");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
