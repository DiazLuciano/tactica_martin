Public Class Venta
    Public Property ID As Integer
    Public Property IDCliente As Integer
    Public Property Fecha As DateTime
    Public Property Total As Double
    Public Property Estado As Integer
    Public Property Items As List(Of VentaItem)

    Public Sub New()
        Items = New List(Of VentaItem)()
        Fecha = DateTime.Now
    End Sub

    Public Sub New(idCliente As Integer)
        Me.IDCliente = idCliente
        Items = New List(Of VentaItem)()
        Fecha = DateTime.Now
    End Sub

    Public Sub CalcularTotal()
        Total = Items.Sum(Function(item) item.PrecioTotal)
    End Sub
End Class