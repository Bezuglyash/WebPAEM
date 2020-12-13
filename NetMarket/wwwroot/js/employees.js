window.onload = init;
var id;
var rewriteRole = "";
var isOpen = false;

function init() {
    //alert("Топчик!");
};

$(document).ready(function () { //или $(function() {
    $("select").change(function (e) {
        console.log(this.id.toString());
        id = this.id;
        rewriteRole = e.target.value;
        $(".editContainer").fadeIn();
        $("#editData").focus();
    });
});

$(document).mouseup(function (e) {
    if (document.getElementById('editContainer').style.display != "none") {
        var divEditData = $("#editData");
        console.log(isOpen.toString());
        if (!divEditData.is(e.target) && divEditData.has(e.target).length === 0) {
            if (isOpen) {
                if (rewriteRole == "Сотрудник") {
                    $(`#${id}`).val('Администратор');
                } else {
                    $(`#${id}`).val('Сотрудник');
                }
                Standard();
            } else {
                isOpen = true;
            }
        }
    }
});

function SaveClick() {
    $.ajax({
        type: "POST",
        url: "/Staff/UpdateRole",
        data: { id, rewriteRole },
        success: function () {
            Standard();
        },
        error: function (req, status, error) {
            alert(error);
        }
    });
}

function CancelClick() {
    if (rewriteRole == "Сотрудник") {
        $(`#${id}`).val('Администратор');
    } else {
        $(`#${id}`).val('Сотрудник');
    }
    Standard();
}

function Standard() {
    $(".editContainer").fadeOut();
    id = null;
    rewriteRole = "";
    isOpen = false;
}