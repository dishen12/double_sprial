   M S B A M L   `   `   `  ���� A  =WPFInk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WPFInkWPFInk  &     WPFInk.VideoSummarizationControl   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     &localclr-namespace:WPFInk     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     �Name$  Window�� � Title$ VideoSummarizationControl�� � WindowState$ 	Maximized=�$	��983��$
��1280��.��� -    �Name$ 
LayoutRoot��.+�� -   $ 	TableGrid��
���   � � ColumnDefinitions �� $��0.25* ��� $��0.50* ��� $��0.25* � � RowDefinitions �� $
��0.2* ��� $
��0.8* �.+��� 
�������
���   �$��1��$y�	Collapsed=��� -    ��Name$ border��$��0q�$��1q� �� Padding$ 2,2,2,4q�$��0��
���   �
�������._�r� -    r�Name$ mediaPlayer��	 r� Stretch$
	 Fill=�
 J� OpacityMask
 �	#FF060202 r� UnloadedBehavior$ Manual=� J� RenderTransformOrigin$ 0.5,0.5&�   WPFInk.InkFrame  -     Name$ summarization��$��0q�$��2��$��0��$��1��$��2��
��������� -    ��Name$ keyFrameListScrollViewer��$��0q�$��3��$��1��$y�	Collapsed=�.��  -   $ keyFrameList��$��0q��� -	   $ keyFrameListScrollViewer2��$��2��$��0q�$��3��$��1��$y�	Collapsed=�.��  -
   $ keyFrameList2��$��0q��� -    ��Name$ AnnotationInkCanvas��$��0��$��0q� �� EditingModeInverted$
 None=� �� EditingMode$
 None=� � Cursor$ Arrow�� �� 
Background Z� r� -   $ hyperLinkPlayer��$��2,2,0,0q�$ Manual=�$
��Left=�$	��100��$��1��$��80��$	��Top=�$
 Hand��

 �   �$y�	Collapsed=��� -    ��Name$ _timeBar��$	��Top=�$��1��$��0,2,0,0q� � Maximum$ 1Q�$��2,2,2,0q��� -    ��Name$ VideoProgressText��$��Right=�$	��Top=� �� TextWrapping$
 Wrap=�$��1��$	��120��$��	0,27,10,0q�
���������Z� ��Z� � -   $ GridBtn��$��0,27,0,0q�$��1��.+��� -    ��Name$ SpiralButton��$��2,35,2,0q�$��Spiral Summarization��$��35��$	��Top=� �� IsCancel . �� -   $ BtnMerge��$��Merge��$��	2,105,2,0q�$	��Top=�$��35��{�. �� -   $ BtnCut��$	��Cut��$��	2,105,2,0q�$	��Top=�$��35��{�. $y�	Collapsed=��� -   $ BtnHyperLink��$	��Top=�$��	HyperLink��$��	2,140,2,0q�$��35��{�. �� -   $ 
TileButton��$��2,35,2,0q�$��Tile  Summarization��$��35��$	��Top=��� -   $ TapestryButton��$��2,35,2,0q�$��Tapestry  Summarization��$��35��$	��Top=��� -   $ BtnVideoFullScreen��$	��Top=�$��Video Full Screen��$��35��$��2,70,2,0q��� -   $ BtnNavigation��$
��Left=�$	��Top=�$��75��$��
Navigation��$��35��$��2,0,0,0q�$y�	Collapsed=��� -   $ BtnAutoring��$
��Left=�$	��Top=�$��75��$��Autoring��$��77,0,0,0q�$��35��$y�	Collapsed=�   WPFInk.ControlPanel  -     Name$ _keyframeControlPanel��$
��Left=�$��
0,0,0,-141q�$��1��$	��Top=�$��2��$y�	Collapsed=�$��2���� -    ��Name$ InkCanvasShowRate��$��Right=�$��Bottom=�$
��100%��$��1��$��2��$��	0,0,10,10q�
��������� -   $ 
BdQuestion��$��	0,130,0,0q�$��1q�$��1��$y�Visible=�._�� -   $ QuestionGrid��$��0q� �� $
��0.1* ��� $
��0.9* �.+��� -   $ TitleLbl��$
��Left=�$��Label��$��118.667��$��40��
�������$��
0,11.2,0,0q�$��Bottom=�$y�	Collapsed=��� -   $ BtnStart��$��Start��$��0,8.727,8,8q�$��Right=�$	��100��{�.  $y�	Collapsed=�� -   $ QuestionContentGrid��$��	1,1,-1,-1q�$��6��$��1�� �� $
��0.2* ��� $
��0.2* ��� $
��0.2* ��� $
��0.2* ��� $
��0.2* �   WPFInk.UserStudySettings  -      Name$ _userStudySettings��