$(function () {

    // Método RegEx reutilizable
    $.validator.addMethod("regex", function (value, element, pattern) {
        var regex = new RegExp(pattern);
        return this.optional(element) || regex.test(value);
    });

    $("#FormAgregarCliente").validate({
        rules: {
            Nombre: {
                required: true,
                regex: /^[A-Za-zÁÉÍÓÚáéíóúñÑ\s]{2,40}$/ // letras y espacios
            },
            Apellidos: {
                required: true,
                regex: /^[A-Za-zÁÉÍÓÚáéíóúñÑ\s]{2,60}$/
            },
            Correo: {
                required: true,
                email: true,
                maxlength: 150
            },
            Telefono: {
                required: true,
                regex: /^[0-9\-]{8,15}$/  // teléfonos CR tipo 8888-8888 o 88888888
            }
          
        },
        messages: {
            Nombre: {
                required: "Ingrese el nombre del cliente",
                regex: "Solo se permiten letras y espacios (mínimo 2 caracteres)"
            },
            Apellidos: {
                required: "Ingrese los apellidos del cliente",
                regex: "Solo se permiten letras y espacios (mínimo 2 caracteres)"
            },
            Correo: {
                required: "Ingrese el correo electrónico",
                email: "Debe ingresar un correo válido",
                maxlength: "Máximo 150 caracteres"
            },
            Telefono: {
                required: "Ingrese un número de teléfono",
                regex: "Formato inválido. Ejemplos: 8888-8888 o 88888888"
            }
            
        }
    });

});
