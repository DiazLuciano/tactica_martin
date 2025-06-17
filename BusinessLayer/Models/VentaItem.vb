Public Class VentaItem
    Public Property ID As Integer
    Public Property IDVenta As Integer
    Public Property IDProducto As Integer
    Public Property PrecioUnitario As Double
    Public Property Cantidad As Double
    Public Property PrecioTotal As Double
    Public Property Producto As Producto

    Public Sub New()
    End Sub

    Public Sub New(idProducto As Integer, precioUnitario As Double, cantidad As Double)
        Me.IDProducto = idProducto
        Me.PrecioUnitario = precioUnitario
        Me.Cantidad = cantidad
        Me.PrecioTotal = precioUnitario * cantidad
    End Sub

    Public Sub CalcularTotal()
        PrecioTotal = PrecioUnitario * Cantidad
    End Sub
End Class