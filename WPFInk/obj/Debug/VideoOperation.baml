   M S B A M L   `   `   `  ����A  =WPFInk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WPFInkWPFInk  5          WPFInk.VideoOperation   5      W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      $localclr-namespace:WPFInk    5      >d2http://schemas.microsoft.com/expression/blend/2008    6%   Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    6b   -   5          �Name$  Window��5       � Title$ VideoOperation��5      $	��640��5      $	��480��6    � WindowState$ 	Maximized=�6,   .��5
      � -   5         �Name$ 
LayoutRoot��5
      $��1,1,1,1q�6   .+�5   
   � -   5        $ 	TableGrid��5       � ColumnDefinitions 5      �� 5      $
��0.5* �6   �� 5      $��0 �6    � RowDefinitions 5      �� 5      $
��0.4* �6   �� 5      $
��0.6* �6   .+�5      �� -   5         ��Name$ rectangleWhole��5   P   $��0��6   $��0��6   $��2��6+   $��2��6?   $��2q�6h   �� 5      -   5        $ rectangleRight��5      $��1��6   ���	#FFCECCCC
���   �$��2,2,5,3q�6V    J� RenderTransformOrigin$ 0.5,0.5&�6g   $��0��6�   $y�	Collapsed=�6�   z�5      V� 5      .~�5      ��  �� ScaleX$ -1Q�6   �� 5      �� 5      T� 5      5      5       �� 5!      -   5        $ rectangleLeft��5!   +   $��0��6   $��0��6   ���	#FFCECCCC
���   �$��1,2,0,2q�6b   	 �� RadiusY$	 5��6s   $��Center=�6   $	��506��6�   $y�	Collapsed=�6�   �� 5"   
   -   5        $ rectangleBottom��5"      ���	#FFCECCCC
���   �$��1��6M   $��4,0,4,3q�6Z   $��2��6k   �� 5#      -   5        
 ��Name$
 _searchInkCanvas��5#      $��Right=�6(   $	��250��6D   $��0q�6P    �� 
Background
 �����6n      WPFInk.ThumbInk  5$      -	   5          Name$ 	_thumbInk��5$      $��
1,7.5,1,13q�6&   $��1��6:   $��Stretch=�6G   $��2��6e      WPFInk.TitleInk  5%      -
   5          Name$ 	_titleInk��5%      $��	5,5,10,11q�6&   $��1��69   $��1��6I   � 5&      -   5        $ RadioButtonGrid��5&   	   $
��Left=�6"   $	��Top=�6=   $	��200��6U   $	��100��6a   $��100,100,0,0q�6n   $y�	Collapsed=�6�   .+�5'      	� -   5         	�Name$ radioButton_1��5'      $��0,10,0,0q�6(   $	��Top=�6:   $��20��6R   $��KeyWords��6^   $
��Left=�6�   	� 5(      -   5        $ radioButton_2��5(      $��0,40,0,0q�6(   $	��Top=�6:   $��16��6R   $��	KeyFrames��6^   $
��Left=�6�   	� 5)      -   5        $ radioButton_3��5)      $��0,70,0,0q�6(   $��Sketches��6:   $	��Top=�6M   $��20��6e   $
��Left=�6�   5*      �� 5+      $��Center=�6   $	��606��6(   
���   �$��1q�6H    �� Padding$ 2q�6\   $��0,0,0,2q�6h   ._�5,         WPFInk.VideoAnnotation  -   5          Name$ _videoAnnotation��5,      $��5,0,5,2q�65   5-      5/         WPFInk.VideoList  51   
   -   5          Name$ 
_videoList��51      $��484,3.7,0,0q�6.   $
��Left=�6C   $	��150��6^   $��20��6j   $	��Top=�6v   $y�	Collapsed=�6�   � 52   
   -   5        $ KeyWordsImagesGrid��52      $��0,0,0,3q�6+   $y�	Collapsed=�6<    53      �� 54      $��0.251* �6   �� 55      $��0.749* �6   .+�57         WPFInk.ScrollInkFrame  -   5          Name$ _scrollInkFrame��57       $��0,0,8,0q�6T   $��2��6e      WPFInk.ControlPanel  58      -   5          Name$  KeyWordsImagesControlPanel��58      $
��Left=�6B   $��8,5,0,0q�6]   $	��Top=�6�   5:      $   WPFInk.UpdateRecognitionRusult  5;      -   5          Name$ _updateRecogntionRusult��5;   "   $��	0,0,0,100q�6C   $��Center=�6V   $��Center=�6s   $y�	Collapsed=�6�   5=      5>      