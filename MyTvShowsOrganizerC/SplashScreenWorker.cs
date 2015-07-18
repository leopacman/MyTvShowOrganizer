using System;
using System.Threading;

namespace MyTvShowsOrganizer
{
    class SplashScreenWorker
    {

        public event EventHandler<HardWorkerEventArgs> ProgressChanged;
        public event EventHandler HardWorkDone;

        public void DoHardWork()
        {

            for (int i = 1; i < 101; i++)
            {
                Thread.Sleep(20);
                this.OnProgressChanged(i);
            }
            Thread.Sleep(500);
            this.OnHardWorkDone();
        }

        private void OnProgressChanged(int progress)
        {
            var handler = this.ProgressChanged;
            if (handler != null)
            {
                handler(this, new HardWorkerEventArgs(progress));
            }
        }

        private void OnHardWorkDone()
        {
            var handler = this.HardWorkDone;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    public class HardWorkerEventArgs : EventArgs
    {
        public HardWorkerEventArgs(int progress)
        {
            this.Progress = progress;
        }
        public int Progress
        {
            get;
            private set;
        }
    }
}
