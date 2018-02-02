using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WPFInk.Global
{
    public class GlobalValues
    {
        //public static GlobalValues singleTon = null;
        //识别语言
        public static int InkAnalyzerLanguageId = 1033;//笔迹识别语言，2052：简体中文 1033：英语

        //path
        public static readonly string FilesPath = "E:";
        public static List<List<byte>> templates = new List<List<byte>>();//模板存储空间
        //螺旋摘要
        //public static byte[] templates2 = new byte[2310 * 60000];
        public static Color color = Colors.Black;//螺旋的背景颜色和螺旋线颜色
        public static string videoName = "麋鹿王";//、、小熊维尼与跳跳虎、大头儿子小头爸爸憨豆先生1\大雄兔
        public static bool isShowHalf=false;
        //螺旋摘要聚类展示
        public static int PartitionShowTypeNo=0;//聚类展示类型，0代表高亮展示，1代表扇区展示

        //螺旋摘要测试
        public static int summarizationTypeNo=0;//0:螺旋，1：平铺，2：tap，3：无意义
        public static int questionType=0;//问题类型，0为阅读理解，1为定位
        public static string userNo="01";
        public static List<string> locationQuestions = new List<string>();//定位题目
        public static List<List<int>> locationAnswers = new List<List<int>>();//定位题目答案
        public static Question LocationQuestion;
        public static int CurrLocationId=0;
        public static List<Color> LocationQuestionBackGrouds = new List<Color>();
        //MyGraphic
        public static int MyGraphic_PauseTime = 2;//多笔识别时间间隔限制
        public static int MyGraphic_SpacingDistance = 20;//spacing distance多个图形之间的间隔阈值
        public static bool MyGraphic_IsDirectionRecognize = true;//是否根据方向和笔序识别，true时根据方向和笔序识别
        //public static bool MyGraphic_IsStrokeShow = true;//是显示stroke还是图形，true为stroke

        //ControlPanel
        public static int ControlPanel_InkModeHeight = 622;//controlPanel在Ink模式时的高度
        public static int ControlPanel_InsertTextHeight = 420;//controlPanel在InsertText模式时的高度
        public static int ControlPanel_OtherModeHeight = 330;//controlPanel在其他模式时的高度
    }
}
