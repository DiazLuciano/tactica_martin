Imports System.Data.SqlClient

Public Class ProductoRepository
    Inherits BaseRepository

    ' Métodos similares a ClienteRepository, adaptados para productos
    Public Sub Insertar(producto As Producto)
        Dim query = "INSERT INTO productos (Nombre, Precio, Categoria) VALUES (@Nombre, @Precio, @Categoria)"
        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@Nombre", producto.Nombre),
            New SqlParameter("@Precio", producto.Precio),
            New SqlParameter("@Categoria", If(producto.Categoria, DBNull.Value))
        }
        ExecuteNonQuery(query, parameters)
    End Sub

    Public Function ObtenerPorNombre(nombre As String) As Producto
        Dim query = "SELECT TOP 1 ID, Nombre, Precio, Categoria FROM productos WHERE Nombre = @Nombre"
        Dim parameters = New List(Of SqlParameter) From {
        New SqlParameter("@Nombre", nombre)
    }

        Dim dt = ExecuteQuery(query, parameters)

        If dt.Rows.Count > 0 Then
            Dim row = dt.Rows(0)
            Return New Producto() With {
            .ID = Convert.ToInt32(row("ID")),
            .Nombre = row("Nombre").ToString(),
            .Precio = Convert.ToDouble(row("Precio")),
            .Categoria = If(IsDBNull(row("Categoria")), Nothing, row("Categoria").ToString())
        }
        End If

        Return Nothing
    End Function


    Public Function ObtenerPorId(id As Integer) As Producto
        Dim query = "SELECT ID, Nombre, Precio, Categoria FROM productos WHERE ID = @ID"
        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@ID", id)
        }
        Dim dt = ExecuteQuery(query, parameters)

        If dt.Rows.Count > 0 Then
            Dim row = dt.Rows(0)
            Return New Producto() With {
                .ID = Convert.ToInt32(row("ID")),
                .Nombre = row("Nombre").ToString(),
                .Precio = Convert.ToDouble(row("Precio")),
                .Categoria = If(IsDBNull(row("Categoria")), Nothing, row("Categoria").ToString())
            }
        End If
        Return Nothing
    End Function

    Public Function ObtenerTodos() As List(Of Producto)
        Dim query = "SELECT ID, Nombre, Precio, Categoria FROM productos"
        Dim dt = ExecuteQuery(query, New List(Of SqlParameter))
        Dim lista = New List(Of Producto)

        For Each row As DataRow In dt.Rows
            lista.Add(New Producto() With {
            .ID = Convert.ToInt32(row("ID")),
            .Nombre = row("Nombre").ToString(),
            .Precio = Convert.ToDouble(row("Precio")),
            .Categoria = If(IsDBNull(row("Categoria")), Nothing, row("Categoria").ToString())
        })
        Next

        Return lista
    End Function

    Public Function Buscar(filtro As String) As List(Of Producto)
        Dim query = "SELECT ID, Nombre, Precio, Categoria FROM productos WHERE Nombre LIKE @Filtro OR Categoria LIKE @Filtro"
        Dim parameters = New List(Of SqlParameter) From {
        New SqlParameter("@Filtro", "%" & filtro & "%")
    }

        Dim dt = ExecuteQuery(query, parameters)
        Dim lista = New List(Of Producto)

        For Each row As DataRow In dt.Rows
            lista.Add(New Producto() With {
            .ID = Convert.ToInt32(row("ID")),
            .Nombre = row("Nombre").ToString(),
            .Precio = Convert.ToDouble(row("Precio")),
            .Categoria = If(IsDBNull(row("Categoria")), Nothing, row("Categoria").ToString())
        })
        Next

        Return lista
    End Function



    Public Sub Actualizar(producto As Producto)
        Dim query = "UPDATE productos SET Nombre = @Nombre, Precio = @Precio, Categoria = @Categoria WHERE ID = @ID"
        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@Nombre", producto.Nombre),
            New SqlParameter("@Precio", producto.Precio),
            New SqlParameter("@Categoria", If(producto.Categoria, DBNull.Value)),
            New SqlParameter("@ID", producto.ID)
        }
        ExecuteNonQuery(query, parameters)
    End Sub

    Public Sub Eliminar(id As Integer)
        Dim query = "DELETE FROM productos WHERE ID = @ID"
        Dim parameters = New List(Of SqlParameter) From {
        New SqlParameter("@ID", id)
    }
        ExecuteNonQuery(query, parameters)
    End Sub

End Class