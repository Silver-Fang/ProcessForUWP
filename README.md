这是对System.Diagnostics.Process类的高仿。我们知道，在UWP中一般是不允许对此类直接的使用的。而通过本高仿类可以间接地使用此类，实现用UWP启动桌面进程，并与其进行标准输入输出的交互。使用方法：
在你的解决方案中添加一个打包项目和一个空白桌面应用项目。在打包项目中引用你的UWP项目和桌面应用项目。在UWP项目中添加引用“UWP端.dll”，在桌面应用项目中引用“桌面端.dll”。注意UWP端.dll是依赖桌面端.dll的，所以桌面端.dll也需要添加到你的UWP项目中，但是不需要添加引用。然后在打包项目的Package.appxmanifest中：
在Package根节点下添加属性：xmlns:desktop="http://schemas.microsoft.com/appx/manifest/desktop/windows10"
在Package\Applications\Application下添加子节点：
			<Extensions>
				<desktop:Extension Category="windows.fullTrustProcess" Executable="【桌面应用项目的路径，如：远程代理\远程代理.exe】">
					<desktop:FullTrustProcess>
						<desktop:ParameterGroup GroupId="环回配置" Parameters="【包系列名+空格+TCP环回端口号，如：642671AC6A72D.52333923F7214_9vcz5tcd8ce5e 32768】"  />
					</desktop:FullTrustProcess>
				</desktop:Extension>
			</Extensions>
在你的空白桌面项目中添加新代码文件，输入以下代码：
Module 远程代理
	Sub Main()
		ProcessForUWP.桌面端.启动远程代理(Command)
	End Sub
End Module
在项目属性中将启动项目设为Sub Main。
在你的UWP项目中需要使用Process类的地方，务必在代码文件中Imports ProcessForUWP.UWP端，否则会变成使用原本的Process类了。其它代码不需要修改。
在解决方案配置管理器中，三个项目的平台需保持一致，建议都设为x64。生成都要勾选。部署只需勾选打包项目，UWP项目和桌面项目都不需部署。
解决方案的启动项目应设为打包项目。
如果你有其它需求也可以尝试在UWP项目中直接调用桌面端DLL，但是此用法未经测试，可用性未知，谨慎使用。

此类库的实现原理是用runFullTrust功能启动代理桌面进程，再利用TCP本地环回实现UWP与代理进程的通信，通过代理进程的中介最终实现与目标进程的交互。商店应用默认不允许使用TCP环回，因此需使用CheckNetIsolation解除此限制，具体参见源码。欢迎建议和代码贡献。
本人使用此类库实现了一个示例应用并成功通过审核发布到商店。你可以在商店搜索“控制台应用外壳”试用。
