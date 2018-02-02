using System;
using System.Collections.Generic;
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
using WPFInk.Global;
using System.IO;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for UserStudySettings.xaml
	/// </summary>
	public partial class UserStudySettings : UserControl
	{
        private VideoSummarizationControl videoSummarizationControl;
		public UserStudySettings()
		{
			this.InitializeComponent();
            Init();
		}
        public void setVideoSummarizationControl(VideoSummarizationControl v)
        {
            videoSummarizationControl = v;
        }

        private void VisibleTapestry()
        {
            videoSummarizationControl.SpiralButton.Visibility = Visibility.Collapsed;
            videoSummarizationControl.TileButton.Visibility = Visibility.Collapsed;
            videoSummarizationControl.TapestryButton.Visibility = Visibility.Visible;
        }

        private void VisibleTile()
        {
            videoSummarizationControl.SpiralButton.Visibility = Visibility.Collapsed;
            videoSummarizationControl.TileButton.Visibility = Visibility.Visible;
            videoSummarizationControl.TapestryButton.Visibility = Visibility.Collapsed;
        }

        private void VisibleSpiral()
        {
            videoSummarizationControl.SpiralButton.Visibility = Visibility.Visible;
            videoSummarizationControl.TileButton.Visibility = Visibility.Collapsed;
            videoSummarizationControl.TapestryButton.Visibility = Visibility.Collapsed;
        }
        private void Init()
        {
            for (int i = 1; i <= 4; i++)
            {
                ComboBoxItem task = new ComboBoxItem();
                task.Content = "任务"+i;
                CbbTaskSelect.Items.Add(task);
            }

            ComboBoxItem taskStudy = new ComboBoxItem();
            taskStudy.Content = "学习";
            CbbTaskSelect.Items.Add(taskStudy);
            //for (int i = 5; i != 7; i++)
            //{
            //    ComboBoxItem task = new ComboBoxItem();
            //    task.Content = "学习任务" + i;
            //    CbbTaskSelect.Items.Add(task);
            //}

            for (int i = 1; i < 4; i++)
            {
                ComboBoxItem task = new ComboBoxItem();
                task.Content = "第" + i + "组";
                CbbGroupSelect.Items.Add(task);
            }


            //ComboBoxItem task1 = new ComboBoxItem();
            //task1.Content = "卡通";
            //CbbVideoTypeSelect.Items.Add(task1);
            //ComboBoxItem task2 = new ComboBoxItem();
            //task2.Content = "真人";
            //CbbVideoTypeSelect.Items.Add(task2);  
            
        }


        //private void CbbTaskSelect_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    setTask();
        //}

        private void BtnStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            setTask();
        }
        private void setTask()
        {
            int taskId = CbbTaskSelect.SelectedIndex;
            int groupId = CbbGroupSelect.SelectedIndex;
            //int videoTypeId=CbbVideoTypeSelect.SelectedIndex;
            try
            {
                int no = int.Parse(number.Text);
            }
            catch
            {
                MessageBox.Show("请输入正确的编号！");
                return;
            }
            
            if (taskId == -1)
            {
                MessageBox.Show("请选择任务！");
                return;
            }
            if (groupId == -1)
            {
                MessageBox.Show("请选择小组！");
                return;
            }
            //if (videoTypeId == -1)
            //{
            //    MessageBox.Show("请选择视频类型！");
            //    return;
            //}
            //为该任务新建记事本
            string txtPath = @"resource\Record\" + (groupId + 1).ToString() + "_" + (taskId + 1).ToString() + "_" + number.Text + ".txt";
            GlobalValues.userNo = number.Text;
            FileStream myStream;
            if (!File.Exists(txtPath))
            {
                //创建一个新的txt文件
                myStream = new FileStream(txtPath, FileMode.Create, FileAccess.Write);
            }
            else
            {
                myStream = new FileStream(txtPath, FileMode.Append, FileAccess.Write);

            }
            StreamWriter writer = new StreamWriter(myStream);
            writer.WriteLine("用户编号：" + number.Text);
            writer.WriteLine("当前时间：" + System.DateTime.Now.ToString("s") + ":" + System.DateTime.Now.Millisecond.ToString());
            writer.WriteLine("任务号:" + (taskId + 1).ToString() + ",任务：" + ((ComboBoxItem)CbbTaskSelect.Items[taskId]).Content.ToString());
            writer.WriteLine("小组号:" + (groupId + 1).ToString() + ",小组:" + ((ComboBoxItem)CbbGroupSelect.Items[groupId]).Content.ToString());
            //writer.WriteLine("视频类型:" + (videoTypeId==0?"卡通":"真人"));
            switch (taskId)
            {
                case 0:
                    GlobalValues.isShowHalf = false;
                    GlobalValues.videoName = "大雄兔";
                    //switch (videoTypeId)
                    //{
                    //    case 0:
                    //        //GlobalValues.videoName = "大雄兔";
                    //        //GlobalValues.videoName = "小熊维尼与跳跳虎";
                    //        break;
                    //    case 1:
                    //        GlobalValues.videoName = "憨豆先生1";
                    //        break;
                    //}
                    switch (groupId)
                    {
                        case 0:
                            GlobalValues.summarizationTypeNo = 0;
                            break;
                        case 1:
                            GlobalValues.summarizationTypeNo = 1;
                            break;
                        case 2:
                            GlobalValues.summarizationTypeNo = 2;
                            break;
                    }
                    GlobalValues.questionType = 0;
                    videoSummarizationControl.TitleLbl.Content = "Comprehension";
                    break;
                case 1:
                    GlobalValues.isShowHalf = false;
                    //GlobalValues.videoName = "憨豆先生1"; 
                    GlobalValues.videoName = "小熊维尼与跳跳虎";
                    //switch (videoTypeId)
                    //{
                    //    case 0:
                    //        //GlobalValues.videoName = "大雄兔";
                    //        //GlobalValues.videoName = "小熊维尼与跳跳虎";
                    //        break;
                    //    case 1:
                    //        GlobalValues.videoName = "憨豆先生1";
                    //        break;
                    //}
                    switch (groupId)
                    {
                        case 0:
                            GlobalValues.summarizationTypeNo = 0;
                            break;
                        case 1:
                            GlobalValues.summarizationTypeNo = 1;
                            break;
                        case 2:
                            GlobalValues.summarizationTypeNo = 2;
                            break;
                    }
                    GlobalValues.questionType = 0;
                    videoSummarizationControl.TitleLbl.Content = "Comprehension";
                    break;
                case 2:
                    GlobalValues.isShowHalf = true;
                    GlobalValues.videoName = "麋鹿王";
                    //switch (videoTypeId)
                    //{
                    //    case 0:
                    //        //GlobalValues.videoName = "大雄兔";
                    //        //GlobalValues.videoName = "小熊维尼与跳跳虎";
                    //        break;
                    //    case 1:
                    //        GlobalValues.videoName = "憨豆先生1";
                    //        break;
                    //}
                    switch (groupId)
                    {
                        case 0:
                            GlobalValues.summarizationTypeNo = 0;
                            break;
                        case 1:
                            GlobalValues.summarizationTypeNo = 1;
                            break;
                        case 2:
                            GlobalValues.summarizationTypeNo = 2;
                            break;
                    }
                    GlobalValues.questionType = 0;
                    videoSummarizationControl.TitleLbl.Content = "Comprehension";
                    break;
                case 3:
                    GlobalValues.isShowHalf = true;
                    GlobalValues.videoName = "绝望主妇1";
                    //switch (videoTypeId)
                    //{
                    //    case 0:
                    //        //GlobalValues.videoName = "大雄兔";
                    //        //GlobalValues.videoName = "小熊维尼与跳跳虎";
                    //        break;
                    //    case 1:
                    //        GlobalValues.videoName = "憨豆先生1";
                    //        break;
                    //}
                    switch (groupId)
                    {
                        case 0:
                            GlobalValues.summarizationTypeNo = 0;
                            break;
                        case 1:
                            GlobalValues.summarizationTypeNo = 1;
                            break;
                        case 2:
                            GlobalValues.summarizationTypeNo = 2;
                            break;
                    }
                    GlobalValues.questionType = 0;
                    videoSummarizationControl.TitleLbl.Content = "Comprehension";
                    break;                
                case 4:
                    GlobalValues.isShowHalf = true;
                    GlobalValues.videoName = "大头儿子小头爸爸";
                    //switch (videoTypeId)
                    //{
                    //    case 0:
                    //        GlobalValues.videoName = "小熊维尼与跳跳虎";
                    //        break;
                    //    case 1:
                    //        GlobalValues.videoName = "绝望主妇2";
                    //        break;
                    //}
                    switch (groupId)
                    {
                        case 0:
                            GlobalValues.summarizationTypeNo = 0;
                            break;
                        case 1:
                            GlobalValues.summarizationTypeNo = 1;
                            break;
                        case 2:
                            GlobalValues.summarizationTypeNo = 2;
                            break;
                    }
                    GlobalValues.questionType = 0;
                    videoSummarizationControl.TitleLbl.Content = "Comprehension";
                    break;
                //case 5:
                //    GlobalValues.isShowHalf = true;
                //    switch (videoTypeId)
                //    {
                //        case 0:
                //            GlobalValues.videoName = "小熊维尼与跳跳虎";
                //            break;
                //        case 1:
                //            GlobalValues.videoName = "绝望主妇2";
                //            break;
                //    }
                //    switch (groupId)
                //    {
                //        case 0:
                //            GlobalValues.summarizationTypeNo = 0;
                //            //GlobalValues.isShowHalf = true;
                //            break;
                //        case 1:
                //            GlobalValues.summarizationTypeNo = 1;
                //            break;
                //        case 2:
                //            GlobalValues.summarizationTypeNo = 2;
                //            //GlobalValues.isShowHalf = false;
                //            break;
                //    }
                //    GlobalValues.questionType = 1;
                //    videoSummarizationControl.TitleLbl.Content = "Location";
                //    break;
            }
            switch (GlobalValues.summarizationTypeNo)
            {
                case 0:
                    VisibleSpiral();
                    break;
                case 1:
                    VisibleTile();
                    break;
                case 2:
                    VisibleTapestry();
                    break;
                //case 3:
                //    MessageBox.Show("第2小组没有任务2和任务4");
                //    return;
                    //break;
            }

            writer.WriteLine("视频名称:" + GlobalValues.videoName);
            writer.WriteLine("问题类型:" + videoSummarizationControl.TitleLbl.Content.ToString());
            writer.Flush();
            writer.Close();
            myStream.Close();
            videoSummarizationControl.summarization.InkCollector.addImages();
            videoSummarizationControl.setFileStream(txtPath);
            this.Visibility = Visibility.Collapsed;
        }
	}
}