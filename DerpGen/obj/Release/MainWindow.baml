   M S B A M L   `   `   `  ���� B  >DerpGen, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:DerpGenDerpGen      DerpGen.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:DerpGen     -     � Title$  DerpGen 1.0��$
��1100��$
��3048��$
��1500��$	��800�����	#FF6C6C6C   CustomWindowStyle#���    � 	Resources  �� 
TargetType 
 DarkBG �� Property �� Value 
 GradBG �� EndPoint �� 
StartPoint � Color � Offset  inpDark  btnDark  sepDark  prgDark  lstDark��%  &	       &	 +     &	 �     &	 
    &	 q    &	 �    &	 T    �� " �.���  � �� �	#FF252526�� " �.���  � �� �� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF0F0F0F��$ 0Q�� $ 	#FF252526��$ 1Q��� " ��.���  � �� �	#FF333337��  � �� �	#FF434346��  � �� �	#FFEBEBEB�� " ��.���  � �� �	#FF007ACC��  � �� �	#FF0C67A4��  � �� �	#FFEBEBEB�� " ��.���  � �� �	#FF3F3F46�� " �.���  � �� �	#FF333337��  � �� �	#FF434346��  � �� �� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF0098FF��$ 0.177Q�� $ 	#FF005893��$ 1Q��� " ��.���  � �� �	#FF333337��  � �� �	#FF434346��  � �� �	#FFEBEBEB.��� .+�]� .+�o� -   	 o�Name$	 MainMenu��$	��Top=����	#FF313134.@�m� $��_File��.@�m� -   $
��_New��$��1q�  SingleDropDownMenuItem#���  m� -   $��_Open��#���  m� -   
 m�Name$
 recentMenuItem��$��Open Recent�� 	 DropDownMenuItemStyle#��� 	 .@�m� $��
Clear List��#���  �� #��[ m� -   $��_Save��#���  m� -   $��_Save as...��#���  m� -   $��_Export as PNG��#���  m� -	   $��_Properties��#���  �� #��[ m� $��_Exit��#���  m� $
��Help��.@�m� -
   $��_Documentation��#���  � $��0,20,0,0q�#��[  � ColumnDefinitions �� $��* �$	��200���� $
��auto �$	��500���� $��* �$	��200��.+��� $��0��.+��� $��Noise Settings��$��Center=�$��0,8,0,20q�$��16(�
�������� $��10q� �� $��* ��� $��* �.+��� $��0��.+��� $��Map Size��$	��3.1q��� $
��Seed��$	��3.1q��� $��Noise Scale��$	��3.1q��� $��Octaves��$	��3.1q��� $��Persistence��$	��3.1q��� $��
Lacunarity��$	��3.1q��� $��1��.+��� -   $��inpSize��$��2q� � ToolTip$) #Set the resolution of you heightmap��#��[ ���� *	MapSize+ �� Mode$ TwoWay=��  �� $��* ��� $��* �.+��� -   $��inpSeed��$��2q�$��0��$ Seed value for heightmap��#��[ ���� *Seed+$ TwoWay=��� -   $��Random��$��1��$��2q�$! Generates random seed value��#��[ �� -   $��inpScale��$��2q�$F @Scales the heightmap with out affecting size of the actual image��#��[ ���� *Scale+$ TwoWay=��  �� $��90 ��� $��* �.+��� -   $��inpOct��$��2q� � Minimum$ 1Q� � Maximum$ 10Q�$��0��$ number of LOD for noise�� � Value �� *	Octaves+$ TwoWay=��� $��2q�$��1��#��[ ���� *	Octaves+$ TwoWay=��  �� $��90 ��� $��3* ��� $��4* �.+��� -   $��inpPer�� �� IsSnapToTickEnabled .  $��2q�$ 0Q�$ 1Q�$��0��$< 6How much each octave contributes to the overall shape.�� �� *Persistence+$ TwoWay=��� $��2q�$��1��$��2��#��[ ���� *Persistence+$ TwoWay=��� -   $��inpLac��$��2q�$9 3how much detail is added or removed at each octave.��#��[ ���� *
Lacunarity+$ TwoWay=��� $��0,10,0,0q�#��[ �� $��FallOff Settings��$��Center=�$�� 0, 20, 0, 10q�� $��10q� �� $��* ��� $��* �.+��� $��0��.+��� $��Radius��$��2q��� $��1��.+��� -   $��	inpRadius��$��2q�$ Radius for falloff size��#��[ ���� *Radius+$ TwoWay=��� $��Offset Settings��$��Center=�$�� 0, 20, 0, 10q�� $��10q� �� $��* ��� $��* �.+��� $��0��.+��� $��Offset x��$��2q��� $��Offset y��$��2q��� $��1��.+��� -   $��
inpOffsetX��$��2q�#��[ ���� *	OffsetX+$ TwoWay=��� -   $��
inpOffsetY��$��2q�#��[ ���� *	OffsetY+$ TwoWay=��  �� $��* ��� $��* �.+��� -   $��btnGen��$��Generate��$��	5,10,5,20q�$��Bottom=�$ Renders the image preview��$��24�����	#FF007ACC���	#FF0C67A4�� -   $��btnAbort��$��Abort��{�.  $��1��$��	5,10,5,20q�$��Bottom=�$ Renders the image preview��$��24�����	#FF0C67A4���	#FF007ACC� $��1��#��[  � RowDefinitions �� $��* ��� $
��auto �.+��� -    ��Name$ 	MainImage��$��
40,40,40,0q�$��0��� -    �Name$
 pBar��$��1��$��Bottom=�$��25��$��
40,0,40,20q�#��[ � $��0,20,0,0q�$��2��#��[  �� $
��auto ��� $��* ��� $
��auto �.+��� $��
Output Log��$��Center=�$��16(�$��0���� -    ��Name$ outpotLogBox��$��1��$��
10,10,10,0q�#��[ �� -   $��Clear��$��
10,0,10,20q�$��2��$��20��$��Bottom=�#��[ 