namespace Cardio.Util
{
    /// <summary>
    /// UI 线程调用：查询串行执行，连续请求合并为最新一次，旧结果不回写界面。
    /// </summary>
    internal sealed class LatestSearch
    {
        private Func<Task>? pending;
        private bool running;
        private int version;

        public async Task RunAsync<T>(Func<T> query, Action<T> apply, Action<bool> setBusy)
        {
            int requestVersion = ++version;
            pending = async () =>
            {
                T result = await Task.Run(query);
                if (requestVersion == version)
                    apply(result);
            };
            if (running) return;

            running = true;
            setBusy(true);
            try
            {
                while (pending != null)
                {
                    var next = pending;
                    pending = null;
                    try { await next(); }
                    catch (Exception ex) { LogUtil.Error("列表查询", ex.ToString()); }
                }
            }
            finally
            {
                running = false;
                setBusy(false);
            }
        }

        public void Invalidate()
        {
            ++version;
            pending = null;
        }
    }
}
