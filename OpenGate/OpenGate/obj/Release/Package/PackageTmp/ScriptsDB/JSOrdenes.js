$("#guardar").click(function () {
    var $modal = $("#processing-modal");
    $modal.modal('show');
    setTimeout(function () {
        $.ajax({
            url: '/ordenes/ConfirmarEliminarCorte/',
            dataType: "json",
            type: "GET",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data === 'Correcto') {
                    toastr.success('Eliminacion Correcta');
                    $modal.modal('hide');
                }
                else {
                    toastr.error('Ha ocurrido un error.');
                    $modal.modal('hide');
                }
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    }, 1800);
});