   M S B A M L   `   `   `  ���� C  ?Autokeys, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:Autokeys2.ViewsAutokeys2.Views       Autokeys2.Views.MacrosView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     /localclr-namespace:Autokeys2.Views     -   $��brdLeft��|�.   � FocusVisualStyle  Z�  � 	Resources    splitter �� 
TargetType �� Property �� Value  treeItemStyle �� Color �� 
TargetType �� CornerRadius J� SnapsToDevicePixels	 �� Triggers
 P� Property P� Value  treeItem �� DataType%   Autokeys2.Views.LibraryTreeItem ��Name  
treeFolder'   !Autokeys2.Views.LibraryTreeFolder �� ItemTemplate �� ItemsSource  clearTextbox�� %&  &	        &	 v     &	 �    7  &	 .    7  7  �� " ��.���  � ��$ 4����  � �� �	#FF7ED3FB��  � ��$ Stretch=���  � ��$ Stretch=��� " ��.���  � �� �� $ Transparent����  � ��
 �������  � ��
 ���� ��  �@��
CaretBrush �� $ White����  � {� .  ��  � �� �� " ��.R��� $��Bd��#��z��$ 5��#��z��#��z�� . ._��� $��PART_ContentHost��#��z��# z 	 P� 
 �@J�IsKeyboardFocused . . ���  � �� �	#FF7ED3FB�� "  .R�]� ��. .+��� $ 	txtHeader��8��  ���� *Header+�� "  8    �� *
Children+.R��� $��2q�._�]� ��. .+��� $
��Left=�$��20��$5��/pack://siteoforigin:,,,/Resources/imgFolder.png���� $ 	txtHeader��8�� ���� *Header+.���� -     
leftBorder#��[  �� Padding$ 10q�._�� $	��200�� � ColumnDefinitions �� $	��300��$	��500 ��� $��5 ��� $��* �.+�]� $��0��.+�]� $	��Top=�$��30��.+���  �� Orientation$ 
Horizontal=�$��Right=�.+��� -    ��Name$ btnSave��$
��Save��|�.   
utilButton#��[ $��Stretch=��� -   $ btnLoad��$
��Load��|�. #��[ $��Stretch=��� #��[ $ 5q�._��� -   $ txtFileName��  fileNameBox#��[ ]� .+�� $	��Top=�$��
0, 3, 0, 0q� �� $��61 ��� $��2 ��� $	��150 ��� $��2 ��� .+��� $��Do at
(sec)��  keyframeHeaderBox#��[ �� $��1�� 	 	separator#��[	 �� $��Action��$��2��#��[ �� $��3��#��[	 �� $��Description��$��4��#��[ �� $��Bottom=�.���� -    ��Name$ keyframesControl��
������ ���� *	Keyframes+�� $��1��#��[  {�.  ]� $��2��$��12, 70, 0, 0q�$	��120��$	��180��$
��Left=�$	��Top=�.+�� $	��Top=�$��Stretch=�$��Stretch=� �� ��  � RowDefinitions �� �� �� .+��� -   $ btnUp��$��Move
Up��$��0��$��0�� 
 trayUtilButton#��[
 �� -   $ btnDn��$��	Move
Down��$��0��$��1��#��[
 �� -	   $ btnAddM��$��	Add
Mouse��$��1��$��0��{�. #��[
 �� -
   $ btnAddK��$��Add
Key��$��1��$��1��{�. #��[
 �� -   $ btnDel��$��Delete��$��0��$��2��#��[
 �� -   $ btnDis��$��
Distribute��$��1��$��2��{�. #��[
 