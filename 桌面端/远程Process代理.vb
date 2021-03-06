﻿''' <summary>
''' 你需要新建一个Windows窗体应用，删除窗体类，新建一个Module，添加Sub Main并设为启动项，在此Sub Main中调用“启动远程代理”方法。最后别忘了在打包项目中引用此项目。
''' </summary>
Public Module 远程Process代理
	WithEvents 进程 As New Process, 控制服务器 As TCP客户端, 数据服务器 As TCP客户端, 错误服务器 As TCP客户端
	''' <summary>
	''' 启动一个远程代理负责操纵桌面应用程序
	''' </summary>
	''' <param name="Command">直接将你的代理进程的命令行参数输入进来，一般就是Command</param>
	Public Sub 启动远程代理(Command As String)
		Dim a As String() = Command.Split(" ")
		Dim e As New TCP侦听器(a(3))
		Process.Start(New ProcessStartInfo("CheckNetIsolation.exe", "LoopbackExempt -a -n=""" & a(2) & """") With {.Verb = "runas"})
		Dim b As 控制类型, c(4) As Boolean, d As Reflection.PropertyInfo
		e.Start()
		控制服务器 = e.AcceptTcpClient
		数据服务器 = e.AcceptTcpClient
		错误服务器 = e.AcceptTcpClient
		e.Stop()
		Do
			b = 控制服务器.接收字节
			Select Case b
				Case 控制类型.Start
					With 进程.StartInfo
						c = 控制服务器.接收布尔数组()
						.Arguments = 控制服务器.接收字符串
						.CreateNoWindow = c(0)
						.RedirectStandardError = c(1)
						.RedirectStandardInput = c(2)
						.RedirectStandardOutput = c(3)
						.UseShellExecute = c(4)
						.FileName = 控制服务器.接收字符串
					End With
					进程.Start()
					Threading.Tasks.Task.Run(Sub()
												 Do
													 进程.StandardInput.WriteLine(数据服务器.接收字符串)
												 Loop
											 End Sub)
				Case 控制类型.BeginErrorReadLine
					进程.BeginErrorReadLine()
				Case 控制类型.BeginOutputReadLine
					进程.BeginOutputReadLine()
				Case 控制类型.PropertyGet
					d = GetType(Process).GetProperty(控制服务器.接收字符串)
					控制服务器.发送(d.GetValue(进程), d.GetType)
				Case 控制类型.PropertySet
					d = GetType(Process).GetProperty(控制服务器.接收字符串)
					d.SetValue(进程, 控制服务器.接收(d.GetType))
				Case 控制类型.Close
					进程.Close()
				Case 控制类型.Dispose
					Exit Do
				Case 控制类型.Kill
					进程.Kill()
			End Select
		Loop
	End Sub

	Private Sub 进程_ErrorDataReceived(sender As Object, e As DataReceivedEventArgs) Handles 进程.ErrorDataReceived
		错误服务器.发送(e.Data)
	End Sub

	Private Sub 进程_OutputDataReceived(sender As Object, e As DataReceivedEventArgs) Handles 进程.OutputDataReceived
		数据服务器.发送(e.Data)
	End Sub

	Private Sub 进程_Exited(sender As Object, e As EventArgs) Handles 进程.Exited
		控制服务器.发送字节(控制类型.Exited)
	End Sub
End Module
