   M S B A M L   `   `   `  ����J  FChineseChess_G1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:ChineseChess_G1ChineseChess_G1  5   	        ChineseChess_G1.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:ChineseChess_G1     5   	     � Icon$  Resources/logo.png��5   	    � Title$! WPF Chinese Chess - Group 1��6#    � SizeToContent$ WidthAndHeight=�6G    � WindowStartupLocation$ CenterScreen=�6f    � 
ResizeMode$ CanMinimize=�6�   ��5	      �� 5
   
    �� ImageSource$ Resources/background.jpg��6   .��5      � -   5        $��Window��5      .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��25,25,25,25q�6.   .+�5      � -   5        $��
chessPanel��5      $	��675��6%   $	��600��62   $	��Top=�6>   $
��Left=�6V   ��5      �� 5      $ Resources/panel.png��6!   5      �� 5      -   5        $��Sidebar��5      $	��250��6(   $��35,0,0,0q�64   .+�5      �� -   5        $��GameControl��5      .+�5      �� -   5        $��GameControlTitle��5       $��Founction Buttons: ��68   $��Segoe Script)�6S   $��	ExtraBold"�6m   $��22(�6�   �� 5      -   5        $��
btnRestart��5      $��Restart��6/   $y�	Collapsed=�6[      MyButton#��[  6r   �� 5      -   5        $��btnStart��5      $��Start��6-   #��[  6U   �� 5      -   5        $��btnWithdraw��5      $��Withdraw��60   $y�	Collapsed=�6^   #��[  6u   �� 5      -	   5        $��	btnRegret��5      $��Regret��6.   $y�	Collapsed=�6W   #��[  6n   5      �� 5      -
   5        $��
ManualDemo��5      $y�	Collapsed=�6/   .+�5       �� $��Resources/boarder.png��6   $��	0,15,0,15q�6;   �� 5!      -   5        $��ManualDemoTitle��5!       $��Manual Demo:��67   $��Segoe Script)�6K   $
��Bold"�6e   $��20(�6w   �� 5"      -   5        $��btnPlay��5"      $��Play Manual��6,   $y�Visible=�6Y   #��[  6n   �� 5#      -   5        $��btnOpen��5#      $��Open Manual��6,   $y�Visible=�6Y   #��[  6n   �� 5$      -   5         ��Name$	 URL��5$   !   $��0,10,0,0q�6.   $y�	Collapsed=�6A   ��5%      �� 5&       �� Color$ AntiqueWhite��6.   	 �� Opacity$		 0.8Q�6C   .+�5(      �� $��Current Manual URL:��6$   $��15(�6?   $��Italic$�6M   $
��Bold"�6`   �� 5)      -   5        $��	txtblkUrl��5)   $   $��12(�65   $��url address��6C   
����  �$
��Wrap=�6h   5*      5,      �� 5.      -   5        $��Message��5.      $y�	Collapsed=�6,   .+�5/      �� $��Resources/boarder.png��6   $��	0,20,0,20q�6;   �� 50      ��51      �� 52      $ AntiqueWhite��6.   $		 0.8Q�6C   .+�54      �� $��Message:��6$   $��20(�64   $��Italic$�6B   $
��Bold"�6U   �� 56      $ 
Horizontal=�6%   $��Center=�6>   $��0,20,0,0q�6[   .+�57      �� -   5        
 ��Name$
 TimerR��57   (   $��Remain Time:  ��68   $��18(�6N   
���   �$��40��6o   �� 58      -   5        $��txtblkTimer��58   (   $��18(�6<   $
��Bold"�6J    �� TextAlignment$ Center=�6\   �� 59      -   5        $
 TimerS��59   (   $��	  seconds��68   $��18(�6I   
���   �$��40��6j   5:      �� 5;      -   5        $��txtblkRound��5;   $   $��15(�67   $
��Bold"�6E   $��Round��6W   $y�	Collapsed=�6d   �� 5<      -   5        $��txtblkCrrClr��5<   "   $��Bottom=�66   $��0,0,0,15q�6Q   
�������$��20(�6v    �� TextAlignment$ Center=�6�   $y�	Collapsed=�6�   �� 5=      -   5        $��txtblkMessage��5=   $   $��0,0,0,15q�69   $
��Bold"�6K   $��20(�6]   $ Center=�6k   $
��Wrap=�6�   5>      5?      5A      5B      5C      5D      