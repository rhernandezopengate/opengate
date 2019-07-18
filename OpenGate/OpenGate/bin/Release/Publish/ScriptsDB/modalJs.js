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