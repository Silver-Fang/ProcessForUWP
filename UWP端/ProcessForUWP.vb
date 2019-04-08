Imports ProcessForUWP.桌面端
Public Class DataReceivedEventArgs
	Inherits EventArgs
	Property Data As String
	Sub New(Data As String)
		Me.Data = Data
	End Sub
End Class
Public Delegate Sub DataReceivedEventHandler(sender As Process, e As DataReceivedEventArgs)
''' <summary>
'''<para>继承自System.Diagnostics.Process，但可以在UWP平台使用！使用方法基本和原版一样。此类必须配合打包项目中的Package.appxmanifest中的设定使用。具体方法：</para>
'''<para>1、在Package节点中增加以下属性：</para>
'''<para>xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"</para>
'''<para>xmlns:desktop="http://schemas.microsoft.com/appx/manifest/desktop/windows10"</para>
'''<para>2、在Package.appxmanifest中的Package\Applications节点中增加以下子节点：</para>
'''<para>&lt;Extensions&gt;</para>
'''<para>&#9;&lt;desktop:Extension Category = "windows.fullTrustProcess" Executable="【此处替换为你的代理进程所在路径，通常为项目名\可执行文件名.exe】"&gt;</para>
'''<para>&#9;&#9;&lt;desktop: FullTrustProcess&gt;</para>
'''<para>&#9;&#9;&#9;&lt;desktop:ParameterGroup GroupId = "环回配置" Parameters="【此处替换为你的包系列名，例：642671AC6A72D.52333923F7214_9vcz5tcd8ce5e】【此处替换为空格1个】【此处替换为TCP回环所使用的端口号，例：32768】"  /&gt;</para>
'''<para>&#9;&#9;&lt;/desktop:FullTrustProcess&gt;</para>
'''<para>&#9;&lt;/desktop:Extension&gt;</para>
'''<para>&lt;/Extensions&gt;</para>
'''<para>3、在Package\Capabilities节点中增加以下子节点：</para>
'''<para>&lt;rescap:Capability Name="runFullTrust" /&gt;</para>
'''<para>当然，别忘了在打包项目中引用你的UWP应用项目</para>
''' </summary>
Public Class Process
	Inherits System.Diagnostics.Process
	WithEvents 控制客户端 As TCP客户端, 错误客户端 As TCP客户端
	''' <summary>
	''' 如有需要，你可以直接从TCP客户端接收信息。
	''' </summary>
	Public WithEvents 数据客户端 As TCP客户端
	Shadows Event OutputDataReceived As DataReceivedEventHandler
	Shadows Event ErrorDataReceived As DataReceivedEventHandler
	Shadows Event Exited As EventHandler
	Private Function PropertyGet(Name As String) As Object
		With 控制客户端
			.发送字节(控制类型.PropertyGet)
			.发送(Name)
			PropertyGet = .接收([GetType].GetProperty(Name).GetType)
		End With
	End Function
	Private Sub PropertySet(Name As String, value As Object)
		With 控制客户端
			.发送字节(控制类型.PropertySet)
			.发送(Name)
			.发送(value, [GetType].GetProperty(Name).GetType)
		End With
	End Sub
	Overloads Property BasePriority As Integer
		Get
			Return PropertyGet("BasePriority")
		End Get
		Set(value As Integer)
			PropertySet("BasePriority", value)
		End Set
	End Property
	Overloads Property HasExited As Boolean
		Get
			Return PropertyGet("HasExited")
		End Get
		Set(value As Boolean)
			PropertySet("HasExited", value)
		End Set
	End Property
	Overloads ReadOnly Property ProcessName As String
		Get
			Return PropertyGet("ProcessName")
		End Get
	End Property
	Overloads ReadOnly Property WorkingSet64 As Integer
		Get
			Return PropertyGet("WorkingSet64")
		End Get
	End Property
	Overloads ReadOnly Property StandardInput As BinaryWriter
	Overloads ReadOnly Property StandardOutput As BinaryReader
	Overloads ReadOnly Property StandardError As BinaryReader
	''' <summary>
	''' 新建一个Process实例
	''' </summary>
	''' <param name="端口">环回通信的端口号，必须和你在打包项目的Package.appxmanifest中设定的值一致。</param>
	Sub New(Optional 端口 As UShort = 32768)
		Call FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync("环回配置")
		控制客户端 = New TCP客户端(端口)
		数据客户端 = New TCP客户端(端口)
		错误客户端 = New TCP客户端(端口)
		StandardInput = 数据客户端.流写入器
		StandardOutput = 数据客户端.流读取器
		StandardError = 错误客户端.流读取器
	End Sub
	Overloads Sub Start()
		控制客户端.发送字节(控制类型.Start)
		With StartInfo
			控制客户端.发送({ .CreateNoWindow, .RedirectStandardError, .RedirectStandardInput, .RedirectStandardOutput, .UseShellExecute})
			控制客户端.发送(.Arguments)
			控制客户端.发送(.FileName)
		End With
		控制客户端.监听字节()
	End Sub
	Overloads Sub BeginErrorReadLine()
		控制客户端.发送字节(控制类型.BeginErrorReadLine)
		错误客户端.监听字符串()
	End Sub
	Overloads Sub BeginOutputReadLine()
		控制客户端.发送字节(控制类型.BeginOutputReadLine)
		数据客户端.监听字符串()
	End Sub
	Overloads Sub Close()
		控制客户端.发送字节(控制类型.Close)
	End Sub

	Overloads Sub Dispose()
		控制客户端.发送字节(控制类型.Dispose)
		MyBase.Dispose()
	End Sub
	Overloads Sub Kill()
		控制客户端.发送字节(控制类型.Kill)
	End Sub
	Private Sub 数据客户端_收到消息(sender As TCP客户端, 消息 As String) Handles 数据客户端.收到字符串
		RaiseEvent OutputDataReceived(Me, New DataReceivedEventArgs(消息))
	End Sub

	Private Sub 错误客户端_收到消息(sender As TCP客户端, 消息 As String) Handles 错误客户端.收到字符串
		RaiseEvent ErrorDataReceived(Me, New DataReceivedEventArgs(消息))
	End Sub

	Private Sub 控制客户端_收到字节(sender As TCP客户端, 字节 As Byte) Handles 控制客户端.收到字节
		Select Case 字节
			Case 控制类型.Exited
				RaiseEvent Exited(Me, New EventArgs)
		End Select
	End Sub
End Class
