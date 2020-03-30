' https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板
Imports Windows.Storage, 模拟神经系统
''' <summary>
''' 可用于自身或导航至 Frame 内部的空白页。
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page
	ReadOnly 保存对话框 As New Pickers.FileSavePicker, 打开对话框 As New Pickers.FileOpenPicker, 节点控件s As New List(Of I节点), 突触控件s As New List(Of 突触控件)
	Private 当前文件 As StorageFile, 神经系统 As New 神经系统, 画布内容 As UIElementCollection
	Private Async Sub 另存为_Click(sender As Object, e As RoutedEventArgs) Handles 另存为.Click
		Dim a As StorageFile = Await 保存对话框.PickSaveFileAsync
		If a IsNot Nothing Then
			当前文件 = a
			写入当前文件()
		End If
	End Sub
	Private Sub 添加神经元控件(控件 As 神经元控件, Left As Double, Top As Double)
		Canvas.SetLeft(控件, Left)
		Canvas.SetTop(控件, Top)
		Canvas.SetZIndex(控件, 1)
		画布内容.Add(控件)
		节点控件s.Add(控件)
		AddHandler 控件.Tapped, AddressOf 可选定_Tapped
		AddHandler 控件.RightTapped, AddressOf 节点控件_RightTapped
		AddHandler 控件.Drop, AddressOf 节点控件_Drop
	End Sub
	Private Sub 画布_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles 画布.Tapped
		If 工具箱.SelectedItem IsNot Nothing Then
			Dim a As Point = e.GetPosition(sender)
			Select Case DirectCast(工具箱.SelectedItem, NavigationViewItem).Content
				Case "神经元"
					Dim b As New 神经元
					神经系统.Add(b)
					Dim c As New 神经元控件(b)
					添加神经元控件(New 神经元控件(b), a.X - c.Width / 2, a.Y - c.Height / 2)
			End Select
		End If
		e.Handled = True
	End Sub
	Private Sub 添加突触控件(控件 As 突触控件)
		画布内容.Add(控件)
		突触控件s.Add(控件)
		AddHandler 控件.Tapped, AddressOf 可选定_Tapped
		AddHandler 控件.RightTapped, AddressOf 突触控件_RightTapped
	End Sub
	''' <summary>
	''' 文件格式：
	''' 节点控件数 As Integer
	''' 第0个控件的：
	'''		基本信息 As I节点
	'''		Left As Double
	'''		Top As Double
	'''	第1个控件的：
	'''		……
	'''	……
	'''	突触控件数 As Integer
	'''	第0个突触的：
	'''		上游节点编号 As Integer
	'''		下游节点编号 As Integer
	'''		基本信息 As 突触控件
	'''	第1个突触的：
	'''		……
	'''	……
	''' </summary>
	Private Async Sub 打开_Click(sender As Object, e As RoutedEventArgs) Handles 打开.Click
		Dim a As StorageFile = Await 打开对话框.PickSingleFileAsync
		If a IsNot Nothing Then
			画布内容.Clear()
			节点控件s.Clear()
			突触控件s.Clear()
			当前文件 = a
			Dim b As New BinaryReader(Await a.OpenStreamForReadAsync)
			For c As Integer = 1 To b.ReadInt32
				添加神经元控件(神经元控件.读入(神经元.读入(b), b), b.ReadDouble, b.ReadDouble)
			Next
			Dim j As I节点, k As I节点
			For c As Integer = 1 To b.ReadInt32
				j = 节点控件s(b.ReadInt32)
				k = 节点控件s(b.ReadInt32)
				添加突触控件(突触控件.读入(突触.读入(b), j, k, b))
			Next
			b.Close()
		End If
	End Sub

	Sub New()

		' 此调用是设计器所必需的。
		InitializeComponent()

		' 在 InitializeComponent() 调用之后添加任何初始化。
		保存对话框.FileTypeChoices.Add("神经网络", {".神经网络"})
		打开对话框.FileTypeFilter.Add(".神经网络")
	End Sub
	''' <summary>
	''' 文件格式：
	''' 节点控件数 As Integer
	''' 第0个控件的：
	'''		基本信息 As I节点
	'''		Left As Double
	'''		Top As Double
	'''	第1个控件的：
	'''		……
	'''	……
	'''	突触控件数 As Integer
	'''	第0个突触的：
	'''		上游节点编号 As Integer
	'''		下游节点编号 As Integer
	'''		基本信息 As 突触控件
	'''	第1个突触的：
	'''		……
	'''	……
	''' </summary>
	Private Async Sub 写入当前文件()
		Dim b As New BinaryWriter(Await 当前文件.OpenStreamForWriteAsync)
		b.Write(节点控件s.Count)
		For Each a As I节点 In 节点控件s
			a.写出(b)
			b.Write(Canvas.GetLeft(a))
			b.Write(Canvas.GetTop(a))
		Next
		b.Write(突触控件s.Count)
		For Each a As 突触控件 In 突触控件s
			b.Write(节点控件s.IndexOf(a.上游))
			b.Write(节点控件s.IndexOf(a.下游))
			a.写出(b)
		Next
		b.Close()
	End Sub

	Private Sub 画布_DragOver(sender As Object, e As DragEventArgs) Handles 画布.DragOver
		e.AcceptedOperation = DataTransfer.DataPackageOperation.Move
		e.Handled = True
	End Sub

	Private Sub 画布_Drop(sender As Object, e As DragEventArgs) Handles 画布.Drop
		Dim c As DataTransfer.DataPackagePropertySetView = e.DataView.Properties, a As 神经元控件 = c.Item("源"), b As Point = e.GetPosition(sender), d As Point = c.Item("相对点")
		Canvas.SetLeft(a, b.X - d.X / 2)
		Canvas.SetTop(a, b.Y - d.Y / 2)
		For Each f As 突触控件 In a.上游突触
			f.定位调色()
		Next
		For Each f As 突触控件 In a.下游突触
			f.定位调色()
		Next
		e.Handled = True
	End Sub

	Private Sub 画布_Loaded(sender As Object, e As RoutedEventArgs) Handles 画布.Loaded
		画布内容 = 画布.Children
	End Sub

	Private Async Sub 保存_Click(sender As Object, e As RoutedEventArgs) Handles 保存.Click
		If 当前文件 Is Nothing Then 当前文件 = Await 保存对话框.PickSaveFileAsync
		If 当前文件 IsNot Nothing Then 写入当前文件()
	End Sub

	Private Sub 可选定_Tapped(sender As Object, e As TappedRoutedEventArgs)
		Static 当前选定 As I可选定
		If 当前选定 IsNot Nothing Then 当前选定.取消()
		当前选定 = sender
		当前选定.选定()
		e.Handled = True
	End Sub

	Private Sub 节点控件_RightTapped(sender As Object, e As RightTappedRoutedEventArgs)
		Dim a As I节点 = sender
		With 画布内容
			.Remove(a)
			For Each b As 突触控件 In a.上游突触
				.Remove(b)
				b.上游.下游突触.Remove(b)
			Next
			For Each b As 突触控件 In a.下游突触
				.Remove(b)
				b.下游.上游突触.Remove(b)
			Next
		End With
		神经系统.Remove(a.原型)
		节点控件s.Remove(a)
		e.Handled = True
	End Sub

	Private Sub 突触控件_RightTapped(sender As Object, e As RightTappedRoutedEventArgs)
		Dim a As 突触控件 = sender
		画布内容.Remove(a)
		a.上游.下游突触.Remove(a)
		a.下游.上游突触.Remove(a)
		神经系统.移除突触(a.原型)
	End Sub

	Private Sub 节点控件_Drop(sender As Object, e As DragEventArgs)
		Dim a As I节点 = e.DataView.Properties.Item("源"), b As I节点 = sender, c As New 突触
		添加突触控件(New 突触控件(c, a, b))
		e.Handled = True
	End Sub
End Class
