Imports System.Net, System.Net.Sockets, System.Threading, System.IO
''' <summary>
''' 将TcpListener功能限定为环回通信
''' </summary>
Public Class TCP侦听器
	Inherits TcpListener
	Sub New(端口号 As UShort)
		MyBase.New(New IPEndPoint(IPAddress.Loopback, 端口号))
	End Sub
	Overloads Function AcceptTcpClient() As TCP客户端
		Return New TCP客户端(MyBase.AcceptTcpClient)
	End Function
	Overloads Function AcceptTcpClientAsync() As Task(Of TCP客户端)
		Return Task.Run(Function() As TCP客户端
							Return AcceptTcpClient()
						End Function)
	End Function
End Class
Public Delegate Sub 收到字符串EventHandler(sender As TCP客户端, 字符串 As String)
Public Delegate Sub 收到字节EventHandler(sender As TCP客户端, 字节 As Byte)
''' <summary>
''' 将TcpClient功能限定为环回通信
''' </summary>
Public Class TCP客户端
	Event 收到字符串 As 收到字符串EventHandler
	Event 收到字节 As 收到字节EventHandler
	ReadOnly Property 流读取器 As BinaryReader
	ReadOnly Property 流写入器 As BinaryWriter
	ReadOnly 基础客户端 As TcpClient
	''' <summary>
	''' 注意该方法会阻塞线程，在连接成功之前不会返回。
	''' </summary>
	''' <param name="端口号"></param>
	Sub New(端口号 As UShort)
		Dim b As New Text.StringBuilder
		基础客户端 = New TcpClient
		Do
			Try
				基础客户端.Connect(IPAddress.Loopback, 端口号)
				Exit Do
			Catch ex As Exception
				Thread.Sleep(1000)
			End Try
		Loop
		Dim a As NetworkStream = 基础客户端.GetStream
		流读取器 = New BinaryReader(a)
		流写入器 = New BinaryWriter(a)
	End Sub
	Friend Sub New(基础 As TcpClient)
		基础客户端 = 基础
		Dim a As NetworkStream = 基础客户端.GetStream
		流读取器 = New BinaryReader(a)
		流写入器 = New BinaryWriter(a)
	End Sub
	Sub 发送(字符串 As String)
		流写入器.Write(字符串)
	End Sub
	Sub 发送(布尔数组 As Boolean())
		Dim b As Byte = If(布尔数组(0), 1, 0)
		For a As Byte = 1 To 布尔数组.GetUpperBound(0)
			b = b * 2 + If(布尔数组(a), 1, 0)
		Next
		With 流写入器
			.Write(CByte(布尔数组.Length))
			.Write(b)
		End With
	End Sub
	Sub 发送(整数 As Integer)
		流写入器.Write(整数)
	End Sub
	Sub 发送字节(字节 As Byte)
		流写入器.Write(字节)
	End Sub
	Sub 发送(消息 As Object, 类型 As Type)
		Select Case 类型
			Case GetType(Integer)
				流写入器.Write(DirectCast(消息, Integer))
			Case GetType(Boolean)
				流写入器.Write(DirectCast(消息, Boolean))
			Case GetType(String)
				流写入器.Write(DirectCast(消息, String))
		End Select
	End Sub
	Function 接收字节() As Byte
		Return 流读取器.ReadByte
	End Function
	Function 接收字符串() As String
		Return 流读取器.ReadString
	End Function
	Function 接收布尔数组() As Boolean()
		Dim a As Byte = 流读取器.ReadByte, c As Byte = 流读取器.ReadByte
		Dim d(a - 1) As Boolean
		For b As SByte = a - 1 To 0 Step -1
			d(b) = c Mod 2 = 1
			c = Math.Floor(c / 2)
		Next
		Return d
	End Function
	Function 接收整数() As Integer
		Return 流读取器.ReadInt32
	End Function
	Function 接收(类型 As Type)
		Select Case 类型
			Case GetType(Integer)
				Return 流读取器.ReadInt32
			Case GetType(Boolean)
				Return 流读取器.ReadBoolean
			Case GetType(String)
				Return 流读取器.ReadString
			Case Else
				Return Nothing
		End Select
	End Function
	''' <summary>
	''' 出于性能考虑，监听一旦开始就无法终止，只能释放整个对象。如果你需要监听功能，请单独设置一个专用的TCP客户端。如果同时使用其它接收功能，将产生未知行为。
	''' </summary>
	Sub 监听字符串()
		Task.Run(Sub()
					 Do
						 RaiseEvent 收到字符串(Me, 流读取器.ReadString)
					 Loop
				 End Sub)
	End Sub
	''' <summary>
	''' 出于性能考虑，监听一旦开始就无法终止，只能释放整个对象。如果你需要监听功能，请单独设置一个专用的TCP客户端。如果同时使用其它接收功能，将产生未知行为。
	''' </summary>
	Sub 监听字节()
		Task.Run(Sub()
					 Do
						 RaiseEvent 收到字节(Me, 流读取器.ReadByte)
					 Loop
				 End Sub)
	End Sub
	Sub 关闭()
		基础客户端.Close()
	End Sub
End Class
