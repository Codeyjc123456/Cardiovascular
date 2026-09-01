using Cardio.DAL;
using Cardio.Model;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cardio.Views.DoctorPage
{
    /// <summary>
    /// RegisterDoctor.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterDoctor
    {
        LoginViewModel loginmodel = new LoginViewModel();
        DoctorInfoEntity entity = null;
        List<DoctorInfoEntity> entitylist = new List<DoctorInfoEntity>();
        DoctorInfoDAL doctorDAL = null;
        private readonly Action tabAction;

        public RegisterDoctor(Action tAction)
        {
            InitializeComponent();
            DataContext = loginmodel;
            tabAction = tAction;
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            doctorDAL = DoctorInfoDAL.getInstance();
            entity = new DoctorInfoEntity();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;//判断当前界面是否可见
            if (!isVisible) GC.Collect();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            loginmodel?.CloseAction();
            tabAction.Invoke();
        }

        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            if (loginmodel.RemoveAndAdd > 0)
            {
                Growl.Warning("信息校验失败，请修改", "RigisterDoctor");
                return;
            }
            string conditon = "DoctorName= '" + loginmodel.UserID.Trim() + "'";
            entitylist = doctorDAL.Finds(conditon);
            if (entitylist != null && entitylist.Count > 0)
            {
                Growl.Info("当前医师账号已被使用！", "RigisterDoctor");
                return;
            }
            entity.DoctorName = loginmodel.UserID;
            entity.DoctorPwd = loginmodel.UserPWD;
            if (doctorDAL.Insert(entity) > 0)
            {
                Growl.Success("医师注册成功！", "RigisterDoctor");
            }
        }
    }
}
