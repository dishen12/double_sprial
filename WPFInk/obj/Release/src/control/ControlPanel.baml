   M S B A M L   `   `   `  ���� h  dMicrosoft.Expression.Interactions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35�`clr-namespace:Microsoft.Expression.Interactivity.Core;assembly=Microsoft.Expression.Interactions'Microsoft.Expression.Interactivity.Core  A =WPFInk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WPFInkWPFInk    WPFInk.ControlPanel   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     oic`clr-namespace:Microsoft.Expression.Interactivity.Core;assembly=Microsoft.Expression.Interactions     &localclr-namespace:WPFInk     -     >�Name$  UserControl��$	��140��$��622.05�� � 	Resources    ButtonFocusVisual �� Property �� Value �� StrokeDashArray J� SnapsToDevicePixels  ButtonNormalBackgroundFill �� EndPoint �� 
StartPoint � Color	 � Offset  ButtonBorder
 �� Color  ButtonStyle �� 
TargetType �� 
TargetType � 	Resources  Key g� 	BeginTime �� 
TargetName �� TargetProperty O� KeyTime O� Value �Name'  !System.Windows.VisualStateManager   CustomVisualStateManagerH   BMicrosoft.Expression.Interactivity.Core.ExtendedVisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   UseFluidLayout   States   System.Windows.VisualState   
Storyboard g� Duration p� KeyTime p� Value �� KeyTime �� Value ��Name �� RadiusX  �� RadiusY! J� Opacity" ��Name��%@  &	        &	 R     &	 �     &	 �     7   7  7  �� .���  � �� �� .R��� 
���   �$	 1 2R�$��1��$��3q� . �� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FFFFFFFF��$	 0Q�� $ 	#FFF0F0EA��$		 0.9Q��� $
 	#FF003C74���� " ��.���  �@�FocusVisualStyle8   ��  � ��8  ��  � ��8  ��  � ��# � ����  � ��$ Center=���  � ��$ Center=���  � �� �� " �� ���� 
Border .��U� $ 00:00:00X�$ 	rectangle��$ (UIElement.Opacity)�.\��� $ 00:00:00��$ 0Q��� $ 00:00:00.5000000��$ 1Q��� 	BorderOff .��U� $ 00:00:00X�$ 	rectangle��$ (UIElement.Opacity)�.\��� $ 00:00:00��$ 1Q��� $ 00:00:00.5000000��$ 0Q�.R�� $
 grid��      
Common�� . .   	MouseOver��. �� .��r� $ 00:00:00X�$ 00:00:00.0010000D�$ contentPresenter��$ (FrameworkElement.Margin)�.	��� $ 00:00:00��$ 	1,1,-1,-1q�  
Normal��  Pressed��. �� .��r� $ 00:00:00X�$ 00:00:00.0010000D�$ contentPresenter��$ (FrameworkElement.Margin)�.	��� $ 00:00:00��$ 	2,2,-2,-2q��� $ 00:00:00X�$ 00:00:00.0010000D�$ 	rectangle��$, &(Shape.Stroke).(SolidColorBrush.Color)�.g��� $ 00:00:00��$ 	#FF27272C��  	Focus��.   Focused��. �� .��U� $ 00:00:00X�$ 00:00:00.0010000D�$ 	rectangle��$ (UIElement.Opacity)�.\��� $ 00:00:00��$ 1Q�  	Unfocused��.+��� $ 	rectangle��
����������	#FF6F9EB7$ 3��$  3��$! 0Q��� $" contentPresenter��#��z��#��z��# z ��. .��� -   $ 
LayoutRoot��$	��140��$��
0,0,0,0.55q�# � RowDefinitions# �� �� $��0* �.+��� ���	#FFC9C9C9
����  �$	��140��$ 5��$  5���� $��0,5,0,0q�
����  �$	��Top=�$��15��$	��140���� -   $ HeadArea��$��0q�
����  �$	��Top=�$��20��$	��140��$ 5��$  5���� -   $ ��Name$
$ Open��$��26.667,25.5,0,0q�$	��Top=�$��36��$
��Left=�$��36��#���  .���� $��30��$��30��$��images/openButton.png��% � ToolTip$
% Open���� -   $
$ Save��$��0,26.481,28,0q�$	��Top=�$��36��$��Right=�$��36��#���  .���� $��30��$��30��$��images/saveButton.png��$
% Save���� -   $$ PointEraserButton��$
��Left=�$��26,102.5,0,0q�$	��Top=�$��36��$��36��#���  .���� $��30��$��30��$.��(images/点橡皮-默认状态_r2_c11.png��$% Erase By Point���� -   $$ RubberButton��$��0,102.5,28,0q�$	��Top=�$��36��$��Right=�$��36��#���  .���� $��30��$��30��$*��$images/橡皮-默认状态_r2_c7.png��$% Erase By Stroke���� -   $$ Pencil��$��26,64.5,0,0q�$	��Top=�$��36��$
��Left=�$��36��#���  .���� $��30��$��30��$*��$images/画笔-默认状态_r2_c5.png��$	% Pen���� -	   $$ Button_Select��$��0,64.5,28,0q�$	��Top=�$��36��$��Right=�$��36��#���  .���� $��30��$��30��$*��$images/选择-默认状态_r2_c2.png��$% Select���� -
   $ RectangleSlider��$��9.5,339.5,10.5,0q����	#FFFFFFFF���	#FF000000$	��Top=�$	��120��$	��120���� -   & ��Name$& WidthSlider��$��24.5,360.5,0,0q�' �� Orientation$' Vertical=�( � Maximum$( 50Q�) � SmallChange$) 1Q�$
��Left=�$��22��$	��Top=�$��94���� -   * ��Name$* PenWidth��$��8.5,339.5,0,0q�$��PenWidth��$
��Left=�$	��Top=�$��10.667(��� -   + ��Name$+ WidthInt��$��47.5,364,0,0q�$��1��$
��Wrap=����	#FF000000$
��Left=�$��21��$	��Top=�$��20���� -   $$ 
insertText��$��26,139.978,0,0q�#���  $
��Left=�$��36��$��36��$	��Top=�.���� $��30��$��30��$*��$images/文字-默认状态_r9_c2.png��, J� RenderTransformOrigin$, 	0.5,0.517&�$
% Text���� -   $$ insertImage��$��77,139.978,0,0q�#���  $��36��$	��Top=�$
��Left=�$��36��.���� $��30��$��30��$*��$images/图片-默认状态_r8_c5.png��$% Image���� $��7.5,329.924,0,0q����	#FF595959���	#FF000000$
��Left=�$	��Top=�$��2��$	��124���� -   $$ 
undoButton��$��26,215.424,0,0q�#���  $
��Left=�$��36��$	��Top=�$��36������ *CopyImageCommand+.���� $��30��$��30��$+��%images/撤销-默认状态_r13_c2.png��$
% Undo���� -   $$ 
redoButton��$��0,214.424,27,0q�$��Right=�$��36��#���  $	��Top=�$��36��.���� $��30��$��30��$+��%images/重做-默认状态_r12_c5.png��$
% Redo���� -   $$ ClearAll��$��26,176.424,0,0q�#���  $
��Left=�$��36��$	��Top=�$��36��.���� $��30��$��30��$��images/clearAllButton.png��$% 	Clear All��  WPFInk.ColorPicker  -   -  Name$- ColorPicker��  WPFInk.ColorPicker.   ColorChanged$. ColorPicker_ColorChanged��$��10.25,467,0,0q�$
��Left=�$	��Top=�$, 0.827,0.837&��� -   $$ Gesture��$��0,175.924,26.5,0q�$��Right=�$��36��#���  $	��Top=�$��36��.���� $��30��$��30��$��images/gestureButton.png��$
% Redo���� $��60.5,339.5,0,0q�$��
PenOpacity��$
��Left=�$	��Top=�$��10.667(��� -   $+ penOpacityTextBlock��$��70.5,363,0,0q�$	��1.0��$
��Wrap=����	#FF000000$
��Left=�$��20��$	��Top=�$��16���� -   $& OpacitySlider��$��90.5,360.5,0,0q�$' Vertical=�$( 1Q�$
��Left=�$��23��/ � Value$/ 1Q�$% Opacity��$��94��$	��Top=��� -   $$ 	MinButton��$��	115,0,5,0q�$	��Top=�$��20��$��20��  ButtonStyle2#���  .���� $��images/MinButton.png��$��16��$��16���� -   $$ 	MaxButton��$��	115,0,5,0q�$	��Top=�$��20��$��20��#���  $y�	Collapsed=�.���� $��images/MaxButton.png��$��16��$��16��  WPFInk.FontChooser  -   0  Name$0 _fontChooser��$��	9,340,0,0q�$
��Left=�$	��Top=��� -   $$ 
MoveButton��$��28.625,253.424,75.375,0q�$	��Top=�#���  $��36��$��36��.���� $
��Left=�$	��Top=�$��30��$��30��$,��&images/移动-当前状态_r13_c20.png���� -   $$ StrokeToImageButton��$��0,252.424,27,0q�$��Right=�$��36��#���  $	��Top=�$��36��.���� $��30��$��30��$��images/strokeToImage.png��$% StrokeToImage���� -   $$ DrawGraphicButton��$��28.625,288.424,75.375,0q�$	��Top=�#���  $��36��$��36��.���� $
��Left=�$	��Top=�$��30��$��30��$��images/graphicButton.png��$% DrawGraphic���� -   $$ StrokeInGraphicButton��$��0,290.424,26.5,0q�$��36��#���  $	��Top=�$��Right=�$��36��.���� $��30��$��30��$&�� images/strokeInGraphicButton.png��$% StrokeInGraphic���� -   $$ hidden��$��	93,0,27,0q�$	��Top=�$��20��$��20��#���  
���   �.���� $��images/CloseButton.png��$��16��$��16��