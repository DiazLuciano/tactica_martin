Public Class Cliente
    Public Property ID As Integer
    Public Property Nombre As String
    Public Property Telefono As String
    Public Property Correo As String

    Public Sub New()
    End Sub

    Public Sub New(nombre As String, telefono As String, correo As String)
        Me.Nombre = nombre
        Me.Telefono = telefono
        Me.Correo = correo
    End Sub
End Class
