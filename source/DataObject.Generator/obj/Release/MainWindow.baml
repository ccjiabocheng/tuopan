   M S B A M L   `   `   `  ���� O  KDataObject.Generator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:DataObject.GeneratorDataObject.Generator  %    DataObject.Generator.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     4local"clr-namespace:DataObject.Generator       � WindowStartupLocation$  CenterScreen=� � Title$ AR实体类生成器��$	��600��$	��800�� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     
BlurRadius$ 24Q�   Color$ 	#FF858484��   	Direction$ 90Q�   ShadowDepth$ 3Q�.���  � RowDefinitions �� $��35 ��� $��55 ��� $��300,* ��� $��35 �.+�o� -    o�Name$
 menu��$��0��$��Stretch=�$��35��$	��Top=�.@�m� $��文件��.@�m� $��	存储...��m� $��退出��m� $��连接��.@�m� $��设置��m� $��帮助��.@�m� -   $��关于���� 	 �� Orientation$	 
Horizontal=�$��1��.+��� -   
 ��Name$
 cbDbs�� �� DisplayMemberPath$
 Name�� �� SelectedValuePath$
 Name��$��35�� �� 
LineHeight$ 20��$	��180��$
��Left=�$��
15,10,0,10q�$��10q�   $
 Gray��$ 5Q�$ 2Q�   Opacity$	 0.6Q�$	 270Q��� -    ��Name$ button��$��刷新��$��80��$
��Left=�$��35��$	��Top=�$��
15,10,0,10q�   $
 Gray��$ 5Q�$ 2Q�$	 0.6Q�$	 270Q��� -   $ btnGen��$��生成��{�.  $
��Left=�$	��Top=�$��80��$��35��$��
15,10,0,10q�   $
 Gray��$ 5Q�$ 2Q�$	 0.6Q�$	 270Q�]� $��2��.+�R� -    R�Name$ tvTables��$
 Name��$ 25�� R� SelectedValuePath$
 Name��$
��Left=�$��Stretch=�$	��180��&   System.Windows.Controls.DataGrid  -     Name$ dgFields��$��Stretch=�$��Stretch=��� $��3��$��35��$��Stretch=�$	��Top=�