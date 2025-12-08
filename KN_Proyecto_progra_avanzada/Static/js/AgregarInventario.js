$(function () {

    // Método RegEx reutilizable
    $.validator.addMethod("regex", function (value, element, pattern) {
        var regex = new RegExp(pattern);
        return this.optional(element) || regex.test(value);
    });

    // Validar tamaño máximo de archivo
    $.validator.addMethod("filesize", function (value, element, maxSize) {
        if (element.files.length === 0) return true; // si no hay archivo, que lo maneje "required"
        return element.files[0].size <= maxSize;
    });

    // Validar extensión de archivo
    $.validator.addMethod("extension", function (value, element, param) {
        if (value.length === 0) return true; // si es opcional, pasa y lo controla "required"

        var allowed = param.split('|');
        var ext = value.split('.').pop().toLowerCase();

        return allowed.includes(ext);
    }, "Formato de archivo no permitido");

    // Validación del formulario de Inventario
    $("#FormAgregarInventario").validate({
        rules: {
            Nombre: {
                required: true
            },
            Descripcion: {
                // opcional, sin required
            },
            Categoria: {
                required: true
            },
            Unidad: {
                required: true
            },
            Stock: {
                required: true,
                number: true,
                min: 0
            },
            PrecioVenta: {
                required: true,
                number: true,
                min: 0.01,
                regex: /^\d{1,8}(\.\d{1,2})?$/  // máximo 8 enteros y 2 decimales
            },
            ImgInventario: {
                required: true,
                extension: "jpg|jpeg|png",
                filesize: 2097152 // 2MB
            }
        },
        messages: {
            Nombre: {
                required: "El nombre es obligatorio."
            },
            Categoria: {
                required: "La categoría es obligatoria."
            },
            Unidad: {
                required: "La unidad es obligatoria."
            },
            Stock: {
                required: "El stock es obligatorio.",
                number: "Debe ingresar un número válido.",
                min: "El stock no puede ser negativo."
            },
            PrecioVenta: {
                required: "Ingrese el precio de venta.",
                number: "Debe ser un número válido.",
                min: "Debe ser un valor mayor a 0.",
                regex: "Máximo 8 dígitos enteros y 2 decimales permitidos."
            },
            ImgInventario: {
                required: "Debe seleccionar una imagen.",
                extension: "Solo se permiten archivos jpg, jpeg o png.",
                filesize: "El tamaño del archivo no debe exceder 2 MB."
            }
        }
    });

});
