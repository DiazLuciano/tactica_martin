Public Class Producto
    Public Property ID As Integer
    Public Property Nombre As String
    Public Property Precio As Double
    Public Property Categoria As String

    Public Sub New()
    End Sub

    Public Sub New(nombre As String, precio As Double, categoria As String)
        Me.Nombre = nombre
        Me.Precio = precio
        Me.Categoria = categoria
    End Sub
End Class