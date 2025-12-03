$(function () {

    $.validator.addMethod("regex", function (value, element, pattern) {
        var regex = new RegExp(pattern);
        return this.optional(element) || regex.test(value);
    });

    $.validator.addMethod("filesize", function (value, element, maxSize) {
        if (element.files.length === 0) return true;
        return element.files[0].size <= maxSize;
    });


    $.validator.addMethod("extension", function (value, element, param) {
        if (value.length === 0) return true; // si es opcional, pasa

        var allowed = param.split('|');
        var ext = value.split('.').pop().toLowerCase();

        return allowed.includes(ext);
    }, "Formato de archivo no permitido");


    $("#FormAgregarCatalogo").validate({
        rules: {
            Nombre: { required: true },
            Descripcion: { required: true },
            Precio: {
                required: true,
                number: true,
                min: 0.01,
                regex: /^\d{1,8}(\.\d{1,2})?$/
            },
            Stock: { required: true },
            ImgCatalogo: {
                required: true,
                extension: "jpg|jpeg|png",
                filesize: 2097152
            },
            IdCategoria: { required: true }
        },
        messages: {
            Nombre: { required: "Este campo es obligatorio" },
            Descripcion: { required: "Este campo es obligatorio" },
            Precio: {
                required: "Ingrese un precio",
                number: "Debe ser un número válido",
                min: "Debe ser un valor positivo",
                regex: "Máximo 8 dígitos y 2 decimales permitidos"
            },
            Stock: { required: "Este campo es obligatorio" },
            ImgCatalogo: {
                required: "Este campo es obligatorio",
                extension: "Solo se permiten archivos jpg, jpeg o png",
                filesize: "El tamaño del archivo no debe exceder 2 MB"
            },
            IdCategoria: { required: "Este campo es obligatorio" }
        }
    });

});
