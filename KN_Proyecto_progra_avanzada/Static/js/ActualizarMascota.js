$(function () {

    $.validator.addMethod("regex", function (value, element, pattern) {
        var regex = new RegExp(pattern);
        return this.optional(element) || regex.test(value);
    });

    $("#FormActualizarMascota").validate({
        rules: {
            Nombre: {
                required: true,
                regex: /^[A-Za-zÁÉÍÓÚáéíóúñÑ\s]{2,50}$/
            },
            Especie: {
                required: true,
                regex: /^[A-Za-zÁÉÍÓÚáéíóúñÑ\s]{2,50}$/
            },
            Raza: {
                required: true,
                regex: /^[A-Za-zÁÉÍÓÚáéíóúñÑ\s0-9]{2,50}$/
            },
            Sexo: {
                required: true,
                regex: /^(Macho|Hembra|macho|hembra|M|H)$/
            },
            FechaNacimiento: {
                required: true,
                date: true
            },
            IdCliente: {
                required: true
            }
        },
        messages: {
            Nombre: {
                required: "Ingrese el nombre de la mascota",
                regex: "Solo letras y espacios (mínimo 2 caracteres)"
            },
            Especie: {
                required: "Ingrese la especie",
                regex: "Formato inválido (solo letras)"
            },
            Raza: {
                required: "Ingrese la raza",
                regex: "Formato inválido (solo letras y números)"
            },
            Sexo: {
                required: "Ingrese el sexo",
                regex: "Debe ser: Macho / Hembra / M / H"
            },
            FechaNacimiento: {
                required: "Seleccione la fecha de nacimiento",
                date: "Seleccione una fecha válida"
            },
            IdCliente: {
                required: "Debe seleccionar un cliente"
            }
        }
    });

});
