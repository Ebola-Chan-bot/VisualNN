'https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板
Imports 模拟神经系统
Public NotInheritable Class 神经元控件
	Inherits UserControl
	Implements I可选定, I节点
	Shared ReadOnly 激活色 As New SolidColorBrush(Windows.UI.Colors.Red), 静息色 As New SolidColorBrush(Windows.UI.Colors.Cyan)
	Property RefractoryPeriod As Single
		Get
			Return 原型.不应期
		End Get
		Set(value As Single)
			原型.不应期 = value
		End Set
	End Property
	Property HighlightPeriod As UShort

	Public ReadOnly Property 上游突触 As New List(Of 突触控件) Implements I节点.上游突触

	Public ReadOnly Property 下游突触 As New List(Of 突触控件) Implements I节点.下游突触

	Public Property 半径 As Double Implements I节点.半径
		Get
			Return Width / 2
		End Get
		Set(value As Double)
			Width = value * 2
		End Set
	End Property

	WithEvents I原型 As I神经元
	Public ReadOnly Property 原型 As I神经元 Implements I节点.原型
		Get
			Return I原型
		End Get
	End Property

	Sub New(原型 As I神经元, Optional 高亮期 As UShort = 500, Optional 标签 As String = "")

		' 此调用是设计器所必需的。
		InitializeComponent()

		' 在 InitializeComponent() 调用之后添加任何初始化。
		I原型 = 原型
		胞体.Fill = 静息色
		HighlightPeriod = 高亮期
		Tag = 标签
	End Sub
	''' <summary>
	''' 文件格式：
	''' 原型 As I神经元
	''' HighlightPeriod As UShort
	''' Tag as String
	''' </summary>
	Shared Function 读入(原型 As I神经元, 源 As BinaryReader) As 神经元控件
		Return New 神经元控件(原型, 源.ReadUInt16, 源.ReadString)
	End Function
	Private Sub UserControl_PointerEntered(sender As Object, e As PointerRoutedEventArgs)
		Static 指向放大 As New ScaleTransform With {.ScaleX = 1.5, .ScaleY = 1.5}
		RenderTransform = 指向放大
		e.Handled = True
	End Sub

	Private Sub UserControl_PointerExited(sender As Object, e As PointerRoutedEventArgs)
		RenderTransform = Nothing
		e.Handled = True
	End Sub

	Private Sub UserControl_DragStarting(sender As UIElement, args As DragStartingEventArgs)
		args.Data.Properties.Add("源", sender)
		args.Data.Properties.Add("相对点", args.GetPosition(sender))
	End Sub

	Public Sub 选定() Implements I可选定.选定
		胞体.Stroke = 选定边框
		选定动画.Begin()
		属性框.ShowAt(Me)
	End Sub

	Public Sub 取消() Implements I可选定.取消
		选定动画.Stop()
		胞体.Stroke = 普通边框
	End Sub

	Private Sub UserControl_DragOver(sender As Object, e As DragEventArgs)
		If e.DataView.Properties.Item("源") Is sender Then
			e.AcceptedOperation = DataTransfer.DataPackageOperation.None
		Else
			e.AcceptedOperation = DataTransfer.DataPackageOperation.Link
		End If
		e.Handled = True
	End Sub

	Private Sub Button_Click()
		Task.Run(Sub() 原型.强制激活())
	End Sub

	Private Sub 原型_兴奋() Handles I原型.兴奋
		Task.Run(Sub()
					 Call Dispatcher.RunIdleAsync(Sub() 胞体.Fill = 激活色)
					 Threading.Thread.Sleep(HighlightPeriod)
					 Call Dispatcher.RunIdleAsync(Sub() 胞体.Fill = 静息色)
				 End Sub)
	End Sub
	Private Sub 神经元控件_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged
		If e.NewSize.Width <> e.NewSize.Height Then
			If e.NewSize.Width = e.PreviousSize.Width Then
				Width = Height
			Else
				Height = Width
			End If
		End If
	End Sub
	''' <summary>
	''' 文件格式：
	''' 原型 As I神经元
	''' HighlightPeriod As UShort
	''' Tag as String
	''' </summary>
	Public Sub 写出(库 As BinaryWriter) Implements I可写出.写出
		原型.写出(库)
		库.Write(HighlightPeriod)
		库.Write(DirectCast(Tag, String))
	End Sub
End Class
