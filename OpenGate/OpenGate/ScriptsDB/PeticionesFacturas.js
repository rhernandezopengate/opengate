$(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal]").on("click", function (e) {
        $("#MyModalContent").load(this.href, function () {
            $("#MyModal").modal({
                //backdrop: 'static',
                keyboard: false
            }, 'show');
            bindForm(this);
        });
        return false;
    });
});

$(function () {
    $("#txtProveedor").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/proveedors/AutoComplete/',
                data: "{ 'prefix': '" + request.term + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }));
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        change: function (event, ui) { validarfacturaproveedor(); },
        select: function (e, i) {
            $("#hfCustomer").val(i.item.val);
        },
        minLength: 0
    }).focus(function () {
        $(this).autocomplete("search");
    });
});

$(function () {

    $(function () {
        if ($("#Numero").val() === "") {
            $("#txtProveedor").prop("disabled", true);
        } else {
            $("#txtProveedor").prop("disabled", false);
        }
    });

    $(function () {
        $("#FechaFactura").datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true
        });
    });

    $(function () {
        $("#FechaPago").datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true
        });
    });    

    $(function () {
        $("#FechaVencimiento").datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true
        });
    });

    $(function () {
        $("#FechaSello").datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true
        });
    });

    $(function () {
        $("#datepicker_from").datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true
        });
    });

    $(function () {
        $("#datepicker_to").datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true
        });
    });

    $(function () {
        $("#fechaVInicio").datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true
        });
    });

    $(function () {
        $("#fechaVFin").datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true
        });
    });

    $(function () {
        $("#fechaPInicio").datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true
        });
    });

    $(function () {
        $("#fechaPFin").datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true
        });
    });
});

function bindForm(dialog) {
    $('form', dialog).submit(function () {
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    $("#MyModal").modal('hide');
                    toastr.success(result.message);
                    table.draw();                    
                }
                else {                    
                    $("#MyModal").modal('show');
                    toastr.error(result.ErrorMessage);
                    bindForm(dialog);
                }
            },
            error: function (xml, message, text) {
                toastr.error("Msg: " + message + ", Text: " + text);
            }
        });
        return false;
    });
}

$(document).keypress(function (e) {
    if (e.keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

function validarfacturaproveedor() {
    $.ajax({
        type: "POST",
        url: "/facturas/ValidarProveedorFactura",        
        data: {
            'numero': $("#Numero").val(),
            'razon': $("#txtProveedor").val()
        },
        dataType: "html",
        success: function (result, status, xhr) {  
            if ($("#Numero").val() === "") {
                $("#txtProveedor").prop("disabled", true);
            } else {
                $("#txtProveedor").prop("disabled", false);
            }
            if (result === "Sin facturas") {
                $("#mensaje").html("<div class='alert alert-success alert-dismissible fade in' role='alert'> <h2>" + result + "</h2> </div>");
            }
            else {
                $("#mensaje").html("<div class='alert alert-danger alert-dismissible fade in' role='alert'> <h2>" + result + "</h2> </div>");
            }
        },
        error: function (xhr, status, error) {
            $("#mensaje").html("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
        }
    });
}

function sumar() {
    var subtotal = $('#Subtotal').val();
    var descuento = $('#Descuento').val();
    var retencion = $('#Retencion').val();
    var ivaValor = 1.16;

    var iva = 0;
    var total = 0;
    var totalRetencion = 0;
    var totalDescuento = 0;

    total = subtotal * ivaValor;
    totalRetencion = total - retencion;
    totalDescuento = totalRetencion - descuento;

    $('#Iva').val((total - subtotal).toFixed(3));
    $('#Total').val(totalDescuento.toFixed(3));
}

function limpiar() {    
    location.href = "/facturas/index";
}

function tazaCero() {
    var subtotal = $('#Subtotal').val();
    var descuento = $('#Descuento').val();
    var retencion = $('#Retencion').val();
    
    var total = 0;
    var totalRetencion = 0;
    var totalDescuento = 0;

    total = subtotal;
    totalRetencion = total - retencion;
    totalDescuento = totalRetencion - descuento;

    $('#Iva').val((total - subtotal).toFixed(3));
    $('#Total').val(totalDescuento.toFixed(3));
}

function retencion() {
    var subtotal = $('#Subtotal').val();        
    var retencionValor = 0.04;
        
    var total = 0;
    var totalRetencion = 0;    

    total = subtotal * retencionValor;    

    $('#Retencion').val(total.toFixed(3));
    sumar();
}