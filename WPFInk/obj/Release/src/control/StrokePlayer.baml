   M S B A M L   `   `   `  ���� 
  WPFInk    WPFInk.StrokePlayer   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     $��30��  � 	Resources     StrokeplayerProgress �� 
TargetType �� Property �� Value �� EndPoint �� 
StartPoint � Color � Offset �� 
TargetType	 �Name  Key
 J� Opacity'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard g� Duration �� 
TargetName �� TargetProperty R� KeyTime R� Value  System.Windows.Visibility � ColumnDefinitions ��Name �� RadiusX �� RadiusY ��Name  RepeatButtonTemplate n�Name � RowDefinitions��%	  &	        7  7  7  7  �� " ��.���  � ��$ 1q���  �@�Maximum$ 10Q���  �@�Minimum$ 0Q���  �@�Value$ 0Q���  � �� �� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FFA3AEB9��$ 0Q�� $ 	#FF8399A9��$ 0.375Q�� $ 	#FF718597��$ 0.375Q�� $ 	#FF617584��$ 1Q���  � �� .  ��  � �� �� " ��.R�� $
	 Root��  ���� RepeatButtonTemplate .R�� $
	 Root��$
 0Q�
������    CommonStates��.   
Normal��  	MouseOver��  Disabled��. �� .��T� $ 0D�$- 'HorizontalTrackRectangleDisabledOverlay��$ 
Visibility�..�i� $ 0��  Visible=�T� $ 0D�$ ThumbDisabledOverlay��$ 
Visibility�..�i� $ 0��  Visible=�T� $ 0D�$+ %VerticalTrackRectangleDisabledOverlay��$ 
Visibility�..�i� $ 0��  Visible=�.+�� $	 HorizontalTemplate��#��z�� �� $
��Auto ��� $
��Auto ��� $��* �.+��� $ TrackRectangle�����	#FF55BFDC���	#FFA3AEB9#��z��$ 1��$ 1��$��3��$��5,0,5,0q�$��0��$��3���� $- 'HorizontalTrackRectangleDisabledOverlay�����	#FF084784$ 1��$ 1��$��3��$��5,0,5,0q�$	
 .55Q�$y�	Collapsed=�$��0��$��3���� $4 .HorizontalTrackLargeChangeDecreaseRepeatButton����.  8��  $��18��$��0��n� $ HorizontalThumb����. $��18��$��11��$��1�����	#FF1E3278���� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FFA3AEB9��$ 0Q�� $ 	#FF8399A9��$ 0.375Q�� $ 	#FF718597��$ 0.375Q�� $ 	#FF34668C��$ 1Q��� $ ThumbDisabledOverlay�����	#FF244363$ 2��$ 2��$��11��$	
 .55Q�$y�	Collapsed=�$��1���� $4 .HorizontalTrackLargeChangeIncreaseRepeatButton����.  8�� $��18��$��2��� $	 VerticalTemplate��$y�	Collapsed=�#��z�� �� $��* ��� $
��Auto ��� $
��Auto �.+��� ���	#FFE6EFF7���	#FFA3AEB9#��z��$ 1��$ 1��$��0,5,0,5q�$��3��$��0��$��3���� $+ %VerticalTrackRectangleDisabledOverlay��
�������$ 1��$ 1��$��0,5,0,5q�$��3��$	
 .55Q�$y�	Collapsed=�$��0��$��3���� $2 ,VerticalTrackLargeChangeDecreaseRepeatButton����.  8�� $��18��$��2��n� $ VerticalThumb����. $��11��$��18��$��1���� $2 ,VerticalTrackLargeChangeIncreaseRepeatButton����.  8�� $��18��$��0��.��� -   $	 
LayoutRoot��
�������$��30��.+��� 
����������	#FF3D3B3B$ 3��$ 3���� -   $ StartBackground�����	#FF19B4D4$
��Left=�$��8,5,0,5q�$��20���� -    ��Name$ ProgressSlider��$��72,4.5,8,3.5q� � LargeChange$
 0.01Q� � Maximum$ 1Q� � SmallChange$
 0.01Q� J� IsHitTestVisible .  �� -    ��Name$ 	EndButton��$
��Left=�$��43,0,0,8q�$��14��$��Button��
���   �$��0q�$��14��$��Bottom=�    EndButtonStyle  �� KeyTime! �� Value" ��Name��%�  &	       �� " ��.���  � �� �	#FF1F3B53��  � �� �	#FF000000��  � ��$ 3q���  � ��$ 1q���  � �� �� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FFA3AEB9��$ 0Q�� $ 	#FF8399A9��$ 0.375Q�� $ 	#FF718597��$ 0.375Q�� $ 	#FF617584��$ 1Q���  � �� �� " ��.R��    CommonStates��.   
Normal��  	MouseOver��  Pressed��. �� .���� $ 
Background��$1 +(Border.Background).(SolidColorBrush.Color)�.g��� $  0��$! 	#FF6DBDD1��  Disabled��  FocusStates��.   Focused��  	Unfocused��.+��� $" 
Background��
���   �#��z��#��z��._�� $��1q�#��z������ # �� ResourceKey$# EndButtonStyle���� -   $ 
StopButton��$
��Left=�$��8,5,0,6q�$��20��$y�	Collapsed=�.���� -   $ ��Name$$ StopImg��$��12��$��12��$��images/Stop.PNG���� -   $ 
PlayButton��$
��Left=�$��8,5,0,6q�$��20��$y�Visible=�  ButtonStyle2#���  .���� $��12��$��12��$��images/Play.PNG���� -   $ 
GoOnButton��$
��Left=�$��8,5,0,6q�$��20��$y�	Collapsed=�.���� $��12��$��12��$��images/Play.PNG��