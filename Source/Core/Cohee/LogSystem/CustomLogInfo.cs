﻿namespace Cohee
{
    public abstract class CustomLogInfo
    {
        private string name;
        private string id;

        public string Name
        {
            get { return this.name; }
        }
        public string Id
        {
            get { return this.id; }
        }

        public CustomLogInfo(string name, string id)
        {
            this.name = name;
            this.id = id;
        }
    }
}
