   M S B A M L   `   `   `  ����[  WSystem.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Y@clr-namespace:System.Windows.Forms;assembly=System.Windows.FormsSystem.Windows.Forms  5      A =WPFInk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WPFInkWPFInk 5         WPFInk.ShotCut.ShotCut   5      W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      Mwf@clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms    5      %WPFInkclr-namespace:WPFInk    5        � Title$  ShotCut��5      $	��764��6   $
��1232��6"    � 
ResizeMode$ 	CanResize=�6/    � WindowStyle$ SingleBorderWindow=�6F    � Topmost .  ���	#FFFFFFFF.��5      � .+�5   
   �� -   5         ��Name$ 	groupBox1��5   *   $��Step1:ShotCut��6   $��90��6=   $
��Left=�6I   $	��Top=�6d   $	��335��6|   ���	#FFFFFFFF���	#FF333333$��6,0,0,0q�6�   .��5	      � $
��Auto��6   $
��Auto��6!   ��5
      �� 5       �� EndPoint$ 0.495,-0.436&�6$    �� 
StartPoint$ 0.498,1.242&�6<   .��5      �  � Color$ 	#FF000000��6    � Offset$ 0Q�60   � 5      $ 	#FFFFFFFF��6   $ 0.254Q�60   � 5      $ 	#FF7D7D7D��6   $ 1Q�60   � 5      $ 	#FFFFFFFF��6   $ 0.004Q�60   � 5      $ 	#FF1F1F1F��6   $ 0.996Q�60   5      .+�5      �� -   5        	 ��Name$	 label1��5   P   $��28��6   $
��Left=�6$   $��6,6,0,0q�6?   $	��Top=�6`   $��72��6x   }�.  $��
VideoPath:��6�   �� 5      -   5        
 ��Name$
 textBox1��5   3   $��98,10.687,125,0q�6   $��23.313��6E   $	��Top=�6U   $��RightToLeft=�6m   �� 5      -   5         ��Name$ buttonBrowse��5   T   $��23��6   $��Right=�6%   $��	0,10,29,0q�6A   $	��Top=�6j   $��75��6�   $��Browse��6�   6�   �� 5      -   5        $ buttonPause��5   L   $��Right=�6   $��0,38.313,29,9q�65   $��75��6a   $��Pause��6�   �� 5      -   5        $ 	buttonRun��5   8   $��23��6   $��	123,0,0,8q�6%   $��Bottom=�6K   $
��Left=�6f   $��75��6�   $��ShotCut��6�   5      5      �� 5   
   -   5        $ 	groupBox2��5   ;   $��Step2:Filter��6   $��6,96,0,0q�6)   $
��Left=�6N   $	��335��6i   $	��101��6u   $	��Top=�6�   ���	#FF000000���	#FFFFFFFF.��5      � $
��Auto��6   $	��323��6!   ��5      �� 5      $ 0.499,-0.156&�6$   $ 0.498,1.406&�6<   .��5      � $ 	#FFFFFFFF��6   $ 0.786Q�60   � 5      $ 	#FF0A0A0A��6   $ 0Q�60   5       .+�5"      �� -   5        $	 label2��5"   5   $��28��6   $��6,6,0,0q�6$   $	��Top=�6E   $
��Left=�6]   $��84��6x   $��NumberLimit:��6�   �� 5#      -	   5        $ buttonFilter��5#   T   $��23��6   $��Right=�6%   $��	0,11,29,0q�6A   $	��Top=�6j   $��75��6�   $��Filter��6�   �� 5$      -
   5        $	 label3��5$   5   $��28��6   $��6,0,0,6q�6$   $��Bottom=�6E   $
��Left=�6`   $��40��6{   $��Para1��6�   �� 5%      -   5         ��Name$ slider1��5%   8   $��22��6   $��	52,0,0,12q�6%   $��Bottom=�6I   $
��Left=�6d   $	��100��6   �� 5&      -   5        $	 label4��5&   R   $��28��6   $
��Left=�6$   $��	167,0,0,6q�6?   $��Bottom=�6b   $��45��6}   $��Para2��6�   �� 5'      -   5        $ slider2��5'   S   $��22��6   $��Right=�6%   $��0,0,5,12q�6A   $��Bottom=�6d   $	��100��6   7  1System.Windows.Forms.Integration.WindowsFormsHost  5(      -   5          Name$ windowsFormsHostNumeric��5(   8   $��96,11,125,0q�6#   $��22.313��6Y   $	��Top=�6i      Child. 5)      (   "System.Windows.Forms.NumericUpDown  "   System.Windows.Forms.Control   Name$ numericUpDownKeyFrameNum��6'   5*      5+      5,      �� 5-   
   -   5        $ 
buttonSave��5-   M   $��23��6   $
��Left=�6   $��6,451.5,0,0q�68   $	��Top=�6a   $��82��6y   $
��Save��6�   6�   �� 5.   
   -   5        $ buttonAddThis��5.   2   $��23��6   $��261,452,0,0q�6   $	��Top=�6I   $
��Left=�6a   $��80��6|   $��AddImage��6�   �� 5/   
   -   5        $ buttonLayout��5/   K   $��23��6   $
��Left=�6   $��	6,675,0,0q�68   $	��Top=�6a   $��75��6y   $��Layout��6�   �� 50   
   -   5        $ 
buttonNext��50   2   $��23��6   $��266,675,0,0q�6   $	��Top=�6F   $��75��6^   $
��Left=�6i   $��NextPage��6�   �� 51   
   -   5        $ 
buttonLast��51   M   $��23��6   $
��Left=�6   $��186,675,0,0q�68   $	��Top=�6a   $��75��6y   $��LastPage��6�     52   
   -   5        $ windowsFormsHostFrame��52   .   $��	6,203,0,0q�6   $
��Left=�6M   $	��335��6h      
Background �	#FF171717$	��228��6�   $	��Top=�6�     53   
   -   5        $ windowsFormsHostProgress��53   :   $��15��6   $��	6,431,0,0q�6'   $	��Top=�6\   $
��Left=�6t   $	��335��6�    �	#FF555555. 54      %   System.Windows.Forms.PictureBox  $ pictureBoxProgress��6   55        56   
   -   5        $ windowsFormsHostWave��56   ;   $	��188��6   $��	6,482,0,0q�6(   $	��Top=�6Y   $
��Left=�6q   $	��335��6�    �	#FFFFFFFF. 57        $ pictureBoxWave��6   58        59   
   -   5        $ windowsFormsHostTimer��59   <   $��23��6   $��157,453,0,0q�6'   $	��Top=�6[   $
��Left=�6s   $��39��6�     5:   
   -   5        $ windowsFormsHostShotList��5:       �	#FFD9D9D9$��	350,0,0,0q�6T   . 5;        $ pictureBoxShotList��6   5<      5>      5?      