Imports System.Data.SqlClient

Public Class ClienteRepository
    Inherits BaseRepository

    Public Sub Insertar(cliente As Cliente)
        Dim query = "INSERT INTO clientes (Cliente, Telefono, Correo) VALUES (@Nombre, @Telefono, @Correo)"
        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@Nombre", cliente.Nombre),
            New SqlParameter("@Telefono", If(cliente.Telefono, DBNull.Value)),
            New SqlParameter("@Correo", If(cliente.Correo, DBNull.Value))
        }
        ExecuteNonQuery(query, parameters)
    End Sub

    ' Actualizar un cliente existente
    Public Sub Actualizar(cliente As Cliente)
        Dim query = "UPDATE clientes SET Cliente = @Nombre, Telefono = @Telefono, Correo = @Correo WHERE ID = @ID"
        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@ID", cliente.ID),
            New SqlParameter("@Nombre", cliente.Nombre),
            New SqlParameter("@Telefono", If(cliente.Telefono, DBNull.Value)),
            New SqlParameter("@Correo", If(cliente.Correo, DBNull.Value))
        }
        ExecuteNonQuery(query, parameters)
    End Sub

    ' Eliminar un cliente por ID
    Public Sub Eliminar(id As Integer)
        Dim query = "DELETE FROM clientes WHERE ID = @ID"
        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@ID", id)
        }
        ExecuteNonQuery(query, parameters)
    End Sub

    ' Obtener todos los clientes
    Public Function ObtenerTodos() As List(Of Cliente)
        Dim query = "SELECT ID, Cliente AS Nombre, Telefono, Correo FROM clientes"
        Dim dt = ExecuteQuery(query, Nothing)

        Return (From row As DataRow In dt.Rows
                Select New Cliente() With {
                    .ID = Convert.ToInt32(row("ID")),
                    .Nombre = row("Nombre").ToString(),
                    .Telefono = If(IsDBNull(row("Telefono")), Nothing, row("Telefono").ToString()),
                    .Correo = If(IsDBNull(row("Correo")), Nothing, row("Correo").ToString())
                }).ToList()
    End Function

    ' Buscar clientes por nombre
    Public Function ObtenerPorNombre(filtro As String) As List(Of Cliente)
        Dim query = "SELECT ID, Cliente AS Nombre, Telefono, Correo FROM clientes WHERE Cliente LIKE @Filtro"
        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@Filtro", $"%{filtro}%")
        }
        Dim dt = ExecuteQuery(query, parameters)

        Return (From row In dt.Rows.Cast(Of DataRow)()
                Select New Cliente() With {
                    .ID = Convert.ToInt32(row("ID")),
                    .Nombre = row("Nombre").ToString(),
                    .Telefono = If(IsDBNull(row("Telefono")), Nothing, row("Telefono").ToString()),
                    .Correo = If(IsDBNull(row("Correo")), Nothing, row("Correo").ToString())
                }).ToList()
    End Function

    Public Function ObtenerPorId(id As Integer) As Cliente
        Dim query = "SELECT ID, Cliente AS Nombre, Telefono, Correo FROM clientes WHERE ID = @ID"
        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@ID", id)
        }
        Dim dt = ExecuteQuery(query, parameters)

        If dt.Rows.Count > 0 Then
            Dim row = dt.Rows(0)
            Return New Cliente() With {
                .ID = Convert.ToInt32(row("ID")),
                .Nombre = row("Nombre").ToString(),
                .Telefono = If(IsDBNull(row("Telefono")), Nothing, row("Telefono").ToString()),
                .Correo = If(IsDBNull(row("Correo")), Nothing, row("Correo").ToString())
            }
        End If
        Return Nothing
    End Function
End Class