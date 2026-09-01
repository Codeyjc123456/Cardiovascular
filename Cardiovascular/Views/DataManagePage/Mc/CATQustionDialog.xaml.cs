using Cardio.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cardiovascular.Views.DataManagePage.Mc
{
    /// <summary>
    /// CATQustionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CATQustionDialog 
    {
        
        UserViewModel model = new UserViewModel();
        string[,] copdanswer =
        {
            { "从不吸烟","1-15包·年 ","15-30包·年","≥30包·年"},
            {"<18.5kg/㎡ ","18.5～23.9kg/㎡ ","24.0～27.9kg/㎡ ","≥28.0kg/㎡ " },
            {"是","否" ,"",""},
            {"没有气促","在平地急行或爬小坡时感觉气促 ","平地正常行走时感觉气促 ","" },
            {"是","否","","" },
            {"是","否","","" }
        };
        public CATQustionDialog()
        {
            InitializeComponent();
            DataContext = model;
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            //DAL = QuestionnaireDAL.getInstance();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            model.CloseAction();
        }
        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            //entity = new QuestionnaireEntity();
            int selectedIndex = QSTable.SelectedIndex;
            string header = "";
            int sum = 0;
            if (selectedIndex >= 0)
            {
                //通过索引获取tabItem
                TabItem selectedTab = (TabItem)QSTable.Items[selectedIndex];
                header = selectedTab.Header.ToString();
            }
            if (header == "CAT问卷")
            {
                //foreach (int answer in model.Answers)
                //{
                //    if (answer <= 0)
                //    {
                //        HandyControl.Controls.Growl.Warning("尚有题目未填写！");
                //        return;
                //    }
                //    sum += answer;
                //}
                //entity.Score = sum;
                //if (DAL.Insert(entity) > 0)
                //{
                //    HandyControl.Controls.Growl.Info("问卷保存成功！");
                //}
            }
            else
            {
                //for (int i = 0; i < 6; i++)
                //{
                //    if (model.Answers[i] <= 0)
                //    {
                //        HandyControl.Controls.Growl.Warning("尚有题目未填写！");
                //        return;
                //    }
                //}
                //entity.QSId = DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "_");
                //entity.QS1 = copdanswer[0, model.Answers[0] - 1];
                //entity.QS2 = copdanswer[1, model.Answers[1] - 1];
                //entity.QS3 = copdanswer[2, model.Answers[2] - 1];
                //entity.QS4 = copdanswer[3, model.Answers[3] - 1];
                //entity.QS5 = copdanswer[4, model.Answers[4] - 1];
                //entity.QS6 = copdanswer[5, model.Answers[5] - 1];
                //if (DAL.Insert(entity) > 0)
                //{
                //    HandyControl.Controls.Growl.Info("问卷保存成功！");
                //}
            }
        }

        private void RadioButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.IsChecked == true)
            {
                // 取消选中当前RadioButton
                radioButton.IsChecked = false;
                e.Handled = true; // 阻止事件继续传递

                // 如果需要，可以在这里添加取消选中后的逻辑
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
            {
                // 获取当前选中的TabItem
                TabItem selectedTab = tabControl.SelectedItem as TabItem;

                // 根据选中的选项卡调整其他控件
                if (selectedTab != null)
                {
                    string header = selectedTab.Header.ToString();

                    // 示例：根据选项卡显示/隐藏控件
                    if (header == "CAT问卷")
                    {
                        CheckBox7.Visibility = Visibility.Visible;
                        CheckBox8.Visibility = Visibility.Visible;
                       // model.Answers = new ObservableCollection<int>(Enumerable.Repeat(0, model.Answers.Count));
                    }
                    else if (header == "COPD问卷")
                    {
                        CheckBox7.Visibility = Visibility.Hidden;
                        CheckBox8.Visibility = Visibility.Hidden;
                       // model.Answers = new ObservableCollection<int>(Enumerable.Repeat(0, model.Answers.Count));
                    }
                }
            }
        }
    }
}
