Public Class ProductoService
    Private ReadOnly _repository As New ProductoRepository()

    ' Registrar nuevo producto
    Public Sub RegistrarProducto(producto As Producto)
        If String.IsNullOrWhiteSpace(producto.Nombre) Then
            Throw New ArgumentException("El nombre del producto es obligatorio")
        End If

        If producto.Precio <= 0 Then
            Throw New ArgumentException("El precio debe ser mayor que cero")
        End If

        ' Validación opcional: evitar duplicados por nombre
        Dim existente = _repository.ObtenerPorNombre(producto.Nombre)
        If existente IsNot Nothing Then
            Throw New InvalidOperationException("Ya existe un producto con ese nombre.")
        End If

        _repository.Insertar(producto)
    End Sub

    ' Actualizar un producto
    Public Sub ActualizarProducto(producto As Producto)
        If producto Is Nothing OrElse producto.ID <= 0 Then
            Throw New ArgumentException("Producto no válido para actualizar.")
        End If

        If String.IsNullOrWhiteSpace(producto.Nombre) Then
            Throw New ArgumentException("El nombre del producto es obligatorio.")
        End If

        If producto.Precio <= 0 Then
            Throw New ArgumentException("El precio debe ser mayor que cero.")
        End If

        _repository.Actualizar(producto)
    End Sub

    ' Eliminar un producto
    Public Sub EliminarProducto(id As Integer)
        If id <= 0 Then
            Throw New ArgumentException("ID inválido.")
        End If

        _repository.Eliminar(id)
    End Sub

    ' Obtener todos los productos
    Public Function ObtenerTodos() As List(Of Producto)
        Return _repository.ObtenerTodos()
    End Function

    ' Obtener producto por ID
    Public Function ObtenerProducto(id As Integer) As Producto
        Return _repository.ObtenerPorId(id)
    End Function

    ' Buscar productos por nombre o categoría
    Public Function BuscarProductos(filtro As String) As List(Of Producto)
        Return _repository.Buscar(filtro)
    End Function

    ' Lógica adicional: precio con descuento
    Public Function CalcularPrecioConDescuento(idProducto As Integer, porcentajeDescuento As Decimal) As Decimal
        Dim producto = _repository.ObtenerPorId(idProducto)
        If producto Is Nothing Then
            Throw New ArgumentException("Producto no encontrado.")
        End If

        If porcentajeDescuento < 0 OrElse porcentajeDescuento > 100 Then
            Throw New ArgumentException("El descuento debe estar entre 0 y 100.")
        End If

        Return producto.Precio * (1 - (porcentajeDescuento / 100D))
    End Function
End Class
