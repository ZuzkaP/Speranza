
$("#SetMessageForClients").click(function () {
    $('#NewUserMessage').modal('show');
});

$(function () {
    $(".NewMessageFormDatePicker").datepicker({
        beforeShow: function () {
            setTimeout(function () {
                $('.ui-datepicker').css('z-index', 99999999999999);
            }, 0);
        },
        dateFormat: "dd.mm.yy",
        minDate: 0,
        constrainInput: true,
        monthNames: ["Január", "Február", "Marec", "Apríl", "Máj", "Jún", "Júl", "August", "September", "Október", "November", "December"],
        monthNamesShort: ["Jan", "Feb", "Mar", "Apr", "Máj", "Jún", "Júl", "Aug", "Sep", "Okt", "Nov", "Dec"],
        dayNames: ["Nedeľa", "Pondelok", "Utorok", "Streda", "Štvrtok", "Piatok", "Sobota"],
        dayNamesMin: ["Ne", "Po", "Ut", "St", "Št", "Pi", "So"],
        firstDay: 1
    });
});


$('#CreateMessage').click(function () {
    var dateFrom = $('#MessageDateFrom').prop('value');
    var dateTo = $('#MessageDateTo').prop('value');
    var message = $('#Message').val();
    $.ajax({
        url: "AddNewMessage",
        data: { dateFrom: dateFrom, dateTo: dateTo, message: message},
        type: 'POST',
        dataType: "json",
        success: function(response) {
            switch (response) {
            case 0:
                $('#messageBoxInfoForUsersInModal').html("Nie je možné zobraziť správu v minulosti!");
                $('#messageBoxInfoForUsersInModal').show();
                return;
            case 2:
                $('#messageBoxInfoForUsersInModal')
                    .html("Nastavte kratšiu správu.");
                $('#messageBoxInfoForUsersInModal').show();
                return;
            case 3:
                $('#messageBoxInfoForUsersInModal')
                    .html("Nie je možné nastaviť prázdnu správu.");
                $('#messageBoxInfoForUsersInModal').show();
                return;
            default: //case 1
                $('#NewUserMessage').modal('hide');
                $('#usermessageBoxOnAdminUserPage')
                    .html("Správa bola úspešne nastavená a bude zobrazená v predvolenom termíne.");
                $('#usermessageBoxOnAdminUserPage').show();
                return;
            }
        }
    });
});


$(function () {
    $('.IsAdmin').change(function () {

        var self = $(this);
        var id = self.attr('id');
        var value = self.prop('checked');
        $.ajax({
            url: "ToggleAdmin",
            data: { id: id, isAdmin: value },
            type: 'POST',
            dataType: "json",
            success: function (response) {
                if (response.Message == 1)
                    $('#messageBox').html("<b>Zrušenie</b> administrátorských práv pre používateľa: <b>" + response.Email + "</b>");
                else if (response.Message == 2)
                    $('#messageBox').html("<b>Pridanie</b> administrátorských práv pre používateľa: <b>" + response.Email + "</b>");

                $('#messageBox').show();
            }
        });

    });
});

$(function () {
    $('.Category').change(function () {

        var self = $(this);
        var id = self.attr('id');
        var value = self.val();
        $.ajax({
            url: "UserCategory",
            data: { id: id, category: value },
            type: 'POST',
            dataType: "json",
            success: function (response) {
                if (response.Message == 3)
                    $('#messageBox').html("Kategória pre používateľa <b>" + response.Email + "</b> bola zmenená na: <b>" + response.Category + "</b>");
                $('#messageBox').show();
            }
        });

    });
});

$(function () {
    $('.SignUpCount').click(function () {
        var self = $(this);
        var count = self.data("count");
        var email = self.data("email");
        $.ajax({
            url: "UpdateSignUpCount",
            data: { id: email, countUpdate: count },
            type: 'POST',
            dataType: "json",
            success: function (response) {
                if (response.Message == 4)
                    $('#messageBox').html("Počet vstupov užívateľovi <b>" + response.Email + "</b> bol zvýšený o <b>" + response.ChangeNumberOfSignUps + "</b>");
                else if (response.Message == 5) {
                    var changedCount = response.ChangeNumberOfSignUps;
                    if ($('#freeSignUpsCell-' + getEmailForJS(response.Email)).text() < response.ChangeNumberOfSignUps) {
                        changedCount = $('#freeSignUpsCell-' + getEmailForJS(response.Email)).text();
                    }
                    $('#messageBox').html("Počet vstupov užívateľovi <b>" + response.Email + "</b> bol znížený o <b>" + changedCount + "</b>");

                }

                $('#messageBox').show();
                $('#freeSignUpsCell-' + getEmailForJS(response.Email)).html(response.AfterChangeNumberOfSignUps);
            }
        });

    });
});


$(function () {
    $('.trainingsDetails').click(function () {
        var self = $(this);
        var email = self.data("email");
        $.ajax({
            url: "TrainingsDetails",
            data: { id: email },
            type: 'POST',
            dataType: "html",
            success: function (response) {
                $('.modal-body').html(response);
                $('#modalTitleEmail').html(email);
                $('#trainingDetailsModal').modal('show');
            }
        });

    });
});

$(document).ready(function () {
    $("#UsersTable").tablesorter({
        sortList: [[1, 0], [0, 0]],
        headers: {
            3: { sorter: false },
        },
        //textExtraction: function (node) {
        //    if (node.childNodes[0].tagName == "SELECT")
        //    {
        //        alert(node.childNodes[0]);
        //    }
        //    return node.innerHTML;
        //}
    });
    //$("#UsersTable").bind("sortStart", function () {
    //    .show();
    //}).bind("sortEnd", function () {
    //    .hide();
    //})

}
);


$('#searchBox').keyup(function () {
    var self = $(this);
    var data = noDiacritics(self.val().toLowerCase());
    var table = $('#UsersTable');
    var rows = $('#UsersTable tr');
    for (var i = 1; i < rows.length; i++) {
        if (noDiacritics(rows.eq(i).children('td').eq(1).text().toLowerCase()).indexOf(data) < 0)
        {
            rows.eq(i).hide();
        }
        else
        {
            rows.eq(i).show();
        }
       
    }
});

function noDiacritics(input) {
    return input.

        replace('á', 'a').
        replace('ä', 'a').
        replace('č', 'c').
        replace('ď', 'd').
        replace('é', 'e').
        replace('í', 'i').
        replace('ĺ', 'l').
        replace('ľ', 'l').
        replace('ň', 'n').
        replace('ó', 'o').
        replace('ô', 'o').
        replace('ö', 'o').
        replace('ŕ', 'r').
        replace('ř', 'r').
        replace('š', 's').
        replace('ť', 't').
        replace('ú', 'u').
        replace('ý', 'y').
        replace('ž', 'z');
}