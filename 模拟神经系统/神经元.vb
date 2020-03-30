Imports System.IO, System.Threading.Monitor

Public Class 神经元
	Implements I神经元
	Shared ReadOnly 随机生成器 As New Random
	Shared Function 读入(源 As BinaryReader) As 神经元
		Return New 神经元(源.ReadSingle)
	End Function

	Public Event 兴奋() Implements I神经元.兴奋

	Public Sub 写出(库 As BinaryWriter) Implements I可写出.写出
		库.Write(不应期)
	End Sub
	Sub New(Optional 不应期 As Single = 1)
		Me.不应期 = 不应期
	End Sub
	Public Property 上次兴奋 As Date Implements I神经元.上次兴奋

	Public ReadOnly Property 上游突触 As IList(Of I突触) = New List(Of I突触) Implements I神经元.上游突触

	Public ReadOnly Property 下游突触 As IList(Of I突触) = New List(Of I突触) Implements I神经元.下游突触

	Public Property 不应期 As Single Implements I神经元.不应期
	Private 累积疲劳 As Single
	Public Sub 尝试激活() Implements I神经元.尝试激活
		If TryEnter(Me) Then
			Dim a As Single, d As Date = Date.Now, e As Single
			Dim b As Single = 1
			For Each c As I突触 In 上游突触
				e = (d - c.上次上游兴奋).TotalMilliseconds + 1
				a += c.激活权 / e
				b += c.抑制权 / e
			Next
			累积疲劳 /= Math.Exp((d - 上次兴奋).TotalMilliseconds)
			a /= (累积疲劳 + 1)
			If 随机生成器.NextDouble * (a + b) < a Then
				上次兴奋 = d
				Task.Run(Sub() RaiseEvent 兴奋())
				累积疲劳 += 不应期
			End If
			[Exit](Me)
		End If
	End Sub

	Public Sub 强制激活() Implements I神经元.强制激活
		Enter(Me)
		上次兴奋 = Date.Now
		Task.Run(Sub() RaiseEvent 兴奋())
		[Exit](Me)
	End Sub
End Class
