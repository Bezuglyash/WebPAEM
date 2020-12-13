window.onload = init;
var currentElementId = "";
var currentText = "";
var currentImage = "";

function init() {
    //alert("Топчик!");
};

function readURL(input) {

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#image').attr('src', e.target.result);
        };

        reader.readAsDataURL(input.files[0]);
        var buttonSave = document.getElementById("buttonSave");
        buttonSave.style.display = "block";
        var buttonCancel = document.getElementById("buttonCancel");
        buttonCancel.style.display = "block";
    }
}

$("#file").change(function () {
    if (currentImage == "") {
        currentImage = $("#image").attr('src');;
    }
    readURL(this);
});

function CancelClick() {
    console.log(currentImage);
    var input = document.getElementById("file");
    input.value = '';
    $('#image').attr('src', currentImage);
    var buttonSave = document.getElementById("buttonSave");
    buttonSave.style.display = "none";
    var buttonCancel = document.getElementById("buttonCancel");
    buttonCancel.style.display = "none";
    currentImage = "";
}

function NameClick(phoneId) {
    if ($("#button-Name").text() == "Изменить") {
        if (currentElementId != "") {
            UpdateElements();
        }
        $("#name").prop("readonly", false);
        $("#name").css("border", "1px solid blue");
        $("#name").focus();
        $("#button-Name").text("Сохранить");
        currentText = $("#name").val();
        currentElementId = "#name";
    }
    else {
        if ($("#name").val() != "" && $("#name").val() != currentText) {
            var typeOfData = "name";
            var newData = $("#name").val();
            $.ajax({
                type: "POST",
                url: "/Staff/EditData",
                data: { phoneId, typeOfData, newData },
                success: function () { },
                error: function (req, status, error) {
                    alert(error);
                }
            });
        }
        else {
            $("#name").val(currentText);
        }
        $("#name").prop("readonly", true);
        $("#name").css("border", "none");
        $("#button-Name").text("Изменить");
        currentElementId = "";
        currentText = "";
    }
}

function CompanyClick(phoneId) {
    if ($("#button-Company").text() == "Изменить") {
        if (currentElementId != "") {
            UpdateElements();
        }
        $("#company").prop("readonly", false);
        $("#company").css("border", "1px solid blue");
        $("#company").focus();
        $("#button-Company").text("Сохранить");
        currentText = $("#company").val();
        currentElementId = "#company";
    }
    else {
        if ($("#company").val() != "" && $("#company").val() != currentText) {
            var typeOfData = "company";
            var newData = $("#company").val();
            $.ajax({
                type: "POST",
                url: "/Staff/EditData",
                data: { phoneId, typeOfData, newData },
                success: function () { },
                error: function (req, status, error) {
                    alert(error);
                }
            });
        }
        else {
            $("#company").val(currentText);
        }
        $("#company").prop("readonly", true);
        $("#company").css("border", "none");
        $("#button-Company").text("Изменить");
        currentElementId = "";
        currentText = "";
    }
}

function ColorClick(phoneId) {
    if ($("#button-Color").text() == "Изменить") {
        if (currentElementId != "") {
            UpdateElements();
        }
        $("#color").prop("readonly", false);
        $("#color").css("border", "1px solid blue");
        $("#color").focus();
        $("#button-Color").text("Сохранить");
        currentText = $("#color").val();
        currentElementId = "#color";
    }
    else {
        if ($("#color").val() != "" && $("#color").val() != currentText) {
            var typeOfData = "color";
            var newData = $("#color").val();
            $.ajax({
                type: "POST",
                url: "/Staff/EditData",
                data: { phoneId, typeOfData, newData },
                success: function () { },
                error: function (req, status, error) {
                    alert(error);
                }
            });
        }
        else {
            $("#color").val(currentText);
        }
        $("#color").prop("readonly", true);
        $("#color").css("border", "none");
        $("#button-Color").text("Изменить");
        currentElementId = "";
        currentText = "";
    }
}

function PriceClick(phoneId) {
    if ($("#button-Price").text() == "Изменить") {
        if (currentElementId != "") {
            UpdateElements();
        }
        $("#price").prop("readonly", false);
        $("#price").css("border", "1px solid blue");
        $("#price").focus();
        $("#button-Price").text("Сохранить");
        currentText = $("#price").val();
        currentElementId = "#price";
    }
    else {
        if ($("#price").val() != "" && $("#price").val() != currentText) {
            var typeOfData = "price";
            var newData = $("#price").val();
            $.ajax({
                type: "POST",
                url: "/Staff/EditData",
                data: { phoneId, typeOfData, newData },
                success: function () { },
                error: function (req, status, error) {
                    alert(error);
                }
            });
        }
        else {
            $("#price").val(currentText);
        }
        $("#price").prop("readonly", true);
        $("#price").css("border", "none");
        $("#button-Price").text("Изменить");
        currentElementId = "";
        currentText = "";
    }
}

function OperationSystemClick(phoneId) {
    if ($("#button-OperationSystem").text() == "Изменить") {
        if (currentElementId != "") {
            UpdateElements();
        }
        $("#operationSystem").prop("readonly", false);
        $("#operationSystem").css("border", "1px solid blue");
        $("#operationSystem").focus();
        $("#button-OperationSystem").text("Сохранить");
        currentText = $("#operationSystem").val();
        currentElementId = "#operationSystem";
    }
    else {
        if ($("#operationSystem").val() != "" && $("#operationSystem").val() != currentText) {
            var typeOfData = "operationSystem";
            var newData = $("#operationSystem").val();
            $.ajax({
                type: "POST",
                url: "/Staff/EditData",
                data: { phoneId, typeOfData, newData },
                success: function () { },
                error: function (req, status, error) {
                    alert(error);
                }
            });
        }
        else {
            $("#operationSystem").val(currentText);
        }
        $("#operationSystem").prop("readonly", true);
        $("#operationSystem").css("border", "none");
        $("#button-OperationSystem").text("Изменить");
        currentElementId = "";
        currentText = "";
    }
}

function StorageCardClick(phoneId) {
    if ($("#button-StorageCard").text() == "Изменить") {
        if (currentElementId != "") {
            UpdateElements();
        }
        $("#storageCard").prop("readonly", false);
        $("#storageCard").css("border", "1px solid blue");
        $("#storageCard").focus();
        $("#button-StorageCard").text("Сохранить");
        currentText = $("#storageCard").val();
        currentElementId = "#storageCard";
    }
    else {
        if ($("#storageCard").val() != "" && $("#storageCard").val() != currentText) {
            var typeOfData = "storageCard";
            var newData = $("#storageCard").val();
            $.ajax({
                type: "POST",
                url: "/Staff/EditData",
                data: { phoneId, typeOfData, newData },
                success: function () { },
                error: function (req, status, error) {
                    alert(error);
                }
            });
        }
        else {
            $("#storageCard").val(currentText);
        }
        $("#storageCard").prop("readonly", true);
        $("#storageCard").css("border", "none");
        $("#button-StorageCard").text("Изменить");
        currentElementId = "";
        currentText = "";
    }
}

function WeightClick(phoneId) {
    if ($("#button-Weight").text() == "Изменить") {
        if (currentElementId != "") {
            UpdateElements();
        }
        $("#weight").prop("readonly", false);
        $("#weight").css("border", "1px solid blue");
        $("#weight").focus();
        $("#button-Weight").text("Сохранить");
        currentText = $("#weight").val();
        currentElementId = "#weight";
    }
    else {
        if ($("#weight").val() != "" && $("#weight").val() != currentText) {
            var typeOfData = "weight";
            var newData = $("#weight").val();
            $.ajax({
                type: "POST",
                url: "/Staff/EditData",
                data: { phoneId, typeOfData, newData },
                success: function () { },
                error: function (req, status, error) {
                    alert(error);
                }
            });
        }
        else {
            $("#weight").val(currentText);
        }
        $("#weight").prop("readonly", true);
        $("#weight").css("border", "none");
        $("#button-Weight").text("Изменить");
        currentElementId = "";
        currentText = "";
    }
}

function DescriptionClick(phoneId) {
    if ($("#button-Description").text() == "Изменить") {
        if (currentElementId != "") {
            UpdateElements();
        }
        $("#description").prop("readonly", false);
        $("#description").css("border", "1px solid blue");
        $("#description").focus();
        $("#button-Description").text("Сохранить");
        currentText = $("#description").val();
        currentElementId = "#description";
    }
    else {
        if ($("#description").val() != "" && $("#description").val() != currentText) {
            var typeOfData = "description";
            var newData = $("#description").val();
            $.ajax({
                type: "POST",
                url: "/Staff/EditData",
                data: { phoneId, typeOfData, newData },
                success: function () { },
                error: function (req, status, error) {
                    alert(error);
                }
            });
        }
        else {
            $("#description").val(currentText);
        }
        $("#description").prop("readonly", true);
        $("#description").css("border", "none");
        $("#button-Description").text("Изменить");
        currentElementId = "";
        currentText = "";
    }
}

function ExistenceClick(phoneId) {
    var typeOfData = "existence";
    var newData = $("#selectExistence").val();
    $.ajax({
        type: "POST",
        url: "/Staff/EditData",
        data: { phoneId, typeOfData, newData },
        success: function () { },
        error: function (req, status, error) {
            alert(error);
        }
    });
    $("#button-Existence").css("visibility", "hidden");
    currentElementId = "";
    currentText = "";
}

document.querySelector("select").addEventListener('change', function (e) {
    if (currentElementId != "#selectExistence") {
        if (currentElementId != "") {
            UpdateElements();
        }
        currentElementId = "#selectExistence";
        if (e.target.value == "Нет в наличии") {
            currentText = "Есть в наличии";
        }
        else {
            currentText = "Нет в наличии";
        }
        $("#button-Existence").css("visibility", "visible");
    }
    else {
        $("#button-Existence").css("visibility", "hidden");
        currentElementId = "";
        currentText = "";
    }
});

function UpdateElements() {
    switch (currentElementId) {
        case "#name":
            $("#name").val(currentText);
            $("#name").prop("readonly", true);
            $("#name").css("border", "none");
            $("#button-Name").text("Изменить");
            break;
        case "#company":
            $("#company").val(currentText);
            $("#company").prop("readonly", true);
            $("#company").css("border", "none");
            $("#button-Company").text("Изменить");
            break;
        case "#color":
            $("#color").val(currentText);
            $("#color").prop("readonly", true);
            $("#color").css("border", "none");
            $("#button-Color").text("Изменить");
            break;
        case "#price":
            $("#price").val(currentText);
            $("#price").prop("readonly", true);
            $("#price").css("border", "none");
            $("#button-Price").text("Изменить");
            break;
        case "#operationSystem":
            $("#operationSystem").val(currentText);
            $("#operationSystem").prop("readonly", true);
            $("#operationSystem").css("border", "none");
            $("#button-OperationSystem").text("Изменить");
            break;
        case "#storageCard":
            $("#storageCard").val(currentText);
            $("#storageCard").prop("readonly", true);
            $("#storageCard").css("border", "none");
            $("#button-StorageCard").text("Изменить");
            break;
        case "#weight":
            $("#weight").val(currentText);
            $("#weight").prop("readonly", true);
            $("#weight").css("border", "none");
            $("#button-Weight").text("Изменить");
            break;
        case "#description":
            $("#description").val(currentText);
            $("#description").prop("readonly", true);
            $("#description").css("border", "none");
            $("#button-Description").text("Изменить");
            break;
        case "#selectExistence":
            $("#selectExistence").val(currentText);
            $("#button-Existence").css("visibility", "hidden");

        default:
            break;
    }
}