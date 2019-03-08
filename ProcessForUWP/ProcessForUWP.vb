Imports 简单环回运输层通信, 控制协议
Public Class DataReceivedEventArgs
	Inherits EventArgs
	Property Data As String
	Sub New(Data As String)
		Me.Data = Data
	End Sub
End Class
Public Delegate Sub DataReceivedEventHandler(sender As Process, e As DataReceivedEventArgs)
''' <summary>
''' 继承自System.Diagnostics.Process，但可以在UWP平台使用！使用方法基本和原版一样。
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
	''' 启动一个代理进程
	''' </summary>
	''' <param name="端口">环回通信的端口号，必须和你的Package.appxmanifest中设定的值一致。</param>
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
