   M S B A M L   `   `   `  ���� A  =WPFInk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WPFInkWPFInk      WPFInk.VideoAnnotation   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:WPFInk     -     >�Name$  UserControl�� � 	Resources    videoGradientStroryBoardIn g� 	BeginTime �� 
TargetName �� TargetProperty O� KeyTime O� Value  videoGradientStroryBoardOut  RightToLeftStoryboardIn p� KeyTime p� Value  LeftToRightStoryboardOut  RightToLeftStoryboardOut  LeftToRightStoryboardIn	 �� KeyTime
 �� Value  TopToBottomStoryboardIn  TopToBottomStoryboardOut  BottomToTopStoryboardIn 	 BottomToTopStoryboardOut 
 RotateStoryboardIn��%�  &	        &	 �     &	     &	 �    &	 (    &	 �    &	 �    &	 T    &	 �    &		 v    &	
     �� .��U� $ 00:00:00X�$ mediaPlayer��$ (UIElement.Opacity)�.\��� $ 00:00:00��$ 0Q��� $ 00:00:03��$ 1Q��� .��U� $ 00:00:00X�$ mediaPlayer��$ (UIElement.Opacity)�.\��� $ 00:00:00��$ 1Q��� $ 00:00:03��$ 0Q��� .��r� $ 00:00:00X�$ mediaPlayer��$ (FrameworkElement.Margin)�.	��� $ 00:00:00��$ 	610,0,0,0q��� $ 00:00:03��$ 0q��� .��r� $ 00:00:00X�$ mediaPlayer��$ (FrameworkElement.Margin)�.	��� $ 00:00:00��$ 0q��� $ 00:00:03��$ 	610,0,0,0q��� .��r� $ 00:00:00X�$ mediaPlayer��$ (FrameworkElement.Margin)�.	��� $ 00:00:00��$ 0q��� $ 00:00:03��$ 	0,0,610,0q��� .��r� $ 00:00:00X�$ mediaPlayer��$ (FrameworkElement.Margin)�.	��� $ 00:00:00��$ 	0,0,610,0q��� $ 00:00:03��$ 0q��� $ 00:00:00X�$ border��$0 *(Panel.Background).(SolidColorBrush.Color)�.g��� $	 00:00:02��$
 	#FF080707���� .��r� $ 00:00:00X�$ mediaPlayer��$ (FrameworkElement.Margin)�.	��� $ 00:00:00��$ 	0,0,0,490q��� $ 00:00:03��$ 0q��� .��r� $ 00:00:00X�$ mediaPlayer��$ (FrameworkElement.Margin)�.	��� $ 00:00:00��$ 0q��� $ 00:00:03��$ 	0,490,0,0q��� .��r� $ 00:00:00X�$ mediaPlayer��$ (FrameworkElement.Margin)�.	��� $ 00:00:00��$ 	0,490,0,0q��� $ 00:00:03��$ 0q��� .��r� $ 00:00:00X�$ mediaPlayer��$ (FrameworkElement.Margin)�.	��� $ 00:00:00��$ 0q��� $ 00:00:03��$ 	0,0,0,490q��� .��U� $ 00:00:00X�$ mediaPlayer_rotate��$ Angle�.\��� $ 00:00:00��$ 0Q��� $ 00:00:01��$	 360Q�.��� -    �Name$ 
LayoutRoot�����	#FFC7C7C7$��0q�.+��� -    ��Name$ border��$��1,1,1,26q����	#FF080707
���   �$��1q�._�r� -    r�Name$ mediaPlayer�� r� Stretch$
 Fill=� J� OpacityMask �	#FF060202 r� UnloadedBehavior$ Manual=� J� RenderTransformOrigin$ 0.5,0.5&�z�V� .~��� -   mediaPlayer_scale���� �� -   mediaPlayer_rotate��T� -   mediaPlayer_Translate���� $
��Left=� �� Orientation$ 
Horizontal=�$��1,0,0,-7.664q�$��Bottom=�$��40��.+��� -    ��Name$ OpenFile��$
��10,5q�$
��Open��  ButtonStyle2#���  $
��Left=��� -	   $ 
PlayButton��$��	-2,5,10,5q�$
��Play��#���  $
��Left=��� -
   $ PauseButton��$��	-2,5,10,5q�$��Pause��#���  $
��Left=��� -   $ 
StopButton��$��	-2,5,10,5q�$
��Stop��#���  $
��Left=��� -   $ ButtonDynamicAdd��$
��DAdd��$
��10,5q�#���  $��1q�$
��Left=�$y�	Collapsed=��� -   $ ButtonStaticAdd��$
��SAdd��$
��10,5q�#���  $��30��$
��Left=�$y�	Collapsed=��� -   $ ButtonStaticOk��$��ok��$��280,0,0,-7.664q� J� Opacity$
 0.99Q�$
��Left=�#���  $��Bottom=�$��30��$��0,2,0,0q�$y�	Collapsed=��� -   $ ButtonStaticStart��$��Start��$��275,0,0,-7.664q�$
��Left=�#���  $��Bottom=�$��30��$��0,4,0,0q�$y�	Collapsed=��� -    ��Name$ VideoProgress��$��0,0,-19,1.336q�$��20��$ 	0.5,0.375&� �� TextWrapping$
 Wrap=�$��Right=�$��Bottom=�$��126.5����Z� ��Z� �� -    ��Name$ timelineSlider��$��158.59,0,101.5,-0.664q� � Maximum$ 1Q�$��Bottom=�$��24��   WPFInk.InkFrame  -     Name$ Annotation_InkFrame��$��1,1,1,26q�$ 0.5,0.5&����� *Width+ �� ElementName$ border�� �� Mode$ Default=����� *Height+$ border��$ Default=�z�V� .~��� �� �� T�    WPFInk.ControlPanel  -     Name$ Annotation_ControlPanel��$
��Left=�$	��Top=�$��-150,-6,0,-263q� � Cursor$ Arrow���� -    ��Name$ 	timeLabel��$
��Left=�$�� ��$��	0,539,0,0q�$	��Top=�$��1q�$��10.667(���Z� 