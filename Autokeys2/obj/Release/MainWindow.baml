   M S B A M L   `   `   `  ���� C  ?Autokeys, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:Autokeys2.ViewsAutokeys2.Views      Autokeys2.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     /localclr-namespace:Autokeys2.Views     -     � Title$  
Autokeys 2�� � SizeToContent$ WidthAndHeight=� � WindowStyle$
 None=� � AllowsTransparency . ���	#00000000 � 
ResizeMode$ NoResize=� � TaskbarItemInfo *  $System.Windows.Shell.TaskbarItemInfo  -   taskbarInfo��.���  � ColumnDefinitions �� $��* ��� $��20 ��� $	��200 ��� $��20 ���  � RowDefinitions �� $	��335 �$	��335���� $��0��$	��200 �$	��200��.+�    Autokeys2.Views.MacrosView  -     Name$ vewLeft��$��0��$��2���� $��0��$��1��	 �� Orientation$	 Vertical=�$��	0,200,0,0q�.+���    
mainBorder#��[  $��1q�._��� -   $��btnExL��$��>��  expandButton#��[ �� -   $��brdAll��$��0��$��2��#��[  |�. ._�� $	��Top=�  mainGrid#��[  �� $	��200 ��� $��60 ��� $��65��$��65 �.+��� $��0�� 
 vPanel#��[ $
��0,20q�.+��� -   $��	btnRecord��  	recButton#��[ �� -   $��btnPlay��  
playbutton#��[ � $��1�� �� ��  �� $	��80* ��� $	��19* ��� $	��99* �.+��� $��0��$��0��$��Repeats��  myLabel#��[ $��2��$��0,1,1,1q��� $��0��$��1��$��Speed��#��[ $��2��$��0,1,1,1q��� -   $��
txtRepeats��$��2��$��0�� 
 myText#��[ $��1,1,0,1q����� *	Repeats+�� -	   $��txtSpeed��$��2��$��1��#��[ $��1,1,0,1q����� *Speed+� $��2��$��
10,0,10,10q�$��Bottom=�$��40��$��2�� �� �� .+��� -
   $��btnMin��$
��Hide��  
utilButton#��[ $��40��$	��7,0q�$	��Top=��� -   $��btnClose��$��Close��$��1��#��[ 