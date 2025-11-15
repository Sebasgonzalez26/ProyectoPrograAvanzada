// VerPerfil.js

function consultarCedula(identificacion) {
    $.ajax({
        url: urlConsultarCedulaPerfil, // 👈 usamos la variable definida en la vista
        type: 'GET',
        data: { id: identificacion },
        dataType: 'json',
        success: function(result) {
            if (result.results && result.results.length > 0) {
                var nombreCompleto = result.results[0].fullname;
                $("#Nombre").val(nombreCompleto);
                $("#Nombre").closest(".form-group, .input-group").addClass("is-focused is-filled");
            } else {
                console.log("No se encontró el nombre");
                $("#Nombre").val("No encontrado");
                $("#Nombre").closest(".form-group, .input-group").addClass("is-focused is-filled");
            }
        },
        error: function() {
            console.log("Error en la consulta");
            $("#Nombre").val("Error al consultar");
            $("#Nombre").closest(".form-group, .input-group").addClass("is-focused is-filled");
        }
    });
}

$(document).ready(function() {
    $("#Identificacion").on("keyup", function() {
        var identificacion = $(this).val().trim();
        if (identificacion.length >= 8) { // o 9 si quieres
            consultarCedula(identificacion);
        }
    });
});
