Imports System.Net, System.Net.Sockets, System.Threading, System.IO
''' <summary>
''' ��TcpListener�����޶�Ϊ����ͨ��
''' </summary>
Public Class TCP������
	Inherits TcpListener
	Sub New(�˿ں� As UShort)
		MyBase.New(New IPEndPoint(IPAddress.Loopback, �˿ں�))
	End Sub
	Overloads Function AcceptTcpClient() As TCP�ͻ���
		Return New TCP�ͻ���(MyBase.AcceptTcpClient)
	End Function
	Overloads Function AcceptTcpClientAsync() As Task(Of TCP�ͻ���)
		Return Task.Run(Function() As TCP�ͻ���
							Return AcceptTcpClient()
						End Function)
	End Function
End Class
Public Delegate Sub �յ��ַ���EventHandler(sender As TCP�ͻ���, �ַ��� As String)
Public Delegate Sub �յ��ֽ�EventHandler(sender As TCP�ͻ���, �ֽ� As Byte)
''' <summary>
''' ��TcpClient�����޶�Ϊ����ͨ��
''' </summary>
Public Class TCP�ͻ���
	Event �յ��ַ��� As �յ��ַ���EventHandler
	Event �յ��ֽ� As �յ��ֽ�EventHandler
	ReadOnly Property ����ȡ�� As BinaryReader
	ReadOnly Property ��д���� As BinaryWriter
	ReadOnly �����ͻ��� As TcpClient
	''' <summary>
	''' ע��÷����������̣߳������ӳɹ�֮ǰ���᷵�ء�
	''' </summary>
	''' <param name="�˿ں�"></param>
	Sub New(�˿ں� As UShort)
		Dim b As New Text.StringBuilder
		�����ͻ��� = New TcpClient
		Do
			Try
				�����ͻ���.Connect(IPAddress.Loopback, �˿ں�)
				Exit Do
			Catch ex As Exception
				Thread.Sleep(1000)
			End Try
		Loop
		Dim a As NetworkStream = �����ͻ���.GetStream
		����ȡ�� = New BinaryReader(a)
		��д���� = New BinaryWriter(a)
	End Sub
	Friend Sub New(���� As TcpClient)
		�����ͻ��� = ����
		Dim a As NetworkStream = �����ͻ���.GetStream
		����ȡ�� = New BinaryReader(a)
		��д���� = New BinaryWriter(a)
	End Sub
	Sub ����(�ַ��� As String)
		��д����.Write(�ַ���)
	End Sub
	Sub ����(�������� As Boolean())
		Dim b As Byte = If(��������(0), 1, 0)
		For a As Byte = 1 To ��������.GetUpperBound(0)
			b = b * 2 + If(��������(a), 1, 0)
		Next
		With ��д����
			.Write(CByte(��������.Length))
			.Write(b)
		End With
	End Sub
	Sub ����(���� As Integer)
		��д����.Write(����)
	End Sub
	Sub �����ֽ�(�ֽ� As Byte)
		��д����.Write(�ֽ�)
	End Sub
	Sub ����(��Ϣ As Object, ���� As Type)
		Select Case ����
			Case GetType(Integer)
				��д����.Write(DirectCast(��Ϣ, Integer))
			Case GetType(Boolean)
				��д����.Write(DirectCast(��Ϣ, Boolean))
			Case GetType(String)
				��д����.Write(DirectCast(��Ϣ, String))
		End Select
	End Sub
	Function �����ֽ�() As Byte
		Return ����ȡ��.ReadByte
	End Function
	Function �����ַ���() As String
		Return ����ȡ��.ReadString
	End Function
	Function ���ղ�������() As Boolean()
		Dim a As Byte = ����ȡ��.ReadByte, c As Byte = ����ȡ��.ReadByte
		Dim d(a - 1) As Boolean
		For b As SByte = a - 1 To 0 Step -1
			d(b) = c Mod 2 = 1
			c = Math.Floor(c / 2)
		Next
		Return d
	End Function
	Function ��������() As Integer
		Return ����ȡ��.ReadInt32
	End Function
	Function ����(���� As Type)
		Select Case ����
			Case GetType(Integer)
				Return ����ȡ��.ReadInt32
			Case GetType(Boolean)
				Return ����ȡ��.ReadBoolean
			Case GetType(String)
				Return ����ȡ��.ReadString
			Case Else
				Return Nothing
		End Select
	End Function
	''' <summary>
	''' �������ܿ��ǣ�����һ����ʼ���޷���ֹ��ֻ���ͷ����������������Ҫ�������ܣ��뵥������һ��ר�õ�TCP�ͻ��ˡ����ͬʱʹ���������չ��ܣ�������δ֪��Ϊ��
	''' </summary>
	Sub �����ַ���()
		Task.Run(Sub()
					 Do
						 RaiseEvent �յ��ַ���(Me, ����ȡ��.ReadString)
					 Loop
				 End Sub)
	End Sub
	''' <summary>
	''' �������ܿ��ǣ�����һ����ʼ���޷���ֹ��ֻ���ͷ����������������Ҫ�������ܣ��뵥������һ��ר�õ�TCP�ͻ��ˡ����ͬʱʹ���������չ��ܣ�������δ֪��Ϊ��
	''' </summary>
	Sub �����ֽ�()
		Task.Run(Sub()
					 Do
						 RaiseEvent �յ��ֽ�(Me, ����ȡ��.ReadByte)
					 Loop
				 End Sub)
	End Sub
	Sub �ر�()
		�����ͻ���.Close()
	End Sub
End Class
