   M S B A M L   `   `   `  ���� A  =WPFInk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WPFInkWPFInk      WPFInk.VideoOperation   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     &localclr-namespace:WPFInk     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     �Name$  Window�� � Title$ VideoOperation��$	��640��$	��480�� � WindowState$ 	Maximized=�.��� -    �Name$ 
LayoutRoot��$��1,1,1,1q�.+�� -   $ 	TableGrid�� � ColumnDefinitions �� $
��0.5* ��� $��0 � � RowDefinitions �� $
��0.4* ��� $
��0.6* �.+��� -    ��Name$ rectangleWhole��$��0��$��0��$��2��$��2��$��2q��� -   $ rectangleRight��$��1�����	#FFCECCCC
���   �$��2,2,5,3q� J� RenderTransformOrigin$ 0.5,0.5&�$��0��$y�	Collapsed=�z�V� .~���  �� ScaleX$ -1Q��� �� T� �� -   $ rectangleLeft��$��0��$��0�����	#FFCECCCC
���   �$��1,2,0,2q�	 �� RadiusY$	 5��$��Center=�$	��506��$y�	Collapsed=��� -   $ rectangleBottom�����	#FFCECCCC
���   �$��1��$��4,0,4,3q�$��2���� -   
 ��Name$
 _searchInkCanvas��$��Right=�$	��250��$��0q� �� 
Background
 �����   WPFInk.ThumbInk  -	     Name$ 	_thumbInk��$��
1,7.5,1,13q�$��1��$��Stretch=�$��2��   WPFInk.TitleInk  -
     Name$ 	_titleInk��$��	5,5,10,11q�$��1��$��1��� -   $ RadioButtonGrid��$
��Left=�$	��Top=�$	��200��$	��100��$��100,100,0,0q�$y�	Collapsed=�.+�	� -    	�Name$ radioButton_1��$��0,10,0,0q�$	��Top=�$��20��$��KeyWords��$
��Left=�	� -   $ radioButton_2��$��0,40,0,0q�$	��Top=�$��16��$��	KeyFrames��$
��Left=�	� -   $ radioButton_3��$��0,70,0,0q�$��Sketches��$	��Top=�$��20��$
��Left=��� $��Center=�$	��606��
���   �$��1q� �� Padding$ 2q�$��0,0,0,2q�._�   WPFInk.VideoAnnotation  -     Name$ _videoAnnotation��$��5,0,5,2q�   WPFInk.VideoList  -     Name$ 
_videoList��$��484,3.7,0,0q�$
��Left=�$	��150��$��20��$	��Top=�$y�	Collapsed=�� -   $ KeyWordsImagesGrid��$��0,0,0,3q�$y�	Collapsed=� �� $��0.251* ��� $��0.749* �.+�   WPFInk.ScrollInkFrame  -     Name$ _scrollInkFrame��$��0,0,8,0q�$��2��   WPFInk.ControlPanel  -     Name$  KeyWordsImagesControlPanel��$
��Left=�$��8,5,0,0q�$	��Top=�$   WPFInk.UpdateRecognitionRusult  -     Name$ _updateRecogntionRusult��$��	0,0,0,100q�$��Center=�$��Center=�$y�	Collapsed=�