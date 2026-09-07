using System.Windows.Threading;

namespace Cardio.Util
{
    /// <summary>采集线程只写缓存；UI 每 100ms 批量更新一次，避免跨线程修改绘图集合。</summary>
    internal sealed class LiveWaveform : IDisposable
    {
        private readonly object sync = new();
        private readonly List<double> samples = new();
        private readonly List<double> display;
        private readonly Action refresh;
        private readonly DispatcherTimer timer;
        private bool dirty;
        private bool disposed;

        public LiveWaveform(List<double> display, Action refresh)
        {
            this.display = display;
            this.refresh = refresh;
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Tick += Tick;
            timer.Start();
        }

        public int Count { get { lock (sync) return samples.Count; } }

        public void Add(double value)
        {
            lock (sync)
            {
                if (disposed) return;
                if (samples.Count >= 6000) samples.Clear();
                samples.Add(value);
                dirty = true;
            }
        }

        public void Clear()
        {
            lock (sync) { samples.Clear(); dirty = true; }
        }

        public void Replace(IEnumerable<double> values)
        {
            lock (sync)
            {
                if (disposed) return;
                samples.Clear();
                samples.AddRange(values);
                dirty = true;
            }
        }

        private void Tick(object? sender, EventArgs e)
        {
            double[] snapshot;
            lock (sync)
            {
                if (!dirty || disposed) return;
                snapshot = samples.ToArray();
                dirty = false;
            }
            display.Clear();
            display.AddRange(snapshot);
            refresh();
        }

        public void Dispose()
        {
            timer.Stop();
            timer.Tick -= Tick;
            lock (sync) { disposed = true; samples.Clear(); }
        }
    }
}
