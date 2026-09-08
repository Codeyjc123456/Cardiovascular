using System;
using System.IO;
using System.Windows;

namespace Cardiovascular
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 加载 log4net 配置（配置文件已复制到输出目录）
            //try
            //{
            //    string cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            //    if (File.Exists(cfgPath))
            //    {
            //        log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(cfgPath));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("log4net 配置加载失败：" + ex.Message);
            //}
        }
    }

}
