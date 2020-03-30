Imports System.ComponentModel
Imports System.IO, System.Threading.Monitor
Public Class 突触
	Implements I突触
	Shared Function 读入(源 As BinaryReader) As 突触
		Return New 突触(源.ReadSingle, 源.ReadSingle, 源.ReadSingle, 源.ReadSingle)
	End Function
	Public Property 激活权 As Single Implements I突触.激活权

	Public Property 抑制权 As Single Implements I突触.抑制权
	WithEvents I上游 As I神经元
	Public Property 上游 As I神经元 Implements I突触.上游
		Get
			Return I上游
		End Get
		Set(value As I神经元)
			If I上游 IsNot Nothing Then RemoveHandler I上游.兴奋, AddressOf I上游_兴奋
			I上游 = value
			AddHandler I上游.兴奋, AddressOf I上游_兴奋
		End Set
	End Property

	WithEvents I下游 As I神经元
	Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

	Public Property 下游 As I神经元 Implements I突触.下游
		Get
			Return I下游
		End Get
		Set(value As I神经元)
			If I下游 IsNot Nothing Then RemoveHandler I下游.兴奋, AddressOf I下游_兴奋
			I下游 = value
			AddHandler I下游.兴奋, AddressOf I下游_兴奋
		End Set
	End Property

	Public Sub 写出(库 As BinaryWriter) Implements I可写出.写出
		库.Write(激活权)
		库.Write(抑制权)
		库.Write(激活学习)
		库.Write(抑制学习)
	End Sub
	Sub New(Optional 激活权 As Single = 1, Optional 抑制权 As Single = 1, Optional 激活学习 As Single = 2, Optional 抑制学习 As Single = 2)
		Me.激活权 = 激活权
		Me.抑制权 = 抑制权
		Me.激活学习 = 激活学习
		Me.抑制学习 = 抑制学习
	End Sub

	Public ReadOnly Property 上次上游兴奋 As Date Implements I突触.上次上游兴奋
		Get
			Return I上游.上次兴奋
		End Get
	End Property

	Public Property 激活学习 As Single Implements I突触.激活学习

	Public Property 抑制学习 As Single Implements I突触.抑制学习

	Private Sub I上游_兴奋() Handles I上游.兴奋
		If TryEnter(Me) Then
			Dim a As Date = Date.Now
			抑制权 += 抑制学习 / ((a - I下游.上次兴奋).TotalMilliseconds + 1)
			Static 抑制权改变 As New PropertyChangedEventArgs("抑制权")
			RaiseEvent PropertyChanged(Me, 抑制权改变)
			Task.Run(Sub() I下游.尝试激活())
			[Exit](Me)
		End If
	End Sub

	Private Sub I下游_兴奋() Handles I下游.兴奋
		Enter(Me)
		Dim a As Date = Date.Now
		激活权 += 激活学习 / ((a - I上游.上次兴奋).TotalMilliseconds + 1)
		Static 激活权改变 As New PropertyChangedEventArgs("激活权")
		Task.Run(Sub() RaiseEvent PropertyChanged(Me, 激活权改变))
		[Exit](Me)
	End Sub
End Class
