Public Interface I神经元
	Inherits I可写出
	ReadOnly Property 上游突触 As IList(Of I突触)
	ReadOnly Property 下游突触 As IList(Of I突触)
	Event 兴奋()
	Sub 尝试激活()
	Sub 强制激活()
	ReadOnly Property 上次兴奋 As Date
	Property 不应期 As Single
End Interface
