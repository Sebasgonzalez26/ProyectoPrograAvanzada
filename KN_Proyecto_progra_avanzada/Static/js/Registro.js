$(document).ready(function () {

    // Cuando el usuario escribe en identificación
    $("#Identificacion").on("keyup", function () {
        var identificacion = $(this).val().trim();

        if (identificacion.length >= 8) {
            $("#Nombre").val("Consultando...");
            $("#Nombre").closest(".input-group").addClass("is-focused is-filled");

            consultarCedula(identificacion);
        } else {
            $("#Nombre").val("");
            $("#Nombre").closest(".input-group").removeClass("is-focused is-filled");
        }
    });

    function consultarCedula(identificacion) {
        $.ajax({
            url: urlConsultarCedula, // variable global que definiremos en la vista
            type: 'GET',
            data: { id: identificacion },
            dataType: 'json',
            success: function (result) {
                if (result.results && result.results.length > 0) {
                    var nombreCompleto = result.results[0].fullname;
                    $("#Nombre").val(nombreCompleto);
                    $("#Nombre").closest(".input-group").addClass("is-focused is-filled");
                } 
            },
            error: function () {
                $("#Nombre").val("Error al consultar");
                $("#Nombre").closest(".input-group").addClass("is-focused is-filled");
            }
        });
    }

});

$(function () {
    $("#FormRegistro").validate({
        // 👇 cómo se van a ver y dónde se ponen
        errorElement: 'span',
        errorClass: 'text-danger text-sm', // puedes cambiarla

        errorPlacement: function (error, element) {
            // el input está dentro de un .input-group ... queremos el mensaje debajo
            if (element.parent(".input-group").length) {
                // lo metemos después del contenedor
                element.parent().after(error);
            } else {
                error.insertAfter(element);
            }
        },

        highlight: function (element) {
            // cuando hay error
            $(element).closest('.input-group').addClass('is-invalid');
        },
        unhighlight: function (element) {
            // cuando se corrige
            $(element).closest('.input-group').removeClass('is-invalid');
        },

        rules: {
            Identificacion: {
                required: true,
                minlength: 9
            },
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
            Identificacion: {
                required: "* Requerido",
                minlength: "* Debe tener al menos 9 dígitos"
            },
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






