   M S B A M L   `   `   `  ���� [  WSystem.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Y@clr-namespace:System.Windows.Forms;assembly=System.Windows.FormsSystem.Windows.Forms  A =WPFInk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WPFInkWPFInk    WPFInk.ShotCut.ShotCut   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Owf@clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms     'WPFInkclr-namespace:WPFInk       � Title$  ShotCut��$	��764��$
��1232�� � 
ResizeMode$ 	CanResize=� � WindowStyle$ SingleBorderWindow=� � Topmost .  ���	#FFFFFFFF.��� .+��� -    ��Name$ 	groupBox1��$��Step1:ShotCut��$��90��$
��Left=�$	��Top=�$	��335�����	#FFFFFFFF���	#FF333333$��6,0,0,0q�.��� $
��Auto��$
��Auto������  �� EndPoint$ 0.495,-0.436&� �� 
StartPoint$ 0.498,1.242&�.���  � Color$ 	#FF000000�� � Offset$ 0Q�� $ 	#FFFFFFFF��$ 0.254Q�� $ 	#FF7D7D7D��$ 1Q�� $ 	#FFFFFFFF��$ 0.004Q�� $ 	#FF1F1F1F��$ 0.996Q�.+��� -   	 ��Name$	 label1��$��28��$
��Left=�$��6,6,0,0q�$	��Top=�$��72��}�.  $��
VideoPath:���� -   
 ��Name$
 textBox1��$��98,10.687,125,0q�$��23.313��$	��Top=�$��RightToLeft=��� -    ��Name$ buttonBrowse��$��23��$��Right=�$��	0,10,29,0q�$	��Top=�$��75��$��Browse���� -   $ buttonPause��$��Right=�$��0,38.313,29,9q�$��75��$��Pause���� -   $ 	buttonRun��$��23��$��	123,0,0,8q�$��Bottom=�$
��Left=�$��75��$��ShotCut���� -   $ 	groupBox2��$��Step2:Filter��$��6,96,0,0q�$
��Left=�$	��335��$	��101��$	��Top=����	#FF000000���	#FFFFFFFF.��� $
��Auto��$	��323������ $ 0.499,-0.156&�$ 0.498,1.406&�.��� $ 	#FFFFFFFF��$ 0.786Q�� $ 	#FF0A0A0A��$ 0Q�.+��� -   $	 label2��$��28��$��6,6,0,0q�$	��Top=�$
��Left=�$��84��$��NumberLimit:���� -	   $ buttonFilter��$��23��$��Right=�$��	0,11,29,0q�$	��Top=�$��75��$��Filter���� -
   $	 label3��$��28��$��6,0,0,6q�$��Bottom=�$
��Left=�$��40��$��Para1���� -    ��Name$ slider1��$��22��$��	52,0,0,12q�$��Bottom=�$
��Left=�$	��100���� -   $	 label4��$��28��$
��Left=�$��	167,0,0,6q�$��Bottom=�$��45��$��Para2���� -   $ slider2��$��22��$��Right=�$��0,0,5,12q�$��Bottom=�$	��100��7  1System.Windows.Forms.Integration.WindowsFormsHost  -     Name$ windowsFormsHostNumeric��$��96,11,125,0q�$��22.313��$	��Top=�   Child. (   "System.Windows.Forms.NumericUpDown  "   System.Windows.Forms.Control   Name$ numericUpDownKeyFrameNum���� -   $ 
buttonSave��$��23��$
��Left=�$��6,451.5,0,0q�$	��Top=�$��82��$
��Save���� -   $ buttonAddThis��$��23��$��261,452,0,0q�$	��Top=�$
��Left=�$��80��$��AddImage���� -   $ buttonLayout��$��23��$
��Left=�$��	6,675,0,0q�$	��Top=�$��75��$��Layout���� -   $ 
buttonNext��$��23��$��266,675,0,0q�$	��Top=�$��75��$
��Left=�$��NextPage���� -   $ 
buttonLast��$��23��$
��Left=�$��186,675,0,0q�$	��Top=�$��75��$��LastPage��  -   $ windowsFormsHostFrame��$��	6,203,0,0q�$
��Left=�$	��335��   
Background �	#FF171717$	��228��$	��Top=�  -   $ windowsFormsHostProgress��$��15��$��	6,431,0,0q�$	��Top=�$
��Left=�$	��335�� �	#FF555555. %   System.Windows.Forms.PictureBox  $ pictureBoxProgress��  -   $ windowsFormsHostWave��$	��188��$��	6,482,0,0q�$	��Top=�$
��Left=�$	��335�� �	#FFFFFFFF.   $ pictureBoxWave��  -   $ windowsFormsHostTimer��$��23��$��157,453,0,0q�$	��Top=�$
��Left=�$��39��  -   $ windowsFormsHostShotList�� �	#FFD9D9D9$��	350,0,0,0q�.   $ pictureBoxShotList��