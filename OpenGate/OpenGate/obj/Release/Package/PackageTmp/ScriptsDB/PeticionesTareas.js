tinymce.init({ selector: 'textarea' });

$('#HoraInicioPicker').datetimepicker({
    format: 'hh:mm A'
});

$('#HoraFinPicker').datetimepicker({
    format: 'hh:mm A'
});

$(function () {
    $("#Fecha").datepicker({
        dateFormat: 'dd/mm/yy',
        changeMonth: true,
        changeYear: true
    });

    $("#FechaEntrega").datepicker({
        dateFormat: 'dd/mm/yy',
        changeMonth: true,
        changeYear: true
    });
});