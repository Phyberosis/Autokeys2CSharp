   M S B A M L   `   `   `  ����D  @Autokeys2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:Autokeys2.ViewsAutokeys2.Views  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   TreeViewItemFocusVisual  �� Property �� Value  	TreeArrow  ExpandCollapseToggleStyle �� 
TargetType �� 
TargetType �� Padding F�Name �� Angle �� CenterX �� CenterY	 �� Triggers
 P� Property P� Value �� 
TargetName ]� 
Conditions �� Property �� Value  macrosTreeItemStyle �� BasedOn � ColumnDefinitions � RowDefinitions d�Name �� 	ClickMode d� 	IsChecked �� RelativeSource ��Name �� CornerRadius J� SnapsToDevicePixels ��Name �� HeaderStringFormat �� ContentStringFormat ��Name�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      'localclr-namespace:Autokeys2.Views  5      %?  &	        &	 n     &	 �     7  &	     7Q�7  �� 5      .�5   
   ��   � �� 5      �� 5      .R�5	      �� 5
      5      5      B� 5      $��M0,0 L0,6 L6,0 zC�6%   �� 5      " d�6.   .�5   
   ��   � |� .  6&   �� 5   
     � ��$ 16��6"   �� 5   
     � ��$ 16��6#   �� 5   
     � �� 5      �� 5      " d�6"   .R�5      �� $��16��6   $��16��6(   
������ $ 5,5,5,5q�6M   ._�5      F� $ 
ExpandPath��6   ���	#FF7ED3FB���	#FF7ED3FB8��  z�5      �� 5   "   $	 135Q�62   $ 3Q�6>   $ 3Q�6J   5      5      	 5      P� 5       
 �@J�IsMouseOver . . �5!      �� $ 
ExpandPath��67     � ��
 �����6O   �� 5"      $ 
ExpandPath��65     � ��
 ���� 6M   5#      ]� 5%       5&      �� 5'   "    �@J�IsMouseOver . 6C   �� 5(   "    �@d�	IsChecked$
 TrueY�6A   ./�5*      �� $ 
ExpandPath��6@     � z� 5+   "   �� 5,   &   $	 180Q�66   $ 3Q�6B   $ 3Q�6N   5.      �� 5/      $ 
ExpandPath��67     � ��
 �����6O   �� 50      $ 
ExpandPath��65     � �� �	#FF7ED3FB6M   51      ]� 53       54      �� 55   "    �@J�IsMouseOver .  6C   �� 56   "    �@d�	IsChecked$
 TrueY�6A   ./�58      �� $ 
ExpandPath��6@     � z� 59   "   �� 5:   &   $	 180Q�66   $ 3Q�6B   $ 3Q�6N   5<      �� 5=      $ 
ExpandPath��65     � ��
 ���� 6M   �� 5>      $ 
ExpandPath��67     � �� �	#FF7ED3FB6O   5?      5A      5C      5D      �� 5F      " Q�6(   8   .�5G   
   ��   � ��$ Center=�67   �� 5H   
     � ��
 �����6'   �� 5I   
     � �� 5J      �� 5K      " Q�6"   .R�5L      �� .+�5M      �  5N      �� 5O   "   $
��Auto �63   $��19��5P   !   �� 5Q   "   $��* �63    5S      �� 5T   "   $
��Auto �60   �� 5U   "   .+�5W      d� $ Expander��6+   8�� $ Press=�6p    6�   �� *
IsExpanded+ �� *TemplatedParent+�� 5X      $ Bd��6%   $ 3��5Y   %   $��Stretch=�5Z   %   $��1q�5[   %   # z��5\   %   #��z��5]   %    . $��1��5_   %   ._�5`   "   �� $ PART_Header��63   #��z��5a   5   #��z��5b   5   # z 5c   5   #��z��5d   5   $��Header��5e   5   $��Stretch=�5f   5   # z 5g   5   5h      �� 5i      $ 	ItemsHost��6-   $��1��5j      $��1��5k      5l      5m      	 5n      P� 5o      
 �@Q�
IsExpanded .  . �5p      �� $ 	ItemsHost��6%     � y�$ 	Collapsed=�6\   5q      P� 5r      
 �@��HasItems .  . �5s      �� $ Expander��6%     � y�$ Hidden=�6[   5t      P� 5u      
 �@Q�
IsSelected . . �5v      �� $ Bd��6%   # � ��6L     � ���� 5w      # � ��5x   %     � ��5y      5�      5�      5�      5�      