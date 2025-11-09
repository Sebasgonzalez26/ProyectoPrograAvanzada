$(function () {
    $("#FormIniciarSesion").validate({
        errorElement: 'span',
        errorClass: 'validation-msg',

        errorPlacement: function (error, element) {
            if (element.parent(".input-group").length) {
                element.parent().after(error);
            } else {
                error.insertAfter(element);
            }
        },

        highlight: function (element) {
            $(element).closest('.input-group').addClass('has-error');
        },
        unhighlight: function (element) {
            $(element).closest('.input-group').removeClass('has-error');
        },

        rules: {
            CorreoElectronico: {
                required: true,
                email: true
            },
            Contrasena: {
                required: true,
                minlength: 6
            }
        },
        messages: {
            CorreoElectronico: {
                required: "* Requerido",
                email: "* Ingresa un correo válido"
            },
            Contrasena: {
                required: "* Requerido",
                minlength: "* Mínimo 6 caracteres"
            }
        }
    });
});
