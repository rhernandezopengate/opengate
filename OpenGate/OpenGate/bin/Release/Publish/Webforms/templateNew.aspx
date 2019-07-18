<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="templateNew.aspx.cs" Inherits="OpenGate.Webforms.templateNew" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="~/vendors/datatables.net-bs/css/dataTables.bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.datatables.net/buttons/1.5.2/css/buttons.dataTables.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">        
    <div class="row">
        <div class="col-md-12 col-sm-12 col-xs-12">
            <div class="x_panel">
                <div class="x_title">
                    <h2> Estado Complemento <small>Index</small></h2> &nbsp;&nbsp;&nbsp;
                    <button type="submit" class="btn btn-primary"><i class="glyphicon glyphicon-floppy-disk"></i> Guardar</button>
                    <a class="btn btn-danger" href="@Url.Action("Index")"> <i class="glyphicon glyphicon-remove-circle"></i> Cancelar</a>
                    <a class="btn btn-success" href="@Url.Action("Create")"> <i class="fa fa-plus-circle"></i> Agregar Nuevo Registro</a>
                    <div class="clearfix"></div>
                </div>
                <div class="x_content">
                                        
                </div>
            </div>
        </div>
    </div>    
    </form>
    <label class="control-label col-md-2">Descripcion</label>   
    <!-- Custom Theme Style -->
    <script src="../build/js/custom.min.js"></script>

    <script src="https://cdn.datatables.net/1.10.19/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.4/js/dataTables.buttons.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.4.0/js/buttons.flash.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js"></script>
    <script src="https://cdn.rawgit.com/bpampuch/pdfmake/0.1.27/build/pdfmake.min.js"></script>
    <script src="https://cdn.rawgit.com/bpampuch/pdfmake/0.1.27/build/vfs_fonts.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.4.0/js/buttons.html5.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.4.0/js/buttons.print.min.js"></script>
    <script src="https://cdn.datatables.net/select/1.2.7/js/dataTables.select.min.js"></script>

    <script src="~/vendors/datatables.net-bs/js/dataTables.bootstrap.min.js"></script>
</body>
</html>
