
    $('#ChangePass').click(function () {
        $('#OldPass').val('');
        $('#NewPass').val('');
        $('#ConfirmPass').val('');
        $('#ModalMessageBoxSuccess').hide();
        $('#ModalMessageBox').hide();
        $('#ChangePassButton').show();
        $('#ChangePassModal').modal('show');
    });


$("#ChangePassButton").click(function () {
    var self = $(this);
    var oldPass =  $('#OldPass').prop('value');
    var newPass =  $('#NewPass').prop('value');
    var confirmPass = $('#ConfirmPass').prop('value');
    $.ajax({
        url: '../Accounts/ChangeUserPassword',
        data: { oldPass: oldPass, newPass: newPass, confirmPass: confirmPass },
        type: 'POST',
        dataType: "json",
        success: function (response) {
            switch (response) {
                case 9:
                    showMessageBox("Vaše heslo je nesprávne!");
                    return;
                case 1:
                    showMessageBox("Nové heslo nemôže byť prázdne!");
                    return;
                case 2:
                    showMessageBox("Potvrdenie hesla sa nezhoduje!");
                    return;
                case 3:
                    showMessageBox("Nové heslo musí mať aspoň 6 znakov!");
                    return;
                case 4:
                    showMessageBox("Nové heslo musí obsahovať aspoň jedno číslo!");
                    return;
                case 5:
                    showMessageBox("Nové heslo musí obsahovať aspoň jedno písmeno!");
                    return;
                case 6:
                    showMessageBox("Nové heslo a potvrdenie hesla sa nezhodujú!");
                    return;
                default:
                    if (response.Message == 7)
                    {
                        showMessageBoxSuccess("Vaše heslo bolo úspešne zmenené!");
                        $('#ChangePassButton').hide();
                    }
                    return;
            }

                
        }
    });
});

function showMessageBox(text) {
    $('#ModalMessageBoxSuccess').hide();
    $("#ModalMessageBox").html(text);
    $("#ModalMessageBox").show();
}

function showMessageBoxSuccess(text) {
    $('#ModalMessageBox').hide();
    $("#ModalMessageBoxSuccess").html(text);
    $("#ModalMessageBoxSuccess").show();
}

$('[data-toggle=confirmation]').confirmation({
        rootSelector: '[data-toggle=confirmation]',
        onConfirm: function (value) {
            location.href='RemoveAccount';
        }
});


$("#NextTrainingsBtn").click(function () {
    var shownTrainingsCount = $("#UserTrainingsTable tr.obsolete").length;
    $.ajax({
        url: "GetNextUserTrainings",
        data: {numberOfAlreadyShownTrainings: shownTrainingsCount},
        type: 'POST',
        dataType: "json",
        success: function (response) {
            if (response.length === 0) {
                $("#NextTrainingsBtn").remove();
                return;
            }
            var table = $("#UserTrainingsTable");
            for (var i = 0; i < response.length; i++) {
                table.append(
                    '<tr class="obsolete d-flex">' +
                    '<td class="col-lg-2">' + response[i].DateStr + '</td>' +
                    '<td class="col-lg-2">' + response[i].TimeStr + '</td>' +
                    '<td class="col-lg-4">' + response[i].Description + '</td>' +
                    '<td class="col-lg-2">' + response[i].Trainer + '</td>' +
                    '<td class="col-lg-2"></td>' +
                    '</tr>'
                    );
            }
        }
    });
    });