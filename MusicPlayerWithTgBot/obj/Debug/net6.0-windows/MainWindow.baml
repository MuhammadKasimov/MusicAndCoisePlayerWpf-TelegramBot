   M S B A M L   `   `   `  ????O  KMusicPlayerWithTgBot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:MusicPlayerWithTgBot.Controllers MusicPlayerWithTgBot.Controllers  5      %    MusicPlayerWithTgBot.MainWindow   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Ccontrols.clr-namespace:MusicPlayerWithTgBot.Controllers     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      _ [MaterialDesignThemes.Wpf, Version=4.6.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  5      -   5          ? Title$  
MainWindow??5	      $	??800??5
      $	??450??5       ? 
ResizeMode$ CanMinimize=?5       ? WindowStyle$
 None=?5       ? 	Resources 5      '  !MaterialDesignThemes.Wpf.PackIcon   PlayIcon   Kind  	PauseIcon??%Q   &	        &	        5   
   $
 Play=?6'     5   
   $ Pause=?6(   5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $??30 ?6   ?? 5      ?? 5      $	??200 ?6   .+?5   
   ?? -   5         ??Name$ SettingsBtn??5      $??30??5      $??30??5      $
??40,0q?5      $??5q?5      $??Right=?5       .??5"        $ Settings=?6   5#      ?? 5$   
   -   5        $??0??5%      $	??725??5&      $
??Left=?5'      
???????5)      ?? 5*   
   -   5        $ CloseBtn??5+      $??30??5,      $??30??5-      $	??5,0q?5.      $??5q?5/      $??Right=?50      
???  ??$??0q?52      .??54        $ 	CloseBold=?6   55      r? 56   
   -   5         r?Name$ MusicMediaEl??57      $??1??58       r? LoadedBehavior$ Manual=?59      	 r? Stretch$
	 Fill=?5<      
 r? UnloadedBehavior$

 Stop=?5=      ?? 5>   
   -   5         ??Name$ FileNameTxt??5?      $??2??5@      $??	0,100,0,0q?5A      $
??Left=?5B      $??Center=?5C      $??10(?5D      ?? 5E   
   -   5        $ StopBtn??5F      $??2??5G      $??0,110,200,0q?5H       & "MaterialDesignFloatingActionButton#??[ 5J      .??5K        $??40??5L      $??40??5M      
???   ?$
 Stop=?5O      5P      ?? 5Q   
   -   5        $ PLayOrPauseBtn??5R      $??2??5S      $??70??5T      $??70??5U      $??10q?5V      $??Center=?5W      $??Bottom=?5X      $??0q?5Y      #??[ 5[      .??5\      ?? -	   5         ??Name$ PlayOrStopIcon??5]      $??50??5^      $??50??5_      $??Center=?5`      $??Center=?5a      $??Images/pause.png??5b      5c      ?? 5d   
   $??1??5e      $	??220??5f      $
??Left=?5g      $	??Top=?5h      .??5i      ?? -
   5         ??Name$ 
AudiosList??5i      $	??780??6/   5j      ?? 5l   
   -   5         ??Name$ VolumeSlider??5m      $??2??5n      $	??150??5o      $??
450,0,0,40q?5p      $??Bottom=?5q       ? Maximum$ 1Q?5r       ? Minimum$ 0Q?5s       ? Value$ 1Q?5u      ?? 5w   
   -   5        $??TimelineSlider??5x      $??2??5y      $	??400??5z      $??25??5{      $??5q?5|      5~      5?      5?      