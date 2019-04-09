Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' 有关程序集的一般信息由以下
' 控制。更改这些特性值可修改
' 与程序集关联的信息。

'查看程序集特性的值

<Assembly: AssemblyTitle("ProcessForUWP.UWP端")>
<Assembly: AssemblyDescription("这是对System.Diagnostics.Process类的高仿。我们知道，在UWP中一般是不允许对此类直接的使用的。而通过本高仿类可以间接地使用此类，实现用UWP启动桌面进程，并与其进行标准输入输出的交互。此项目分两个包，此包是UWP端，依赖桌面端")>
<Assembly: AssemblyCompany("翁悸会")>
<Assembly: AssemblyProduct("ProcessForUWP")>
<Assembly: AssemblyCopyright("")>
<Assembly: AssemblyTrademark("")> 

' 程序集的版本信息由下列四个值组成: 
'
'      主版本
'      次版本
'      生成号
'      修订号
'
'可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值
'通过使用 "*"，如下所示:
' <Assembly: AssemblyVersion("1.0.*")> 

<Assembly: AssemblyVersion("1.0.0.0")> 
<Assembly: AssemblyFileVersion("1.0.0.0")> 
<Assembly: ComVisible(false)>