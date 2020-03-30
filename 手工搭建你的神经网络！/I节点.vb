Imports 模拟神经系统
Public Interface I节点
	Inherits I可写出
	ReadOnly Property 上游突触 As List(Of 突触控件)
	ReadOnly Property 下游突触 As List(Of 突触控件)
	ReadOnly Property 半径 As Double
	ReadOnly Property 原型 As I神经元
End Interface
