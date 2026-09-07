using Cardio.Util;
using System.Windows.Threading;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Contains("--four-cores"))
        {
            using var process = System.Diagnostics.Process.GetCurrentProcess();
            long available = process.ProcessorAffinity.ToInt64();
            long mask = 0;
            int selected = 0;
            for (int i = 0; i < 64 && selected < 4; i++)
            {
                long bit = 1L << i;
                if ((available & bit) == 0) continue;
                mask |= bit;
                ++selected;
            }
            process.ProcessorAffinity = (nint)mask;
            Console.WriteLine($"测试进程限制为 {selected} 个逻辑处理器。");
        }
        var dispatcher = Dispatcher.CurrentDispatcher;
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(dispatcher));
        Task checks = CheckAsync();
        _ = checks.ContinueWith(_ => dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal));
        Dispatcher.Run();
        try { checks.GetAwaiter().GetResult(); return 0; }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static async Task CheckAsync()
    {
        int uiThread = Environment.CurrentManagedThreadId;
        var search = new LatestSearch();
        using var release = new ManualResetEventSlim();
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var applied = new List<int>();
        var busy = new List<bool>();
        int calls = 0;
        void Apply(int value)
        {
            Assert(Environment.CurrentManagedThreadId == uiThread, "结果必须回写到 UI 线程");
            applied.Add(value);
        }
        Task first = search.RunAsync(() =>
        {
            Interlocked.Increment(ref calls);
            started.SetResult();
            if (!release.Wait(TimeSpan.FromSeconds(5))) throw new TimeoutException();
            return 0;
        }, Apply, busy.Add);
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        for (int i = 1; i <= 100; i++)
        {
            int value = i;
            await search.RunAsync(() => { Interlocked.Increment(ref calls); return value; }, Apply, busy.Add);
        }
        Assert(calls == 1, "慢查询未结束前不能并行启动其他查询");
        release.Set();
        await first.WaitAsync(TimeSpan.FromSeconds(5));
        Assert(calls == 2 && applied.SequenceEqual(new[] { 100 }), "连续请求必须合并且只应用最新结果");
        Assert(busy.SequenceEqual(new[] { true, false }), "忙状态不能提前结束");

        release.Reset();
        Task invalidated = search.RunAsync(() =>
        {
            if (!release.Wait(TimeSpan.FromSeconds(5))) throw new TimeoutException();
            return 101;
        }, Apply, busy.Add);
        await search.RunAsync(() => 102, Apply, busy.Add);
        search.Invalidate();
        release.Set();
        await invalidated.WaitAsync(TimeSpan.FromSeconds(5));
        Assert(applied.SequenceEqual(new[] { 100 }), "离开页面后不得回写或执行待处理查询");
        await search.RunAsync<int>(() => throw new InvalidOperationException("模拟查询失败"), Apply, busy.Add);
        await search.RunAsync(() => 103, Apply, busy.Add);
        Assert(applied.Last() == 103 && !busy.Last() && LogUtil.Errors == 1, "异常后必须能继续查询并恢复忙状态");

        var display = new List<double>();
        int refreshes = 0;
        var waveform = new LiveWaveform(display, () =>
        {
            Assert(Environment.CurrentManagedThreadId == uiThread, "绘图必须发生在 UI 线程");
            ++refreshes;
        });
        await Task.Run(() => { for (int i = 0; i < 20000; i++) waveform.Add(i); });
        await Task.Delay(250);
        Assert(display.Count > 0 && display.Count <= 6000 && display.Last() == 19999, "高频采样必须有界且保留最新采样");
        Assert(refreshes > 0 && refreshes <= 3, "高频采样不得逐点刷新");
        waveform.Replace(new double[] { 7, 8, 9 });
        await Task.Delay(200);
        Assert(display.SequenceEqual(new double[] { 7, 8, 9 }), "完整波形必须批量替换");
        waveform.Dispose();
        int stopped = refreshes;
        waveform.Add(10);
        await Task.Delay(200);
        Assert(refreshes == stopped, "释放后不得继续绘图");
        Console.WriteLine("PASS: 请求合并、串行执行、UI 回写、失效结果、异常恢复、波形限频/有界缓存/释放。");
    }
}

namespace Cardio.Util
{
    internal static class LogUtil
    {
        public static int Errors;
        public static void Error(string category, string message) => ++Errors;
    }
}
