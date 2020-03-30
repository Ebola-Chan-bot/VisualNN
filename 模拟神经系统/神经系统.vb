Imports System.IO

Public Class 神经系统
	Inherits List(Of I神经元)
	Implements I可写出
	Shared Sub 突触连接(上游 As I神经元, 突触 As I突触, 下游 As I神经元)
		上游.下游突触.Add(突触)
		突触.上游 = 上游
		突触.下游 = 下游
		下游.上游突触.Add(突触)
	End Sub
	Shared Sub 移除突触(突触 As I突触)
		突触.上游.下游突触.Remove(突触)
		突触.下游.上游突触.Remove(突触)
	End Sub
	''' 二进制记录结构：
	''' 神经元总数 As Integer
	''' 第0号神经元基本信息 As I神经元
	''' 第1号神经元基本信息 As I神经元
	''' ……
	''' 第0号神经元的：
	'''		上游数目 As Integer
	'''		第0号突触的：
	'''			上游编号 As Integer
	'''			基本信息 As I突触
	'''		第1号突触的：
	'''			……
	'''		……
	'''	第1号神经元的：
	'''		……
	'''	……
	Shared Function 读入(源 As BinaryReader, 神经元工厂 As Func(Of BinaryReader, I神经元), 突触工厂 As Func(Of BinaryReader, I突触)) As 神经系统
		Dim a(源.ReadInt32 - 1) As I神经元
		For b As Integer = 0 To a.GetUpperBound(0)
			a(b) = 神经元工厂.Invoke(源)
		Next
		For Each c As I神经元 In a
			For b As Integer = 1 To 源.ReadInt32
				突触连接(a(源.ReadInt32), 突触工厂(源), c)
			Next
		Next
		读入 = New 神经系统
		读入.AddRange(a)
	End Function
	Shadows Sub Remove(item As I神经元)
		For Each a As I突触 In item.上游突触
			a.上游.下游突触.Remove(a)
		Next
		For Each a As I突触 In item.下游突触
			a.下游.上游突触.Remove(a)
		Next
	End Sub
	Public Sub 写出(库 As BinaryWriter) Implements I可写出.写出
		库.Write(Count)
		For Each a As I神经元 In Me
			a.写出(库)
		Next
		Dim 上游突触 As IReadOnlyCollection(Of I突触), 突触数目 As Integer, 序号表 As Integer(), 下标上限 As Integer, c As Integer
		For Each a As I神经元 In Me
			上游突触 = a.上游突触
			下标上限 = 上游突触.Count - 1
			ReDim 序号表(下标上限)
			突触数目 = 0
			For b As Integer = 0 To 下标上限
				c = IndexOf(上游突触(b).上游)
				序号表(b) = c
				突触数目 += If(c < 0, 0, 1)
			Next
			库.Write(突触数目)
			For b As Integer = 0 To 下标上限
				c = 序号表(b)
				If c > -1 Then
					库.Write(c)
					上游突触(b).写出(库)
				End If
			Next
		Next
	End Sub
End Class
