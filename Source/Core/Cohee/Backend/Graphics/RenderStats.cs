﻿namespace Cohee.Backend
{
    public class RenderStats
    {
        private int drawcalls = 0;

        public int DrawCalls
        {
            get { return this.drawcalls; }
            set { this.drawcalls = value; }
        }

        public void Reset()
        {
            this.drawcalls = 0;
        }
    }
}
