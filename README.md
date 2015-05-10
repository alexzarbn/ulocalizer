# ULocalizer - Tool that makes localization process of Unreal Engine 4 games easier

License: MIT


If you are not familiar with localization process in Unreal Engine 4, check out [documentation page](https://docs.unrealengine.com/latest/INT/Gameplay/Localization/index.html). 
Just for test create any blueprint and add FText variable(NSLOCTEXT for C++), save it and follow the steps below.

Getting started
----------------------------------------
+ When the app is started, click on menu File -> New Project.
+ Choose the path to editor. (example: C:\Program Files\Epic Games\4.7\Engine\Binaries\Win64\UE4Editor.exe)
+ Choose the path to project. (example: D:\Dev\Games\DevEnv\DevEnv.uproject)
+ Localization project name will be generated according to selected .uproject file. If you don't want to have the same name, feel free to change it :)
+ Basically, you will use UTF-8 encoding most of the time, but other encodings are also supported.
+ If you want to change the location of generated localization files, Source and Destination paths fields is what you need.
+ Click Create and wait until files will be generated, then you are ready to go. Don't forget to click on Project -> Build to generate updated localization data for ue4 after changes.

Automatic translation usage
----------------------------------------
+ Go to [Yandex Translate API](https://tech.yandex.com/translate/)
+ Click on "Get a free API key"
+ If you don't have an account there, first it will ask you to create such one, then you can access API key request.
+ Agree with terms and click "Get API key" and copy the presented key string.
+ Open ULocalizer, go to Settings (right corner of the app) and paste previously copied API key.
+ That's it, you're ready to go! Open your project and choose what you would like to translate: whole project, selected language, selected node or selected items.