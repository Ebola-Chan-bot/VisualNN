Public Interface I突触
	Inherits I可写出, ComponentModel.INotifyPropertyChanged
	Property 激活权 As Single
	Property 抑制权 As Single
	Property 激活学习 As Single
	Property 抑制学习 As Single
	Property 上游 As I神经元
	Property 下游 As I神经元
	ReadOnly Property 上次上游兴奋 As Date
End Interface
