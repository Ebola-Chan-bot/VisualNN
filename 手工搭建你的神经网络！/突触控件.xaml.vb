'https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板
Imports 模拟神经系统
Public NotInheritable Class 突触控件
	Inherits UserControl
	Implements I可选定, INotifyPropertyChanged, I可写出
	Public WithEvents 原型 As I突触
	Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
	Property 上游 As I节点
	Property 下游 As I节点
	Property X1 As Double
		Get
			Return 线条.X1
		End Get
		Set(value As Double)
			线条.X1 = value
		End Set
	End Property
	Property X2 As Double
		Get
			Return 线条.X2
		End Get
		Set(value As Double)
			线条.X2 = value
		End Set
	End Property
	Property Y1 As Double
		Get
			Return 线条.Y1
		End Get
		Set(value As Double)
			线条.Y1 = value
		End Set
	End Property
	Property Y2 As Double
		Get
			Return 线条.Y2
		End Get
		Set(value As Double)
			线条.Y2 = value
		End Set
	End Property
	Property ActiveWeight As Single
		Get
			Return 原型.激活权
		End Get
		Set(value As Single)
			原型.激活权 = value
		End Set
	End Property
	Property InhibitoryWeight As Single
		Get
			Return 原型.抑制权
		End Get
		Set(value As Single)
			原型.抑制权 = value
		End Set
	End Property
	Property ActiveLearn As Single
		Get
			Return 原型.激活学习
		End Get
		Set(value As Single)
			原型.激活学习 = value
		End Set
	End Property
	Property InhibitoryLearn As Single
		Get
			Return 原型.抑制学习
		End Get
		Set(value As Single)
			原型.抑制学习 = value
		End Set
	End Property
	Sub New(原型 As I突触, 上游 As I节点, 下游 As I节点, Optional 标签 As String = "")

		' 此调用是设计器所必需的。
		InitializeComponent()

		' 在 InitializeComponent() 调用之后添加任何初始化。
		Me.原型 = 原型
		Me.上游 = 上游
		Me.下游 = 下游
		神经系统.突触连接(上游.原型, 原型, 下游.原型)
		定位调色()
		Tag = 标签
		上游.下游突触.Add(Me)
		下游.上游突触.Add(Me)
	End Sub
	Shared Function 读入(原型 As I突触, 上游 As I节点, 下游 As I节点, 源 As BinaryReader) As 突触控件
		Return New 突触控件(原型, 上游, 下游, 源.ReadString)
	End Function
	Public Sub 选定() Implements I可选定.选定
		选定动画.Begin()
		属性框.ShowAt(Me)
	End Sub

	Public Sub 取消() Implements I可选定.取消
		选定动画.Stop()
	End Sub
	Sub 定位调色()
		X1 = Canvas.GetLeft(上游) + 上游.半径
		Y1 = Canvas.GetTop(上游) + 上游.半径
		X2 = Canvas.GetLeft(下游) + 下游.半径
		Y2 = Canvas.GetTop(下游) + 下游.半径
		Dim Dx As Double = X2 - X1, Dy As Double = Y2 - Y1, Dr As Double = 2 * Math.Sqrt(Dx ^ 2 + Dy ^ 2)
		X1 += Dy * 上游.半径 / Dr
		Y1 -= Dx * 上游.半径 / Dr
		X2 += Dy * 下游.半径 / Dr
		Y2 -= Dx * 下游.半径 / Dr
		Static 渐变刷 As LinearGradientBrush = 线条.Stroke
		With 渐变刷
			Dim X As Double = Math.Abs(X2 - X1), Y As Double = Math.Abs(Y2 - Y1), XS As Double, XE As Double, YS As Double, YE As Double
			If X > Y Then
				If X2 > X1 Then
					XS = 0
					XE = 1
				Else
					XS = 1
					XE = 0
				End If
				YS = (1 + (Y1 - Y2) / X) / 2
				YE = (1 + (Y2 - Y1) / X) / 2
			Else
				If Y2 > Y1 Then
					YS = 0
					YE = 1
				Else
					YS = 1
					YE = 0
				End If
				XS = (1 + (X1 - X2) / Y) / 2
				XE = (1 + (X2 - X1) / Y) / 2
			End If
			渐变刷.StartPoint = New Point(XS, YS)
			渐变刷.EndPoint = New Point(XE, YE)
		End With
	End Sub

	Private Sub UserControl_PointerEntered(sender As Object, e As PointerRoutedEventArgs)
		线条.StrokeThickness = 6
		e.Handled = True
	End Sub

	Private Sub UserControl_PointerExited(sender As Object, e As PointerRoutedEventArgs)
		线条.StrokeThickness = 4
		e.Handled = True
	End Sub

	Private Sub 原型_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles 原型.PropertyChanged
		Select Case e.PropertyName
			Case "激活权"
				Static 激活权改变 As New PropertyChangedEventArgs("ActiveWeight")
				Call Dispatcher.RunIdleAsync(Sub() RaiseEvent PropertyChanged(Me, 激活权改变))
			Case "抑制权"
				Static 抑制权改变 As New PropertyChangedEventArgs("InhibitoryWeight")
				Call Dispatcher.RunIdleAsync(Sub() RaiseEvent PropertyChanged(Me, 抑制权改变))
		End Select
	End Sub

	Public Sub 写出(库 As BinaryWriter) Implements I可写出.写出
		原型.写出(库)
		库.Write(DirectCast(Tag, String))
	End Sub
End Class
