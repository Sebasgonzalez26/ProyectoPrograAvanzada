document.addEventListener('DOMContentLoaded', function () {
    var table = new DataTable('#tVeterinarios', {
        scrollX: true, // activa scroll horizontal si la tabla es muy ancha
        language: {
            url: '//cdn.datatables.net/plug-ins/2.3.5/i18n/es-ES.json'
        },
        // Opcional: personalizar opciones iniciales
        pageLength: 10,     // cuántas filas mostrar por página
        lengthMenu: [5, 10, 25, 50, 100], // opciones del selector
    });
});